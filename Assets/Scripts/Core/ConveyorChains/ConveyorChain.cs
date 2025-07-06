using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts.Core
{
    public class ConveyorChain
    {
        public List<Vector3Int> positions;
        public Dictionary<int, Item> items;
        public Direction inputDirection;
        public Direction outputDirection;

        public void AdvanceItems()
        {

        }
    }
}
