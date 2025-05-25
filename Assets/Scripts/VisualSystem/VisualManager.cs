using System.Collections.Generic;
using Assets.Scripts.EntitySystem;

using UnityEngine;

public class VisualManager : MonoBehaviour
{
    [SerializeField] private EntityLibrary entityLibrary;
    private Dictionary<Vector3Int, GameObject> _spawnedVisuals = new();

    private void OnEnable()
    {
        EntityManager.EntityPlaced += HandleEntityPlaced;
    }

    private void OnDisable()
    {
        EntityManager.EntityPlaced -= HandleEntityPlaced;
    }

    private void HandleEntityPlaced(Vector3Int cellPosition, Entity entity)
    {
        if (_spawnedVisuals.ContainsKey(cellPosition))
        {
            Debug.LogWarning($"Visual already exists at {cellPosition}");
            return;
        }

        GameObject prefab = entityLibrary.GetPrefab(entity.Data.id);

        if (prefab == null)
        {
            Debug.LogError($"No prefab found for entity ID: {entity.Data.id}");
            return;
        }

        Vector3 worldPosition = GridManager.Instance.CellToWorld(cellPosition);

        GameObject instance = Instantiate(prefab, worldPosition, Quaternion.identity);
        _spawnedVisuals[cellPosition] = instance;
    }
}
