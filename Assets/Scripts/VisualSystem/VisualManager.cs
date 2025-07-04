using System.Collections.Generic;

using Assets.Scripts.Core;
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
        WorldManager.EntityPlaced += HandleEntityPlaced;
        WorldManager.ItemTransferred += HandleItemTransferred;
    }

    private void OnDisable()
    {
        WorldManager.EntityPlaced -= HandleEntityPlaced;
        WorldManager.ItemTransferred -= HandleItemTransferred;
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

        Vector3 worldPosition = WorldManager.Instance.CellToWorld(cellPosition);

        GameObject instance = Instantiate(prefab, worldPosition, Quaternion.identity);
        _spawnedEntities[cellPosition] = instance;
    }

    private void HandleItemTransferred(ItemTransfer transfer)
    {
        Vector3 worldTo = WorldManager.Instance.CellToWorld(transfer.to);

        if (_spawnedItems.TryGetValue(transfer.item, out var obj))
        {
            obj.transform.position = worldTo;
        }
        else
        {
            GameObject visual = Instantiate(transfer.item.prefab, worldTo, quaternion.identity);
            _spawnedItems[transfer.item] = visual;
        }
    }
}
