using Xunit;
using NSubstitute;

public class LandingDoorTests
{
    private const float OpenDurationSeconds = 3.0f;
    private const float CloseDurationSeconds = 3.0f;
    private ILandingDoor _door;

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public void LandingDoor_WhenOpening_BecomesOpened(int floor)
    {
        // Arrange
        _door = new LandingDoor(floor);
        CloseFully();

        // Act
        OpenFully();

        // Assert
        Assert.True(_door.IsFullyOpened);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public void LandingDoor_WhenClosingWithNoObstruction_BecomesClosed(int floor)
    {
        // Arrange
        _door = new LandingDoor(floor);
        OpenFully();

        // Act
        CloseFully();

        // Assert
        Assert.True(_door.IsFullyClosed);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public void LandingDoor_WhenClosingWithObstruction_BecomesStalled(int floor)
    {
        // Arrange
        _door = new LandingDoor(floor);
        OpenFully();

        // Act
        TryCloseWithObstruction();

        // Assert
        Assert.True(_door.IsStalled);
        Assert.False(_door.IsFullyClosed);
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