using System;
using System.Collections.Generic;
using Assets.Scripts.EntitySystem;
using Assets.Scripts.Data.Entities;

using UnityEngine;
using Assets.Scripts.EntitySystem.Interfaces;
using System.Linq;

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

    public bool PlaceEntity(Vector3Int position, EntityData entityData)
    {
        if(HasEntityAt(position))
            return false;

        Entity entity = EntityFactory.CreateEntity(entityData);

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
        _entities.TryGetValue(position, out Entity entity);

        return entity;
    }

    public bool HasEntityAt(Vector3Int position)
    {
        return _entities.ContainsKey(position);
    }

    public void TickEntities()
    {
        Debug.Log($"EntityManager::Ticking All Entities: {_entities.Count}");
        foreach(KeyValuePair<Vector3Int, Entity> entityPair in _entities)
        {
            Vector3Int entityPosition = entityPair.Key;
            Entity entity = entityPair.Value;

            if (entity is IItemConsumer consumer)
            {
                Direction inputDirection = consumer.InputDirections.First();
                Vector3Int inputPos = entityPosition + DirectionUtils.ToVector3Int(inputDirection);
                Entity inputEntity = GetEntityAt(inputPos);

                if (inputEntity is IItemProducer itemProducer && itemProducer.HasItem)
                {
                    Item item = itemProducer.PeekItem();
                    if (item != null)
                    {
                        if (consumer.TryConsumeItem(item))
                        {
                            itemProducer.RemoveItem();
                            Debug.Log($"Consumer at {entityPosition} consumed item from {inputPos}");
                        }
                        else
                        {
                            Debug.Log($"Consumer at {entityPosition} FAILED to consume item from {inputPos}");
                        }
                    }
                }
            }

            entity.OnTick();
        }
    }
}
