public interface ILiftController
{
    bool IsIdle { get; }
    int CurrentFloor { get; }
    int? TargetFloor { get; }

    void RegisterCarCall(int floor);
    void RegisterLandingCall(int floor);
    
    void Update(float deltaTime);
}