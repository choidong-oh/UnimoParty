using System.Collections;
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
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.CreateRoom(null, null);
    }

    public override void OnJoinedRoom()
    {
        TestStartBtn.interactable = true;
    }
}
