using Xunit;
using NSubstitute;

public class SimpleModerniftIntegrationTests
{
    private const float TickSize = 0.01f;
    private const float MotorSpeed = 0.71f;
    private const float FloorHeight = 3.0f;
    private const float SecondsPerFloor = FloorHeight / MotorSpeed;

    private static readonly int _minFloor = -1;
    private static readonly int _maxFloor = 4;
    private readonly ILogger _logger;
    private readonly Guid _liftId;
    private readonly ILiftController _controller;
    private readonly ILocationService _locationService;
    private readonly IHelpdeskGateway _helpdeskGateway;
    private readonly IAlarmButton _carAlarmButton;
    private IDictionary<int, IFloorButton> _carFloorButtons;
    private IDictionary<int, IFloorButton> _landingButtons;
    private readonly ITimedDoor _carDoor;
    private readonly IDictionary<int, ITimedDoor> _landingDoors;
    private readonly IObstructionDetector _carDoorObstructionDetector;
    private readonly IDictionary<int, IObstructionDetector> _landingDoorObstructionDetectors;
    private readonly IOverloadDetector _overloadDetector;

    public SimpleModerniftIntegrationTests()
    {
        _carDoorObstructionDetector = Substitute.For<IObstructionDetector>();
        _carDoorObstructionDetector.IsClear.Returns(true);
        _overloadDetector = Substitute.For<IOverloadDetector>();
        _overloadDetector.IsOverloaded.Returns(false);
        _locationService = Substitute.For<ILocationService>();
        _helpdeskGateway = Substitute.For<IHelpdeskGateway>();

        _liftId = Guid.NewGuid();
        _logger = new LiftLogger();
        _carAlarmButton = new AlarmButton(_liftId, _locationService, _helpdeskGateway, _logger);
        _carFloorButtons = new Dictionary<int, IFloorButton>();
        _landingButtons = new Dictionary<int, IFloorButton>();
        _carDoor = new TimedDoor("Car", _carDoorObstructionDetector, _logger);
        _landingDoorObstructionDetectors = new Dictionary<int, IObstructionDetector>();
        _landingDoors = new Dictionary<int, ITimedDoor>();

        for (int i = _minFloor; i <= _maxFloor; i++)
        {
            var obstructionDetector = Substitute.For<IObstructionDetector>();
            obstructionDetector.IsClear.Returns(true);
            _landingDoorObstructionDetectors[i] = obstructionDetector;

            var door = new TimedDoor("Landing", obstructionDetector, _logger);
            _landingDoors[i] = door;
            _landingButtons[i] = new FloorButton(i);
            _carFloorButtons[i] = new FloorButton(i);
        }

        _controller = new LiftController(MotorSpeed, FloorHeight, _carDoorObstructionDetector, _overloadDetector, _carDoor, _landingDoors.AsReadOnly(), _landingButtons.AsReadOnly(), _carFloorButtons.AsReadOnly(), _logger);
    }

    [Theory]
    [MemberData(nameof(GetDescendingJourneyCombinations))]
    public void DescendingJourney_WhenLandingButtonPressed_LiftOpensSuccessfullyAtTargetFloor(int currentFloor, int targetFloor)
    {
        // Arrange
        MoveLiftToFloor(currentFloor);

        // Act
        _landingButtons[targetFloor].Press();

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
        _carFloorButtons[targetFloor].Press();

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
        _landingButtons[targetFloor].Press();

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
        _carFloorButtons[targetFloor].Press();

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
        _landingButtons[floor].Press();

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
        _carFloorButtons[floor].Press();

        // Assert
        ImitateSecondsPassed(TickSize);
        AssertReachedTargetFloorAndDoorsOpening(floor);
    }

    [Theory]
    [MemberData(nameof(GetAllFloors))]
    public void Lift_WhenOverloaded_DoesntCloseDoors(int floor)
    {
        // Arrange
        OpenDoors();
        _overloadDetector.IsOverloaded.Returns(true);

        // Act
        _carFloorButtons[floor].Press();

        // Assert
        ImitateSecondsPassed(10);
        AssertDoorsInState(_controller.CurrentFloor, DoorState.FullyOpened);
    }

    [Theory]
    [MemberData(nameof(GetAllFloors))]
    public void Lift_WhenOverloadCleared_ClosesDoors(int floor)
    {
        // Arrange
        OpenDoors();
        _overloadDetector.IsOverloaded.Returns(true);
        _carFloorButtons[floor].Press();
        ImitateSecondsPassed(10);

        // Act
        _overloadDetector.IsOverloaded.Returns(false);
        ImitateSecondsPassed(TickSize);

        // Assert
        AssertDoorsInState(_controller.CurrentFloor, DoorState.Closing);
    }

    private void OpenDoors()
    {
        _carFloorButtons[_controller.CurrentFloor].Press();
        while (_carDoor.State != DoorState.FullyOpened)
        {
            ImitateSecondsPassed(TickSize);
        }
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

    private void AssertAllLandingDoorsFullyClosed()
    {
        Assert.All(_landingDoors.Values, door => Assert.Equal(DoorState.FullyClosed, door.State));
    }

    private void AssertReachedTargetFloorAndDoorsOpening(int targetFloor)
    {
        ImitateSecondsPassed(TickSize);
        Assert.Equal(targetFloor, _controller.CurrentFloor);
        AssertDoorsInState(targetFloor, DoorState.Opening);
    }

    private void AssertDoorsInState(int floor, DoorState state)
    {
        Assert.Equal(state, _carDoor.State);
        Assert.Equal(state, _landingDoors[floor].State);
    }

    private void ImitateSecondsPassed(float seconds)
    {
        var ticks = (int) Math.Ceiling(seconds / TickSize);
        for (int i = 0; i < ticks; i++)
        {
            _controller.Update(TickSize);
        }
    }
}