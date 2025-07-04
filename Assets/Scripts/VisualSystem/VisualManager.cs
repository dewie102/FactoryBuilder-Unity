using System.Collections.Generic;
using Assets.Scripts.EntitySystem;
using Unity.Mathematics;
using UnityEngine;

public class VisualManager : MonoBehaviour
{
    [SerializeField] private EntityLibrary entityLibrary;
    private Dictionary<Vector3Int, GameObject> _spawnedEntities = new();
    private Dictionary<Item, GameObject> _spawnedItems = new();

    private void OnEnable()
    {
        EntityManager.EntityPlaced += HandleEntityPlaced;
        EntityManager.ItemTransferred += HandleItemTransferred;
    }

    private void OnDisable()
    {
        EntityManager.EntityPlaced -= HandleEntityPlaced;
        EntityManager.ItemTransferred -= HandleItemTransferred;
    }

    private void HandleEntityPlaced(Vector3Int cellPosition, Entity entity)
    {
        if (_spawnedEntities.ContainsKey(cellPosition))
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
        _spawnedEntities[cellPosition] = instance;
    }

    private void HandleItemTransferred(Item item, Vector3Int from, Vector3Int to)
    {
        Vector3 worldTo = GridManager.Instance.CellToWorld(to);

        if (_spawnedItems.TryGetValue(item, out var obj))
        {
            obj.transform.position = worldTo;
        }
        else
        {
            GameObject visual = Instantiate(item.prefab, worldTo, quaternion.identity);
            _spawnedItems[item] = visual;
        }
    }
}
