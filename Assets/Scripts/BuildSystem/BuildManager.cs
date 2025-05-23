using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public GameObject buildingPrefab;

    public void HandleClick(Vector3 worldPos)
    {
        Vector3Int cell = GridManager.Instance.WorldToCell(worldPos);
        if (EntityManager.Instance.HasEntityAt(cell)) return;

        GameObject building = Instantiate(buildingPrefab, cell + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
        EntityManager.Instance.PlaceEntity(cell, building);
        Debug.Log($"Placed {buildingPrefab} at {cell}");
    }
}
