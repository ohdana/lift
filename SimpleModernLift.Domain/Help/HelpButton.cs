public class HelpButton : IHelpButton
{
    public bool IsConnectionEstablished { get; private set; }

    private readonly Guid _liftId;
    private readonly ILocationServices _locationServices;
    private readonly IHelpdeskGateway _helpdeskGateway;

    public HelpButton(Guid liftId, ILocationServices locationServices, IHelpdeskGateway helpdeskGateway)
    {
        _liftId = liftId;
        _locationServices = locationServices;
        _helpdeskGateway = helpdeskGateway;

        _helpdeskGateway.ConnectionEstablished += OnConnectionEstablished;
        _helpdeskGateway.ConnectionFailed += OnConnectionFailed;
    }

    public void Press()
    {
        var location = _locationServices.GetCurrentLocation();
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