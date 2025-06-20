using System.Collections.Generic;

namespace Assets.Scripts.EntitySystem.Interfaces
{
    public interface IItemConsumer
    {
        IEnumerable<Direction> InputDirections { get; }
        bool TryConsumeItem(Item item);
    }
}
