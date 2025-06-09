using Assets.Scripts.Data.Entities;
using Assets.Scripts.EntitySystem.Interfaces;
using UnityEngine;

namespace Assets.Scripts.EntitySystem.Logistics
{
    public class ConveyorEntity : Entity, IItemConsumer, IItemProducer
    {
        public Direction inputDirection;
        public Direction outputDirection;
        public Item inventory;
        public ConveyorEntity(EntityData data) : base(data)
        {
        }

        public bool HasItem => inventory == null;

        public override void OnTick()
        {
            Debug.Log($"{this} is ticking.");
        }

        public Item ProduceItem()
        {
            //EntityManager.Instance.GetEntityAt();
            return inventory;
        }

        public bool TryConsumeItem(Item item)
        {
            if (inventory == null)
            {
                inventory = item;
                return true;
            }
            return false;
        }
    }
}
