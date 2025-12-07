namespace LazyPocoTester.Enums
{
    [Flags]
    public enum TestedDataMembers
    {
        None = 0,
        Properties = 1,
        Fields = 1 << 1,
        NoBackingFields = 1 << 2,

        // Short hands
        PropertiesAndFields = Properties | Fields,
        PropertiesAndFieldsNoBacking = Properties | Fields | NoBackingFields
    }
}
