using System;
using System.Collections.Generic;
using Assets.Scripts.Data.Entities;
using Assets.Scripts.Data.Items;
using Assets.Scripts.EntitySystem.Interfaces;
using UnityEngine;

namespace Assets.Scripts.EntitySystem.Resources
{
    public class ResourceNodeEntity : Entity, IItemProducer
    {
        private readonly HashSet<Direction> _outputDirections = new();
        private ItemData _itemToProduce;
        private Item _currentItem;

        // Events
        public event Action<Item> ItemAdded;
        public event Action ItemRemoved;

        public ResourceNodeEntity(EntityData data) : base(data)
        {
            _outputDirections.Add(Direction.RIGHT);
            _itemToProduce = data.itemToProduce;
        }

        public IEnumerable<Direction> OutputDirections => _outputDirections;

        public bool HasItem => _currentItem != null;

        public override void OnTick()
        {
            Debug.Log($"ResourceNodeEntity: Ticking - HasItem: {HasItem}");
            if(_currentItem == null)
            {
                _currentItem = new Item(_itemToProduce);
                ItemAdded?.Invoke(_currentItem);
                Debug.Log($"ResourceNodeEntity: Produced {_currentItem.DisplayName}");
            }
            else
            {
                Debug.Log($"ResourceNodeEntity: Already has item: {_currentItem.DisplayName}");
            }
        }

        public Item PeekItem()
        {
            return _currentItem;
        }

        public void RemoveItem()
        {
            _currentItem = null;
            ItemRemoved?.Invoke();
        }

        public override void Rotate(Dictionary<Direction, Entity> _)
        {
            foreach(var direction in OutputDirections)
            {
                Direction newOutputDirection = DirectionUtils.GetRotatedDirection(direction);
                SetOrientation(newOutputDirection);
                Debug.Log($"ResourceNodeEntity.Rotate: Rotated | outputDirection={newOutputDirection}");
            }
        }

        public void SetOrientation(Direction output)
        {
            _outputDirections.Clear();

            _outputDirections.Add(output);
        }
    }
}
