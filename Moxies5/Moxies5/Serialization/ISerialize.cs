using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moxies5.Serialization
{
    interface ISerialize
    {
        SaveObject Serialize(int ID);
    }
}
