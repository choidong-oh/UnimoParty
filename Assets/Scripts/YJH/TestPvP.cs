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


    [Header("방 입장 버튼 들")]
    [SerializeField] Button developerJoinRoomBtn;
    [SerializeField] Button designerJoinRoomBtn;
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();

        designerGameStartBtn.interactable = false;
        designerJoinRoomBtn.interactable = false;

        designerGameStartBtn.onClick.AddListener(() =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(3);
            }
        });
        designerJoinRoomBtn.onClick.AddListener(() => DesignerPVPJoinRoom());

        developerGameStartBtn.interactable = false;
        developerJoinRoomBtn.interactable = false;

        developerGameStartBtn.onClick.AddListener(() =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(3);
            }
        });
        developerJoinRoomBtn.onClick.AddListener(() => DeveloperPVPJoinRoom());
    }


    //기획 방 생성
    public void DesignerPVPCreatRoom()
    {
        PhotonNetwork.CreateRoom("Designer", new RoomOptions { MaxPlayers = 8 });
        
    }
    //기획 방 들어가기
    public void DesignerPVPJoinRoom()
    {
        PhotonNetwork.JoinRoom("Designer");
    }




    //개발 방 생성
    public void DeveloperPVPCreatRoom()
    {
        PhotonNetwork.CreateRoom("Developer", new RoomOptions { MaxPlayers = 8 });
    }
    //개발 방 들어가기
    public void DeveloperPVPJoinRoom()
    {
        PhotonNetwork.JoinRoom("Developer");
    }



    //방이 들어가면
    public override void OnJoinedRoom()
    {
        //기획자 방 들어옴
        int designerInRoom = 1;
        if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.Name== "Designer")
        {
            designerGameStartBtn.interactable = true;
            designerJoinRoomBtn.interactable = true;
            designerGameStartBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"{designerInRoom} 명 준비완료\n게임 시작";
        }
        else if(PhotonNetwork.CurrentRoom.Name == "Designer")
        {
            designerInRoom++;
            designerGameStartBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"{designerInRoom} 명 입장";
        }



        //개발 방 들어옴
        int developerInRoom = 1;
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.Name== "Developer")
        {
            developerGameStartBtn.interactable = true;
            developerJoinRoomBtn.interactable = true;
            developerGameStartBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"{developerInRoom} 명 준비완료\n게임 시작";
        }
        else if(PhotonNetwork.CurrentRoom.Name == "Developer")
        {
            developerInRoom++;
            developerGameStartBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"{developerInRoom} 명 입장";
        }


    }
}
