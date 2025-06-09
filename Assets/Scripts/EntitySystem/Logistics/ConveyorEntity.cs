using Assets.Scripts.Data.Entities;
using UnityEngine;

namespace Assets.Scripts.EntitySystem.Logistics
{
    public class ConveyorEntity : Entity
    {
        public ConveyorEntity(EntityData data) : base(data)
        {
        }

        public override void OnTick()
        {
            Debug.Log($"{this} is ticking.");
        }
    }
}
