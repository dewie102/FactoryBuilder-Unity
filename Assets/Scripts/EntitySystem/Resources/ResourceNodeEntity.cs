using Assets.Scripts.Data.Entities;
using UnityEngine;

namespace Assets.Scripts.EntitySystem.Resources
{
    public class ResourceNodeEntity : Entity
    {
        public ResourceNodeEntity(EntityData data) : base(data)
        {
        }

        public override void OnTick()
        {
            Debug.Log($"{this} is ticking.");
        }
    }
}
