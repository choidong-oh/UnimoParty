using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonRoomMgr : MonoBehaviourPunCallbacks
{
    public Transform RoomListPanel;
    public GameObject RoomUser;
    List<int> readyPlayerIDs = new List<int>();
    public int readyCount => readyPlayerIDs.Count;


    private  void  OnEnable()
    {
        base.OnEnable();


    }

    //겟차일드 찾고 
    //그거에 텍스를 찾아서 변경
    void Start()
    {
        UpdatePlayerList();
    }

    //방들어오면
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("누군가 들어옴");
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (readyPlayerIDs.Contains(otherPlayer.ActorNumber))
        {
            readyPlayerIDs.Remove(otherPlayer.ActorNumber);
        }
        Debug.Log("누군가 퇴장함");
        UpdatePlayerList();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("IsReady"))
        {
            UpdatePlayerList();
        }
    }

    //0 텍스트 닉네임
    //1 텍스트 게임준비
    //2 버튼 게임준비버튼

    public void UpdatePlayerList()
    {
        for (int i = 0; i < RoomListPanel.childCount; i++)
        {
            Destroy(RoomListPanel.GetChild(i).gameObject);
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            var RoomList = Instantiate(RoomUser, RoomListPanel); //룸 리스트 패널 하에 하나 생성
            RoomList.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[i].NickName;
            //RoomList.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = playerMoney.ToString();

            var player = PhotonNetwork.PlayerList[i];
            bool isLocalPlayer = player == PhotonNetwork.LocalPlayer;

            //방장이냐
            if (PhotonNetwork.PlayerList[i].IsMasterClient)
            {
                RoomList.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Master";

                var btn = RoomList.transform.GetChild(2).GetComponent<Button>();
                var btnText = btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                btnText.text = "GameStart";

                if (isLocalPlayer)
                {
                    btn.onClick.AddListener(StartBtn);
                    //btn.onClick.AddListener(()=>Destroy(dd.transform.GetChild(2).gameObject));
                }
                else
                {
                    Destroy(btn.gameObject);
                }
            }
            //방장아니냐
            else
            {
                var statusText = RoomList.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                var btn = RoomList.transform.GetChild(2).GetComponent<Button>();

                // 레디한 유저인지 확인
                //if (readyPlayerIDs.Contains(player.ActorNumber))
                //{
                //    statusText.text = "GameReady";
                //    Destroy(btn.gameObject); // 버튼 숨김
                //}
                //else
                //{
                //    statusText.text = "GameNoReady";
                //    if (isLocalPlayer)
                //    {
                //        btn.onClick.AddListener(ReadyCountBtn);
                //        btn.onClick.AddListener(() => Destroy(btn.gameObject));
                //        btn.onClick.AddListener(() => statusText.text = "GameReady");
                //    }
                //    else
                //    {
                //        Destroy(btn.gameObject);
                //    }
                //}
                object isReadyObj;
                bool isReady = player.CustomProperties.TryGetValue("IsReady", out isReadyObj) && (bool)isReadyObj;

                if (isReady)
                {
                    statusText.text = "GameReady";
                    Destroy(btn.gameObject); // 버튼 숨김
                }
                else
                {
                    statusText.text = "GameNoReady";
                    if (isLocalPlayer)
                    {
                        btn.onClick.AddListener(ReadyCountBtn);
                        btn.onClick.AddListener(() => Destroy(btn.gameObject));
                        btn.onClick.AddListener(() => statusText.text = "GameReady");
                    }
                    else
                    {
                        Destroy(btn.gameObject);
                    }
                }


            }
        }



    }

    void ReadyCountBtn()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
    {
        { "IsReady", true }
    };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
            try
            {


                photonView.RPC("ReadyCount", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);

            }
            catch (System.Exception ee)
            {
                Debug.Log(ee);
            }
        }
    }


    //레디버튼 누를경우 전체의 readyCount가 오름
    //public void ReadyCountBtn()
    //{
    //    if (!PhotonNetwork.IsMasterClient)
    //    {
    //        Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
    //        try
    //        {


    //            photonView.RPC("ReadyCount", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);

    //        }
    //        catch (System.Exception ee)
    //        {
    //            Debug.Log(ee);
    //        }
    //    }
    //}



    [PunRPC]
    public void ReadyCount(int playerID)
    {
        Debug.Log("여기는 들어오냐?");
        //readyCount++;
        if (!readyPlayerIDs.Contains(playerID))
        {
            readyPlayerIDs.Add(playerID);
        }

        for (int i = 0; i < RoomListPanel.childCount; i++)
        {
            Transform playerUI = RoomListPanel.GetChild(i);
            TextMeshProUGUI playerNameText = playerUI.GetChild(0).GetComponent<TextMeshProUGUI>();

            Player player = null;
            foreach (var p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == playerID)
                {
                    player = p;
                    break;
                }
            }

            // 현재 업데이트해야 하는 플레이어 찾기
            //Player player = PhotonNetwork.PlayerList.FirstOrDefault(p => p.ActorNumber == playerID);
            //if (player != null && player.NickName == playerNameText.text)
            //{
            //    playerUI.GetChild(1).GetComponent<TextMeshProUGUI>().text = "준비 완료"; // 모든 클라이언트에서 UI 변경
            //    break;
            //}
        }
    }

    //전체 유저수가 레디를 누를경우 클릭가능하게
    [PunRPC]
    public void StartBtn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("게임 시작 버튼 눌럿음");
            if (readyCount >= PhotonNetwork.PlayerList.Length - 1)
            {
                Debug.Log("게임시작 버튼 눌러서 인게임 씬으로 넘김 ");
                PhotonNetwork.CurrentRoom.IsOpen = false; //게임 시작 후 방 못들어옴
                PhotonNetworkMgr.Instance.changeScene("InGame");
            }
        }
    }
   
}