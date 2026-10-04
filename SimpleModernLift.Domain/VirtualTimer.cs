public struct VirtualTimer
{
    public readonly bool IsExpired => _elapsed >= _duration;

    private readonly float _duration;
    private float _elapsed;

    public VirtualTimer(float durationSeconds)
    {
        _duration = durationSeconds;
        _elapsed = 0f;
    }

    public void Tick(float deltaTime) => _elapsed += deltaTime;
    public void Reset() => _elapsed = 0f;
}