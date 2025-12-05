using FluentAssertions;
using LazyPocoTester;
using LazyPocoTester.Enums;
using System.Reflection;
using TestPOCOs.TestObjects;

namespace UnitTests
{
    public class LazyPocoLocatorTests
    {
        /// <summary>
        /// Shorthand for Public + NonPublic Instance binding flags.
        /// </summary>
        private const BindingFlags PubNonPubInstanceFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        [Fact]
        public void Locator_CanFindClass_WithPublicProperties()
        {
            LazyPOCOLocator locator = new();
            Type testedType = typeof(PublicClassWithOnlyPublicProperties);

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.Public, TestedDataMembers.Properties, false);
            locator.LocateTestObjects(lazyPocoConfiguration);
            PropertyInfo[] locatedProperties = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Properties;

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);
            locatedProperties.Should().BeEquivalentTo(testedType.GetProperties(BindingFlags.Instance | BindingFlags.Public));
        }

        [Fact]
        public void Locator_CanFindClass_WithPrivateProperties()
        {
            LazyPOCOLocator locator = new();
            Type testedType = typeof(PublicClassWithOnlyPrivateProperties);

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.NonPublic, TestedDataMembers.Properties, false);
            locator.LocateTestObjects(lazyPocoConfiguration);
            PropertyInfo[] locatedProperties = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Properties;

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);
            locatedProperties.Should().BeEquivalentTo(testedType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic));
        }

        [Fact]
        public void Locator_CanFindClass_WithPublicFields()
        {
            LazyPOCOLocator locator = new();
            Type testedType = typeof(PublicClassWithOnlyPublicFields);

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.Public, TestedDataMembers.Fields, false);
            locator.LocateTestObjects(lazyPocoConfiguration);
            FieldInfo[] locatedFields = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Fields;

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);
            locatedFields.Should().BeEquivalentTo(testedType.GetFields(BindingFlags.Instance | BindingFlags.Public));
        }

        [Fact]
        public void Locator_CanFindClass_WithPrivateFields()
        {
            LazyPOCOLocator locator = new();
            Type testedType = typeof(PublicClassWithOnlyPrivateFields);

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.NonPublic, TestedDataMembers.Fields, false);
            locator.LocateTestObjects(lazyPocoConfiguration);
            FieldInfo[] locatedFields = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Fields;

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);
            locatedFields.Should().BeEquivalentTo(testedType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
        }

        [Fact]
        public void Locator_WontFind_PrivateProperties_WithoutPermission()
        {
            LazyPOCOLocator locator = new();
            Type testedType = typeof(PublicClassWithOnlyPrivateProperties);

            // Make it so we can't find private properties
            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.Public, TestedDataMembers.Properties, false);
            locator.LocateTestObjects(lazyPocoConfiguration);
            PropertyInfo[] locatedProperties = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Properties;

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);
            locatedProperties.Should().BeEquivalentTo(testedType.GetProperties(BindingFlags.Instance | BindingFlags.Public));
            testedType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Should().HaveCountGreaterThan(0, "otherwise this test is not really validating anything");
        }

        [Fact]
        public void Locator_WontFind_PrivateFields_WithoutPermission()
        {
            LazyPOCOLocator locator = new();
            Type testedType = typeof(PublicClassWithOnlyPrivateFields);

            // Make it so we can't find private properties
            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.Public, TestedDataMembers.Fields, false);
            locator.LocateTestObjects(lazyPocoConfiguration);
            FieldInfo[] locatedProperties = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Fields;

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);
            locatedProperties.Should().BeEquivalentTo(testedType.GetFields(BindingFlags.Instance | BindingFlags.Public));
            testedType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Should().HaveCountGreaterThan(0, "otherwise this test is not really validating anything");
        }

        [Fact]
        public void Locator_CanFindAllPropertiesAndFields_WhenMixed()
        {
            LazyPOCOLocator locator = new();
            Type testedType = typeof(PublicClassWithPublicFieldsAndProperties);

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.PublicAndNonPublic, TestedDataMembers.PropertiesAndFields, false);
            locator.LocateTestObjects(lazyPocoConfiguration);
            LocatedTypeInformation locatedTypesInformation = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!];

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);
            locatedTypesInformation.Properties.Should().BeEquivalentTo(testedType.GetProperties(PubNonPubInstanceFlags));
            locatedTypesInformation.Fields.Should().BeEquivalentTo(testedType.GetFields(PubNonPubInstanceFlags));

            testedType.GetProperties(PubNonPubInstanceFlags).Should().HaveCountGreaterThan(0, "otherwise this test is not really validating anything");
            testedType.GetFields(PubNonPubInstanceFlags).Should().HaveCountGreaterThan(0, "otherwise this test is not really validating anything");
        }

        [Theory]
        [InlineData(typeof(PositionalRecord))]
        [InlineData(typeof(PositionalRecordWithAdditionalPropery))]
        [InlineData(typeof(NonPositionalRecord))]
        [InlineData(typeof(PositionalRecordWithCustomCtor))]
        [InlineData(typeof(SealedRecord))]
        [InlineData(typeof(RecordStruct))]
        [InlineData(typeof(RecordStructWithComputedMember))]
        [InlineData(typeof(BasePositionalRecordAnimal))]
        [InlineData(typeof(DerivedPositionalRecordDog))]
        [InlineData(typeof(RecordWithFactoryToken))]
        public void Locator_CanFindAllPublicRecords_OfType(Type testedType)
        {
            LazyPOCOLocator locator = new();

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.Public, TestedDataMembers.Properties, true);
            locator.LocateTestObjects(lazyPocoConfiguration);
            PropertyInfo[] locatedProperties = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Properties;

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);
            locatedProperties.Should().BeEquivalentTo(testedType.GetProperties(BindingFlags.Instance | BindingFlags.Public));
        }
    }
}
