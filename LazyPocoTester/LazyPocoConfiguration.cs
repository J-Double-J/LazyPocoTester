using LazyPocoTester.Enums;

namespace LazyPocoTester
{
    public class LazyPocoConfiguration
    {
        public AccessibilityFlags AccessibilityFlags { get; internal set; } = AccessibilityFlags.Public;

        public TestedDataMembers TestedDataMembers { get; internal set; } = TestedDataMembers.Properties;

        /// <summary>
        /// If <see langword="true"/> tells the <see cref="POCOTestCoordinator"/> to try to find a constructor
        /// that only takes simple types
        /// </summary>
        public bool TryToCreateNonDefaultConstructors { get; internal set; } = false;

        /// <summary>
        /// Pre-located type information with <see cref="Type.AssemblyQualifiedName"/> used as keys.
        /// </summary>
        internal Dictionary<string, LocatedTypeInformation> LocatedTypeInformation { get; set; } = new();

        public LazyPocoConfiguration(AccessibilityFlags accessibilityFlags = AccessibilityFlags.Public,
                                     TestedDataMembers testedDataMembers = TestedDataMembers.Properties,
                                     bool tryToCreateNonDefaultConstructors = false)
        {
            AccessibilityFlags = accessibilityFlags;
            TestedDataMembers = testedDataMembers;
            TryToCreateNonDefaultConstructors = tryToCreateNonDefaultConstructors;
        }
    }
}
