using Xunit;
using NSubstitute;

public class AutomatedDoorTests
{
    private const float TickSize = 0.01f;
    private const float OpenDurationSeconds = 3.0f;
    private const float CloseDurationSeconds = 3.0f;

    private IAutomatedDoor _door;
    private IObstructionDetector _obstructionDetector;
    private ILogger _logger;

    public AutomatedDoorTests()
    {
        _obstructionDetector = Substitute.For<IObstructionDetector>();
        _logger = Substitute.For<ILogger>();

        var label = "Test";
        _obstructionDetector.IsClear.Returns(true);
        _door = new AutomatedDoor(label, _obstructionDetector, _logger);
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
    public void AutomatedDoor_WhenClosingWithObstruction_StartsReopening()
    {
        // Arrange
        OpenFully();

        // Act
        TryCloseWithObstruction();

        // Assert
        Assert.Equal(DoorState.Opening, _door.State);
    }

    private void OpenFully()
    {
        _door.StartOpening();
        ImitateSecondsPassed(OpenDurationSeconds);
        ImitateSecondsPassed(TickSize);
    }

    private void CloseFully()
    {
        _door.StartClosing();
        ImitateSecondsPassed(CloseDurationSeconds);
        ImitateSecondsPassed(TickSize);
    }

    private void TryCloseWithObstruction()
    {
        _door.StartClosing();
        ImitateSecondsPassed(CloseDurationSeconds / 2);
        _obstructionDetector.IsClear.Returns(false);
        ImitateSecondsPassed(TickSize);
    }

    private void ImitateSecondsPassed(float seconds)
    {
        var ticks = (int) Math.Ceiling(seconds / TickSize);
        for (int i = 0; i < ticks; i++)
        {
            _door.Update(TickSize);
        }
    }
}