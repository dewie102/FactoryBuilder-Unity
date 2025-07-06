using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Assets.Scripts.Data.Entities;
using Assets.Scripts.EntitySystem;
using Assets.Scripts.EntitySystem.Interfaces;

using UnityEngine;
using UnityEngine.Tilemaps;

using static UnityEditor.ShaderData;

namespace Assets.Scripts.Core
{
    /// <summary>
    /// Manages the game world, including entity placement, removal, and item transfers.
    /// </summary>
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager Instance { get; private set; }

        // Events
        public static event Action<Vector3Int, Entity> EntityPlaced;
        public static event Action<Vector3Int> EntityRemoved;
        public static event Action<ItemTransfer> ItemTransferred;

        [Header("Grid Configuration")]
        [SerializeField] private Grid grid;
        [SerializeField] private Tilemap[] tilemaps;

        // Entity Management
        private Dictionary<Vector3Int, Entity> _entities = new();

        // Item Transfer System
        private Queue<ItemTransfer> _pendingTransfers = new();
        private HashSet<Vector3Int> _queuedTargets = new();

        // Track planned state for this tick
        private HashSet<Vector3Int> _plannedToReceiveItem = new();
        private HashSet<Vector3Int> _plannedToLoseItem = new();

        #region Unity Lifecycle
        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        #region Entity Management
        public bool PlaceEntity(Vector3Int position, EntityData entityData)
        {
            if(HasEntityAt(position))
            {
                Debug.LogWarning($"Entity already exists at {position}");
                return false;
            }

            if(!IsValidPlacement(position, entityData))
            {
                Debug.LogWarning($"Invalid placement for entity {entityData.id} at {position}");
                return false;
            }

            Entity entity = EntityFactory.CreateEntity(entityData);
            _entities.Add(position, entity);

            EntityPlaced?.Invoke(position, entity); // Notify listeners
            Debug.Log($"Placed entity {entityData.id} at {position}");
            return true;
        }

        public bool RemoveEntity(Vector3Int position)
        {
            if(!HasEntityAt(position))
            {
                Debug.LogWarning($"No entity found at {position}");
                return false;
            }

            _entities.Remove(position);
            EntityRemoved?.Invoke(position); // Notify listeners
            Debug.Log($"Removed entity from {position}");
            return true;
        }

        public Entity GetEntityAt(Vector3Int position)
        {
            _entities.TryGetValue(position, out Entity entity);
            return entity;
        }

        public bool HasEntityAt(Vector3Int position)
        {
            return _entities.ContainsKey(position);
        }

        public IEnumerable<KeyValuePair<Vector3Int, Entity>> GetAllEntities()
        {
            return _entities;
        }
        #endregion

        #region Grid Operation
        public Vector3Int WorldToCell(Vector3 worldPos)
        {
            return tilemaps[0].WorldToCell(worldPos);
        }

        public Vector3 CellToWorld(Vector3Int cellPos)
        {
            Vector3 worldPos = tilemaps[0].CellToWorld(cellPos);
            worldPos += tilemaps[0].cellSize / 2f; // Center the position
            return worldPos;
        }

        public bool IsValidPlacement(Vector3Int position, EntityData entityData)
        {
            // Basic validation - can be expanded
            return !HasEntityAt(position);
        }

        public List<Vector3Int> GetNeighbors(Vector3Int position)
        {
            return new List<Vector3Int>
            {
                position + Vector3Int.up,
                position + Vector3Int.down,
                position + Vector3Int.left,
                position + Vector3Int.right
            };
        }

        public Vector3Int GetNeighborsInDirection(Vector3Int position, Direction direction)
        {
            Vector3Int offset = DirectionUtils.ToVector3Int(direction);
            return position + offset;
        }
        #endregion

        #region Simulation Tick
        public void TickWorld()
        {
            Debug.Log($"WorldManager::Ticking World with {_entities.Count} entities");

            // Clear previous frame's transfer queue and planned state
            ClearTransferQueue();
            ClearPlannedState();
            
            // Process all entities and queue transfers
            ProcessEntities();
            
            // Apply all queued transfers
            ApplyTransfers();
        }

        private void ClearPlannedState()
        {
            Debug.Log($"ClearPlannedState: Clearing {_plannedToReceiveItem.Count} receive plans and {_plannedToLoseItem.Count} lose plans");
            _plannedToReceiveItem.Clear();
            _plannedToLoseItem.Clear();
        }

        private void ProcessEntities()
        {
            // Multi-pass approach using the existing ProcessItemConsumer logic
            // Each pass tries to find new transfer opportunities

            var allEntityPositions = _entities.Keys.ToList();
            int pass = 0;
            int transfersFoundThisPass;

            do
            {
                pass++;
                int transfersBeforePass = _pendingTransfers.Count;

                Debug.Log($"Processing pass {pass} - Current queued transfers: {transfersBeforePass}");

                foreach(var position in allEntityPositions)
                {
                    if(!_entities.TryGetValue(position, out Entity entity))
                        continue;

                    // Try to process this consumer using existing logic
                    if(entity is IItemConsumer consumer)
                    {
                        ProcessItemConsumer(position, consumer);
                    }

                    // Tick entities only on first pass
                    if(pass == 1)
                    {
                        entity.OnTick();
                    }
                }

                transfersFoundThisPass = _pendingTransfers.Count - transfersBeforePass;
                Debug.Log($"Pass {pass} completed. New transfers found: {transfersFoundThisPass}");

            } while(transfersFoundThisPass > 0 && pass < 5); // Continue while finding new transfers

            Debug.Log($"Processing completed after {pass} passes with {_pendingTransfers.Count} total transfers");
        }

        private void ProcessItemConsumer(Vector3Int consumerPosition, IItemConsumer consumer)
        {
            Debug.Log($"Processing consumer at {consumerPosition}");

            // Skip if this consumer is already planned to receive an item
            if(_plannedToReceiveItem.Contains(consumerPosition))
            {
                // But if this consumer is also a producer with an item, let it try to send that item
                if(consumer is IItemProducer producer && producer.HasItem)
                {
                    Debug.Log($"  Consumer is planned to receive, but has item to send - continuing");
                }
                else
                {
                    Debug.Log($"  Consumer already planned to receive item - skipping");
                    return;
                }
            }

            // Check all input directions for this consumer
            foreach(Direction inputDirection in consumer.InputDirections)
            {
                Vector3Int inputPos = consumerPosition + DirectionUtils.ToVector3Int(inputDirection);
                Entity inputEntity = GetEntityAt(inputPos);

                Debug.Log($"  Checking input direction {inputDirection} at position {inputPos}");
                Debug.Log($"  Found entity: {inputEntity?.GetType().Name ?? "None"}");

                if(inputEntity is IItemProducer producer && producer.HasItem)
                {
                    Debug.Log($"  Producer has item: {producer.PeekItem()?.DisplayName ?? "None"}");
                    Item item = producer.PeekItem();
                    if(item != null && consumer.CanConsumeItem(item))
                    {
                        Debug.Log($"  Queueing transfer: {item.DisplayName} from {inputPos} to {consumerPosition}");

                        // Queue the transfer
                        QueueTransfer(new ItemTransfer(item, inputPos, consumerPosition, inputEntity, consumer as Entity));

                        // Mark planned state
                        _plannedToReceiveItem.Add(consumerPosition);
                        _plannedToLoseItem.Add(inputPos);

                        // Only take from one input per consumer per tick
                        break;
                    }
                    else
                    {
                        Debug.Log($"  Cannot consume item - CanConsumeItem returned: {consumer.CanConsumeItem(item)}");
                    }
                }
                else
                {
                    Debug.Log($"  No valid producer or producer has no item");
                }
            }
        }
        #endregion

        #region Item Transfer System
        private void ClearTransferQueue()
        {
            _pendingTransfers.Clear();
            _queuedTargets.Clear();
        }

        private void QueueTransfer(ItemTransfer transfer)
        {
            // Prevent multiple transfers to the same target in one tick
            if(_queuedTargets.Contains(transfer.to))
            {
                Debug.Log($"Transfer to {transfer.to} already queued - skipping.");
                return;
            }

            _pendingTransfers.Enqueue(transfer);
            _queuedTargets.Add(transfer.to);
            Debug.Log($"Queued transfer of item {transfer.item.DisplayName} from {transfer.from} to {transfer.to}");
        }

        private void ApplyTransfers()
        {
            int transfersApplied = 0;

            while(_pendingTransfers.Count > 0)
            {
                ItemTransfer transfer = _pendingTransfers.Dequeue();

                if(ExecuteTransfer(transfer))
                {
                    transfersApplied++;
                    ItemTransferred?.Invoke(transfer); // Notify listeners
                    Debug.Log($"Transfer applied: {transfer.item.DisplayName} from {transfer.from} to {transfer.to}");
                }
                else
                {
                    Debug.LogWarning($"Transfer failed: {transfer.item.DisplayName} from {transfer.from} to {transfer.to}");
                }
            }

            if(transfersApplied > 0)
            {
                Debug.Log($"Applied {transfersApplied} item transfers this tick.");
            }
        }

        private bool ExecuteTransfer(ItemTransfer transfer)
        {
            Debug.Log($"Executing transfer: {transfer.item.DisplayName} from {transfer.from} to {transfer.to}");
            Debug.Log($"  To Entity is IItemConsumer: {transfer.toEntity is IItemConsumer}");
            Debug.Log($"  From Entity is IItemProducer: {transfer.fromEntity is IItemProducer}");

            // Validate entities still exist and are in expected state
            if(transfer.toEntity is not IItemConsumer consumer || 
               transfer.fromEntity is not IItemProducer producer)
            {
                Debug.LogError("ExecuteTransfer: Entity type validation failed");
                return false; // Transfer failed
            }

            Debug.Log($"  consumer.TryConsumeItem: attempting...");

            // Attempt the transfer
            if(consumer.TryConsumeItem(transfer.item))
            {
                Debug.Log($"  consumer.TryConsumeItem: SUCCESS");
                producer.RemoveItem();
                Debug.Log($"  producer.RemoveItem: called");
                return true;
            }
            else
            {
                Debug.Log($"  consumer.TryConsumeItem: FAILED");
            }

                return false;

        }
        #endregion

        #region Debug/ Utility
        public void LogWorldState()
        {
            Debug.Log($"World State: {_entities.Count} entities, {_pendingTransfers.Count} pending transfers");

            foreach(var entityPair in _entities)
            {
                Debug.Log($"  Entity at {entityPair.Key}: {entityPair.Value.Data.name}");
            }
        }

        public string GetTileDebugInfo(Vector3Int position)
        {
            StringBuilder sb = new();

            foreach(Tilemap tilemap in tilemaps)
            {
                TileBase tile = tilemap.GetTile(position);
                string tileName = tile != null ? tile.name : "None";
                sb.AppendLine($"{tilemap.name}: {tileName}");
            }

            return sb.ToString();
        }

        public bool HasTileAt(Vector3Int position)
        {
            foreach(Tilemap tilemap in tilemaps)
            {
                if(tilemap.HasTile(position))
                {
                    return true;
                }
            }
            return false;
        }

        public TileBase GetTileFromMap(Vector3Int position, int tilemapIndex)
        {
            if(tilemapIndex >= 0 && tilemapIndex < tilemaps.Length)
            {
                return tilemaps[tilemapIndex].GetTile(position);
            }

            return null;
        }
        #endregion
    }
}
