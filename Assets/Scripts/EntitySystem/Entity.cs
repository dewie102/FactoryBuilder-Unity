using System.Collections.Generic;

using Assets.Scripts.Data.Entities;
using UnityEngine;

namespace Assets.Scripts.EntitySystem
{
    public class Entity
    {
        public EntityData Data { get; private set; }
        public string Name { get; set; }

        public Entity(EntityData data)
        {
            Data = data;
        }

        public virtual void OnTick()
        {
            Debug.Log("Base Entity tick called");
        }

        public virtual void Rotate(Dictionary<Direction, Entity> neighbors)
        {

        }

        public virtual void OnPlaced(Direction outputDirection, Dictionary<Direction, Entity> neighbors)
        {

        }
    }
}
