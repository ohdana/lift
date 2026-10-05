using Xunit;
using NSubstitute;

public class AlarmButtonTests
{
    private readonly Guid _liftId;
    private readonly ILocation _currentLocation;
    private readonly ILocationService _locationService;
    private readonly IHelpdeskGateway _helpdeskGateway;
    private readonly ILogger _logger;
    private readonly IAlarmButton _alarmButton;

    public AlarmButtonTests()
    {
        _liftId = Guid.NewGuid();

        _currentLocation = Substitute.For<ILocation>();
        _locationService = Substitute.For<ILocationService>();
        _helpdeskGateway = Substitute.For<IHelpdeskGateway>();
        _logger = Substitute.For<ILogger>();

        _locationService.GetCurrentLocation().Returns(_currentLocation);
        _alarmButton = new AlarmButton(_liftId, _locationService, _helpdeskGateway, _logger);
    }

    [Fact]
    public void AlarmButton_WhenPressCalled_BeginsHelpdeskConnection()
    {
        // Arrange
        // Act
        _alarmButton.Press();

        // Assert
        _helpdeskGateway.Received().BeginHelpdeskConnection(_liftId, _currentLocation);
    }

    [Fact]
    public void AlarmButton_WhenHelpdeskConnectionEstablished_SetsIsConnectionEstablishedTrue()
    {
        // Arrange
        // Act
        _helpdeskGateway.ConnectionEstablished += Raise.Event<Action>();

        // Assert
        Assert.True(_alarmButton.IsConnectionEstablished);
    }

    [Fact]
    public void AlarmButton_WhenHelpdeskConnectionFailed_SetsIsConnectionEstablishedFalse()
    {
        // Arrange
        _helpdeskGateway.ConnectionEstablished += Raise.Event<Action>();

        // Act
        _helpdeskGateway.ConnectionFailed += Raise.Event<Action>();

        // Assert
        Assert.False(_alarmButton.IsConnectionEstablished);
    }

    [Fact]
    public void AlarmButton_WhenDisposeCalled_UnsubscribesFromHelpdesk()
    {
        // Arrange
        // Act
        _alarmButton.Dispose();

        // Assert
        _helpdeskGateway.Received().ConnectionEstablished -= Arg.Any<Action>();
        _helpdeskGateway.Received().ConnectionFailed -= Arg.Any<Action>();
    }
}