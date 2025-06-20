using System.Collections.Generic;
using Assets.Scripts.Data.Entities;
using Assets.Scripts.EntitySystem.Interfaces;
using UnityEngine;

namespace Assets.Scripts.EntitySystem.Logistics
{
    public class ConveyorEntity : Entity, IItemConsumer, IItemProducer
    {
        private readonly HashSet<Direction> _inputDirection = new();
        private readonly HashSet<Direction> _outputDirection = new();

        private Item _currentItem;
        private GameObject _itemVisual;
        public ConveyorEntity(EntityData data) : base(data)
        {
            SetOrientation(Direction.LEFT, Direction.RIGHT);
        }

        public bool HasItem => _currentItem == null;

        public IEnumerable<Direction> InputDirections => _inputDirection;

        public IEnumerable<Direction> OutputDirections => _outputDirection;

        public override void OnTick()
        {
            Debug.Log($"{this} is ticking.");
        }

        public void RemoveItem()
        {
            _currentItem = null;
        }

        public Item PeekItem()
        {
            return _currentItem;
        }

        public bool TryConsumeItem(Item item)
        {
            if (_currentItem == null)
            {
                _currentItem = item;
                return true;
            }
            return false;
        }

        public void SetOrientation(Direction input, Direction output)
        {
            _inputDirection.Clear();
            _outputDirection.Clear();

            _inputDirection.Add(input);
            _outputDirection.Add(output);
        }
    }
}
