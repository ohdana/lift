public class LandingDoor : ILandingDoor
{
    public int Floor { get; private set; }
    public bool IsObstructed { get; set; }
    public DoorState State { get; private set; }

    private VirtualTimer _openingTimer = new VirtualTimer(3.0f);
    private VirtualTimer _closingTimer = new VirtualTimer(3.0f);

    public LandingDoor(int floor)
    {
        Floor = floor;
        State = DoorState.FullyClosed;
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
        if (State != DoorState.Closing) return;

        if (IsObstructed)
        {
            State = DoorState.Stalled;
            return;
        }

        _closingTimer.Tick(deltaTime);

        if (_closingTimer.IsExpired)
        {
            State = DoorState.FullyClosed;
        }
    }

    private void UpdateOpening(float deltaTime)
    {
        if (State != DoorState.Opening) return;

        _openingTimer.Tick(deltaTime);

        if (_openingTimer.IsExpired)
        {
            State = DoorState.FullyOpened;
        }
    }

    private void TransitionTo(DoorState state)
    {
        State = state;
        _openingTimer.Reset();
        _closingTimer.Reset();
    }
}