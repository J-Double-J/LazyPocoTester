using LazyPocoTester;

namespace TestPOCOs.TestObjects
{
    // 1. Positional record (primary constructor)
    [POCOTest]
    public record PositionalRecord(string Name, int Age);

    // 2. Positional record with additional properties
    [POCOTest]
    public record PositionalRecordWithAdditionalPropery(string Name, int Age, string Role)
    {
        public decimal Salary { get; init; }
    }

    // 3. Non-positional record with explicit properties
    [POCOTest]
    public record NonPositionalRecord
    {
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
    }

    // 4. Positional record with custom constructor logic
    [POCOTest]
    public record PositionalRecordWithCustomCtor(string Name, int Tier)
    {
        public PositionalRecordWithCustomCtor(string name) : this(name, 1) { }
    }

    // 5. Sealed record
    [POCOTest]
    public sealed record SealedRecord(string Key, string Value);

    // 6. Record struct (value-type record)
    [POCOTest]
    public readonly record struct RecordStruct(int X, int Y);

    // 7. Record struct with additional members
    [POCOTest]
    public record struct RecordStructWithComputedMember(int Width, int Height)
    {
        public int Area => Width * Height;
    }

    // 8. Inheritance with positional base
    [POCOTest]
    public record BasePositionalRecordAnimal(string Species);
    
    [POCOTest]
    public record DerivedPositionalRecordDog(string Species, string Breed)
        : BasePositionalRecordAnimal(Species);

    // 9. Private constructor with factory
    [POCOTest]
    public record RecordWithFactoryToken
    {
        public string Value { get; init; }

        private RecordWithFactoryToken(string value) => Value = value;

        public static RecordWithFactoryToken Create(string value)
            => new RecordWithFactoryToken(value);
    }


}
