public interface IAutomatedDoor
{
    bool IsObstructed { get; }
    DoorState State { get; }
    event Action AutoCloseTimerExpired;

    void StartOpening();
    void StartClosing();
    
    void Update(float deltaTime);
}