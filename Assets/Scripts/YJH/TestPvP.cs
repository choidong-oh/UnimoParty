using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestPvP : MonoBehaviourPunCallbacks
{
    [Header("게임 스타트 버튼 들")]
    [SerializeField] Button developerGameStartBtn;
    [SerializeField] Button designerGameStartBtn;

    [Header("방 생성 버튼 들")]
    [SerializeField] Button developerCreateRoom;
    [SerializeField] Button designerCreateRoom;

    int developerInRoom;
    int designerInRoom;
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();

        designerGameStartBtn.interactable = false;

        designerGameStartBtn.onClick.AddListener(() =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(3);
            }
        });

        developerGameStartBtn.interactable = false;

        developerGameStartBtn.onClick.AddListener(() =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(3);
            }
        });
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 생성중");
    }


    //기획 방 생성 및 입장
    public void DesignerPVPJoinOrCreatRoom()
    {
        PhotonNetwork.JoinOrCreateRoom("Designer", new RoomOptions { MaxPlayers = 8 },null);
        designerCreateRoom.interactable = false;

    }

    //개발 방 생성 및 입장
    public void DeveloperPVPJoinOrCreatRoom()
    {
        PhotonNetwork.JoinOrCreateRoom("Developer", new RoomOptions { MaxPlayers = 8 }, null);
        developerCreateRoom.interactable=false;
    }



    public override void OnConnected()
    {
        OnJoinedLobby();
    }


    //방이 들어가면
    public override void OnJoinedRoom()
    {
        //기획자 방 들어옴
        if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.Name== "Designer")
        {
            designerInRoom++;
            designerGameStartBtn.interactable = true;
            designerGameStartBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"게임 시작";
        }
        else if(PhotonNetwork.CurrentRoom.Name == "Designer")
        {
            designerInRoom++;
            designerGameStartBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"{designerInRoom} 명 입장";
        }



        //개발 방 들어옴
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.Name== "Developer")
        {
            developerInRoom++;
            developerGameStartBtn.interactable = true;
            developerGameStartBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"게임 시작";
        }
        else if(PhotonNetwork.CurrentRoom.Name == "Developer")
        {
            developerInRoom++;
            developerGameStartBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"{developerInRoom} 명 입장";
        }

    }
}
