using Xunit;
using NSubstitute;

public class AlarmSystemIntegrationTests
{
    private const float MotorSpeed = 0.71f;
    private const float FloorHeight = 3.0f;

    private readonly int _minFloor = -3;
    private readonly int _maxFloor = 5;
    private readonly ILiftController _controller;
    private readonly IBuzzer _buzzer;
    private readonly IAlarmButton _carAlarmButton;
    private IFloorButton[] _carFloorButtons;
    private readonly IAutomatedDoor _carDoor;

    private ILandingButton[] _landingButtons;
    private IAutomatedDoor[] _landingDoors;

    public AlarmSystemIntegrationTests()
    {
        _buzzer = new Buzzer();
        _carAlarmButton = new AlarmButton(_buzzer);

        var totalFloors = _maxFloor - _minFloor + 1;
        _carDoor = new AutomatedDoor();
        _landingDoors = new AutomatedDoor[totalFloors];
        for (int i = 0; i < totalFloors; i++)
        {
            _landingDoors[i] = new AutomatedDoor();
        }
        
        _controller = new LiftController(_minFloor, _carDoor, _landingDoors, MotorSpeed, FloorHeight);
        _carFloorButtons = new FloorButton[totalFloors];
        _landingButtons = new LandingButton[totalFloors];
        for (int i = 0; i < totalFloors; i++)
        {
            _carFloorButtons[i] = new FloorButton(i, _controller);
            _landingButtons[i] = new LandingButton(i, _controller);
        }
    }

    public void Buzzer_WhileAlarmButtonIsPressed_IsOnUntilButtonIsReleased()
    {
        // Arrange
        // Act
        // Assert
    }
}
