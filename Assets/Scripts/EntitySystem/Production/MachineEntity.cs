using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Data.Entities;
using Assets.Scripts.EntitySystem.Interfaces;

using Unity.VisualScripting;

using UnityEngine;

namespace Assets.Scripts.EntitySystem.Production
{
    internal class MachineEntity : Entity, IItemConsumer
    {
        private readonly HashSet<Direction> _inputDirections = new();

        private Item _currentItem;

        public MachineEntity(EntityData data) : base(data)
        {
            SetOrientation(Direction.LEFT, Direction.RIGHT);
        }

        public bool HasItem => _currentItem != null;

        public IEnumerable<Direction> InputDirections => _inputDirections;

        public event Action<Item> ItemAdded;
        public event Action ItemRemoved;

        public bool CanConsumeItem(Item item)
        {
            bool canConsume = true;
            Debug.Log($"MachineEntity.CanConsumeItem: CanConsume={canConsume}");
            return canConsume;
        }

        public override void OnTick()
        {
            Debug.Log($"MachineEntity.OnTick: is ticking.");
            if(HasItem)
                Debug.Log($"MachineEntity.OnTick: Has item, deleting it (consuming)");
                RemoveItem();
        }

        public bool TryConsumeItem(Item item)
        {
            Debug.Log($"MachineEntity.TryConsumeItem: Trying to consume {item.DisplayName}");
            if(CanConsumeItem(item))
            {
                Debug.Log($"MachineEntity.TryConsumeItem: Successfully consuming {item.DisplayName}");
                AddItem(item);
                return true;
            }
            Debug.Log($"MachineEntity.TryConsumeItem: Cannot consume {item.DisplayName} - already has item: {HasItem}");
            return false;
        }

        public void AddItem(Item item)
        {
            if(_currentItem != null)
            {
                Debug.LogWarning("MachineEntity.AddItem: already has an item, cannot add another.");
                return;
            }

            _currentItem = item;
            ItemAdded?.Invoke(item);
        }

        public void RemoveItem()
        {
            _currentItem = null;
            ItemRemoved?.Invoke();
        }

        public override void Rotate(Dictionary<Direction, Entity> _)
        {
            foreach(var direction in InputDirections)
            {
                Direction newInputDirection = DirectionUtils.GetRotatedDirection(direction);
                SetOrientation(newInputDirection, Direction.LEFT);
                Debug.Log($"MachineEntity.Rotate: Rotated | inputDirection={newInputDirection}");
            }
        }

        public void SetOrientation(Direction input, Direction output)
        {
            _inputDirections.Clear();
            //_outputDirections.Clear();

            _inputDirections.Add(input);
            //_outputDirections.Add(output);
        }
    }
}
