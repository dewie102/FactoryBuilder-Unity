using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.EntitySystem.Interfaces;

using UnityEngine;

namespace Assets.Scripts.Core
{
    public class ConveyorChainManager
    {
        public static event Action<List<ConveyorChain>> ChainsDetected;

        private ChainDetector _detector = new();
        private List<ConveyorChain> _chains = new();

        public void DetectChains()
        {
            _chains = _detector.DetectChains();
            ChainsDetected?.Invoke(_chains);
        }

        public void ProcessAllChains()
        {
            foreach(var chain in _chains)
            {
                chain.AdvanceItems();
            }
        }

        public void HandleChainConnections()
        {
            foreach(var chain in _chains)
            {
                if(WorldManager.Instance.GetEntityAt(chain.OutputPosition) is IItemProducer conveyor)
                {
                    Vector3Int outputDirectionPosition = chain.OutputPosition + DirectionUtils.ToVector3Int(conveyor.OutputDirections.First());
                    if(WorldManager.Instance.GetEntityAt(outputDirectionPosition) is IItemConsumer consumer && consumer is not IChainableEntity)
                    {
                        if(conveyor.PeekItem() != null && consumer.TryConsumeItem(conveyor.PeekItem()))
                            conveyor.RemoveItem();

                    }
                }
            }
        }
    }
}
