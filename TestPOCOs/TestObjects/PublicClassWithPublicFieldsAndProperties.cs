using LazyPocoTester;

namespace TestPOCOs.TestObjects
{
    [POCOTest]
    public class PublicClassWithPublicFieldsAndProperties
    {
        public string MyStringField = "Hello";

        public int MyIntField = 1;

        public DateTime MyDateTimeProperty { get; set; } = DateTime.Now;

        public double MyDoubleProperty { get; set; } = 0.2;
    }
}
