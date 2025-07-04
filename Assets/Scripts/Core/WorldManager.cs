using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assets.Scripts.Data.Entities;
using Assets.Scripts.EntitySystem;
using Assets.Scripts.EntitySystem.Interfaces;

using UnityEngine;
using UnityEngine.Tilemaps;

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
        [SerializeField] public Tilemap[] tilemaps;

        // Entity Management
        private Dictionary<Vector3Int, Entity> _entities = new();

        // Item Transfer System
        private Queue<ItemTransfer> _pendingTransfers = new();
        private HashSet<Vector3Int> _queuedTargets = new();

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

            // Clear previous frame's transfer queue
            //ClearTransferQueue();
            
            // Process all entities and queue transfers
            ProcessEntities();
            
            // Apply all queued transfers
            //ApplyTransfers();
        }

        private void ProcessEntities()
        {
            foreach (var entityPair in _entities)
            {
                Vector3Int entityPosition = entityPair.Key;
                Entity entity = entityPair.Value;

                // Handle item consumption logic
                if(entity is IItemConsumer consumer)
                {
                    ProcessItemConsumer(entityPosition, consumer);
                }

                entity.OnTick(); // Call entity's tick method
            }
        }

        private void ProcessItemConsumer(Vector3Int consumerPosition, IItemConsumer consumer)
        {
            // Check all input directions for this consumer
            foreach(Direction inputDirection in consumer.InputDirections)
            {
                Vector3Int inputPos = consumerPosition + DirectionUtils.ToVector3Int(inputDirection);
                Entity inputEntity = GetEntityAt(inputPos);

                if(inputEntity is IItemProducer producer && producer.HasItem)
                {
                    Item item = producer.PeekItem();
                    /*if(item != null && consumer.CanConsumeItem(item))
                    {
                        QueueTransfer(new ItemTransfer(item, inputPos, consumerPosition, inputEntity, entity));
                    }*/
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
            // Validate entities still exist and are in expected state
            if(transfer.toEntity is not IItemConsumer consumer || 
               transfer.fromEntity is not IItemProducer producer)
            {
                return false; // Transfer failed
            }

            // Attempt the transfer
            if(consumer.TryConsumeItem(transfer.item))
            {
                producer.RemoveItem();
                return true;
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
        #endregion
    }
}
