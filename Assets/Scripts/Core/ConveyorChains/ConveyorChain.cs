using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.EntitySystem;
using Assets.Scripts.EntitySystem.Interfaces;
using Assets.Scripts.EntitySystem.Logistics;

using UnityEngine;

namespace Assets.Scripts.Core
{
    public class ConveyorChain
    {
        public List<Vector3Int> Positions = new(); // Ordered Sequence
        public Vector3Int InputPosition => Positions[0];
        public Vector3Int OutputPosition => Positions[^1];

        public void AdvanceItems()
        {
            WorldManager worldManager = WorldManager.Instance;
            ConveyorEntity previousConveyor = null;
            for(int index = (Positions.Count - 1); index >= 0; index--)
            {
                if(worldManager.GetEntityAt(Positions[index]) is ConveyorEntity conveyor)
                {
                    if(conveyor.HasItem && previousConveyor != null)
                    {
                        if(!previousConveyor.HasItem)
                        {
                            if(previousConveyor.TryConsumeItem(conveyor.PeekItem()))
                            {
                                conveyor.RemoveItem();
                            }
                        }
                    }

                    previousConveyor = conveyor;
                }
            }

            // Pull item from the producer into the first conveyor
            if(Positions.Count > 0 && worldManager.GetEntityAt(Positions[0]) is ConveyorEntity firstConveyor && !firstConveyor.HasItem)
            {
                Entity producerEntity = worldManager.GetNeighborEntityInDirection(Positions[0], firstConveyor.InputDirections.First());

                if(producerEntity is IItemProducer producer && producerEntity is not IChainableEntity && producer.HasItem)
                {
                    if(firstConveyor.TryConsumeItem(producer.PeekItem()))
                        producer.RemoveItem();
                }
            }
        }
    }
}
