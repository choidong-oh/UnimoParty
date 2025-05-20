using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Transform contentParent;
    public GameObject userButtonPrefab;

    private Dictionary<string, GameObject> playerButtons = new Dictionary<string, GameObject>();

    void Start()
    {
        PhotonNetwork.NickName = FirebaseLoginMgr.user.DisplayName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions options = new RoomOptions { MaxPlayers = 20 };
        PhotonNetwork.CreateRoom("LobbyRoom", options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        RefreshPlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerButton(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerButton(otherPlayer);
    }

    void RefreshPlayerList()
    {
        foreach (var btn in playerButtons.Values)
        {
            Destroy(btn);
        }
        playerButtons.Clear();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            AddPlayerButton(p);
        }
    }

    void AddPlayerButton(Player p)
    {
        if (playerButtons.ContainsKey(p.NickName))
            return;

        GameObject button = Instantiate(userButtonPrefab, contentParent);
        button.GetComponentInChildren<Text>().text = p.NickName;
        playerButtons.Add(p.NickName, button);
    }

    void RemovePlayerButton(Player p)
    {
        if (playerButtons.TryGetValue(p.NickName, out GameObject btn))
        {
            Destroy(btn);
            playerButtons.Remove(p.NickName);
        }
    }
}
