using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.EntitySystem.Interfaces
{
    public interface IChainableEntity
    {
        Direction InputDirection { get; }
        Direction OutputDirection { get; }
        bool CanChainWith();
    }
}
