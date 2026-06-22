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
            
            // Process all entities and queue transfers
            ProcessEntities();
        }

        private void ProcessEntities()
        {
            //ConveyorChainManager.ProcessAllChains();

            var allEntityPositions = _entities.Keys.ToList();
            foreach(var entityPosition in allEntityPositions)
            {
                if(!_entities.TryGetValue(entityPosition, out var entity))
                    continue;

                entity.OnTick();
            }
        }
        #endregion

        #region Debug/ Utility
        public void LogWorldState()
        {
            Debug.Log($"World State: {_entities.Count} entities");

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
