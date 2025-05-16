
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

public class DataCenter
{
    public int life;
    public int score;
    public FairyType playerFairyType;

    public DataCenter(DataCenter gamedata) : this(gamedata.life, gamedata.score, gamedata.playerFairyType) { }

    public DataCenter(int initLife, int initSocre, FairyType initfairy)
    {
        life = initLife;
        score = initSocre;
        playerFairyType = initfairy;
    }

    public DataCenter(int initLife, int initSocre)
    {
        life = initLife;
        score = initSocre;
    }

    public DataCenter Clone()
    {
        return new DataCenter(this);
    }
}