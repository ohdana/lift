using System;

public class AlarmButton : IAlarmButton
{
    private readonly IBuzzer _buzzer;
    private readonly ILogger _logger;

    private string _logTimePrefix => $"[{DateTime.Now:HH:mm:ss}]";

    public AlarmButton(IBuzzer buzzer, ILogger logger)
    {
        _buzzer = buzzer;
        _logger = logger;
    }

    public void Press()
    {
        _logger.Log($"{_logTimePrefix} Alarm button pressed!");
        _buzzer.SwitchOn();
    }

    public void Release()
    {
        _logger.Log($"{_logTimePrefix} Alarm button released!");
        _buzzer.SwitchOff();
    }
}