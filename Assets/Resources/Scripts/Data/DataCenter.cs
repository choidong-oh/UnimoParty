
public struct FairyType
{
    public int FairyDataType_1;
    public int FairyDataType_2;
    public int FairyDataType_3;
}

public struct GoalFairyCount
{
    public int GoalFairyValue_1;
    public int GoalFairyValue_2;
    public int GoalFairyValue_3;
}

public enum PlayerState
{
    None, Invincible
}

public class DataCenter
{
    public int life;
    public int score;
    public FairyType playerFairyType;
    public PlayerState _playerState;

    public DataCenter(DataCenter gamedata) : this(gamedata.life, gamedata.score, gamedata.playerFairyType) { }

    public DataCenter(int initLife, int initSocre, FairyType initfairy)
    {
        life = initLife;
        score = initSocre;
        playerFairyType = initfairy;
    }

    public DataCenter(int initLife, int initSocre , PlayerState playerState)
    {
        life = initLife;
        score = initSocre;
        _playerState = playerState;
    }

    public DataCenter Clone()
    {
        return new DataCenter(this);
    }
}