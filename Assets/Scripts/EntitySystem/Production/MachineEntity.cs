using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Data.Entities;
using Assets.Scripts.EntitySystem.Interfaces;

using Unity.VisualScripting;

using UnityEngine;

namespace Assets.Scripts.EntitySystem.Production
{
    internal class MachineEntity : ItemHolderEntity, IItemConsumer
    {
        private readonly HashSet<Direction> _inputDirections = new();

        public MachineEntity(EntityData data) : base(data)
        {
            SetOrientation(Direction.LEFT);
        }

        public IEnumerable<Direction> InputDirections => _inputDirections;

        public override bool CanConsumeItem(Item item)
        {
            bool canConsume = true;
            Debug.Log($"MachineEntity.CanConsumeItem: CanConsume={canConsume}");
            return canConsume;
        }

        public override void OnTick()
        {
            Debug.Log($"MachineEntity.OnTick: is ticking.");
            if(HasItem)
            {
                Debug.Log($"MachineEntity.OnTick: Has item, deleting it (consuming)");
                RemoveItem();
            }
        }

        public override void Rotate(Dictionary<Direction, Entity> _)
        {
            Direction newInputDirection = DirectionUtils.GetRotatedDirection(InputDirections.First());
            SetOrientation(newInputDirection);
            Debug.Log($"MachineEntity.Rotate: Rotated | inputDirection={newInputDirection}");
        }

        // TODO: Don't forget to add output once we add it to this entity
        public void SetOrientation(Direction input)
        {
            _inputDirections.Clear();

            _inputDirections.Add(input);

            NotifyDirectionsChanged();
        }

        public override void SetInputDirection(Direction direction)
        {
            SetOrientation(direction);
        }
    }
}
