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
}
