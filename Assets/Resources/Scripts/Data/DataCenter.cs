
public struct PlayerFairy
{
    public int FairyType1;
    public int FairyType2;
    public int FairyType3;
}

public struct EndFairyCount
{
    public int EndFairyType1;
    public int EndFairyType2;
    public int EndFairyType3;
}

public class DataCenter
{
    public int life;
    public int score;
    public PlayerFairy playerFairyType;

    public DataCenter(DataCenter gamedata) : this(gamedata.life, gamedata.score) { }

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