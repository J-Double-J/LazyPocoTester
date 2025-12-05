using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyPocoTester.Enums
{
    [Flags]
    public enum TestedDataMembers
    {
        Properties = 0,
        Fields = 1 << 0,
        PropertiesAndFields = Properties | Fields
    }
}
