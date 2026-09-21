public interface ILiftController
{
    int CurrentFloor { get; }
    int? TargetFloor { get; }
    bool IsIdle { get; }

    void RegisterCarCall(int floor);
    void RegisterLandingCall(int floor);
    void Update(float deltaTime);
}