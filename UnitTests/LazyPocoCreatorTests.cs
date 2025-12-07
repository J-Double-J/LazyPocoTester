using FluentAssertions;
using LazyPocoTester.Enums;
using System.Reflection;

namespace LazyPocoTester.UnitTests
{
    public class CreatorFixture
    {
        public int FoundPOCOsCount { get; }

        /// <summary>
        /// The contents of this method is irrelevant as <see cref="LazyPocoCreatorAttribute"/> does not care about the method.
        /// </summary>
        public MethodInfo PlaceholderMethod { get; init; }

        public CreatorFixture()
        {
            FoundPOCOsCount = Assembly.GetAssembly(typeof(PublicClassWithOnlyPublicProperties))!
                                      .GetTypes()
                                      .Where(t => LazyPOCOLocator.ValidClass(t) || LazyPOCOLocator.ValidStruct(t))
                                      .Where(t => t.GetCustomAttribute<POCOTestAttribute>() is not null)
                                      .Count();

            PlaceholderMethod = typeof(string).GetMethod(nameof(string.IsInterned))!;
        }
    }

    public class LazyPocoCreatorTests : IClassFixture<CreatorFixture>
    {
        private readonly CreatorFixture _fixture;

        public LazyPocoCreatorTests(CreatorFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void LazyPocoCreator_FindsEveryPOCO()
        {
            LazyPocoCreatorAttribute attribute = new LazyPocoCreatorAttribute(AccessibilityFlags.PublicAndNonPublic,
                                                                              TestedDataMembers.PropertiesAndFieldsNoBacking,
                                                                              true);

            attribute.GetData(_fixture.PlaceholderMethod).ToArray().Should().HaveCount(_fixture.FoundPOCOsCount);
        }

        [Fact]
        public void LazyPocoCreator_ArgArrayIsOrderedAsExpected()
        {
            LazyPocoCreatorAttribute attribute = new LazyPocoCreatorAttribute(AccessibilityFlags.PublicAndNonPublic,
                                                                              TestedDataMembers.PropertiesAndFieldsNoBacking,
                                                                              true);

            object[] argArray = attribute.GetData(_fixture.PlaceholderMethod).First();

            argArray[0].Should().BeOfType<LazyPocoConfiguration>();
            argArray[1].Should().BeAssignableTo<Type>();
        }

        [Fact]
        public void LazyPocoCreator_PassedTypeIsFoundInConfiguration()
        {
            LazyPocoCreatorAttribute attribute = new LazyPocoCreatorAttribute(AccessibilityFlags.PublicAndNonPublic,
                                                                              TestedDataMembers.PropertiesAndFieldsNoBacking,
                                                                              true);

            foreach (object[] args in attribute.GetData(_fixture.PlaceholderMethod))
            {
                LazyPocoConfiguration config = (LazyPocoConfiguration)args[0];
                Type testedType = (Type)args[1];

                config.LocatedTypeInformation.TryGetValue(testedType.AssemblyQualifiedName!, out _).Should().BeTrue();
            }
        }
    }
}
