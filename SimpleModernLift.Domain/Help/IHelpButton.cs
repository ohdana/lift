public interface IHelpButton : IButton, IDisposable
{
    bool IsConnectionEstablished { get; }
}