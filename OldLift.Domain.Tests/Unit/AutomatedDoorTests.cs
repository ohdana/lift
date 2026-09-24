using Xunit;
using NSubstitute;

public class AutomatedDoorTests
{
    private const float OpenDurationSeconds = 3.0f;
    private const float CloseDurationSeconds = 3.0f;

    private IAutomatedDoor _door;
    private ILogger _logger;

    public AutomatedDoorTests()
    {
        _logger = Substitute.For<ILogger>();
        _door = new AutomatedDoor(_logger);
    }

    [Fact]
    public void AutomatedDoor_WhenOpening_BecomesOpened()
    {
        // Arrange
        CloseFully();

        // Act
        OpenFully();

        // Assert
        Assert.Equal(DoorState.FullyOpened, _door.State);
    }

    [Fact]
    public void AutomatedDoor_WhenClosingWithNoObstruction_BecomesClosed()
    {
        // Arrange
        OpenFully();

        // Act
        CloseFully();

        // Assert
        Assert.Equal(DoorState.FullyClosed, _door.State);
    }

    [Fact]
    public void LandingDoor_WhenClosingWithObstruction_BecomesStalled()
    {
        // Arrange
        OpenFully();

        // Act
        TryCloseWithObstruction();

        // Assert
        Assert.Equal(DoorState.Stalled, _door.State);
    }

    private void OpenFully()
    {
        _door.StartOpening();
        _door.Update(OpenDurationSeconds);
    }

    private void CloseFully()
    {
        _door.StartClosing();
        _door.Update(CloseDurationSeconds);
    }

    private void TryCloseWithObstruction()
    {
        _door.StartClosing();
        _door.Update(CloseDurationSeconds / 2);
        _door.IsObstructed = true;
        _door.Update(CloseDurationSeconds / 2);
    }
}