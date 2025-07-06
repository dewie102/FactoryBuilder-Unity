using System;
using System.Collections.Generic;

namespace Assets.Scripts.EntitySystem.Interfaces
{
    public interface IItemConsumer
    {
        IEnumerable<Direction> InputDirections { get; }
        bool TryConsumeItem(Item item);
        bool CanConsumeItem(Item item);

        // Events for visual updates
        event Action<Item> ItemAdded;
        event Action ItemRemoved;
    }
}
