public class AlarmButton : IAlarmButton
{
    private readonly IBuzzer _buzzer;

    public AlarmButton(IBuzzer buzzer)
    {
        _buzzer = buzzer;
    }

    public void Press() => _buzzer.SwitchOn();
    public void Release() => _buzzer.SwitchOff();
}