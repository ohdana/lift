public class LandingButton : ILandingButton
{
    public int Floor { get; private set; }
    private readonly ILiftController _controller;
    private readonly ILogger _logger;

    public LandingButton(int floor, ILiftController controller, ILogger logger)
    {
        Floor = floor;
        _controller = controller;
        _logger = logger;
    }

    public void Press()
    {
        _controller.RegisterLandingCall(Floor);
        _logger.Log($"Landing button {Floor} pressed!");
    }
}