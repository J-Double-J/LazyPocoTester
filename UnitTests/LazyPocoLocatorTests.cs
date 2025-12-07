using FluentAssertions;
using LazyPocoTester.Enums;
using System.Reflection;
using TestPOCOs.TestObjects;

namespace LazyPocoTester.UnitTests
{
    public class LazyPocoLocatorTests
    {
        /// <summary>
        /// Shorthand for Public + NonPublic Instance binding flags.
        /// </summary>
        private const BindingFlags PubNonPubInstanceFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        #region Class Locating Tests

        [Fact]
        public void Locator_CanFindClass_WithPublicProperties()
        {
            LazyPOCOLocator locator = new();
            Type testedType = typeof(PublicClassWithOnlyPublicProperties);

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.Public, TestedDataMembers.Properties, false);
            locator.LocateTestObjects(lazyPocoConfiguration);
            List<PropertyInfo> locatedProperties = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Properties;

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
            List<PropertyInfo> locatedProperties = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Properties;

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
            List<FieldInfo> locatedFields = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Fields;

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
            List<FieldInfo> locatedFields = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Fields;

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
            List<PropertyInfo> locatedProperties = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Properties;

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
            List<FieldInfo> locatedProperties = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Fields;

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

        [Fact]
        public void Locator_CanFindAllPublicProperties_OfBaseAndDerivedClass()
        {
            LazyPOCOLocator locator = new();
            Type testedType = typeof(PublicPOCODerivedClass);

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.Public, TestedDataMembers.Properties, false);
            locator.LocateTestObjects(lazyPocoConfiguration);
            List<PropertyInfo> locatedProperties = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Properties;

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);
            locatedProperties.Should().HaveCount(2, $"there is one base property and one property in {nameof(PublicPOCODerivedClass)}");
            locatedProperties.Should().BeEquivalentTo(testedType.GetProperties(BindingFlags.Public | BindingFlags.Instance));

            testedType.GetProperties(PubNonPubInstanceFlags).Should().HaveCountGreaterThan(0, "otherwise this test is not really validating anything");
            testedType.GetFields(PubNonPubInstanceFlags).Should().HaveCountGreaterThan(0, "otherwise this test is not really validating anything");
        }

        [Fact]
        public void Locator_CanFindMixedDerivedClass_WithNonDefaultConstructors()
        {
            LazyPOCOLocator locator = new();
            Type testedType = typeof(DerivedClassOfMixedBaseClassNoDefaultCtor);

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.PublicAndNonPublic, TestedDataMembers.PropertiesAndFieldsNoBacking, true);
            locator.LocateTestObjects(lazyPocoConfiguration);
            LocatedTypeInformation locatedTypesInformation = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!];

            List<PropertyInfo> expectedProperties = testedType.GetProperties(PubNonPubInstanceFlags)
                                                              .Concat(testedType.BaseType!.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
                                                              .DistinctBy(p => (p.DeclaringType, p.Name))
                                                              .ToList();

            List<FieldInfo> expectedFields = testedType.GetFields(PubNonPubInstanceFlags)
                                                       .Concat(testedType.BaseType!.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                                                       .Where(f => !f.Name.EndsWith("k__BackingField"))
                                                       .DistinctBy(f => (f.DeclaringType, f.Name))
                                                       .ToList();

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);

            locatedTypesInformation.Properties.Should().HaveCount(5);
            locatedTypesInformation.Fields.Should().HaveCount(4);
            locatedTypesInformation.Properties.Should().BeEquivalentTo(expectedProperties);
            locatedTypesInformation.Fields.Should().BeEquivalentTo(expectedFields);
        }

        [Fact]
        public void Locator_CanFindFieldsAndProperties_OfClassWithTwoHigherBaseClasses()
        {
            LazyPOCOLocator locator = new();
            Type testedType = typeof(TwoDeepDerivedClassOfMixedBaseClasses);

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.PublicAndNonPublic, TestedDataMembers.PropertiesAndFieldsNoBacking, true);
            locator.LocateTestObjects(lazyPocoConfiguration);
            LocatedTypeInformation locatedTypesInformation = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!];

            List<PropertyInfo> expectedProperties = testedType.GetProperties(PubNonPubInstanceFlags)
                                                              .Concat(testedType.BaseType!.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
                                                              .Concat(testedType.BaseType!.BaseType!.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
                                                              .DistinctBy(p => (p.DeclaringType, p.Name))
                                                              .ToList();

            List<FieldInfo> expectedFields = testedType.GetFields(PubNonPubInstanceFlags)
                                                       .Concat(testedType.BaseType!.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                                                       .Concat(testedType.BaseType!.BaseType!.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                                                       .Where(f => !f.Name.EndsWith("k__BackingField"))
                                                       .DistinctBy(f => (f.DeclaringType, f.Name))
                                                       .ToList();

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);

            locatedTypesInformation.Properties.Should().HaveCount(6);
            locatedTypesInformation.Fields.Should().HaveCount(5);
            locatedTypesInformation.Properties.Should().BeEquivalentTo(expectedProperties);
            locatedTypesInformation.Fields.Should().BeEquivalentTo(expectedFields);
        }

        #endregion Class Locating Tests

        #region Record Locating Tests

        [Theory]
        [InlineData(typeof(PositionalRecord))]
        [InlineData(typeof(PositionalRecordWithAdditionalPropery))]
        [InlineData(typeof(NonPositionalRecord))]
        [InlineData(typeof(PositionalRecordWithCustomCtor))]
        [InlineData(typeof(SealedRecord))]
        [InlineData(typeof(RecordStruct))]
        [InlineData(typeof(BasePositionalRecordAnimal))]
        [InlineData(typeof(DerivedPositionalRecordDog))]
        public void Locator_CanFindAllPublicRecordsPublicProperties_OfType(Type testedType)
        {
            LazyPOCOLocator locator = new();

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.Public, TestedDataMembers.Properties, true);
            locator.LocateTestObjects(lazyPocoConfiguration);
            List<PropertyInfo> locatedProperties = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Properties;

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);
            locatedProperties.Should().BeEquivalentTo(testedType.GetProperties(BindingFlags.Instance | BindingFlags.Public));
        }

        [Theory]
        [InlineData(typeof(PublicClassWithAComputedProperty))]
        [InlineData(typeof(RecordStructWithComputedMember))]
        [InlineData(typeof(RecordWithFactoryToken))]
        public void Locator_CanIgnoreNonWritableProperties_OnRecords(Type testedType)
        {
            LazyPOCOLocator locator = new();

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.PublicAndNonPublic, TestedDataMembers.Properties, true);
            locator.LocateTestObjects(lazyPocoConfiguration);
            List<PropertyInfo> locatedProperties = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Properties;
            
            PropertyInfo[] propertiesOnType = testedType.GetProperties(PubNonPubInstanceFlags);

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);
            locatedProperties.Should().HaveCount(propertiesOnType.Length - 1, "the type in this test has one readonly computed property")
                             .And.Subject.Should().HaveCountGreaterThan(0);
        }

        #endregion Record Locating Tests

        #region Struct Locating Tests

        [Fact]
        public void Locator_CanFindStruct_WithPublicProperties()
        {
            LazyPOCOLocator locator = new();
            Type testedType = typeof(PublicStructPublicProperties);

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.Public, TestedDataMembers.Properties, false);
            locator.LocateTestObjects(lazyPocoConfiguration);
            List<PropertyInfo> locatedProperties = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Properties;

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);
            locatedProperties.Should().BeEquivalentTo(testedType.GetProperties(BindingFlags.Instance | BindingFlags.Public));
        }

        [Fact]
        public void Locator_CanFindStruct_WithPrivateProperties()
        {
            LazyPOCOLocator locator = new();
            Type testedType = typeof(PublicStructPrivateProperties);

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.NonPublic, TestedDataMembers.Properties, false);
            locator.LocateTestObjects(lazyPocoConfiguration);
            List<PropertyInfo> locatedProperties = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Properties;

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);
            locatedProperties.Should().BeEquivalentTo(testedType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic));
        }

        [Fact]
        public void Locator_CanFindStruct_WithPublicFields()
        {
            LazyPOCOLocator locator = new();
            Type testedType = typeof(PublicStructPublicFields);

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.Public, TestedDataMembers.Fields, false);
            locator.LocateTestObjects(lazyPocoConfiguration);
            List<FieldInfo> locatedFields = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Fields;

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);
            locatedFields.Should().BeEquivalentTo(testedType.GetFields(BindingFlags.Instance | BindingFlags.Public));
        }

        [Fact]
        public void Locator_CanFindStruct_WithPrivateFields()
        {
            LazyPOCOLocator locator = new();
            Type testedType = typeof(PublicStructPrivateFields);

            LazyPocoConfiguration lazyPocoConfiguration = new LazyPocoConfiguration(AccessibilityFlags.NonPublic, TestedDataMembers.Fields, false);
            locator.LocateTestObjects(lazyPocoConfiguration);
            List<FieldInfo> locatedFields = locator.LocatedTypeInformation[testedType.AssemblyQualifiedName!].Fields;

            locator.LocatedTypes.Should().NotBeEmpty();
            locator.LocatedTypes.Should().Contain(testedType);
            locatedFields.Should().BeEquivalentTo(testedType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
        }

        #endregion Struct Locating Tests
    }
}
