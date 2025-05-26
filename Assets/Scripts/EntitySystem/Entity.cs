using UnityEngine;

namespace Assets.Scripts.EntitySystem
{
    public class Entity
    {
        public EntityData Data { get; private set; }
        string Name { get; set; }

        public Entity(EntityData data)
        {
            Data = data;
        }

        public virtual void OnTick()
        {
            Debug.Log("Base Entity tick called");
        }
    }
}
