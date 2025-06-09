namespace Assets.Scripts.EntitySystem.Interfaces
{
    public interface IItemProducer
    {
        bool HasItem { get; }
        Item ProduceItem();
    }
}
