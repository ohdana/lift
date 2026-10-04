public interface IAlarmButton : IButton, IDisposable
{
    bool IsConnectionEstablished { get; }
}