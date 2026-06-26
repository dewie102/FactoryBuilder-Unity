using System.Linq;
using System.Collections.Generic;
using Assets.Scripts.EntitySystem;
using Assets.Scripts.EntitySystem.Interfaces;

using UnityEngine;


namespace Assets.Scripts.Core
{
    public class ChainDetector
    {
        /// Algorithm Flow:
        /// 1. Find all IItemProducer entities that are NOT IChainableEntity
        /// 2. If they are connected to a conveyor, trace the chain forward to a consumer
        /// 3. Create a ConveyorChain object for each discovered sequence
        public List<ConveyorChain> DetectChains()
        {
            List<ConveyorChain> found = new();

            IEnumerable<KeyValuePair<Vector3Int, Entity>> allEntities = WorldManager.Instance.GetAllEntities();
            foreach(var entityWithPosition in allEntities)
            {
                if(entityWithPosition.Value is IItemProducer producer and not IChainableEntity)
                {
                    foreach(Direction direction in producer.OutputDirections)
                    {
                        ConveyorChain currentChain = FollowChain(KeyValuePair.Create(entityWithPosition.Key, producer), direction);
                        if(currentChain != null && currentChain.Positions.Count > 0)
                        {
                            found.Add(currentChain);
                        }
                    }
                }
            }

            return found;
        }

        private ConveyorChain FollowChain(KeyValuePair<Vector3Int, IItemProducer> startProducer, Direction outputDirection)
        {
            ConveyorChain chain = new();

            Direction requiredInputDirection = DirectionUtils.Reverse(outputDirection);
            Vector3Int nextPosition = startProducer.Key + DirectionUtils.ToVector3Int(outputDirection);
            Entity nextEntity = WorldManager.Instance.GetEntityAt(nextPosition);

            while(nextEntity is IChainableEntity && nextEntity is IItemProducer conveyor)
            {
                if(conveyor is not IItemConsumer consumer || !consumer.InputDirections.Contains(requiredInputDirection))
                {
                    break;
                }

                chain.Positions.Add(nextPosition);
                Direction nextEntityOutputDirection = conveyor.OutputDirections.First();
                requiredInputDirection = DirectionUtils.Reverse(nextEntityOutputDirection);
                nextPosition = WorldManager.Instance.GetNeighborPositionInDirection(nextPosition, nextEntityOutputDirection);
                nextEntity = WorldManager.Instance.GetEntityAt(nextPosition);
            }

            return chain;
        }
    }
}
