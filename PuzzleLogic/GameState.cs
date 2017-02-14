namespace Puzzle
{
    public enum GameState
    {
        InPlay = 0,
        KilledByMonster = 1,
        KilledByBlock = 2,
        Won = 3,
        Timeout = 4,
        KilledByDeath = 5
    }
}
