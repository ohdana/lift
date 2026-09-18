public class LiftController : ILiftController
{
    public int? TargetFloor => ComputeTargetFloor();
    public bool IsIdle => ComputeIsIdle();

    private bool _isMoving;
    private bool _isSafetyCircuitComplete => ComputeIsSafetyCircuitComplete();
    private float _carPosition;
    private float _targetPosition => TargetFloor.HasValue ? TargetFloor.Value * FloorHeight : 0f;

    private readonly int _minFloor;
    private readonly int _maxFloor;
    private readonly int _totalFloors;
    
    private readonly bool[] _floorRelays;

    private readonly IAutomatedDoor _carDoor;
    private readonly IAutomatedDoor[] _landingDoors;
    private IAutomatedDoor _currentLandingDoor;

    private const float MotorSpeed = 0.71f;
    private const float FloorHeight = 3.0f;

    public LiftController(int minFloor, int maxFloor, IAutomatedDoor carDoor, IAutomatedDoor[] landingDoors)
    {
        _minFloor = minFloor;
        _maxFloor = maxFloor;
        _totalFloors = maxFloor - minFloor + 1;
        _floorRelays = new bool[_totalFloors];

        _isMoving = false;
        _carPosition = _minFloor;
        _carDoor = carDoor;
        _landingDoors = landingDoors;
        _currentLandingDoor = _landingDoors[0 - _minFloor];
    }

    public void RegisterCarCall(int floor)
    {
        if (TargetFloor != null) return;
        LatchFloorRelay(_floorRelays, floor);
    }

    public void RegisterLandingCall(int floor)
    {
        if (!IsIdle) return;
        LatchFloorRelay(_floorRelays, floor);
    }

    public void Update(float deltaTime)
    {
        UpdateDoors(deltaTime);

        if (_isSafetyCircuitComplete && TargetFloor != null)
        {
            _isMoving = true;
        }

        if (_isMoving)
        {
            var step = deltaTime * MotorSpeed;
            MoveCar(step);

            var isTargetReached = _carPosition == _targetPosition;
            if (isTargetReached)
            {
                _isMoving = false;
                _currentLandingDoor = _landingDoors[TargetFloor!.Value - _minFloor];
                UnlatchFloorRelays();
                StartOpeningDoors();
            }
        }
    }

    private void MoveCar(float distance)
    {
        if (_carPosition < _targetPosition)
        {
            _carPosition += distance;
            if (_carPosition >= _targetPosition)
            {
                _carPosition = _targetPosition;
            }
        }
        else if (_carPosition > _targetPosition)
        {
            _carPosition -= distance;
            if (_carPosition <= _targetPosition)
            {
                _carPosition = _targetPosition;
            }
        }
    }

    private void StartOpeningDoors()
    {
        _carDoor.StartOpening();
        _currentLandingDoor.StartOpening();
    }

    private void UpdateDoors(float deltaTime)
    {
        _carDoor.Update(deltaTime);
        for (int i = 0; i < _totalFloors; i++)
        {
            _landingDoors[i].Update(deltaTime);
        }
    }

    private void LatchFloorRelay(bool[] relays, int floor) => relays[floor - _minFloor] = true;
    private void UnlatchFloorRelays()
    {
        var index = TargetFloor!.Value - _minFloor;
        _floorRelays[index] = false;
    }

    private int? ComputeTargetFloor()
    {
        for (int i = 0; i < _totalFloors; i++)
        {
            if (_floorRelays[i])
            {
                return i + _minFloor; 
            }
        }
        return null;
    }

    private bool ComputeIsIdle()
    {
        return _isSafetyCircuitComplete && (TargetFloor == null);
    }

    private bool ComputeIsSafetyCircuitComplete()
    {
        var isCarDoorClosed = _carDoor.State == DoorState.FullyClosed;
        var isLandingDoorClosed = _currentLandingDoor?.State == DoorState.FullyClosed;

        return isCarDoorClosed && isLandingDoorClosed;
    }
}