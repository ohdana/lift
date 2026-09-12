using Xunit;
using NSubstitute;

public class LiftControllerTests
{
    public LiftControllerTests()
    {
        // TODO
    }

    [Fact]
    public void LiftController_WhenIdleAndLandingCallReceived_StartsMovingToFloor()
    {
        // Arrange
        // Act
        // Assert
    }

    [Fact]
    public void LiftController_WhenBusyAndLandingCallReceived_Ignores()
    {
        // Arrange
        // Act
        // Assert
    }

    [Fact]
    public void LiftController_WhenIdleAndDoorsOpenedAndCarCallReceived_QueuesTheCall()
    {
        // Arrange
        // Act
        // Assert
    }

    [Fact]
    public void LiftController_WhenIdleAndDoorsClosedAndCarCallReceived_StartsMovingToFloor()
    {
        // Arrange
        // Act
        // Assert
    }

    [Fact]
    public void LiftController_WhenBusyAndCarCallReceived_Ignores()
    {
        // Arrange
        // Act
        // Assert
    }

    [Fact]
    public void LiftController_WhenDoorsOpen_StartsAutoCloseTimeout()
    {
        // Arrange
        // Act
        // Assert
    }

    [Fact]
    public void LiftController_WhenDoorsCloseAndCarCallQueued_StartsMovingToFloor()
    {
        // Arrange
        // Act
        // Assert
    }

    [Fact]
    public void LiftController_WhenDoorsCloseAndNoCarCallsQueued_BecomesIdle()
    {
        // Arrange
        // Act
        // Assert
    }
}