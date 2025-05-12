//최동오
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;


public class PhotonLobbyMgr : MonoBehaviourPunCallbacks
{
    public TMP_InputField joinRoomInput;

    public Transform roomListPanel;

    public GameObject roomPrefab;

    public GameObject testImg;

    List<string> names = new List<string>() { "점심내기 한판", "주사위 운빨 겜", "주사위의 신을 찾아라", "내 용돈 줄 사람 구함" };
    static int nameCount;

    //서버연결
    public void isServer()
    {
        //서버 연결
        PhotonNetwork.ConnectUsingSettings();
    }

    string RandomRoomName()
    {
        string roomname = names[nameCount] + " " + UnityEngine.Random.Range(0, 9999);
        nameCount++;
        if (nameCount >= names.Count) nameCount = 0;
        return roomname;
    }

    public void CreateRoom()
    {
        Debug.Log("CreateRoom() 함수 호출됨"); // 맨 처음 무조건 찍히는지 확인

        if (PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("포톤 연결 상태 양호, 방 생성 시도 중");
            string roomName = RandomRoomName();

            RoomOptions options = new RoomOptions
            {
                MaxPlayers = 4,
                EmptyRoomTtl = 0,
                IsOpen = true
            };

            PhotonNetwork.CreateRoom(roomName, options);
        }
        else if (PhotonNetwork.IsConnected == false)
        {
            Debug.LogWarning("포톤 서버에 아직 연결되지 않았습니다.");
            PhotonNetwork.ConnectUsingSettings();
        }
    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"방 생성 실패! 코드: {returnCode}, 이유: {message}");
    }

    public void JoinRoom()
    {
        Debug.Log("JoinRoom");
        PhotonNetwork.JoinRoom(joinRoomInput.text);
    }

    public void JoinRandomRoom()
    {
        Debug.Log("JoinRandomRoom");
        PhotonNetwork.JoinRandomRoom();
    }

    public void QuitRoom()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("룸나감");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 연결 콜백");
        testImg.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("연결 끊김 감지. 사유: " + cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("방 참여 실패. 보통 이러면 새로운 방 생성");
        //PhotonNetwork.CreateRoom(PhotonNetwork.NickName, new RoomOptions()); //방 만들어주는 메서드. 앞엔 방 이름, 뒤엔 옵션
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
        PhotonNetworkMgr.Instance.changeScene("Room");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장");
    }

    public override void OnLeftLobby()
    {
        Debug.Log("로비 퇴장");
    }


    private Dictionary<string, GameObject> roomDictionary = new Dictionary<string, GameObject>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("리스트들어옴"); // 디버그 로그

        // 현재 딕셔너리에 등록된 방 이름들을 복사해서 리스트로 저장 (삭제할 때 사용)
        List<string> removeRoom = new List<string>(roomDictionary.Keys);

        foreach (RoomInfo roomInfo in roomList)
        {
            // 방이 삭제된 경우
            if (roomInfo.RemovedFromList == true)
            {
                // 해당 방이 딕셔너리에 있으면
                if (roomDictionary.ContainsKey(roomInfo.Name) == true)
                {
                    // 버튼 오브젝트 삭제 후 딕셔너리에서도 제거
                    Destroy(roomDictionary[roomInfo.Name]);
                    roomDictionary.Remove(roomInfo.Name);
                }
            }
            else // 새로 생성된 방이거나 기존 방
            {
                // 아직 버튼을 만들지 않은 새 방인 경우
                if (roomDictionary.ContainsKey(roomInfo.Name) == false)
                {
                    // 방 버튼 프리팹을 생성하여 방 리스트 패널 아래에 붙임
                    var roomBtn = Instantiate(roomPrefab, roomListPanel);
                    roomBtn.GetComponentInChildren<TextMeshProUGUI>().text = roomInfo.Name;
                    roomBtn.GetComponent<Button>().onClick.AddListener(() => PhotonNetwork.JoinRoom(roomInfo.Name));

                    // 딕셔너리에 방 이름과 버튼 오브젝트 등록
                    roomDictionary.Add(roomInfo.Name, roomBtn);
                }
            }
        }
    }

   


}