using LazyPocoTester;

namespace TestPOCOs.TestObjects
{
    [POCOTest]
    public class PublicClassWithAComputedProperty
    {
        public int Length { get; set; }
        public int Width { get; set; }
        public int Area => Length * Width;

        public PublicClassWithAComputedProperty(int length, int width)
        {
            Length = length;
            Width = width;
        }
    }

    public abstract class AbstractPublicNonPOCOClass
    {
        public string MyBaseString { get; set; } = "Placeholder";
    }

    [POCOTest]
    public class PublicPOCODerivedClass : AbstractPublicNonPOCOClass
    {
        public int MyDerivedInt { get; set; }
    }

    public abstract class AbstractNonPOCOClassWithMixedAccessibilityFieldsAndProperties
    {
        public double MyBaseDouble;

        protected int MyBaseProctedField;

        private long MyBaseLong;

        public string MyBaseString { get; set; } = "Placeholder";

        protected int MyBaseInt { get; set; }

        private Guid MyBaseGuid { get; set; } = Guid.Empty;

        protected AbstractNonPOCOClassWithMixedAccessibilityFieldsAndProperties(int val)
        {
            MyBaseInt = val;
        }
    }

    [POCOTest]
    public class DerivedClassOfMixedBaseClassNoDefaultCtor : AbstractNonPOCOClassWithMixedAccessibilityFieldsAndProperties
    {
        private int MyDerivedInt;
        private decimal MyDerivedDecimal { get; set; }
        public string MyDerivedString { get; set; } = string.Empty;

        public DerivedClassOfMixedBaseClassNoDefaultCtor(decimal myDerivedDecimal) : base(42)
        {
            MyDerivedDecimal = myDerivedDecimal;
        }
    }

    [POCOTest]
    public class TwoDeepDerivedClassOfMixedBaseClasses : DerivedClassOfMixedBaseClassNoDefaultCtor
    {
        protected string MySecondDerivedString = string.Empty;
        public int MySecondDerivedIntProperty { get; set; }

        public TwoDeepDerivedClassOfMixedBaseClasses() : base(decimal.Zero)
        {
        }
    }
}