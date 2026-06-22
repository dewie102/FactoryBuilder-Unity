using System.Diagnostics;

using Unity.VisualScripting;

using static UnityEngine.InputManagerEntry;

namespace Assets.Scripts.Core
{
    public class ChainDetector
    {
        /// Algorithm Flow:
        /// 1. Find all IItemProducer entities that are NOT IChainableEntity
        /// 2. If they are connected to a conveyor, trace the chain forward to a consumer
        /// 3. Create a ConveyorChain object for each discovered sequence
    }
}
