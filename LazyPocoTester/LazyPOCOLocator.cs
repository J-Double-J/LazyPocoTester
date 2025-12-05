using LazyPocoTester.Enums;
using System.Diagnostics;
using System.Reflection;

namespace LazyPocoTester
{
    internal class LazyPOCOLocator
    {
        private List<Type> _pocoTypes = new List<Type>();

        private static string[] systemLibrarysPrefixes = new string[2]
        {
            "System",
            "Microsoft"
        };

        /// <summary>
        /// Pre-located type information with <see cref="Type.AssemblyQualifiedName"/> used as keys.
        /// </summary>
        internal Dictionary<string, LocatedTypeInformation> LocatedTypeInformation { get; set; } = new();

        internal IReadOnlyList<Type> LocatedTypes => _pocoTypes;

        internal void LocateTestObjects(LazyPocoConfiguration configuration)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => AssemblyIsNotSytemLibraries(a))
                .Where(a => a.Location.Length > 0)  // Non-dynamic assemblies and those that reside in BIN folder
                .ToArray();

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes()
                                              .Where(t => t.IsClass && !t.IsAbstract && !t.IsNestedPrivate)
                                              .Where(t => t.GetCustomAttribute<POCOTestAttribute>() is not null))
                {
                    if (!IsValidPOCOTypeForTesting(type, configuration, out LocatedTypeInformation locatedTypeInformation)) { continue; }

                    LocatedTypeInformation.Add(type.AssemblyQualifiedName!, locatedTypeInformation);
                    _pocoTypes.Add(type);
                }
            }
        }

        private bool AssemblyIsNotSytemLibraries(Assembly assembly)
        {
            Debug.Assert(assembly.FullName != null);

            foreach (string prefix in systemLibrarysPrefixes)
            {
                if(assembly.FullName!.StartsWith(prefix))
                {
                    return false;
                }
            }

            return true;
        }

        internal IEnumerable<Type> GetNextTestableType()
        {
            foreach (Type type in _pocoTypes)
            {
                yield return type;
            }
        }

        private bool IsValidPOCOTypeForTesting(Type type, LazyPocoConfiguration configuration, out LocatedTypeInformation locatedTypeInformation)
        {
            locatedTypeInformation = new();

            if (type.IsAbstract || type.IsInterface || type.BaseType != typeof(object))
            {
                // Not a concrete base class
                return false;
            }


            if (!HasValidConstructor(type, configuration, locatedTypeInformation))
            {
                // No parameterless constructor
                return false;
            }

            // Check fields
            if ((configuration.TestedDataMembers & TestedDataMembers.Fields) == TestedDataMembers.Fields)
            {
                FieldInfo[] testedFields = type.GetFields((BindingFlags)configuration.AccessibilityFlags | BindingFlags.Instance);
                foreach (FieldInfo field in testedFields)
                {
                    if (!POCOTestCoordinator.DefaultSupportedTypes.Contains(field.FieldType))
                    {
                        // Field type is not primitive
                        return false;
                    }
                }

                locatedTypeInformation.Fields = testedFields;
            }

            // Check properties
            if ((configuration.TestedDataMembers & TestedDataMembers.Properties) == TestedDataMembers.Properties)
            {
                PropertyInfo[] testedProperties = type.GetProperties((BindingFlags)configuration.AccessibilityFlags | BindingFlags.Instance);
                foreach (PropertyInfo prop in testedProperties)
                {
                    if (!prop.CanRead || !prop.CanWrite)
                    {
                        // Property is not read-write
                        return false;
                    }

                    if (!POCOTestCoordinator.DefaultSupportedTypes.Contains(prop.PropertyType))
                    {
                        // Property type is not primitive
                        return false;
                    }
                }

                locatedTypeInformation.Properties = testedProperties;
            }

            return true;
        }

        private static bool HasValidConstructor(Type type, LazyPocoConfiguration configuration, LocatedTypeInformation locatedTypeInformation)
        {
            Debug.Assert(locatedTypeInformation != null, "LocatedTypeInformation should be preconstructed");

            PropertyInfo[] properties = Array.Empty<PropertyInfo>();
            FieldInfo[] fields;

            // We search for a public parameterless constructor or non-public if accessibility flags allow.
            ConstructorInfo? parameterlessConstructor = type.GetConstructor((BindingFlags)configuration.AccessibilityFlags | BindingFlags.Public | BindingFlags.Instance,
                                                                            Type.EmptyTypes);
            if (parameterlessConstructor != null)
            {
                // Parameterless ctor found
                locatedTypeInformation.UsuableConstructor = parameterlessConstructor;
                return true;
            }

            if(!configuration.TryToCreateNonDefaultConstructors)
            {
                return false;
            }

            ConstructorInfo[] constructors = type.GetConstructors((BindingFlags)configuration.AccessibilityFlags | BindingFlags.Instance);

            if ((configuration.TestedDataMembers & TestedDataMembers.Properties) == TestedDataMembers.Properties)
            {
                properties = type.GetProperties((BindingFlags)configuration.AccessibilityFlags | BindingFlags.Instance);

                foreach (ConstructorInfo constructor in constructors)
                {
                    // Ensure each paramater matches up with a property on the object
                    if (constructor.GetParameters()
                                   .All(param => properties.Any(prop => string.Equals(prop.Name, param.Name, StringComparison.OrdinalIgnoreCase))))
                    {
                        locatedTypeInformation.UsuableConstructor = constructor;
                        return true;
                    }
                }
            }

            if((configuration.TestedDataMembers & TestedDataMembers.Fields) == TestedDataMembers.Fields)
            {
                fields = type.GetFields((BindingFlags)configuration.AccessibilityFlags | BindingFlags.Instance);

                foreach (ConstructorInfo constructor in constructors)
                {
                    // Ensure each paramater matches up with a field on the object
                    if (constructor.GetParameters()
                                   .All(param => fields.Any(field => string.Equals(field.Name, param.Name, StringComparison.OrdinalIgnoreCase))))
                    {
                        locatedTypeInformation.UsuableConstructor = constructor;
                        return true;
                    }
                }
            }

            

            return false;
        }
    }
}
