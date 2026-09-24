public class LiftLogger : ILogger
{
    public bool Verbose { get; set; } = false;

    public void Log(string message)
    {
        if (Verbose)
        {
            Console.WriteLine(message);
        }
    }
}