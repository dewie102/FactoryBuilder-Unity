using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; private set; }

    private Dictionary<Vector3Int, GameObject> entityDict;

    public void Awake()
    {
        Instance = this;
        entityDict = new();
    }

    public void PlaceEntity(Vector3Int position, GameObject entity)
    {
        entityDict.Add(position, entity);
    }

    public void RemoveEntity(Vector3Int position)
    {
        entityDict.Remove(position);
    }

    public GameObject GetEntityAt(Vector3Int position)
    {
        return entityDict[position];
    }

    public bool HasEntityAt(Vector3Int position)
    {
        return entityDict.ContainsKey(position);
    }
}
