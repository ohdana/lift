public interface ITimedDoor
{
    bool IsObstructed { get; }
    DoorState State { get; }
    event Action TimerExpired;

    void StartOpening();
    void StartClosing();
    
    void Update(float deltaTime);
}