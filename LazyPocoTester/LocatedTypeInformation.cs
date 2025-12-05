using System.Reflection;

namespace LazyPocoTester
{
    /// <summary>
    /// This is a type that will hold metadata type information about located type information to minimize reflection lookup operations
    /// </summary>
    internal class LocatedTypeInformation
    {
        /// <summary>
        /// Gets the properties that needs to be tested.
        /// </summary>
        internal PropertyInfo[] Properties { get; set; } = [];

        /// <summary>
        /// Gets the fields that need to be tested.
        /// </summary>
        internal FieldInfo[] Fields { get; set; } = [];

        /// <summary>
        /// The identified constructor that should be used for creating a type.<br/>
        /// This is the default or primary constructor.
        /// </summary>
        internal ConstructorInfo? UsuableConstructor { get; set; }
    }
}
