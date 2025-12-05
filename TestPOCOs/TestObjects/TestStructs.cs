using LazyPocoTester;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPOCOs.TestObjects
{
    [POCOTest]
    public struct PublicStructPublicProperties
    {
        public bool MyBool { get; set; }

        public byte MyByte { get; set; }

        public sbyte MySByte { get; set; }

        // Added properties for types listed in the comment
        public char MyChar { get; set; }

        public decimal MyDecimal { get; set; }

        public double MyDouble { get; set; }

        public float MyFloat { get; set; }

        public int MyInt { get; set; }

        public uint MyUInt { get; set; }

        public nint MyNInt { get; set; }

        public nuint MyNUInt { get; set; }

        public long MyLong { get; set; }

        public ulong MyULong { get; set; }

        public short MyShort { get; set; }

        public ushort MyUShort { get; set; }

        public string MyString { get; set; }

        public DateTime MyDateTime { get; set; }

        public DateTimeOffset MyDateTimeOffset { get; set; }

        public TimeSpan MyTimeSpan { get; set; }

        public Guid MyGuid { get; set; }
    }

    [POCOTest]
    public struct PublicStructPrivateProperties
    {
        private string MyString { get; set; }

        private int MyInt { get; set; }
    }

    [POCOTest]
    public struct PublicStructPublicFields
    {
        public string MyString;
        public int MyInt;
    }

    [POCOTest]
    public struct PublicStructPrivateFields
    {
        private string MyString;
        private int MyInt;
    }
}
