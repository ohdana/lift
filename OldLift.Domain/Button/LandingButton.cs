public class LandingButton : ILandingButton
{
    public int Floor { get; private set; }
    private readonly ILiftController _controller;

    public LandingButton(int floor, ILiftController controller)
    {
        Floor = floor;
        _controller = controller;
    }

    public void Press()
    {
        _controller.RegisterLandingCall(Floor);
    }
}