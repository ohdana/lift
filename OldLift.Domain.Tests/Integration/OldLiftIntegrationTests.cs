using Xunit;
using NSubstitute;

public class OldLiftIntegrationTests
{
    private const float TickSize = 0.01f;
    private const float MotorSpeed = 0.71f;
    private const float FloorHeight = 3.0f;
    private const float SecondsPerFloor = FloorHeight / MotorSpeed;

    private static readonly int _minFloor = -3;
    private static readonly int _maxFloor = 5;
    private readonly ILogger _logger;
    private readonly ILiftController _controller;
    private readonly IBuzzer _buzzer;
    private readonly IAlarmButton _carAlarmButton;
    private IFloorButton[] _carFloorButtons;
    private readonly IAutomatedDoor _carDoor;

    private ILandingButton[] _landingButtons;
    private IAutomatedDoor[] _landingDoors;

    public OldLiftIntegrationTests()
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
            _carFloorButtons[i] = new FloorButton(i + _minFloor, _controller, _logger);
            _landingButtons[i] = new LandingButton(i + _minFloor, _controller, _logger);
        }
    }

    [Theory]
    [MemberData(nameof(GetDescendingJourneyCombinations))]
    public void DescendingJourney_WhenLandingButtonPressed_LiftOpensSuccessfullyAtTargetFloor(int currentFloor, int targetFloor)
    {
        // Arrange
        MoveLiftToFloor(currentFloor);

        // Act
        _landingButtons[targetFloor - _minFloor].Press();

        // Assert
        AssertMovingTowardsTargetFloorAndDoorsClosed(currentFloor, targetFloor);  
        AssertReachedTargetFloorAndDoorsOpening(targetFloor);
    }

    [Theory]
    [MemberData(nameof(GetDescendingJourneyCombinations))]
    public void DescendingJourney_WhenCarButtonPressed_LiftOpensSuccessfullyAtTargetFloor(int currentFloor, int targetFloor)
    {
        // Arrange
        MoveLiftToFloor(currentFloor);

        // Act
        _carFloorButtons[targetFloor - _minFloor].Press();

        // Assert
        AssertMovingTowardsTargetFloorAndDoorsClosed(currentFloor, targetFloor);  
        AssertReachedTargetFloorAndDoorsOpening(targetFloor);
    }

    [Theory]
    [MemberData(nameof(GetAscendingJourneyCombinations))]
    public void AscendingJourney_WhenLandingButtonPressed_LiftOpensSuccessfullyAtTargetFloor(int currentFloor, int targetFloor)
    {
        // Arrange
        MoveLiftToFloor(currentFloor);

        // Act
        _landingButtons[targetFloor - _minFloor].Press();

        // Assert
        AssertMovingTowardsTargetFloorAndDoorsClosed(currentFloor, targetFloor);  
        AssertReachedTargetFloorAndDoorsOpening(targetFloor);
    }

    [Theory]
    [MemberData(nameof(GetAscendingJourneyCombinations))]
    public void AscendingJourney_WhenCarButtonPressed_LiftOpensSuccessfullyAtTargetFloor(int currentFloor, int targetFloor)
    {
        // Arrange
        MoveLiftToFloor(currentFloor);

        // Act
        _carFloorButtons[targetFloor - _minFloor].Press();

        // Assert
        AssertMovingTowardsTargetFloorAndDoorsClosed(currentFloor, targetFloor);  
        AssertReachedTargetFloorAndDoorsOpening(targetFloor);
    }

    [Theory]
    [MemberData(nameof(GetAllFloors))]
    public void ZeroDistanceJourney_WhenLandingButtonPressed_LiftOpensSuccessfullyAtTargetFloor(int floor)
    {
        // Arrange
        MoveLiftToFloor(floor);

        // Act
        _landingButtons[floor - _minFloor].Press();

        // Assert
        ImitateSecondsPassed(TickSize);
        AssertReachedTargetFloorAndDoorsOpening(floor);
    }

    [Theory]
    [MemberData(nameof(GetAllFloors))]
    public void ZeroDistanceJourney_WhenCarButtonPressed_LiftOpensSuccessfullyAtTargetFloor(int floor)
    {
        // Arrange
        MoveLiftToFloor(floor);

        // Act
        _carFloorButtons[floor - _minFloor].Press();

        // Assert
        ImitateSecondsPassed(TickSize);
        AssertReachedTargetFloorAndDoorsOpening(floor);
    }

    private void ImitateSecondsPassed(float seconds)
    {
        var ticks = (int) Math.Ceiling(seconds / TickSize);
        for (int i = 0; i < ticks; i++)
        {
            _controller.Update(TickSize);
        }
    }

    private void MoveLiftToFloor(int targetFloor)
    {
        _controller.RegisterCarCall(targetFloor);
        ImitateSecondsPassed(SecondsPerFloor * (_maxFloor - _minFloor) + 20);
        Assert.True(_controller.IsIdle);
    }

    private void AssertMovingTowardsTargetFloorAndDoorsClosed(int currentFloor, int targetFloor)
    {
        var floorCount = Math.Abs(currentFloor - targetFloor);
        var direction = currentFloor > targetFloor ? (-1) : 1;

        var floor = currentFloor;
        for (int i = 0; i < floorCount; i++, floor += direction)
        {
            Assert.Equal(floor, _controller.CurrentFloor);
            Assert.Equal(DoorState.FullyClosed, _carDoor.State);
            AssertAllLandingDoorsFullyClosed();
            ImitateSecondsPassed(SecondsPerFloor);
        }
    }

    private void AssertReachedTargetFloorAndDoorsOpening(int targetFloor)
    {
        Assert.Equal(targetFloor, _controller.CurrentFloor);
        AssertDoorsInState(targetFloor - _minFloor, DoorState.Opening);
    }

    private void AssertDoorsInState(int floor, DoorState state)
    {
        Assert.Equal(state, _carDoor.State);
        Assert.Equal(state, _landingDoors[floor].State);
    }

    private void AssertAllLandingDoorsFullyClosed()
    {
        Assert.All(_landingDoors, door => Assert.Equal(DoorState.FullyClosed, door.State));
    }

    public static IEnumerable<object[]> GetDescendingJourneyCombinations()
    {
        for (int i = _maxFloor; i > _minFloor; i--)
        {
            for (int j = i - 1; j >= _minFloor; j--)
            {
                yield return new object[] { i, j };
            }
        }
    }

    public static IEnumerable<object[]> GetAscendingJourneyCombinations()
    {
        for (int i = _minFloor; i < _maxFloor; i++)
        {
            for (int j = i + 1; j <= _maxFloor; j++)
            {
                yield return new object[] { i, j };
            }
        }
    }

    public static IEnumerable<object[]> GetAllFloors()
    {
        for (int i = _minFloor; i < _maxFloor; i++)
        {
            yield return new object[] { i };
        }
    }
}
