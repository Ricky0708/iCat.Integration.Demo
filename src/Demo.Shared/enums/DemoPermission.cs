using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Shared.enums
{
    [Flags]
    public enum DemoPermission
    {
        Add = 1,
        Update = 2,
        Delete = 4,
        Read = 8,
    }
}
