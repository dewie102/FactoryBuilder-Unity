using System;
using System.Collections.Generic;
using Assets.Scripts.Data.Entities;
using Assets.Scripts.EntitySystem.Interfaces;
using UnityEngine;

namespace Assets.Scripts.EntitySystem.Logistics
{
    public class ConveyorEntity : Entity, IItemConsumer, IItemProducer, IChainableEntity
    {
        private readonly HashSet<Direction> _inputDirections = new();
        private readonly HashSet<Direction> _outputDirections = new();

        private Item _currentItem;

        // Events
        public event Action<Item> ItemAdded;
        public event Action ItemRemoved;

        public ConveyorEntity(EntityData data) : base(data)
        {
            SetOrientation(Direction.LEFT, Direction.RIGHT);
        }

        public bool HasItem => _currentItem != null;

        public IEnumerable<Direction> InputDirections => _inputDirections;
        public IEnumerable<Direction> OutputDirections => _outputDirections;

        public override void OnTick()
        {
            Debug.Log($"{this} is ticking.");
        }

        public void RemoveItem()
        {
            _currentItem = null;
            ItemRemoved?.Invoke();
        }

        public Item PeekItem()
        {
            return _currentItem;
        }

        public bool TryConsumeItem(Item item)
        {
            Debug.Log($"ConveyorEntity: Trying to consume {item.DisplayName}");
            if(CanConsumeItem(item))
            {
                Debug.Log($"ConveyorEntity: Successfully consuming {item.DisplayName}");
                AddItem(item);
                return true;
            }
            Debug.Log($"ConveyorEntity: Cannot consume {item.DisplayName} - already has item: {HasItem}");
            return false;
        }

        public bool CanConsumeItem(Item item)
        {
            //return !HasItem;
            bool canConsume = !HasItem;
            Debug.Log($"ConveyorEntity.CanConsumeItem: HasItem={HasItem}, CanConsume={canConsume}");
            return canConsume;
        }

        public void AddItem(Item item)
        {
            if(_currentItem != null)
            {
                Debug.LogWarning("Conveyor already has an item, cannot add another.");
                return;
            }

            _currentItem = item;
            ItemAdded?.Invoke(item);
        }

        public void SetOrientation(Direction input, Direction output)
        {
            _inputDirections.Clear();
            _outputDirections.Clear();

            _inputDirections.Add(input);
            _outputDirections.Add(output);
        }
    }
}
