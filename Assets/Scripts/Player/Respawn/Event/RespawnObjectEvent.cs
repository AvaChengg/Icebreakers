public class RespawnObjectEvent : IMainEvent
{
    private IRespawn _respawn;

    public RespawnObjectEvent(IRespawn respawn)
    {
        _respawn = respawn;
    }

    public IRespawn GetRespawnObject()
    {
        return _respawn;
    }
}
