public class FloorButton : IFloorButton
{
    public event Action<int>? Pressed;
    public int Floor { get; private set; }

    public FloorButton(int floor)
    {
        Floor = floor;
    }

    public void Press() => Pressed?.Invoke(Floor);
}