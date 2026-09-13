using Xunit;
using NSubstitute;

public class AlarmButtonTests
{
    private readonly IAlarmButton _button;
    private readonly IBuzzer _buzzer;

    public AlarmButtonTests()
    {
        _buzzer = Substitute.For<IBuzzer>();
        _button = new AlarmButton(_buzzer);
    }

    [Fact]
    public void AlarmButton_WhenPressed_TriggersBuzzer()
    {
        // Arrange
        _button.Release();
        _buzzer.ClearReceivedCalls();

        // Act
        _button.Press();

        // Assert
        _buzzer.Received(1).SwitchOn();
        _buzzer.DidNotReceive().SwitchOff();
    }

    [Fact]
    public void AlarmButton_WhenReleased_StopsBuzzer()
    {
        // Arrange
        _button.Press();
        _buzzer.ClearReceivedCalls();

        // Act
        _button.Release();

        // Assert
        _buzzer.Received(1).SwitchOff();
        _buzzer.DidNotReceive().SwitchOn();
    }
}