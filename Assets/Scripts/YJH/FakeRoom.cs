using System.Collections.Generic;

public class FakeRoom
{
    public string roomName;
    public string mapName;
    public bool isLocked;
    public int maxPlayer = 8;
    public bool isMaster;

    public List<FakePlayer> Players = new List<FakePlayer>();

    public int CurrentPlayers => Players.Count;
}
public class FakePlayer
{
    public string nickName;
    public bool isReady;
    public string playerText;
}
