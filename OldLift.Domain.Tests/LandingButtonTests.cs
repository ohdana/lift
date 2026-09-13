using Xunit;
using NSubstitute;

public class LandingButtonTests
{
    private readonly ILiftController _controller;

    public LandingButtonTests()
    {
        _controller = Substitute.For<ILiftController>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public void LandingButton_WhenPressed_NotifiesLiftController(int floor)
    {
        // Arrange
        var button = new LandingButton(floor, _controller);

        // Act
        button.Press();

        // Assert
        _controller.Received(1).RegisterLandingCall(floor);
    }
}