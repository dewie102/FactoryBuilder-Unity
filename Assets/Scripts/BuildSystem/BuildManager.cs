using Assets.Scripts.EntitySystem;

using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public EntityData selectedEntity;

    public void HandleClick(Vector3 worldPos)
    {
        Vector3Int cell = GridManager.Instance.WorldToCell(worldPos);
        if (EntityManager.Instance.HasEntityAt(cell)) return;

        EntityManager.Instance.PlaceEntity(cell, buildingPrefab);
    }
}
