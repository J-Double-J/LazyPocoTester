using LazyPocoTester;

namespace UnitTests
{
    public class CoordinatorTests
    {
        [Fact]
        public void Testing()
        {
            Assert.True(true);
        }

        [Theory]
        //[InlineData(typeof(POCOTestObj))]
        [LazyPocoCreator(tryToCreateNonDefaultConstructors: true)]
        public void Test1(LazyPocoConfiguration configuration, Type type)
        {
            POCOTestCoordinator coordinator = new POCOTestCoordinator();

            coordinator.TestPOCO(configuration, type);
        }
    }
}