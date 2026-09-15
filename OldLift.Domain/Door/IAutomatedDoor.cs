public interface IAutomatedDoor : IDoor
{
    bool IsObstructed { get; set; }
    DoorState State { get; }
}