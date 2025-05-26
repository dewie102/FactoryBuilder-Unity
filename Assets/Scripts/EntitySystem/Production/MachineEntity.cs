using System;

using UnityEngine;

namespace Assets.Scripts.EntitySystem.Production
{
    internal class MachineEntity : Entity
    {
        public MachineEntity(EntityData data) : base(data)
        {
        }

        public override void OnTick()
        {
            Debug.Log($"{this} is ticking.");
        }
    }
}
