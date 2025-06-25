using System.Collections;
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
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject PVEPanel;
    [SerializeField] GameObject PVPPanel;
    [SerializeField] GameObject roomPanel;

    [Header("매칭 분류")]
    [SerializeField] Image matchImage;
    [SerializeField] TextMeshProUGUI Count;
    [SerializeField] GameObject failPanel;
    [Header("방 안에")]
    [SerializeField] TextMeshProUGUI roomNumber;
    [SerializeField] GameObject checkPanel;

    [SerializeField] Transform nicknamePanelParent;
    [SerializeField] Button actionButton;
    [SerializeField] TextMeshProUGUI actionButtonText;
    [SerializeField] GameObject gameSettingPanel;
    [SerializeField] GameObject playerPanelPrefab;
    [SerializeField] GameObject gameSettingButton;


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

    public List<Player> playerList = new List<Player>();
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }
    IEnumerator Start()
    {
        yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);
        PhotonNetwork.NickName = FirebaseAuthMgr.user.DisplayName;

        Debug.Log("네트워크 연결 완");

        ShowPanel(lobbyPanel);
        PVEPanel.SetActive(false);
        gameSettingPanel.SetActive(false);
        PVPPanel.SetActive(false);
        roomPanel.SetActive(false);
        failPanel.SetActive(false);

        if (Manager.Instance.IsPlayAgainPending)
        {
            yield return new WaitForSeconds(0.5f);
            Manager.Instance.IsPlayAgainPending = false;

            string roomCode = Manager.Instance.PlayAgainRoomCode;
            codeInput.text = roomCode;

            PhotonNetwork.JoinOrCreateRoom(roomCode, new RoomOptions { MaxPlayers = 8 }, TypedLobby.Default);

            PVPPanel.SetActive(false);
            roomPanel.SetActive(true);
        }
    }
    private void ShowPanel(GameObject nextPanel)
    {
        if (panelStack.Count > 0)
        {
            panelStack.Peek().SetActive(false);
        }
        panelStack.Push(nextPanel);
        nextPanel.SetActive(true);
        currentPanel = panelStack.Peek().gameObject;
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
        if (panelStack.Count > 1)
        {
            GameObject last = panelStack.Pop();
            last.SetActive(false);

            currentPanel = panelStack.Peek();
            currentPanel.SetActive(true);
        }

    }
    //PVE 스테이지 진입
    public void Stage1()
    {
        SceneManager.LoadScene(2);
    }

    public void CreatRoom()
    {
        StartCoroutine(WaitCreatRoom());
    }
    IEnumerator WaitCreatRoom()
    {
        PhotonNetwork.JoinLobby();
        //yield return new WaitUntil(() => PhotonNetwork.InLobby);
        //yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() =>
      PhotonNetwork.NetworkClientState == ClientState.JoinedLobby);

        PhotonNetwork.CreateRoom($"{Random.Range(10000, 99999)}", new RoomOptions { IsVisible = false, MaxPlayers = 8 });
        PVPPanel.SetActive(false);
        roomPanel.SetActive(true);
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

            if (PhotonNetwork.InRoom)
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
            roomNumber.text = PhotonNetwork.CurrentRoom.Name;

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                AddNicknameUI(p);
            }
            UpdateActionButton();
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.Name != "Random")
        {
            AddNicknameUI(newPlayer);
            UpdateActionButton();
            if (PhotonNetwork.IsMasterClient)
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

            UpdateActionButton();
            if (PhotonNetwork.IsMasterClient)
            {
                CheckAllReady();
            }
        }
    }
    private void CheckAllReady()
    {
        int totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        if (readyCount >= totalPlayers - 1)
        {
            actionButton.interactable = true;
        }
        else
        {
            actionButton.interactable = false;
        }
    }

    //
    private void AddNicknameUI(Player player)
    {
        if (playerUIMap.ContainsKey(player.ActorNumber))
            return;

        GameObject panel = Instantiate(playerPanelPrefab, nicknamePanelParent);
        RectTransform rt = panel.GetComponent<RectTransform>();

        PlayerPanel pPanel = panel.GetComponent<PlayerPanel>();
        pPanel.Setup(player);
        pPanel.SetReady(false);
        pPanel.MasterClient(player.ActorNumber == PhotonNetwork.MasterClient.ActorNumber);

        playerUIMap[player.ActorNumber] = panel;

        int charIndex = player.CustomProperties.ContainsKey("CharacterIndex") ? (int)player.CustomProperties["CharacterIndex"] : 0;
        int shipIndex = player.CustomProperties.ContainsKey("ShipIndex") ? (int)player.CustomProperties["ShipIndex"] : 0;

        GameObject[] characters = Resources.LoadAll<GameObject>("Characters");
        GameObject[] ships = Resources.LoadAll<GameObject>("Prefabs");

        Transform characterPos = panel.transform.Find("CharacterPos");
        Transform shipPos = panel.transform.Find("SpaceShipPos");

        GameObject charObj = Instantiate(characters[charIndex], characterPos.position, Quaternion.Euler(0,180,0), characterPos);
        GameObject shipObj = Instantiate(ships[shipIndex], shipPos.position, Quaternion.Euler(0, 180, 0), shipPos);
    }
    


    public void CodeJoinRoom()
    {

        Debug.Log("입장 중");
        PhotonNetwork.JoinRoom(codeInput.text);
        PVPPanel.SetActive(false);
        roomPanel.SetActive(true);
    }



    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        UpdateActionButton();
    }
    public void ReadyButton()
    {
        photonView.RPC("SetReadyState", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);

    }


    public void StartGameButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            
            Manager.Instance.players.Clear();
            Manager.Instance.players.AddRange(PhotonNetwork.PlayerList);


            Manager.Instance.SetGameList();

            for(int i=0; i< Manager.Instance.players.Count ; i++)
            {
                Debug.Log(Manager.Instance.players[i] + "플레이어들 setting");
                Debug.Log(Manager.Instance.players[i].ActorNumber);
            }
            
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
    [PunRPC]
    public void SetReadyState(int actorNumber)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            readyCount++;
            CheckAllReady();
        }

        if (playerUIMap.TryGetValue(actorNumber, out var panel))
        {
            panel.GetComponent<PlayerPanel>().SetReady(true);
        }
    }



    public void GameSettingButton()
    {
        gameSettingPanel.SetActive(true);
    }
    public void ExitSetting()
    {
        gameSettingPanel.SetActive(false);
    }
    public void SaveSetting()
    {
        gameSettingPanel.SetActive(false);
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
