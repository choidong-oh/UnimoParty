
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
    None, 
    Invincible
}

public class DataCenter
{
    public int life;
    public int score;
    public int hitcount;
    public int deliveryCount;
    public int _money;
    public FairyType playerFairyType;
    public PlayerState _playerState;
    public Inventory _Inventory = new Inventory();

    public DataCenter(DataCenter gamedata) : this(gamedata.life, gamedata.score, gamedata._money, gamedata.playerFairyType, gamedata._playerState, gamedata._Inventory) { }

    public DataCenter(int initLife, int initSocre, FairyType initfairy)
    {
        life = initLife;
        score = initSocre;
        playerFairyType = initfairy;
    }

    public DataCenter(int initLife, int initSocre, int money, PlayerState playerstate)
    {
        life = initLife;
        score = initSocre;
        _money = money;
        _playerState = playerstate;
    }

    public DataCenter(int initLife, int initSocre , int money, FairyType initfairy, PlayerState playerstate, Inventory inventory)
    {
        life = initLife;
        score = initSocre;
        _money = money;
        playerFairyType = initfairy;
        _playerState = playerstate;
        _Inventory = inventory;
    }

    public DataCenter Clone()
    {
        return new DataCenter(this);
    }
}