using Xunit;
using NSubstitute;

public class LiftControllerTests
{
    private readonly IAutomatedDoor _carDoor;
    private readonly IAutomatedDoor[] _landingDoors;
    private ILiftController _controller;
    private static readonly int _minFloor = -3;
    private static readonly int _maxFloor = 5;

    public LiftControllerTests()
    {
        var totalFloors = _maxFloor - _minFloor + 1;

        _carDoor = Substitute.For<IAutomatedDoor>();
        _carDoor.State.Returns(DoorState.FullyClosed);

        _landingDoors = new IAutomatedDoor[totalFloors];
        for (int i = 0; i < totalFloors; i++)
        {
            _landingDoors[i] = Substitute.For<IAutomatedDoor>();
            _landingDoors[i].State.Returns(DoorState.FullyClosed);
        }

        _controller = new LiftController(_minFloor, _maxFloor, _carDoor, _landingDoors);
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
        for (int floor = _minFloor; floor <= _maxFloor; floor++)
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
        for (int floor = _minFloor; floor <= _maxFloor; floor++)
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
        for (int floor = _minFloor; floor <= _maxFloor; floor++)
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
        for (int floor = _minFloor; floor <= _maxFloor; floor++)
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
        _landingDoors[targetFloor - _minFloor].State.Returns(state);

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
        _landingDoors[targetFloor - _minFloor].State.Returns(state);

        // Act
        _controller.RegisterLandingCall(targetFloor);

        // Assert
        Assert.Null(_controller.TargetFloor);
    }

    public static IEnumerable<object[]> GetEachFloor()
    {
        for (int floor = _minFloor; floor <= _maxFloor; floor++)
        {
            yield return new object[] { floor };
        }
    }

    public static IEnumerable<object[]> GetEachDoorStateExceptFullyClosed =>
        new List<object[]>
        {
            new object[] { DoorState.Opening },
            new object[] { DoorState.FullyOpened },
            new object[] { DoorState.Closing },
            new object[] { DoorState.Stalled }
        };

    public static IEnumerable<object[]> GetNotFullyClosedDoorStateAndFloorCombinations()
    {
        var doorStates = Enum.GetValues<DoorState>().Where(state => state != DoorState.FullyClosed);
        int totalCount = _maxFloor - _minFloor + 1;

        foreach (var state in doorStates)
        {
            foreach (var floor in Enumerable.Range(_minFloor, totalCount))
            {
                yield return new object[] { state, floor };
            }
        }
    }
}
