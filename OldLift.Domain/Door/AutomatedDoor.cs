public class AutomatedDoor : IAutomatedDoor
{
    public bool IsObstructed { get; set; }
    public DoorState State { get; private set; }

    private const float OpenDurationSeconds = 3.0f;
    private const float CloseDurationSeconds = 3.0f;

    private VirtualTimer _openingTimer;
    private VirtualTimer _closingTimer;

    public AutomatedDoor()
    {
        State = DoorState.FullyClosed;
        IsObstructed = false;

        _openingTimer = new VirtualTimer(OpenDurationSeconds);
        _closingTimer = new VirtualTimer(CloseDurationSeconds);
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
            SetState(DoorState.Stalled);
            return;
        }

        _closingTimer.Tick(deltaTime);
        if (_closingTimer.IsExpired)
        {
            SetState(DoorState.FullyClosed);
        }
    }

    private void UpdateOpening(float deltaTime)
    {
        if (State != DoorState.Opening) return;

        _openingTimer.Tick(deltaTime);
        if (_openingTimer.IsExpired)
        {
            SetState(DoorState.FullyOpened);
        }
    }

    private void TransitionTo(DoorState state)
    {
        SetState(state);
        _openingTimer.Reset();
        _closingTimer.Reset();
    }

    private void SetState(DoorState state) => State = state;
}