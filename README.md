# PSubstitute

A C# mocking library without 3rd party dependencies inspired by [NSubstitute](https://nsubstitute.github.io/) syntax.

## Example Tests

```csharp

public interface MyInterface
{
    void MyMethod();
}

public class SubstituteTests
{
    [Fact]
    public void Test_MyMethod_Gets_Called()
    {
        // Arrange
        var mock = Substitute.For<MyInterface>();
        
        // Act
        mock.MyMethod();

        // Assert
        mock.Received(nameof(mock.MyMethod));
    }
}    
```
