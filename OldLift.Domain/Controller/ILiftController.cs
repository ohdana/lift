public interface ILiftController
{
    float CarPosition { get; }
    int? TargetFloor { get; }
    bool IsIdle { get; }

    void RegisterCarCall(int floor);
    void RegisterLandingCall(int floor);
    void Update(float deltaTime);
}