using Xunit;
using NSubstitute;

public class LandingDoorTests
{
    private const float OpenDurationSeconds = 3.0f;
    private const float CloseDurationSeconds = 3.0f;

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public void LandingDoor_WhenOpening_BecomesOpened(int floor)
    {
        // Arrange
        var door = new LandingDoor(floor);
        CloseFully(door);

        // Act
        OpenFully(door);

        // Assert
        Assert.True(door.IsFullyOpened);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public void LandingDoor_WhenClosingWithNoObstruction_BecomesClosed(int floor)
    {
        // Arrange
        var door = new LandingDoor(floor);
        OpenFully(door);

        // Act
        CloseFully(door);

        // Assert
        Assert.True(door.IsFullyClosed);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public void LandingDoor_WhenClosingWithObstruction_BecomesStalled(int floor)
    {
        // Arrange
        var door = new LandingDoor(floor);
        OpenFully(door);

        // Act
        TryCloseWithObstruction(door);

        // Assert
        Assert.True(door.IsStalled);
        Assert.False(door.IsFullyClosed);
    }

    private void OpenFully(ILandingDoor door)
    {
        door.StartOpening();
        door.Update(OpenDurationSeconds);
    }

    private void CloseFully(ILandingDoor door)
    {
        door.StartClosing();
        door.Update(CloseDurationSeconds);
    }

    private void TryCloseWithObstruction(ILandingDoor door)
    {
        door.StartClosing();
        door.Update(CloseDurationSeconds / 2);
        door.IsObstructed = true;
        door.Update(CloseDurationSeconds / 2);
    }
}