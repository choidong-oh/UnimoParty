using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("판넬들")]
    [SerializeField] GameObject LobbyCanvas;
    [SerializeField] GameObject PVECanvas;
    [SerializeField] GameObject sendInvitePanel;
    [SerializeField] GameObject receiveInvitePopup;

    [Space]
    public Transform contentParent;
    public Button userButtonPrefab;

    private Dictionary<string, Button> playerButtons = new Dictionary<string, Button>();

    void Start()
    {
        PVECanvas.SetActive(false);

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;

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


    //플레이어 들어올때마다 닉네임을 가진 버튼 생성
    void AddPlayerButton(Player p)
    {
        Button button = Instantiate(userButtonPrefab, contentParent);
        button.GetComponentInChildren<TextMeshProUGUI>().text = p.NickName;
        playerButtons.Add(p.NickName, button);

        if (p.IsLocal)
        {
            button.interactable = false;
        }
        else
        {
            //button.onClick.AddListener(()SendInviteButton(Player p));
            button.transform.SetAsFirstSibling();
        }
    }

    //플레이어 나가면 자기 자신 버튼 삭제
    void RemovePlayerButton(Player p)
    {
        if (playerButtons.TryGetValue(p.NickName, out Button btn))
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

    public void SoloPlayButton()
    {
        SceneManager.LoadScene(2);
    }
    //닉네임이 있는 버튼이 누르면 파티초대 버튼 뜨기
    public void SendInviteButton(Player p)
    {
        sendInvitePanel.SetActive(true);
        sendInvitePanel.GetComponentInChildren<TextMeshProUGUI>().text = $"{p.NickName} 님을 초대 하겠습니까?";
    }

    //파티초대를 누를 시 상대에게 파티초대 왔다는 코드 보내기
    [PunRPC]
    public void PartyInvite()
    {
        receiveInvitePopup.SetActive(true);
    }


}
