using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Button makeRoom;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    private void OnConnectedToServer()
    {
    }


}
