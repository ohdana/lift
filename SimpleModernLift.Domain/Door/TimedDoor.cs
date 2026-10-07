public class TimedDoor : ITimedDoor
{
    public bool IsObstructed => !_obstructionDetector.IsClear;
    public DoorState State { get; private set; }

    public event Action? TimerExpired;

    private const float OpenDurationSeconds = 3.0f;
    private const float CloseDurationSeconds = 3.0f;
    private const float TimerTimeoutSeconds = 5f;

    private readonly ILogger _logger;
    private readonly IObstructionDetector _obstructionDetector;
    private readonly string _label;
    private string _logLabelPrefix => $"[{_label}]";
    private string _logTimePrefix => $"[{DateTime.Now:HH:mm:ss}]";

    private VirtualTimer _openingTimer;
    private VirtualTimer _closingTimer;
    private VirtualTimer _timer;

    public TimedDoor(string label, IObstructionDetector obstructionDetector, ILogger logger)
    {
        State = DoorState.FullyClosed;

        _obstructionDetector = obstructionDetector;
        _logger = logger;
        _label = label;

        _openingTimer = new VirtualTimer(OpenDurationSeconds);
        _closingTimer = new VirtualTimer(CloseDurationSeconds);
        _timer = new VirtualTimer(TimerTimeoutSeconds);
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

        _timer.Tick(deltaTime);
        if (_timer.IsExpired)
        {
            TimerExpired?.Invoke();
            return;
        }
    }

    private void TransitionTo(DoorState state)
    {
        SetState(state);
        _openingTimer.Reset();
        _closingTimer.Reset();
        _timer.Reset();
    }

    private void SetState(DoorState state) => State = state;
}