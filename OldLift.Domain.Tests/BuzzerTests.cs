using Xunit;
using NSubstitute;

public class BuzzerTests
{
    private IBuzzer _buzzer;

    public BuzzerTests()
    {
        _buzzer = new Buzzer();
    }

    [Fact]
    public void Buzzer_WhenSwitchedOn_BecomesOn()
    {
        // Arrange
        _buzzer.SwitchOff();

        // Act
        _buzzer.SwitchOn();

        // Assert
        Assert.True(_buzzer.IsOn);
    }

    [Fact]
    public void Buzzer_WhenSwitchedOff_BecomesOff()
    {
        // Arrange
        _buzzer.SwitchOn();

        // Act
        _buzzer.SwitchOff();

        // Assert
        Assert.False(_buzzer.IsOn);
    }
}