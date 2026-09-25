public class AlarmButton : IAlarmButton
{
    private readonly IBuzzer _buzzer;
    private readonly ILogger _logger;

    public AlarmButton(IBuzzer buzzer, ILogger logger)
    {
        _buzzer = buzzer;
        _logger = logger;
    }

    public void Press()
    {
        _logger.Log($"Alarm button pressed!");
        _buzzer.SwitchOn();
    }

    public void Release()
    {
        _logger.Log($"Alarm button released!");
        _buzzer.SwitchOff();
    }
}