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
        _buzzer.SwitchOn();
        _logger.Log($"Alarm button pressed!");
    }

    public void Release()
    {
        _buzzer.SwitchOff();
        _logger.Log($"Alarm button released!");
    }
}