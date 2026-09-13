public class FloorButton : IFloorButton
{
    public int Floor { get; private set; }
    private readonly ILiftController _controller;

    public FloorButton(int floor, ILiftController controller)
    {
        Floor = floor;
        _controller = controller;
    }

    public void Press()
    {
        _controller.RegisterCarCall(Floor);
    }
}