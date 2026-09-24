public class AutomatedDoor : IAutomatedDoor
{
    public bool IsObstructed { get; set; }
    public DoorState State { get; private set; }

    private const float OpenDurationSeconds = 3.0f;
    private const float CloseDurationSeconds = 3.0f;
    private const float AutoCloseTimeoutSeconds = 5f;

    private readonly ILogger _logger;

    private VirtualTimer _openingTimer;
    private VirtualTimer _closingTimer;
    private VirtualTimer _autoCloseTimer;

    public AutomatedDoor(ILogger logger)
    {
        State = DoorState.FullyClosed;
        IsObstructed = false;

        _logger = logger;

        _openingTimer = new VirtualTimer(OpenDurationSeconds);
        _closingTimer = new VirtualTimer(CloseDurationSeconds);
        _autoCloseTimer = new VirtualTimer(CloseDurationSeconds);
    }

    public void StartOpening()
    {
        TransitionTo(DoorState.Opening);
        _logger.Log("Door starts opening...");
    }

    public void StartClosing()
    {
        TransitionTo(DoorState.Closing);
        _logger.Log("Door starts closing...");
    }

    public void Update(float deltaTime)
    {
        if (State == DoorState.Opening)
        {
            UpdateOpening(deltaTime);
        }
        else if (State == DoorState.FullyOpened)
        {
            UpdateAutoClosing(deltaTime);
        }
        else if (State == DoorState.Closing)
        {
            UpdateClosing(deltaTime);
        }
    }

    private void UpdateAutoClosing(float deltaTime)
    {
        if (State != DoorState.FullyOpened) return;

        _autoCloseTimer.Tick(deltaTime);
        if (_autoCloseTimer.IsExpired)
        {
            StartClosing();
        }
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
            _logger.Log("Door fully closed!");
        }
    }

    private void UpdateOpening(float deltaTime)
    {
        if (State != DoorState.Opening) return;

        _openingTimer.Tick(deltaTime);
        if (_openingTimer.IsExpired)
        {
            SetState(DoorState.FullyOpened);
            _autoCloseTimer.Reset();
            _logger.Log("Door fully open!");
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