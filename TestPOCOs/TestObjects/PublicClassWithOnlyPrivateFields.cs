using LazyPocoTester;

namespace TestPOCOs.TestObjects
{
    [POCOTest]
    public class PublicClassWithOnlyPrivateFields
    {
#pragma warning disable CS0414
        private bool MyBool = false;

        private byte MyByte = 0;

        private sbyte MySByte = 0;

        // Added properties for types listed in the comment
        private char MyChar = '\0';

        private decimal MyDecimal = 0m;

        private double MyDouble = 0.0;

        private float MyFloat = 0f;

        private int MyInt = 0;

        private uint MyUInt = 0u;

        private nint MyNInt = 0;

        private nuint MyNUInt = 0;

        private long MyLong = 0L;

        private ulong MyULong = 0UL;

        private short MyShort = 0;

        private ushort MyUShort = 0;

        private string MyString = string.Empty;

        private DateTime MyDateTime = default;

        private DateTimeOffset MyDateTimeOffset = default;

        private TimeSpan MyTimeSpan = default;

        private Guid MyGuid = Guid.Empty;
#pragma warning restore CS0414
    }
}
