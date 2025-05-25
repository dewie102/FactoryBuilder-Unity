using System;
using System.Collections.Generic;
using Assets.Scripts.EntitySystem;

using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; private set; }
    public static event Action<Vector3Int, Entity> EntityPlaced;

    private Dictionary<Vector3Int, Entity> _entities;

    public void Awake()
    {
        Instance = this;
        _entities = new();
    }

    public bool PlaceEntity(Vector3Int position, Entity entity)
    {
        if(HasEntityAt(position))
            return false;

        _entities.Add(position, entity);
        EntityPlaced?.Invoke(position, entity); // Notify Listeners
        return true;
    }

    public void RemoveEntity(Vector3Int position)
    {
        _entities.Remove(position);
    }

    public Entity GetEntityAt(Vector3Int position)
    {
        return _entities[position];
    }

    public bool HasEntityAt(Vector3Int position)
    {
        return _entities.ContainsKey(position);
    }
}
