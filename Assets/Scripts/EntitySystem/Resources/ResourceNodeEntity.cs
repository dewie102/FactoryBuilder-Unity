using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Data.Entities;
using Assets.Scripts.Data.Items;
using Assets.Scripts.EntitySystem.Interfaces;
using UnityEngine;

namespace Assets.Scripts.EntitySystem.Resources
{
    public class ResourceNodeEntity : ItemHolderEntity, IItemProducer
    {
        private readonly HashSet<Direction> _outputDirections = new();
        private ItemData _itemToProduce;

        public ResourceNodeEntity(EntityData data) : base(data)
        {
            _outputDirections.Add(Direction.RIGHT);
            _itemToProduce = data.itemToProduce;
        }

        public IEnumerable<Direction> OutputDirections => _outputDirections;

        public override void OnTick()
        {
            Debug.Log($"ResourceNodeEntity: Ticking - HasItem: {HasItem}");
            if(!HasItem)
            {
                Item producedItem = new Item(_itemToProduce);
                AddItem(producedItem);
                Debug.Log($"ResourceNodeEntity: Produced {producedItem.DisplayName}");
            }
            else
            {
                Debug.Log($"ResourceNodeEntity: Already has item: {PeekItem().DisplayName}");
            }
        }

        public override bool CanConsumeItem(Item item)
        {
            return false;
        }

        public override void Rotate(Dictionary<Direction, Entity> _)
        {
            Direction newOutputDirection = DirectionUtils.GetRotatedDirection(OutputDirections.First());
            SetOrientation(newOutputDirection);
            Debug.Log($"ResourceNodeEntity.Rotate: Rotated | outputDirection={newOutputDirection}");
        }

        public void SetOrientation(Direction output)
        {
            _outputDirections.Clear();

            _outputDirections.Add(output);

            NotifyDirectionsChanged();
        }

        public override void SetOutputDirection(Direction direction)
        {
            SetOrientation(direction);
        }
    }
}
