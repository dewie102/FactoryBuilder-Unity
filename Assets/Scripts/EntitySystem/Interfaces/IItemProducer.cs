using System;
using System.Collections.Generic;

namespace Assets.Scripts.EntitySystem.Interfaces
{
    public interface IItemProducer
    {
        IEnumerable<Direction> OutputDirections { get; }
        bool HasItem { get; }
        void RemoveItem();
        Item PeekItem();

        // Events for visual updates
        event Action<Item> ItemAdded;
        event Action ItemRemoved;
    }
}
