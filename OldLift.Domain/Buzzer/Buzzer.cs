public class Buzzer : IBuzzer
{
    private readonly ILogger _logger;
    public Buzzer(ILogger logger)
    {
        _logger = logger;
    }

    public bool IsOn { get; private set; }

    public void SwitchOn()
    {
        IsOn = true;
        _logger.Log("<buzzer is buzzzzzzzzzzzzzzzing>");
    }

    public void SwitchOff()
    {
        IsOn = false;
        _logger.Log("<buzzer is off>");
    }
}