using System;
using System.Collections.Generic;

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
            // Handle moving items between connected chains
        }
    }
}
