public interface ILandingDoor : IDoor
{
    int Floor { get; }
    bool IsFullyOpened { get; }
    bool IsFullyClosed { get; }
    bool IsObstructed { get; set; }
    bool IsStalled { get; }
}