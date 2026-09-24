using Xunit;
using NSubstitute;

public class FloorButtonTests
{
    private readonly ILiftController _controller;
    private readonly ILogger _logger;

    public FloorButtonTests()
    {
        _controller = Substitute.For<ILiftController>();
        _logger = Substitute.For<ILogger>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public void FloorButton_WhenPressed_NotifiesLiftController(int floor)
    {
        // Arrange
        var button = new FloorButton(floor, _controller, _logger);

        // Act
        button.Press();

        // Assert
        _controller.Received(1).RegisterCarCall(floor);
    }
}