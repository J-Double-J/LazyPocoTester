namespace LazyPocoTester
{
    /// <summary>
    /// Structs that don't have explicit constructors don't show the parameterless constructor when fetched via reflection.<br/>
    /// This is a standin to generate a parameterless constructor so that it can still describe what types need what parameters (in this case none).
    /// </summary>
    public class RuntimeStructCtorPlaceholder
    {
        public RuntimeStructCtorPlaceholder()
        {
        }
    }
}
