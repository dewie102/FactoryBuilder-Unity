using System.Collections.Generic;
using Assets.Scripts.Data.Entities;
using Assets.Scripts.Data.Items;
using Assets.Scripts.EntitySystem.Interfaces;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

namespace Assets.Scripts.EntitySystem.Resources
{
    public class ResourceNodeEntity : Entity, IItemProducer
    {
        private readonly HashSet<Direction> _outputDirection = new();
        private ItemData _itemToProduce;
        private Item _currentItem;

        public ResourceNodeEntity(EntityData data) : base(data)
        {
            _outputDirection.Add(Direction.RIGHT);
            _itemToProduce = data.itemToProduce;
        }

        public IEnumerable<Direction> OutputDirections => _outputDirection;

        public bool HasItem => _currentItem != null;

        public override void OnTick()
        {
            Debug.Log($"{this} is ticking.");
            if (_currentItem == null)
            {
                _currentItem = new Item(_itemToProduce);
                Debug.Log($"Produced item: {_currentItem.DisplayName}");
            }
        }

        public Item PeekItem()
        {
            return _currentItem;
        }

        public void RemoveItem()
        {
            _currentItem = null;
        }
    }
}
