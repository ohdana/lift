public interface ILandingDoor : IDoor
{
    int Floor { get; }
    bool IsObstructed { get; set; }
    DoorState State { get; }
}