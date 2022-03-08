using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manifold.IO
{
    internal interface IOffset
    {
        int AddressOffset { get; }
        bool IsNotNull { get; }
        bool IsNull { get; }
    }
}
