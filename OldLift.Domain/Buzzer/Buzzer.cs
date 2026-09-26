using System;

public class Buzzer : IBuzzer
{
    private readonly ILogger _logger;

    private string _logTimePrefix => $"[{DateTime.Now:HH:mm:ss}]";

    public Buzzer(ILogger logger)
    {
        _logger = logger;
    }

    public bool IsOn { get; private set; }

    public void SwitchOn()
    {
        IsOn = true;
        _logger.Log($"{_logTimePrefix} <buzzer is buzzzzzzzzzzzzzzzing>");
    }

    public void SwitchOff()
    {
        IsOn = false;
        _logger.Log($"{_logTimePrefix} <buzzer is off>");
    }
}