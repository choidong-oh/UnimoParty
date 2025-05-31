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
    private Player selectedPlayerForInvite; //초대 대상 저장용

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

    // 플레이어 들어올 때마다 닉네임을 가진 버튼 생성
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
            //중복 방지
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SendInviteButton(p));
            button.transform.SetAsFirstSibling();
        }
    }

    // 플레이어 나가면 버튼 제거
    void RemovePlayerButton(Player p)
    {
        if (playerButtons.TryGetValue(p.NickName, out Button btn))
        {
            Destroy(btn.gameObject);
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

    // 버튼 클릭 시, 초대 대상 저장
    public void SendInviteButton(Player p)
    {
        selectedPlayerForInvite = null;
        selectedPlayerForInvite = p;

        sendInvitePanel.SetActive(true);
        sendInvitePanel.GetComponentInChildren<TextMeshProUGUI>().text = $"{p.NickName} 님을 초대 하겠습니까?";
    }

    // "예" 버튼 클릭 시 RPC로 초대 전송
    public void YesButton()
    {
        if (selectedPlayerForInvite != null)
        {
            photonView.RPC("PartyInvite", selectedPlayerForInvite);
        }
        sendInvitePanel.SetActive(false);
    }

    public void NoButton()
    {
        sendInvitePanel.SetActive(false);
    }

    // 상대방에게 초대 UI 띄우기
    [PunRPC]
    public void PartyInvite()
    {
        receiveInvitePopup.SetActive(true);
    }
}
