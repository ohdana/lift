using System;

public class LandingButton : ILandingButton
{
    public int Floor { get; private set; }
    private readonly ILiftController _controller;
    private readonly ILogger _logger;

    private string _logTimePrefix => $"[{DateTime.Now:HH:mm:ss}]";

    public LandingButton(int floor, ILiftController controller, ILogger logger)
    {
        Floor = floor;
        _controller = controller;
        _logger = logger;
    }

    public void Press()
    {
        _controller.RegisterLandingCall(Floor);
        _logger.Log($"{_logTimePrefix} Landing button {Floor} pressed!");
    }
}