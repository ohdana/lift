public class LandingDoor : ILandingDoor
{
    public int Floor { get; private set; }
    public bool IsObstructed { get; set; }

    public bool IsFullyOpened => _state == DoorState.FullyOpened;
    public bool IsFullyClosed => _state == DoorState.FullyClosed;
    public bool IsStalled => _state == DoorState.Stalled;
    
    private const float OpenDurationSeconds = 3.0f;
    private const float CloseDurationSeconds = 3.0f;

    private VirtualTimer _openingTimer = new VirtualTimer(3.0f);
    private VirtualTimer _closingTimer = new VirtualTimer(3.0f);
    private DoorState _state;

    public LandingDoor(int floor)
    {
        Floor = floor;
        _state = DoorState.FullyClosed;
        IsObstructed = false;
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

        _closingTimer.Tick(deltaTime);
        
        if (_closingTimer.IsExpired)
        {
            _state = DoorState.FullyClosed;
        }
    }

    private void UpdateOpening(float deltaTime)
    {
        if (_state != DoorState.Opening) return;

        _openingTimer.Tick(deltaTime);

        if (_openingTimer.IsExpired)
        {
            _state = DoorState.FullyOpened;
        }
    }

    private void TransitionTo(DoorState state)
    {
        _state = state;
        _openingTimer.Reset();
        _closingTimer.Reset();
    }
}