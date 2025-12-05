namespace LazyPocoTester
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class POCOTestAttribute : Attribute
    {
        public POCOTestAttribute()
        {
        }
    }
}
