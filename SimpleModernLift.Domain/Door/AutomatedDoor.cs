public class AutomatedDoor : IAutomatedDoor
{
    public bool IsObstructed { get; private set; }
    public DoorState State { get; private set; }

    private const float OpenDurationSeconds = 3.0f;
    private const float CloseDurationSeconds = 3.0f;
    private const float AutoCloseTimeoutSeconds = 5f;

    private readonly ILogger _logger;
    private readonly string _label;
    private string _logLabelPrefix => $"[{_label}]";
    private string _logTimePrefix => $"[{DateTime.Now:HH:mm:ss}]";

    private VirtualTimer _openingTimer;
    private VirtualTimer _closingTimer;
    private VirtualTimer _autoCloseTimer;

    public AutomatedDoor(ILogger logger, string label)
    {
        State = DoorState.FullyClosed;

        _logger = logger;
        _label = label;

        _openingTimer = new VirtualTimer(OpenDurationSeconds);
        _closingTimer = new VirtualTimer(CloseDurationSeconds);
        _autoCloseTimer = new VirtualTimer(AutoCloseTimeoutSeconds);
    }

    public void StartOpening()
    {
        TransitionTo(DoorState.Opening);
        _logger.Log($"{_logTimePrefix} {_logLabelPrefix} Door starts opening...");
    }

    public void StartClosing()
    {
        TransitionTo(DoorState.Closing);
        _logger.Log($"{_logTimePrefix} {_logLabelPrefix} Door starts closing...");
    }

    public void Update(float deltaTime)
    {
        if (State == DoorState.Opening)
        {
            UpdateOpening(deltaTime);
        }
        else if (State == DoorState.FullyOpened)
        {
            UpdateAutoClosingTimeout(deltaTime);
        }
        else if (State == DoorState.Closing)
        {
            UpdateClosing(deltaTime);
        }
    }

    private void UpdateOpening(float deltaTime)
    {
        if (State != DoorState.Opening) return;

        _openingTimer.Tick(deltaTime);
        if (_openingTimer.IsExpired)
        {
            TransitionTo(DoorState.FullyOpened);
            _logger.Log($"{_logTimePrefix} {_logLabelPrefix} Door fully open!");
        }
    }

    private void UpdateClosing(float deltaTime)
    {
        if (State != DoorState.Closing) return;

        if (IsObstructed)
        {
            StartOpening();
            return;
        }

        _closingTimer.Tick(deltaTime);
        if (_closingTimer.IsExpired)
        {
            TransitionTo(DoorState.FullyClosed);
            _logger.Log($"{_logTimePrefix} {_logLabelPrefix} Door fully closed!");
        }
    }

    private void UpdateAutoClosingTimeout(float deltaTime)
    {
        if (State != DoorState.FullyOpened) return;

        if (IsObstructed) return;

        _autoCloseTimer.Tick(deltaTime);
        if (_autoCloseTimer.IsExpired)
        {
            StartClosing();
            return;
        }
    }

    private void TransitionTo(DoorState state)
    {
        SetState(state);
        _openingTimer.Reset();
        _closingTimer.Reset();
        _autoCloseTimer.Reset();
    }

    private void SetState(DoorState state) => State = state;
}