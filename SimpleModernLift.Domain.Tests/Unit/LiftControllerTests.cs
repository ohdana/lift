using Xunit;
using NSubstitute;

public class LiftControllerTests
{
    private const float MotorSpeed = 0.71f;
    private const float FloorHeight = 3.0f;

    private readonly IObstructionDetector _obstructionDetector;
    private readonly IOverloadDetector _overloadDetector;
    private readonly ITimedDoor _carDoor;
    private readonly IDictionary<int, ITimedDoor> _landingDoors;
    private readonly IDictionary<int, IFloorButton> _landingButtons;
    private readonly IDictionary<int, IFloorButton> _carFloorButtons;
    private ILiftController _controller;
    private ILogger _logger;
    private static readonly int _minFloor = -1;
    private static readonly int _maxFloor = 5;

    public LiftControllerTests()
    {
        var totalFloors = AllFloors.Count();

        _logger = Substitute.For<ILogger>();
        _obstructionDetector = Substitute.For<IObstructionDetector>();
        _overloadDetector = Substitute.For<IOverloadDetector>();
        _carDoor = Substitute.For<ITimedDoor>();
        _carDoor.State.Returns(DoorState.FullyClosed);

        _landingDoors = new Dictionary<int, ITimedDoor>();
        _landingButtons = new Dictionary<int, IFloorButton>();
        _carFloorButtons = new Dictionary<int, IFloorButton>();
        for (int i = _minFloor; i <= _maxFloor; i++)
        {
            var door = Substitute.For<ITimedDoor>();
            door.State.Returns(DoorState.FullyClosed);
            _landingDoors[i] = door;
            _landingButtons[i] = Substitute.For<IFloorButton>();
            _carFloorButtons[i] = Substitute.For<IFloorButton>();
        }

        _controller = new LiftController(MotorSpeed, FloorHeight, _obstructionDetector, _overloadDetector, _carDoor, _landingDoors.AsReadOnly(), _landingButtons.AsReadOnly(), _carFloorButtons.AsReadOnly(), _logger);
    }

    [Fact]
    public void LiftController_WhenCreated_IsIdleAndHasNoTargetFloor()
    {
        // Arrange
        // Act
        // Assert
        Assert.True(_controller.IsIdle);
        Assert.Null(_controller.TargetFloor);
    }

    [Theory]
    [MemberData(nameof(GetEachFloor))]
    public void LiftController_WhenIdleAndRegisteredCarCall_BecomesNotIdleAndSetsTargetFloor(int targetFloor)
    {
        // Arrange
        // Act
        _controller.RegisterCarCall(targetFloor);

        // Assert
        Assert.False(_controller.IsIdle);
        Assert.Equal(targetFloor, _controller.TargetFloor!);
    }

    [Theory]
    [MemberData(nameof(GetEachFloor))]
    public void LiftController_WhenRegisteredSubsequentCarCalls_IgnoresAllButFirst(int firstTargetFloor)
    {
        // Arrange
        _controller.RegisterCarCall(firstTargetFloor);

        // Act
        foreach (var floor in AllFloors)
        {
            _controller.RegisterCarCall(floor);
        }

        // Assert
        Assert.Equal(firstTargetFloor, _controller.TargetFloor!);
    }

    [Theory]
    [MemberData(nameof(GetEachFloor))]
    public void LiftController_WhenRegisteredCarCallThenLandingCall_IgnoresLandingCall(int firstTargetFloor)
    {
        // Arrange
        _controller.RegisterCarCall(firstTargetFloor);

        // Act
        foreach (var floor in AllFloors)
        {
            _controller.RegisterLandingCall(floor);
        }

        // Assert
        Assert.Equal(firstTargetFloor, _controller.TargetFloor!);
    }

    [Theory]
    [MemberData(nameof(GetEachFloor))]
    public void LiftController_WhenIdleAndRegisteredLandingCall_BecomesNotIdleAndSetsTargetFloor(int targetFloor)
    {
        // Arrange
        // Act
        _controller.RegisterLandingCall(targetFloor);

        // Assert
        Assert.False(_controller.IsIdle);
        Assert.Equal(targetFloor, _controller.TargetFloor!);
    }

    [Theory]
    [MemberData(nameof(GetEachFloor))]
    public void LiftController_WhenRegisteredSubsequentLandingCalls_IgnoresAllButFirst(int firstTargetFloor)
    {
        // Arrange
        _controller.RegisterLandingCall(firstTargetFloor);

        // Act
        foreach (var floor in AllFloors)
        {
            _controller.RegisterLandingCall(floor);
        }

        // Assert
        Assert.Equal(firstTargetFloor, _controller.TargetFloor!);
    }

    [Theory]
    [MemberData(nameof(GetEachFloor))]
    public void LiftController_WhenRegisteredLandingCallThenCarCall_IgnoresCarCall(int firstTargetFloor)
    {
        // Arrange
        _controller.RegisterLandingCall(firstTargetFloor);

        // Act
        foreach (var floor in AllFloors)
        {
            _controller.RegisterCarCall(floor);
        }

        // Assert
        Assert.Equal(firstTargetFloor, _controller.TargetFloor!);
    }

    [Theory]
    [MemberData(nameof(GetNotFullyClosedDoorStateAndFloorCombinations))]
    public void LiftController_WhenDoorsNotFullyClosedAndRegisteredCarCall_SetsTargetFloor(DoorState state, int targetFloor)
    {
        // Arrange
        _carDoor.State.Returns(state);
        _landingDoors[targetFloor].State.Returns(state);

        // Act
        _controller.RegisterCarCall(targetFloor);

        // Assert
        Assert.Equal(targetFloor, _controller.TargetFloor!);
    }

    [Theory]
    [MemberData(nameof(GetNotFullyClosedDoorStateAndFloorCombinations))]
    public void LiftController_WhenDoorsNotFullyClosedAndRegisteredLandingCall_IgnoresLandingCall(DoorState state, int targetFloor)
    {
        _carDoor.State.Returns(state);
        _landingDoors[targetFloor].State.Returns(state);

        // Act
        _controller.RegisterLandingCall(targetFloor);

        // Assert
        Assert.Null(_controller.TargetFloor);
    }

    public static IEnumerable<object[]> GetEachDoorStateExceptFullyClosed =>
        new List<object[]>
        {
            new object[] { DoorState.Opening },
            new object[] { DoorState.FullyOpened },
            new object[] { DoorState.Closing }
        };

    public static IEnumerable<object[]> GetNotFullyClosedDoorStateAndFloorCombinations()
    {
        var doorStates = Enum.GetValues<DoorState>().Where(state => state != DoorState.FullyClosed);

        foreach (var state in doorStates)
        {
            for (int floor = _minFloor; floor <= _maxFloor; floor++)
            {
                yield return new object[] { state, floor };
            }
        }
    }

    public static IEnumerable<object[]> GetEachFloor()
    {
        foreach (var floor in AllFloors)
        {
            yield return new object[] { floor };
        }
    }

    private static IEnumerable<int> AllFloors => Enumerable.Range(_minFloor, _maxFloor - _minFloor + 1);
}