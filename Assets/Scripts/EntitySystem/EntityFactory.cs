using Assets.Scripts.Data.Entities;
using Assets.Scripts.EntitySystem.Logistics;
using Assets.Scripts.EntitySystem.Production;
using Assets.Scripts.EntitySystem.Resources;

namespace Assets.Scripts.EntitySystem
{
    public static class EntityFactory
    {
        public static Entity CreateEntity(EntityData data)
        {
            return data.type switch
            {
                EntityType.Conveyor => new ConveyorEntity(data),
                EntityType.Machine => new MachineEntity(data),
                EntityType.ResourceNode => new ResourceNodeEntity(data),
                _ => new Entity(data)
            };
        }
    }
}
