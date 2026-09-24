public class FloorButton : IFloorButton
{
    public int Floor { get; private set; }
    private readonly ILiftController _controller;
    private readonly ILogger _logger;

    public FloorButton(int floor, ILiftController controller, ILogger logger)
    {
        Floor = floor;
        _controller = controller;
        _logger = logger;
    }

    public void Press()
    {
        _controller.RegisterCarCall(Floor);
        _logger.Log($"Floor button {Floor} pressed!");
    }
}