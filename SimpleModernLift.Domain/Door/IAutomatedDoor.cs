public interface IAutomatedDoor
{
    DoorState State { get; }

    void StartOpening();
    void StartClosing();
    
    void Update(flot deltaTime);
}