public interface IFloorButton : IButton
{
    int Floor { get; }
    event Action<int>? Pressed;
}