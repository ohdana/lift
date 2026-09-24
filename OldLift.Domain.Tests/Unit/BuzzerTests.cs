using Xunit;
using NSubstitute;

public class BuzzerTests
{
    private IBuzzer _buzzer;
    private readonly ILogger _logger;

    public BuzzerTests()
    {
        _logger = Substitute.For<ILogger>();
        _buzzer = new Buzzer(_logger);
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