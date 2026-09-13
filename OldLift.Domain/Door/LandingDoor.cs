public class LandingDoor : ILandingDoor
{
    public int Floor { get; private set; }
    public bool IsFullyOpened { get; private set; }
    public bool IsFullyClosed { get; private set; }
    public bool IsObstructed { get; set; }
    public bool IsStalled { get; private set; }
    
    private const float OpenDurationSeconds = 3.0f;
    private const float CloseDurationSeconds = 3.0f;

    private bool _isOpening;
    private bool _isClosing;
    private float _openTimer;
    private float _closeTimer;

    public LandingDoor(int floor)
    {
        Floor = floor;
        IsFullyOpened = false;
        IsFullyClosed = true;
        IsObstructed = false;
        IsStalled = false;
        _isOpening = false;
        _isClosing = false;
        _openTimer = 0;
        _closeTimer = 0;
    }

    public void Update(float deltaTime)
    {
        UpdateClosing(deltaTime);
        UpdateOpening(deltaTime);
    }

    private void UpdateClosing(float deltaTime)
    {
        if (!_isClosing) return;
        if (IsObstructed)
        {
            IsStalled = true;
            _isClosing = false;
            return;
        }

        _closeTimer += deltaTime;
        if (_closeTimer >= CloseDurationSeconds)
        {
            IsFullyClosed = true;
            _isClosing = false;
            _closeTimer = 0;
        }
    }

    private void UpdateOpening(float deltaTime)
    {
        if (!_isOpening) return;

        _openTimer += deltaTime;
        if (_openTimer >= OpenDurationSeconds)
        {
            IsFullyOpened = true;
            _isOpening = false;
            _openTimer = 0;
        }
    }

    public void StartOpening()
    {
        _isOpening = true;
        IsFullyOpened = false;
        IsFullyClosed = false;
    }

    public void StartClosing()
    {
        _isClosing = true;
        IsFullyOpened = false;
        IsFullyClosed = false;
    }
}