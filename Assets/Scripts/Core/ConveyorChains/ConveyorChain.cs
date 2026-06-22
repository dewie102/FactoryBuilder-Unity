using System.Collections.Generic;

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

        }
    }
}
