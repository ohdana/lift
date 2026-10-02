using System.Linq;

public class LiftController
{
    public bool IsIdle => ComputeIsIdle();

    public int CurrentFloor => ComputeCurrentFloor();
    public int? TargetFloor;

    private bool _isMoving;
    private bool _isSafetyCircuitComplete => ComputeIsSafetyCircuitComplete();
    private bool _isOverloaded => _overloadDetector.IsOverloaded;
    private bool _isDoorwayClear => _obstructionDetector.IsClear;

    private float _currentPosition;
    private float? _targetPosition => TargetFloor * _floorHeight;
    private IEnumerable<int> _validFloors => _landingDoors.Keys;

    private readonly float _motorSpeed;
    private readonly float _floorHeight;
    private readonly IAutomatedDoor _carDoor;
    private readonly IReadOnlyDictionary<int, IAutomatedDoor> _landingDoors;
    private readonly IObstructionDetector _obstructionDetector;
    private readonly IOverloadDetector _overloadDetector;
    private readonly ILogger _logger;
    private string _logTimePrefix => $"[{DateTime.Now:HH:mm:ss}]";

    public LiftController(
        float motorSpeed,
        float floorHeight,
        IObstructionDetector obstructionDetector,
        IOverloadDetector overloadDetector,
        IAutomatedDoor carDoor,
        IReadOnlyDictionary<int, IAutomatedDoor> landingDoors,
        ILogger logger)
    {
        _carDoor = carDoor;
        _landingDoors = landingDoors;

        _motorSpeed = motorSpeed;
        _floorHeight = floorHeight;
        _obstructionDetector = obstructionDetector;
        _overloadDetector = overloadDetector;
        _logger = logger;

        _isMoving = false;
        _currentPosition = 0f;
    }

    public void RegisterCarCall(int floor)
    {
        if (!_validFloors.Contains(floor))
        {
            _logger.Log($"{_logTimePrefix} Rejected invalid car call to floor: {floor}");
            return;
        }

        if (TargetFloor != null) return;
        
        SetTargetFloor(floor);
    }

    public void RegisterLandingCall(int floor)
    {
        if (!_validFloors.Contains(floor))
        {
            _logger.Log($"{_logTimePrefix} Rejected invalid landing call to floor: {floor}");
            return;
        }

        if (!IsIdle) return;

        SetTargetFloor(floor);
    }
    
    private void Update(float deltaTime)
    {
        UpdateDoors(deltaTime);

        if (_isMoving)
        {
            UpdateMovingLift(deltaTime);
        }
        else
        {
            UpdateNotMovingLift();
        }
    }

    private void UpdateMovingLift(float deltaTime)
    {
        var updateDistance = deltaTime * _motorSpeed;
        MoveCar(updateDistance, _targetPosition!.Value);

        var isTargetReached = _targetPosition == _currentPosition;
        if (isTargetReached)
        {
            SetTargetFloor(null);
            StopMoving();
            StartOpeningDoors();
            return;
        }
    }

    private void UpdateNotMovingLift()
    {
        if (!_isSafetyCircuitComplete) return;
        if (TargetFloor != null)
        {
            StartMoving();
        }
    }

    private void StartMoving()
    {
        _isMoving = true;
        _logger.Log($"{_logTimePrefix} Lift starts moving...");   
    }

    private void StopMoving()
    {
        _isMoving = false;
        _logger.Log($"{_logTimePrefix} Lift stopped.");   
    }

    private void UpdateDoors(float deltaTime)
    {
        _carDoor.Update(deltaTime);
        
        foreach (var door in _landingDoors.Values)
        {
            door.Update(deltaTime);
        }
    }

    private void StartOpeningDoors()
    {
        _carDoor.StartOpening();
        _landingDoors[CurrentFloor].StartOpening();
    }

    private void StartClosingDoors()
    {
        _carDoor.StartClosing();
        _landingDoors[CurrentFloor].StartClosing();
    }

    private void MoveCar(float distance, float targetPosition)
    {
        if (_currentPosition < targetPosition)
        {
            _currentPosition += distance;
            if (_currentPosition >= targetPosition)
            {
                _currentPosition = targetPosition;
            }
        }
        else if (_currentPosition > targetPosition)
        {
            _currentPosition -= distance;
            if (_currentPosition <= targetPosition)
            {
                _currentPosition = targetPosition;
            }
        }
    }

    private bool ComputeIsSafetyCircuitComplete()
    {
        var isCarDoorClosed = _carDoor.State == DoorState.FullyClosed;
        var isLandingDoorClosed = _landingDoors[CurrentFloor].State == DoorState.FullyClosed;

        return isCarDoorClosed && isLandingDoorClosed;
    }

    private bool ComputeIsIdle() => _isSafetyCircuitComplete && TargetFloor == null;

    private int ComputeCurrentFloor() => (int)Math.Round(_currentPosition / _floorHeight);

    private void SetTargetFloor(int? floor) => TargetFloor = floor;
}