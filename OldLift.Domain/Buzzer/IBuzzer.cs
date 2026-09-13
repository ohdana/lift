public interface IBuzzer
{
    bool IsOn { get; }
    
    void SwitchOn();
    void SwitchOff();
}