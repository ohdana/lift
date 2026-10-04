public interface IAutomatedDoor
{
    bool IsObstructed { get; }
    DoorState State { get; }

    void StartOpening();
    void StartClosing();
    
    void Update(float deltaTime);
}