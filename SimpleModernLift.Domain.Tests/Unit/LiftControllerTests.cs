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
    private ILiftController _controller;
    private ILogger _logger;
    private static readonly int _minFloor = -1;
    private static readonly int _maxFloor = 4;

    public LiftControllerTests()
    {
        var totalFloors = AllFloors.Count();

        _logger = Substitute.For<ILogger>();
        _obstructionDetector = Substitute.For<IObstructionDetector>();
        _overloadDetector = Substitute.For<IOverloadDetector>();
        _carDoor = Substitute.For<ITimedDoor>();
        _carDoor.State.Returns(DoorState.FullyClosed);

        _landingDoors = new Dictionary<int, ITimedDoor>();
        for (int i = _minFloor; i < _maxFloor; i++)
        {
            var door = Substitute.For<ITimedDoor>();
            door.State.Returns(DoorState.FullyClosed);
            _landingDoors[i] = door;
        }

        _controller = new LiftController(MotorSpeed, FloorHeight, _obstructionDetector, _overloadDetector, _carDoor, _landingDoors.AsReadOnly(), _logger);
    }

    private static IEnumerable<int> AllFloors => Enumerable.Range(_minFloor, _maxFloor - _minFloor + 1);
}