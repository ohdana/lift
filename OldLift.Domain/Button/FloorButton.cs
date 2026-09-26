using System;

public class FloorButton : IFloorButton
{
    public int Floor { get; private set; }
    private readonly ILiftController _controller;
    private readonly ILogger _logger;

    private string _logTimePrefix => $"[{DateTime.Now:HH:mm:ss}]";

    public FloorButton(int floor, ILiftController controller, ILogger logger)
    {
        Floor = floor;
        _controller = controller;
        _logger = logger;
    }

    public void Press()
    {
        _controller.RegisterCarCall(Floor);
        _logger.Log($"{_logTimePrefix} Floor button {Floor} pressed!");
    }
}