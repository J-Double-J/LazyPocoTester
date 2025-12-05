using LazyPocoTester;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPOCOs.TestObjects
{
    [POCOTest]
    public class PublicClassWithOnlyPrivateProperties
    {
        private bool MyBool { get; set; } = false;

        private byte MyByte { get; set; } = 0;

        private sbyte MySByte { get; set; } = 0;

        // Added properties for types listed in the comment
        private char MyChar { get; set; } = '\0';

        private decimal MyDecimal { get; set; } = 0m;

        private double MyDouble { get; set; } = 0.0;

        private float MyFloat { get; set; } = 0f;

        private int MyInt { get; set; } = 0;

        private uint MyUInt { get; set; } = 0u;

        private nint MyNInt { get; set; } = 0;

        private nuint MyNUInt { get; set; } = 0;

        private long MyLong { get; set; } = 0L;

        private ulong MyULong { get; set; } = 0UL;

        private short MyShort { get; set; } = 0;

        private ushort MyUShort { get; set; } = 0;

        private string MyString { get; set; } = string.Empty;

        private DateTime MyDateTime { get; set; } = default;

        private DateTimeOffset MyDateTimeOffset { get; set; } = default;

        private TimeSpan MyTimeSpan { get; set; } = default;

        private Guid MyGuid { get; set; } = Guid.Empty;
    }
}
