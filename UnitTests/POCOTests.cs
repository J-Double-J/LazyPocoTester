using LazyPocoTester.Enums;

namespace LazyPocoTester.UnitTests
{
    public class POCOTests
    {
        [Theory]
        [LazyPocoCreator(AccessibilityFlags.PublicAndNonPublic, TestedDataMembers.PropertiesAndFieldsNoBacking, tryToCreateNonDefaultConstructors: true)]
        public void TestAllPOCOs(LazyPocoConfiguration configuration, Type type)
        {
            POCOTester coordinator = new POCOTester();

            coordinator.TestPOCO(configuration, type);
        }
    }
}