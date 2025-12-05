using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyPocoTester.Enums
{
    [Flags]
    public enum AccessibilityFlags
    {
        Public = 1 << 4,
        NonPublic = 1 << 5,
        PublicAndNonPublic = Public | NonPublic
    }
}
