public interface ILogger
{
    bool Verbose { get; set; }
    void Log(string message);
}