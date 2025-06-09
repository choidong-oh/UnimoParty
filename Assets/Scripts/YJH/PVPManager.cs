using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class PVPManager : MonoBehaviourPunCallbacks
{
    [Header("판넬들")]
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject PVEPanel;
    [SerializeField] GameObject PVPPanel;
    [SerializeField] GameObject roomPanel;

    [Header("매칭 분류")]
    [SerializeField] Image matchImage;
    [SerializeField] TextMeshProUGUI Count;

    [Header("방 안에")]
    [SerializeField] TextMeshProUGUI roomNumber;
    [SerializeField] GameObject checkPanel;

    [SerializeField] GameObject nicknamePanel;
    [SerializeField] Transform nicknamePanelParent;
    [SerializeField] Button actionButton;
    [SerializeField] TextMeshProUGUI actionButtonText;

    [Header("매칭 분류")]
    [SerializeField] Button makeRoomBtn;
    [SerializeField] TMP_InputField codeInput;


    private byte maxPlayers = 8;
    private const byte minPlayers = 2;

    private Stack<GameObject> panelStack = new Stack<GameObject>();
    private GameObject currentPanel;
    private int readyCount = 0;


    private Dictionary<int, GameObject> playerUIMap = new Dictionary<int, GameObject>();
    Coroutine matchCorutine;
    private bool isMatchMaking;

    
    IEnumerator Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        yield return new WaitUntil(() => PhotonNetwork.IsConnected);

        Debug.Log("네트워크 연결 완");

        ShowPanel(lobbyPanel);
        PVEPanel.SetActive(false);
        PVPPanel.SetActive(false);
        roomPanel.SetActive(false);


        PhotonNetwork.NickName = FirebaseAuthMgr.user.DisplayName;
    }
    private void ShowPanel(GameObject nextPanel)
    {
        if (currentPanel != null)
        {
            panelStack.Push(currentPanel);
            currentPanel.SetActive(false);
        }

        currentPanel = nextPanel;
        currentPanel.SetActive(true);
    }
    //인스펙터 끼워 넣기 용
    public void PVEButton()
    {
        ShowPanel(PVEPanel);
    }
    //인스펙터 끼워 넣기 용
    public void PVPButton()
    {
        ShowPanel(PVPPanel);
    }

    public void BackButton()
    {
        if (panelStack.Count > 0)
        {
            currentPanel.SetActive(false);
            currentPanel = panelStack.Pop();
            currentPanel.SetActive(true);
        }
    }

    public void Stage1()
    {
        SceneManager.LoadScene(2);
    }

    public void CreatRoom()
    {
        PVPPanel.SetActive(false);
        roomPanel.SetActive(true);
        PhotonNetwork.CreateRoom($"{Random.Range(10000, 99999)}", new RoomOptions {IsVisible = false,MaxPlayers=8 });
    }
    public void InRoomBackButton()
    {
        checkPanel.SetActive(true);
    }
    public void StayButton()
    {
        checkPanel.SetActive(false);
    }
    public void LeaveRoomButton()
    {
        PhotonNetwork.LeaveRoom();
        PVPPanel.SetActive(true);
        checkPanel.SetActive(false);
        roomPanel.SetActive(false);
    }

    public void MatchmakingButton()
    {
        isMatchMaking = !isMatchMaking;

        if (!isMatchMaking)
        {
            makeRoomBtn.interactable = true;
            codeInput.interactable = true;
            Count.gameObject.SetActive(false);
            matchImage.color = new Color(1, 1, 0, 0);

            StopCoroutine(matchCorutine);
            matchCorutine = null;
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            makeRoomBtn.interactable = false;
            codeInput.interactable = false;

            Count.gameObject.SetActive(true);
            matchCorutine = StartCoroutine(MatchmakingRoutine()); 
            matchImage.color = new Color(1, 1, 0, 1);
        }
    }

    private IEnumerator MatchmakingRoutine()
    {
        maxPlayers = 8;
        yield return new WaitForSeconds(0.1f);
        int timeElapsed = 0;
        int addTime = 30;
        Count.text = "00:00";
        while (maxPlayers >= minPlayers)
        {
            
            if(PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom(true);
                yield return new WaitUntil(() => !PhotonNetwork.InRoom);
            }
            else
            {
                PhotonNetwork.JoinRandomOrCreateRoom(null, maxPlayers, MatchmakingMode.FillRoom, null, null, "Random", new RoomOptions { MaxPlayers = 8 }, null);
            }
            while (timeElapsed < addTime)
            {
                int minutes = timeElapsed / 60;
                int seconds = timeElapsed % 60;
                Count.text = $"{minutes:D2}:{seconds:D2}";

                yield return new WaitForSeconds(1f);
                timeElapsed++;                
            }
            addTime += 30;
            
            Debug.Log($"[매치메이킹] 인원 감소 → maxPlayers: {maxPlayers}");
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.Name == "Random")
        {
            Debug.Log("매치메이킹 시작");
        }
        else
        {
            Debug.Log("입장 성공");
            UpdateActionButton();

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                AddNicknameUI(p);
            }

            roomNumber.text = PhotonNetwork.CurrentRoom.Name;
        }
    }
    //플레이어 검증
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.Name != "Random")
        {
            AddNicknameUI(newPlayer);
            UpdateActionButton();
            if(PhotonNetwork.IsMasterClient)
            {
                CheckAllReady();
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.Name != "Random")
        {
            if (playerUIMap.TryGetValue(otherPlayer.ActorNumber, out GameObject ui))
            {
                Destroy(ui);
                playerUIMap.Remove(otherPlayer.ActorNumber);
            }

            if(PhotonNetwork.IsMasterClient)
            {
                CheckAllReady();
            }
            UpdateActionButton();
        }
    }
    private void CheckAllReady()
    {
        int totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        actionButton.interactable = (readyCount >= totalPlayers - 1);
    }
    private void AddNicknameUI(Player p)
    {
        GameObject panel = PhotonNetwork.Instantiate("PlayerPanel", Vector3.zero, Quaternion.identity, 0);
        panel.transform.SetParent(nicknamePanelParent, false);

        TextMeshProUGUI texts = panel.GetComponentInChildren<TextMeshProUGUI>();
        texts.text = p.NickName;
        
        if(p.ActorNumber != PhotonNetwork.MasterClient.ActorNumber)
        {
            panel.GetComponentInChildren<Image>().enabled = false;
        }
        playerUIMap[p.ActorNumber] = panel;
    }



    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 찾기 실패");
    }
    public void CodeJoinRoom()
    {
        Debug.Log("입장 중");
        PhotonNetwork.JoinRoom(codeInput.text);
    }




    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        UpdateActionButton();
    }
    public void ReadyButton()
    {
        if(photonView.IsMine)
        {
            photonView.RPC("SetReadyState", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }


    public void StartGameButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(3);
        }
    }
    private void UpdateActionButton()
    {
        actionButton.onClick.RemoveAllListeners();

        if (PhotonNetwork.IsMasterClient)
        {
            actionButtonText.text = "게임 시작";
            actionButton.onClick.AddListener(StartGameButton);
        }
        else
        {
            actionButtonText.text = "준비 완료";
            actionButton.onClick.AddListener(ReadyButton);
        }
    }
    public override void OnLeftRoom()
    {
        maxPlayers--;
        if (PhotonNetwork.CurrentRoom.Name == "Random")
        {
            Debug.Log(maxPlayers + " 명으로 방 생성");
            PhotonNetwork.JoinRandomOrCreateRoom(null, maxPlayers, MatchmakingMode.FillRoom, null, null, "Random", new RoomOptions { MaxPlayers = maxPlayers }, null);
        }
    }
    [PunRPC]
    public void SetReadyState(int actorNumber)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber &&
            !PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            readyCount++;
            if (playerUIMap.TryGetValue(actorNumber, out GameObject ui))
            {
                Image stateImage = ui.GetComponentInChildren<Image>();
                stateImage.gameObject.SetActive(true);

                stateImage.GetComponentInChildren<TextMeshProUGUI>().text = "준비";
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            CheckAllReady();
        }
    }
    #region 수정 사항 과거버전 

    //버튼 생성 및 버튼 클릭시 파티초대
    //public override void OnJoinedLobby()
    //{
    //    foreach (Player p in PhotonNetwork.PlayerList)
    //    {
    //        AddPlayerButton(p);
    //    }
    //}

    //public override void OnConnectedToMaster()
    //{
    //    PhotonNetwork.JoinLobby(TypedLobby.Default);
    //}

    //public override void OnJoinedRoom()
    //{
    //    Debug.Log("방 이름 : " +PhotonNetwork.CurrentRoom.Name);
    //    Debug.Log(PhotonNetwork.PlayerList.Length);

    //    foreach (Player p in PhotonNetwork.PlayerList)
    //    {
    //        AddPlayerButton(p);
    //    }
    //}


    //public override void OnPlayerLeftRoom(Player otherPlayer)
    //{
    //    RemovePlayerButton(otherPlayer);
    //}

    //// 플레이어 들어올 때마다 닉네임을 가진 버튼 생성
    //void AddPlayerButton(Player p)
    //{
    //    Button button = Instantiate(userButtonPrefab, contentParent);
    //    button.GetComponentInChildren<TextMeshProUGUI>().text = p.NickName;
    //    playerButtons.Add(p.NickName, button);

    //    if (p.IsLocal)
    //    {
    //        button.interactable = false;
    //        button.transform.SetAsFirstSibling();
    //    }
    //    else
    //    {
    //        button.onClick.RemoveAllListeners();
    //        button.onClick.AddListener(() => SendInviteButton(p));
    //    }
    //}

    ////// 플레이어 나가면 버튼 제거
    //void RemovePlayerButton(Player p)
    //{
    //    if (playerButtons.TryGetValue(p.NickName, out Button btn))
    //    {
    //        Destroy(btn.gameObject);
    //        playerButtons.Remove(p.NickName);
    //    }
    //}

    // 버튼 클릭 시, 초대 대상 저장
    //public void SendInviteButton(Player p)
    //{
    //    selectedPlayerForInvite = p;

    //    sendInvitePanel.SetActive(true);
    //    inviteText.text = $"{p.NickName} 님을 초대 하겠습니까?";
    //}

    //// 예 버튼 클릭 시 RPC로 초대
    //public void YesButton()
    //{
    //    if (selectedPlayerForInvite != null)
    //    {
    //        photonView.RPC("PartyInvite", selectedPlayerForInvite);
    //    }

    //    sendInvitePanel.SetActive(false);
    //}

    //public void NoButton()
    //{
    //    sendInvitePanel.SetActive(false);
    //    selectedPlayerForInvite = null;
    //}

    // 상대방에게 초대 UI 띄우기
    //[PunRPC]
    //public void PartyInvite()
    //{
    //    inviteText.text = "초대 받음";
    //    receiveInvitePopup.SetActive(true);
    //    selectedPlayerForInvite = null;
    //}





    //fakeroom 만들던거 

    //[Header("Fake룸 관련")]
    //[SerializeField] TextMeshProUGUI roomNameText;

    //public void CreateFakeRoom(string rName, string mName, bool isLock)
    //{
    //    var fakeRoom = new FakeRoom()
    //    {
    //        roomName = rName,
    //        mapName = mName,
    //        isLocked = isLock,
    //        isMaster = true
    //    };

    //    roomNameText.text = fakeRoom.roomName;

    //}
    #endregion
}
