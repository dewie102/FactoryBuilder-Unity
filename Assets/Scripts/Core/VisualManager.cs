using System.Collections.Generic;

using Assets.Scripts.EntitySystem;
using UnityEngine;

namespace Assets.Scripts.Core
{
    /// <summary>
    /// Manages the visual representation of entities and items in the game world.
    /// </summary>
    public class VisualManager : MonoBehaviour
    {
        [SerializeField] private EntityLibrary entityLibrary;
        private readonly Dictionary<Vector3Int, GameObject> _entityVisuals = new();

        private void OnEnable()
        {
            WorldManager.EntityPlaced += HandleEntityPlaced;
            WorldManager.EntityRemoved += HandleEntityRemoved;
            WorldManager.EntityRotated += HandleEntityRotated;
        }

        private void OnDisable()
        {
            WorldManager.EntityPlaced -= HandleEntityPlaced;
            WorldManager.EntityRemoved -= HandleEntityRemoved;
            WorldManager.EntityRotated -= HandleEntityRotated;
        }

        private void HandleEntityPlaced(Vector3Int cellPosition, Entity entity)
        {
            if(_entityVisuals.ContainsKey(cellPosition))
            {
                Debug.LogWarning($"Visual already exists at {cellPosition}");
                return;
            }

            GameObject prefab = entityLibrary.GetPrefab(entity.Data.id);

            if(prefab == null)
            {
                Debug.LogError($"No prefab found for entity ID: {entity.Data.id}");
                return;
            }

            Vector3 worldPosition = WorldManager.Instance.CellToWorld(cellPosition);
            GameObject instance = Instantiate(prefab, worldPosition, Quaternion.identity);

            EntityView entityView = instance.GetComponent<EntityView>();
            if(entityView != null)
            {
                entityView.Initialize(entity);
            }
            else
            {
                Debug.LogWarning($"EntityView component not found on prefab for entity: {entity.Data.displayName}");
            }

            _entityVisuals[cellPosition] = instance;
        }

        private void HandleEntityRemoved(Vector3Int cellPosition)
        {
            if(_entityVisuals.TryGetValue(cellPosition, out GameObject visual))
            {
                Destroy(visual);
                _entityVisuals.Remove(cellPosition);
            }
        }

        private void HandleEntityRotated(Vector3Int cellPosition, Entity entity) { }
    }
}