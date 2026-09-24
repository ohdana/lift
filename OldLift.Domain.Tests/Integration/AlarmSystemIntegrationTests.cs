using Xunit;
using NSubstitute;

public class AlarmSystemIntegrationTests
{
    private const float MotorSpeed = 0.71f;
    private const float FloorHeight = 3.0f;

    private readonly int _minFloor = -3;
    private readonly int _maxFloor = 5;
    private readonly ILogger _logger;
    private readonly ILiftController _controller;
    private readonly IBuzzer _buzzer;
    private readonly IAlarmButton _carAlarmButton;
    private IFloorButton[] _carFloorButtons;
    private readonly IAutomatedDoor _carDoor;

    private ILandingButton[] _landingButtons;
    private IAutomatedDoor[] _landingDoors;

    public AlarmSystemIntegrationTests()
    {
        _logger = new LiftLogger();
        _buzzer = new Buzzer(_logger);
        _carAlarmButton = new AlarmButton(_buzzer, _logger);

        var totalFloors = _maxFloor - _minFloor + 1;
        _carDoor = new AutomatedDoor(_logger);
        _landingDoors = new AutomatedDoor[totalFloors];
        for (int i = 0; i < totalFloors; i++)
        {
            _landingDoors[i] = new AutomatedDoor(_logger);
        }
        
        _controller = new LiftController(_minFloor, _carDoor, _landingDoors, MotorSpeed, FloorHeight);
        _carFloorButtons = new FloorButton[totalFloors];
        _landingButtons = new LandingButton[totalFloors];
        for (int i = 0; i < totalFloors; i++)
        {
            _carFloorButtons[i] = new FloorButton(i, _controller, _logger);
            _landingButtons[i] = new LandingButton(i, _controller, _logger);
        }
    }

    public void Buzzer_WhenAlarmButtonIsPressed_IsOn()
    {
        // Arrange
        // Act
        _carAlarmButton.Press();

        // Assert
        Assert.True(_buzzer.IsOn);
    }

    public void Buzzer_WhenAlarmButtonIsPressedThenReleased_IsOff()
    {
        // Arrange
        _carAlarmButton.Press();

        // Act
        _carAlarmButton.Release();

        // Assert
        Assert.False(_buzzer.IsOn);
    }
}
