using System.Collections.Generic;

namespace Assets.Scripts.Core
{
    public class ConveyorChainManager
    {
        private List<ConveyorChain> _chains = new();

        public void DetectChains()
        {
            // Find connected conveyor sequences
            // Start from conveyor ends and trace connections
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
            // Handle moving items betwwen connected chains
        }
    }
}
