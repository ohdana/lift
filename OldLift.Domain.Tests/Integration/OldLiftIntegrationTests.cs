using Xunit;
using NSubstitute;

public class OldLiftIntegrationTests
{
    private const float TickSize = 0.01f;
    private const float MotorSpeed = 0.71f;
    private const float FloorHeight = 3.0f;
    private const float SecondsPerFloor = FloorHeight / MotorSpeed;

    private static readonly int _minFloor = 0;
    private static readonly int _maxFloor = 4;
    private readonly ILiftController _controller;
    private readonly IBuzzer _buzzer;
    private readonly IAlarmButton _carAlarmButton;
    private IFloorButton[] _carFloorButtons;
    private readonly IAutomatedDoor _carDoor;

    private ILandingButton[] _landingButtons;
    private IAutomatedDoor[] _landingDoors;

    public OldLiftIntegrationTests()
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

    private void AssertMovingTowardsTargetFloorAndDoorsClosed(int currentFloor, int targetFloor)
    {
        for (int i = currentFloor; i > targetFloor ; i--)
        {
            Assert.Equal(i, _controller.CurrentFloor);
            Assert.Equal(DoorState.FullyClosed, _carDoor.State);
            AssertAllLandingDoorsFullyClosed();
            ImitateSecondsPassed(SecondsPerFloor);
        }
    }

    private void MoveLiftToFloor(int targetFloor)
    {
        _controller.RegisterCarCall(targetFloor);
        ImitateSecondsPassed(SecondsPerFloor * (_maxFloor - _minFloor) + 20);
        Assert.True(_controller.IsIdle);
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

    private void ImitateSecondsPassed(float seconds)
    {
        var ticks = (int) Math.Ceiling(seconds / TickSize);
        for (int i = 0; i < ticks; i++)
        {
            _controller.Update(TickSize);
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
}
