using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class TestPvP : MonoBehaviourPunCallbacks
{
    [SerializeField] Button TestStartBtn;

    void Start()
    {
        TestStartBtn.interactable = false;
        TestStartBtn.onClick.AddListener(() => PhotonNetwork.LoadLevel(3));
    }

    public void TestPVPButton()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings(); // °Ê OnConnectedToMaster »£√‚µ 
    }

    public override void OnConnected()
    {
        PhotonNetwork.JoinRandomRoom();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedRoom()
    {
        TestStartBtn.interactable = true;
    }
}
