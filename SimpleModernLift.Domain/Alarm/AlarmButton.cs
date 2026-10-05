public class AlarmButton : IAlarmButton
{
    public bool IsConnectionEstablished { get; private set; }

    private readonly Guid _liftId;
    private readonly ILocationService _locationService;
    private readonly IHelpdeskGateway _helpdeskGateway;
    private readonly ILogger _logger;

    public AlarmButton(Guid liftId,
        ILocationService locationService,
        IHelpdeskGateway helpdeskGateway,
        ILogger logger)
    {
        _liftId = liftId;
        _locationService = locationService;
        _helpdeskGateway = helpdeskGateway;
        _logger = logger;

        _helpdeskGateway.ConnectionEstablished += OnConnectionEstablished;
        _helpdeskGateway.ConnectionFailed += OnConnectionFailed;
    }

    public void Press()
    {
        var location = _locationService.GetCurrentLocation();
        _helpdeskGateway.BeginHelpdeskConnection(_liftId, location);
    }

    public void Dispose()
    {
        _helpdeskGateway.ConnectionEstablished -= OnConnectionEstablished;
        _helpdeskGateway.ConnectionFailed -= OnConnectionFailed;
    }

    private void OnConnectionEstablished() => IsConnectionEstablished = true;
    private void OnConnectionFailed() => IsConnectionEstablished = false;
}