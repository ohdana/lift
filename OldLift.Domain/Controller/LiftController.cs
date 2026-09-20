public class LiftController : ILiftController
{
    public int? TargetFloor => ComputeTargetFloor();
    public bool IsIdle => ComputeIsIdle();

    private bool _isMoving;
    private bool _isSafetyCircuitComplete => ComputeIsSafetyCircuitComplete();

    private int _normalisationOffset;
    private int? _normalisedTargetFloor => ComputeNormalisedTargetFloor();
    private float _carPosition;
    private float? _targetPosition => ComputeTargetPosition();

    private readonly bool[] _floorRelays;

    private readonly IAutomatedDoor _carDoor;
    private readonly IAutomatedDoor[] _landingDoors;

    private readonly float _motorSpeed;
    private readonly float _floorHeight;

    public LiftController(int minFloor, 
        IAutomatedDoor carDoor, 
        IAutomatedDoor[] landingDoors,
        float motorSpeed,
        float floorHeight)
    {
        _motorSpeed = motorSpeed;
        _floorHeight = floorHeight;
        _normalisationOffset = minFloor;
        _floorRelays = new bool[landingDoors.Count()];

        _isMoving = false;
        _carPosition = 0f;
        _carDoor = carDoor;
        _landingDoors = landingDoors;
    }

    public void RegisterCarCall(int floor)
    {
        if (_normalisedTargetFloor != null) return;

        var normalisedRequestedFloor = floor - _normalisationOffset;
        LatchFloorRelay(_floorRelays, normalisedRequestedFloor);
    }

    public void RegisterLandingCall(int floor)
    {
        if (!IsIdle) return;

        var normalisedRequestedFloor = floor - _normalisationOffset;
        LatchFloorRelay(_floorRelays, normalisedRequestedFloor);
    }

    public void Update(float deltaTime)
    {
        UpdateDoors(deltaTime);

        if (_isSafetyCircuitComplete && _normalisedTargetFloor != null)
        {
            _isMoving = true;
        }

        if (_isMoving)
        {
            var step = deltaTime * _motorSpeed;
            MoveCar(step, _targetPosition!.Value);

            var isTargetReached = _carPosition == _targetPosition!;
            if (isTargetReached)
            {
                _isMoving = false;
                UnlatchFloorRelays();
                StartOpeningDoors();
            }
        }
    }

    private void UpdateDoors(float deltaTime)
    {
        _carDoor.Update(deltaTime);
        for (int i = 0; i < _landingDoors.Count(); i++)
        {
            _landingDoors[i].Update(deltaTime);
        }
    }

    private void LatchFloorRelay(bool[] relays, int floor) => relays[floor] = true;
    private void UnlatchFloorRelays() => _floorRelays[_normalisedTargetFloor!.Value] = false;

    private void MoveCar(float distance, float targetPosition)
    {
        if (_carPosition < targetPosition)
        {
            _carPosition += distance;
            if (_carPosition >= targetPosition)
            {
                _carPosition = targetPosition;
            }
        }
        else if (_carPosition > targetPosition)
        {
            _carPosition -= distance;
            if (_carPosition <= targetPosition)
            {
                _carPosition = targetPosition;
            }
        }
    }

    private void StartOpeningDoors()
    {
        _carDoor.StartOpening();

        var landingDoor = GetCurrentLandingDoor();
        landingDoor.StartOpening();
    }

    private IAutomatedDoor GetCurrentLandingDoor()
    {
        var currentFloor = (int)Math.Round(_carPosition / _floorHeight);
        return _landingDoors[currentFloor];
    }

    private int? ComputeNormalisedTargetFloor()
    {
        for (int i = 0; i < _floorRelays.Count(); i++)
        {
            if (_floorRelays[i])
            {
                return i; 
            }
        }
        return null;
    }

    private bool ComputeIsIdle() => _isSafetyCircuitComplete && (_normalisedTargetFloor == null);
    private int? ComputeTargetFloor() => _normalisedTargetFloor.HasValue ? _normalisedTargetFloor.Value + _normalisationOffset : null;
    private float? ComputeTargetPosition() => _normalisedTargetFloor.HasValue ? _normalisedTargetFloor.Value * _floorHeight : null;

    private bool ComputeIsSafetyCircuitComplete()
    {
        var isCarDoorClosed = _carDoor.State == DoorState.FullyClosed;

        var landingDoor = GetCurrentLandingDoor();
        var isLandingDoorClosed = landingDoor?.State == DoorState.FullyClosed;

        return isCarDoorClosed && isLandingDoorClosed;
    }
}