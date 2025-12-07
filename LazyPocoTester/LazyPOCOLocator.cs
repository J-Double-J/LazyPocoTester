using Microsoft.Extensions.DependencyModel;
using LazyPocoTester.Enums;
using System.Diagnostics;
using System.Reflection;

namespace LazyPocoTester
{
    internal class LazyPOCOLocator
    {
        private List<Type> _pocoTypes = new List<Type>();

        private static readonly ConstructorInfo StructCtorStandIn = typeof(RuntimeStructCtorPlaceholder).GetConstructor(Type.EmptyTypes)!;

        /// <summary>
        /// Pre-located type information with <see cref="Type.AssemblyQualifiedName"/> used as keys.
        /// </summary>
        internal Dictionary<string, LocatedTypeInformation> LocatedTypeInformation { get; set; } = new();

        internal IReadOnlyList<Type> LocatedTypes => _pocoTypes;

        internal void LocateTestObjects(LazyPocoConfiguration configuration)
        {
            DependencyContext? context = DependencyContext.Default;

            Debug.Assert(context != null);

            Assembly[] projectAssemblies = context.RuntimeLibraries
                                                  .Where(lib => lib.Type == "project")   // Only project libraries
                                                  .Select(lib => lib.Name)
                                                  .Select(Assembly.Load)                 // Load by simple name
                                                  .ToArray();


            foreach (Assembly assembly in projectAssemblies)
            {
                foreach (Type type in assembly.GetTypes()
                                              .Where(t => ValidClass(t) || ValidStruct(t))
                                              .Where(t => t.GetCustomAttribute<POCOTestAttribute>() is not null))
                {
#if DEBUG
                    string debugTypeName = type.Name; // Only used for pre-release testing.
#endif
                    if (!IsValidPOCOTypeForTesting(type, configuration, out LocatedTypeInformation locatedTypeInformation)) { continue; }

                    LocatedTypeInformation.Add(type.AssemblyQualifiedName!, locatedTypeInformation);
                    _pocoTypes.Add(type);
                }
            }
        }
        internal static bool ValidClass(Type t) => t.IsClass && !t.IsAbstract;
        internal static bool ValidStruct(Type t) => t.IsValueType && !t.IsPrimitive && !t.IsEnum;

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

            if (!HasValidConstructor(type, configuration, locatedTypeInformation))
            {
                // No parameterless constructor or allowable non-default constructor.
                return false;
            }

            // Check fields
            if ((configuration.TestedDataMembers & TestedDataMembers.Fields) == TestedDataMembers.Fields)
            {
                List<FieldInfo> testableFields = FindTestableFields(type, configuration, (BindingFlags)configuration.AccessibilityFlags | BindingFlags.Instance);

                locatedTypeInformation.Fields = testableFields;
            }

            // Check properties
            if ((configuration.TestedDataMembers & TestedDataMembers.Properties) == TestedDataMembers.Properties)
            {
                List<PropertyInfo> testableProperties = FindTestableProperties(type, (BindingFlags)configuration.AccessibilityFlags | BindingFlags.Instance);

                locatedTypeInformation.Properties = testableProperties;
            }

            if (ShouldClimbInheritanceTree(configuration, type, out Type? baseType))
            {
                RecursivelyClimbInheritanceTree_FindNonPublicMembers(baseType!, locatedTypeInformation, configuration);
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

            bool isStruct = ValidStruct(type);

            // If it is a struct we will skip this check so that we can see if there are any explicit ctors first.
            if (!isStruct && !configuration.TryToCreateNonDefaultConstructors)
            {
                return false;
            }

            ConstructorInfo[] constructors = type.GetConstructors((BindingFlags)configuration.AccessibilityFlags | BindingFlags.Instance);
            if (isStruct && constructors.Length == 0)
            {
                // The parameterless constructor is dynamically created for the struct and is not fetchable.
                locatedTypeInformation.UsuableConstructor = StructCtorStandIn;
                return true;
            }

            if ((configuration.TestedDataMembers & TestedDataMembers.Properties) == TestedDataMembers.Properties)
            {
                // Get the properties and filter out the EqualityContract auto-generated property for records
                properties = type.GetProperties((BindingFlags)configuration.AccessibilityFlags | BindingFlags.Instance)
                                 .Where(p => !(p.Name == "EqualityContract" && p.GetType().Name == "RuntimePropertyInfo")) 
                                 .ToArray();


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

            if ((configuration.TestedDataMembers & TestedDataMembers.Fields) == TestedDataMembers.Fields)
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

        /// <summary>
        /// Searches for fields that are testable. Filters through the results of fetching the fields given <paramref name="type"/> and <paramref name="bindingFlags"/>.
        /// </summary>
        /// <remarks>
        /// This does <em>not</em> use <paramref name="configuration"/>'s <see cref="LazyPocoConfiguration.AccessibilityFlags"/> for finding the initial fields.<br/>
        /// This is because when this method is called it may not have the same search parameters (depending on needed logic at time).<br/>
        /// If they do match, assign <paramref name="bindingFlags"/> the same value.
        /// </remarks>
        /// <param name="type">The type to scan through for testable fields.</param>
        /// <param name="configuration">Configuration given to the locator. Used to determine if backing fields need to be filtered out.</param>
        /// <param name="bindingFlags">The flags to use when searching for relevant fields.</param>
        /// <returns>A list of <see cref="FieldInfo"/> that are determined to be testable.</returns>
        private static List<FieldInfo> FindTestableFields(Type type, LazyPocoConfiguration configuration, BindingFlags bindingFlags)
        {
            FieldInfo[] testedFields = type.GetFields(bindingFlags);
            List<FieldInfo> testableFields = new(testedFields.Length);
            foreach (FieldInfo field in testedFields)
            {
                if ((configuration.TestedDataMembers & TestedDataMembers.NoBackingFields | TestedDataMembers.Properties) == (TestedDataMembers.NoBackingFields | TestedDataMembers.Properties))
                {
                    if (field.Name.EndsWith("k__BackingField"))
                    {
                        continue;
                    }
                }

                if (!POCOTester.DefaultSupportedTypes.Contains(field.FieldType))
                {
                    // Field type is not "primitive"
                    continue;
                }

                testableFields.Add(field);
            }

            return testableFields;
        }

        private static List<PropertyInfo> FindTestableProperties(Type type, BindingFlags bindingFlags)
        {
            PropertyInfo[] testedProperties = type.GetProperties(bindingFlags);
            List<PropertyInfo> testableProperties = new(testedProperties.Length);
            bool isValid;

            foreach (PropertyInfo prop in testedProperties)
            {
                isValid = true;

                if (!prop.CanRead || !prop.CanWrite)
                {
                    // Property is not read-write
                    isValid = false;
                }

                if (!POCOTester.DefaultSupportedTypes.Contains(prop.PropertyType))
                {
                    // Property type is not primitive
                    isValid = false;
                }

                if (isValid)
                {
                    testableProperties.Add(prop);
                }
            }

            return testableProperties;
        }

        private static bool ShouldClimbInheritanceTree(LazyPocoConfiguration configuration, Type currentType, out Type? baseType)
        {
            baseType = currentType.BaseType;

            return (configuration.AccessibilityFlags & AccessibilityFlags.NonPublic) == AccessibilityFlags.NonPublic &&
                    baseType != null &&
                    baseType != typeof(object);
        }

        /// <summary>
        /// This will climb the inheritance tree to find all non-public members (fields and/or properties depending on <paramref name="configuration"/>.
        /// </summary>
        /// <param name="type">A base type of another type that will be scanned and then climbed up from.</param>
        /// <param name="locatedTypeInformation">An array describing the current found member data.</param>
        /// <param name="configuration">Configuration describing how we will scan the types.</param>
        private void RecursivelyClimbInheritanceTree_FindNonPublicMembers(Type type, LocatedTypeInformation locatedTypeInformation, LazyPocoConfiguration configuration)
        {
            if ((configuration.TestedDataMembers & TestedDataMembers.Fields) == TestedDataMembers.Fields)
            {
                List<FieldInfo> testableFields = FindTestableFields(type, configuration, BindingFlags.NonPublic | BindingFlags.Instance);

                if (testableFields.Count > 0)
                {
                    locatedTypeInformation.Fields = [.. locatedTypeInformation.Fields.Concat(testableFields).DistinctBy(f => (f.DeclaringType, f.Name))];
                }
            }

            if ((configuration.TestedDataMembers & TestedDataMembers.Properties) == TestedDataMembers.Properties)
            {
                List<PropertyInfo> testableProperties = FindTestableProperties(type, BindingFlags.NonPublic | BindingFlags.Instance);

                if (testableProperties.Count > 0)
                {
                    locatedTypeInformation.Properties = [.. locatedTypeInformation.Properties.Concat(testableProperties).DistinctBy(p => (p.DeclaringType, p.Name))];
                }
            }

            if(ShouldClimbInheritanceTree(configuration, type, out Type? baseType))
            {
                RecursivelyClimbInheritanceTree_FindNonPublicMembers(baseType!, locatedTypeInformation, configuration);
            }
        }
    }
}
