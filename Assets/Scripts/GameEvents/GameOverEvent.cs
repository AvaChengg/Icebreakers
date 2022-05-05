public class GameOverEvent : IMainEvent
{
    private int _winningPlayerIndex;

    public GameOverEvent(int winningPlayerIndex)
    {
        _winningPlayerIndex = winningPlayerIndex;
    }

    public int GetWinningPlayerIndex()
    {
        return _winningPlayerIndex;
    }
}
