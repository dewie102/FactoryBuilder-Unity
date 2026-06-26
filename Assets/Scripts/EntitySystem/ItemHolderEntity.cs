using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assets.Scripts.Data.Entities;

using UnityEngine;

namespace Assets.Scripts.EntitySystem
{
    public abstract class ItemHolderEntity : Entity
    {
        private Item _currentItem;

        public ItemHolderEntity(EntityData data) : base(data)
        {
        }

        // Events
        public event Action<Item> ItemAdded;
        public event Action ItemRemoved;
        public event Action DirectionsChanged;

        public bool HasItem => _currentItem != null;

        public void AddItem(Item item)
        {
            if(_currentItem != null)
            {
                Debug.LogWarning($"ItemHolderEntity {Name} already has an item, cannot add another.");
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

        public Item PeekItem()
        {
            return _currentItem;
        }

        public bool TryConsumeItem(Item item)
        {
            Debug.Log($"ItemHolderEntity {Name}: Trying to consume {item.DisplayName}");
            if(CanConsumeItem(item))
            {
                Debug.Log($"ItemHolderEntity {Name}: Successfully consuming {item.DisplayName}");
                AddItem(item);
                return true;
            }
            Debug.Log($"ItemHolderEntity {Name}: Cannot consume {item.DisplayName} - already has item: {HasItem}");
            return false;
        }

        public abstract bool CanConsumeItem(Item item);

        public virtual void SetInputDirection(Direction direction) { }
        public virtual void SetOutputDirection(Direction direction) { }

        protected void NotifyDirectionsChanged() => DirectionsChanged?.Invoke();
    }
}
