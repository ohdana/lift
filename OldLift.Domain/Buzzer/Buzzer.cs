public class Buzzer : IBuzzer
{
    public bool IsOn { get; private set; }

    public void SwitchOn() => IsOn = true;
    public void SwitchOff() => IsOn = false;
}