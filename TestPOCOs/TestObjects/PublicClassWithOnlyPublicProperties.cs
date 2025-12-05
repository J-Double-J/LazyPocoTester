namespace LazyPocoTester
{
    [POCOTest]
    public class PublicClassWithOnlyPublicProperties
    {
        public bool MyBool { get; set; } = false;

        public byte MyByte { get; set; } = 0;

        public sbyte MySByte { get; set; } = 0;

        // Added properties for types listed in the comment
        public char MyChar { get; set; } = '\0';

        public decimal MyDecimal { get; set; } = 0m;

        public double MyDouble { get; set; } = 0.0;

        public float MyFloat { get; set; } = 0f;

        public int MyInt { get; set; } = 0;

        public uint MyUInt { get; set; } = 0u;

        public nint MyNInt { get; set; } = 0;

        public nuint MyNUInt { get; set; } = 0;

        public long MyLong { get; set; } = 0L;

        public ulong MyULong { get; set; } = 0UL;

        public short MyShort { get; set; } = 0;

        public ushort MyUShort { get; set; } = 0;

        public string MyString { get; set; } = string.Empty;

        public DateTime MyDateTime { get; set; } = default;

        public DateTimeOffset MyDateTimeOffset { get; set; } = default;

        public TimeSpan MyTimeSpan { get; set; } = default;

        public Guid MyGuid { get; set; } = Guid.Empty;
    }
}
