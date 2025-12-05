using LazyPocoTester;

namespace TestPOCOs.TestObjects
{
    [POCOTest]
    public class PublicClassWithOnlyPublicFields
    {
        public bool MyBool = false;

        public byte MyByte = 0;

        public sbyte MySByte = 0;

        // Added properties for types listed in the comment
        public char MyChar = '\0';

        public decimal MyDecimal = 0m;

        public double MyDouble = 0.0;

        public float MyFloat = 0f;

        public int MyInt = 0;

        public uint MyUInt = 0u;

        public nint MyNInt = 0;

        public nuint MyNUInt = 0;

        public long MyLong = 0L;

        public ulong MyULong = 0UL;

        public short MyShort = 0;

        public ushort MyUShort = 0;

        public string MyString = string.Empty;

        public DateTime MyDateTime = default;

        public DateTimeOffset MyDateTimeOffset = default;

        public TimeSpan MyTimeSpan = default;

        public Guid MyGuid = Guid.Empty;
    }
}
