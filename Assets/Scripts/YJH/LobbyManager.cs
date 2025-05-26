using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("ÆÇ³Úµé")]
    [SerializeField] GameObject LobbyCanvas;
    [SerializeField] GameObject PVECanvas;
    

    [Space]
    public Transform contentParent;
    public GameObject userButtonPrefab;

    private Dictionary<string, GameObject> playerButtons = new Dictionary<string, GameObject>();

    IEnumerator Start()
    {
        PVECanvas.SetActive(false);

        yield return new WaitUntil(() => !string.IsNullOrEmpty(FirebaseAuthMgr.user.DisplayName));
        PhotonNetwork.ConnectUsingSettings();

        PhotonNetwork.NickName = FirebaseAuthMgr.user.DisplayName;
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
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            AddPlayerButton(p);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerButton(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerButton(otherPlayer);
    }


    void AddPlayerButton(Player p)
    {
        if (playerButtons.ContainsKey(p.NickName))
            return;

        GameObject button = Instantiate(userButtonPrefab, contentParent);
        button.GetComponentInChildren<TextMeshProUGUI>().text = p.NickName;
        playerButtons.Add(p.NickName, button);

        if (p.IsLocal)
        {
            button.transform.SetAsFirstSibling();
        }
    }

    void RemovePlayerButton(Player p)
    {
        if (playerButtons.TryGetValue(p.NickName, out GameObject btn))
        {
            Destroy(btn);
            playerButtons.Remove(p.NickName);
        }
    }

    public void OnClickPVESceneButton()
    {
        LobbyCanvas.SetActive(false);
        PVECanvas.SetActive(true);
    }

    public void OnClickBackButton()
    {
        LobbyCanvas.SetActive(true);
        PVECanvas.SetActive(false);
    }

    public void GameStartButton()
    {
        SceneManager.LoadScene(2);
    }

}
