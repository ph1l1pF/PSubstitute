using PSubstitute;

namespace UnitTests;

public interface MyInterface
{
    void MyMethod();
    void MySecondMethod();
}

public class SubstituteTests
{
    [Fact]
    public void Received_Throws_Only_If_Given_Method_Was_Not_Called()
    {
        // Arrange
        var mock = Substitute.For<MyInterface>();
        
        // Act
        mock.MyMethod();

        // Assert
        mock.Received(nameof(mock.MyMethod));
        Assert.Throws<Exception>(() => mock.Received(nameof(mock.MySecondMethod)));
    }
    
    [Fact]
    public void Two_Mocks_Received_Throws_Only_If_Given_Method_Was_Not_Called()
    {
        // Arrange
        var mock1 = Substitute.For<MyInterface>();
        var mock2 = Substitute.For<MyInterface>();
        
        // Act
        mock1.MyMethod();
        mock2.MySecondMethod();

        // Assert
        mock1.Received(nameof(mock1.MyMethod));
        Assert.Throws<Exception>(() => mock1.Received(nameof(mock1.MySecondMethod)));
        
        mock2.Received(nameof(mock2.MySecondMethod));
        Assert.Throws<Exception>(() => mock2.Received(nameof(mock2.MyMethod)));
    }
    
    [Fact]
    public void Received_With_Count_Throws_Only_If_Given_Method_Was_Not_Called_Given_Number_Times()
    {
        // Arrange
        var mock = Substitute.For<MyInterface>();
        
        // Act
        mock.MyMethod();
        mock.MyMethod();

        // Assert
        mock.Received(nameof(mock.MyMethod));
        mock.Received(nameof(mock.MyMethod), 2);
        Assert.Throws<Exception>(() => mock.Received(nameof(mock.MySecondMethod), 1));
        Assert.Throws<Exception>(() => mock.Received(nameof(mock.MySecondMethod), 3));
    }
}