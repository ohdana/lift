public class LandingDoor : ILandingDoor
{
    public int Floor { get; private set; }
    public bool IsObstructed { get; set; }

    public bool IsFullyOpened => _state == DoorState.FullyOpened;
    public bool IsFullyClosed => _state == DoorState.FullyClosed;
    public bool IsStalled => _state == DoorState.Stalled;
    
    private const float OpenDurationSeconds = 3.0f;
    private const float CloseDurationSeconds = 3.0f;

    private float _timer;
    private DoorState _state;

    public LandingDoor(int floor)
    {
        Floor = floor;
        _state = DoorState.FullyClosed;
        IsObstructed = false;
        _timer = 0f;
    }

    public void StartOpening() => TransitionTo(DoorState.Opening);
    public void StartClosing() => TransitionTo(DoorState.Closing);

    public void Update(float deltaTime)
    {
        UpdateClosing(deltaTime);
        UpdateOpening(deltaTime);
    }

    private void UpdateClosing(float deltaTime)
    {
        if (_state != DoorState.Closing) return;

        if (IsObstructed)
        {
            _state = DoorState.Stalled;
            return;
        }

        _timer += deltaTime;
        if (_timer >= CloseDurationSeconds)
        {
            _state = DoorState.FullyClosed;
        }
    }

    private void UpdateOpening(float deltaTime)
    {
        if (_state != DoorState.Opening) return;

        _timer += deltaTime;
        if (_timer >= OpenDurationSeconds)
        {
            _state = DoorState.FullyOpened;
        }
    }

    private void TransitionTo(DoorState state)
    {
        _state = state;
        _timer = 0f;
    }
}