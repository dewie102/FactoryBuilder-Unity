using System.Diagnostics;

using Unity.VisualScripting;

using static UnityEngine.InputManagerEntry;

namespace Assets.Scripts.Core
{
    public class ChainDetector
    {
        /// Algorithm Flow:
        /// 1. Find all "chain starts" (conveyors with no input OR input from non-conveyor)
        /// 2. Trace forward following output directions until chain ends
        /// 3. Create ConveyorChain objects for each discovered sequence
    }
}
