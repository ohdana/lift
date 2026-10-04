public interface IHelpdeskGateway 
{
    event Action ConnectionEstablished;
    event Action ConnectionFailed;

    void BeginHelpdeskConnection(Guid lliftId, ILocation location);
}