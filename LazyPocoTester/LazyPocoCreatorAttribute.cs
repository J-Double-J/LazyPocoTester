using LazyPocoTester.Enums;
using System.Reflection;
using Xunit.Sdk;

namespace LazyPocoTester
{
    public class LazyPocoCreatorAttribute : DataAttribute
    {
        LazyPocoConfiguration _configuration;

        public LazyPocoCreatorAttribute()
        {
            _configuration = new LazyPocoConfiguration();
        }

        public LazyPocoCreatorAttribute(AccessibilityFlags accessibilityFlags = AccessibilityFlags.Public,
                                        TestedDataMembers testedDataMembers = TestedDataMembers.Properties,
                                        bool tryToCreateNonDefaultConstructors = false)
        {
            _configuration = new LazyPocoConfiguration(accessibilityFlags, testedDataMembers, tryToCreateNonDefaultConstructors);
        }

        /// <inheritdoc/>
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            LazyPOCOLocator locator = new LazyPOCOLocator();
         
            locator.LocateTestObjects(_configuration);
            _configuration.LocatedTypeInformation = locator.LocatedTypeInformation;

            foreach (object[] objArray in locator.GetNextTestableType()
                                                    .Select(type => new object[] { _configuration, type }) ?? [])
            {
                yield return objArray;
            }
        }

    }
}
