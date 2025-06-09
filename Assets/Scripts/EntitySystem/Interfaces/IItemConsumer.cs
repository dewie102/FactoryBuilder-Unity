namespace Assets.Scripts.EntitySystem.Interfaces
{
    public interface IItemConsumer
    {
        bool TryConsumeItem(Item item);
    }
}
