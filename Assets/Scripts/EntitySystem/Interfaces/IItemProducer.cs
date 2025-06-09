using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets.Scripts.EntitySystem.Interfaces
{
    public interface IItemProducer
    {
        bool HasItem { get; }
        //Item ProduceItem();
    }
}
