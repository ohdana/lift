using Xunit;
using NSubstitute;

public class FloorButtonTests
{
    private readonly ILiftController _controller;

    public FloorButtonTests()
    {
        _controller = Substitute.For<ILiftController>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public void FloorButton_WhenPressed_NotifiesLiftController(int floor)
    {
        // Arrange
        var button = new FloorButton(floor, _controller);

        // Act
        button.Press();

        // Assert
        _controller.Received(1).RegisterCarCall(floor);
    }
}