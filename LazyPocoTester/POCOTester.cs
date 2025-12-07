using System.Diagnostics;
using System.Reflection;
using Xunit;

namespace LazyPocoTester
{
    public class POCOTester
    {
        private static readonly HashSet<Type> _defaultSupportedTypes;
        
        internal static IReadOnlySet<Type> DefaultSupportedTypes => _defaultSupportedTypes;

        static POCOTester()
        {
            _defaultSupportedTypes = new HashSet<Type>
            {
                // Primitives or built in types
                typeof(bool),
                typeof(byte),
                typeof(sbyte),
                typeof(char),
                typeof(decimal),
                typeof(double),
                typeof(float),
                typeof(int),
                typeof(uint),
                typeof(nint),
                typeof(nuint),
                typeof(long),
                typeof(ulong),
                typeof(short),
                typeof(ushort),
                typeof(string),

                // Common structs
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid),
            };
        }

        public POCOTester()
        {
        }

        public void TestPOCO(LazyPocoConfiguration configuration, Type type)
        {
            if (!configuration.LocatedTypeInformation.TryGetValue(type.AssemblyQualifiedName!, out LocatedTypeInformation? information))
            {
                throw new InvalidOperationException($"{nameof(POCOTester)} expected type information to be located and cached and is not able to find it in the {nameof(configuration)}.");
            }

            object obj = CreateInstance(information!, type);

            TestProperties(information, type, obj);
            TestFields(information, type, obj);
        }

        private static void TestProperties(LocatedTypeInformation information, Type type, object obj)
        {
            foreach (PropertyInfo prop in information.Properties)
            {
                // Set the value to a default value.
                prop.SetValue(obj, CreateDefaultForValue(prop.PropertyType));

                object testValue = TestValueForType(prop.PropertyType);

                // Set the value to the test value.
                prop.SetValue(obj, testValue);

                object propValue = prop.GetValue(obj)!;

                Assert.True(Equals(testValue, propValue), $"Property {type.FullName}.{prop.Name} did not retain the expected value.");
            }
        }
        private static void TestFields(LocatedTypeInformation information, Type type, object obj)
        {
            foreach (FieldInfo field in information.Fields)
            {
                // Set the value to a default value.
                field.SetValue(obj, CreateDefaultForValue(field.FieldType));

                object testValue = TestValueForType(field.FieldType);

                // Set the value to the test value.
                field.SetValue(obj, testValue);

                object propValue = field.GetValue(obj)!;

                Assert.True(Equals(testValue, propValue), $"Property {type.FullName}.{field.Name} did not retain the expected value.");
            }
        }

        private static object CreateInstance(LocatedTypeInformation typeInformation, Type type)
        {
            Debug.Assert(typeInformation.UsuableConstructor != null);

            try
            {
                ParameterInfo[] ctorParams = typeInformation.UsuableConstructor!.GetParameters();

                // Parameter-less constructor?
                if (ctorParams.Length == 0)
                {
                    return Activator.CreateInstance(type)!;
                }

                object?[] defaultValues = new object?[ctorParams.Length];
                for (int i = 0; i < ctorParams.Length; i++)
                {
                    defaultValues[i] = CreateDefaultForValue(ctorParams[i].ParameterType);
                }

                return typeInformation.UsuableConstructor.Invoke(defaultValues);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"{nameof(POCOTester)} was unable to create an instance of {type.FullName}", ex);
            }
        }

        private static object? CreateDefaultForValue(Type type)
        {
            try
            {
                if (type == typeof(string))
                {
                    return string.Empty;
                }
                else
                {
                    return Activator.CreateInstance(type);
                }
            }
            catch
            {
                return null;
            }
            
        }

        private static object TestValueForType(Type type)
        {
            return type switch
            {
                // Primitives or built in types
                Type t when t == typeof(bool) => true,
                Type t when t == typeof(byte) => byte.MaxValue,
                Type t when t == typeof(sbyte) => sbyte.MaxValue,
                Type t when t == typeof(char) => char.MaxValue,
                Type t when t == typeof(decimal) => decimal.MaxValue,
                Type t when t == typeof(double) => double.MaxValue,
                Type t when t == typeof(float) => float.MaxValue,
                Type t when t == typeof(int) => int.MaxValue,
                Type t when t == typeof(uint) => uint.MaxValue,
                Type t when t == typeof(nint) => nint.MaxValue,
                Type t when t == typeof(nuint) => nuint.MaxValue,
                Type t when t == typeof(long) => long.MaxValue,
                Type t when t == typeof(ulong) => ulong.MaxValue,
                Type t when t == typeof(short) => short.MaxValue,
                Type t when t == typeof(ushort) => ushort.MaxValue,
                Type t when t == typeof(string) => "LazyTestString",

                // Common structs
                Type t when t == typeof(DateTime) => DateTime.MaxValue,
                Type t when t == typeof(DateTimeOffset) => DateTimeOffset.MaxValue,
                Type t when t == typeof(TimeSpan) => TimeSpan.MaxValue,
                Type t when t == typeof(Guid) => Guid.Parse("275034d1-1aba-42e5-8ac2-44f3c858ee76"),

                _ => throw new NotImplementedException($"Test value generation for type {type.FullName} is not implemented."),
            };
        }
    }
}
