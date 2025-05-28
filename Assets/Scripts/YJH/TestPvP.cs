using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestPvP : MonoBehaviourPunCallbacks
{
    [SerializeField] Button TestStartBtn;

    void Start()
    {
        TestStartBtn.interactable = false;
        TestStartBtn.onClick.AddListener(() =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(3);
            }
        });
    }

    public void TestPVPButton()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings(); // °Ê OnConnectedToMaster »£√‚µ 
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            TestStartBtn.interactable = true;
        }
        else
        {
            TestStartBtn.GetComponentInChildren<TextMeshProUGUI>().text = "IS READY";
        }
    }
}
