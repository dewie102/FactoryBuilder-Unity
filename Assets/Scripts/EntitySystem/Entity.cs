using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.EntitySystem
{
    public class Entity
    {
        public EntityData Data { get; private set; }
        string Name { get; set; }

        public Entity(EntityData data)
        {
            Data = data;
        }
    }
}
