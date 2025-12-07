# LazyPocoTester

Lazy POCO Tester is a testing package that helps automate testing getters and setters on Plain Old Clr Objects (POCOs) aka Data Transfer Objects (DTOs).
Whether you need to ensure that your POCOs are setting and getting a value correctly, or you just want code coverage over all your objects, this tool will help you save time writing those tests.

This package is not intended to do much more than testing simple get/set on fields and properties. It aims to follow valid constraints on JSON deserialization rules as these objects are typically serialized and deserialized
from APIs.

## Requirements
Requires XUnit

## Getting Started

Create a simple class that has Properties that aren't manipulated when being set. Add the `[POCOTest]` attribute to it.

```csharp
[POCOTest]
public class Rectangle
{
    public int Length { get; set; }
    public int Width { get; set; }

    public Rectangle(int length, int width)
    {
        Length = length;
        Width = width;
    }
}
```
Add a project reference between your test suite and the project containing your POCO.
Create a new test like so:

```csharp
public class POCOTests
{
    [Theory]
    [LazyPocoCreator(AccessibilityFlags.PublicAndNonPublic, TestedDataMembers.PropertiesAndFieldsNoBacking, tryToCreateNonDefaultConstructors: true)]
    public void TestAllPOCOs(LazyPocoConfiguration configuration, Type type)
    {
        POCOTester coordinator = new POCOTester();

        coordinator.TestPOCO(configuration, type);
    }
}
```
The above LazyPocoCreator can be configured on what it considers to be a valid POCO, but this is the least opinionated configuration.

Run your tests and you should see all your objects found auto-magically (assumes Visual Studio Test Runner):

<img width="1768" height="661" alt="image" src="https://github.com/user-attachments/assets/83d96d69-b887-4c5c-8b74-6670613f291e" />
This will allow you to see which objects were found and their individual test results.

### *What is Supported?*

Currently supported types:
- Classes
- Records
- Structs

Currently tested:
- Assigning and reading properties
- Assigning and reading fields

### *Limitations*

Constructors:
- One of the following:
  - Parameterless Constructor
  - A constructor whose parameters all pair with a property or field (Constructor accessibility requirements determined by attribute configuration)

Accessibility Modifiers:
- Does not test static properties or fields
- If allowing Public accessors will only check for public constructors (Same with NonPublic)

Testing Tools:
- Only works with XUnit
- Visual Studio sees all the tests as one larger test

Testing:
- Skips computed properties
- Does not test for enforceable simplicity
  - For example you could add `[POCOTest]` to nearly any class as long as it had read/write properties and a valid constructor. It doesn't enforce that POCOs should be simple.
