using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("ÆÇ³Úµé")]
    [SerializeField] GameObject LobbyPanel;
    [SerializeField] GameObject PVEPanel;


    private void Start()
    {
        
        PhotonNetwork.ConnectUsingSettings();

    }

    public void PVEButton()
    {
        LobbyPanel.SetActive(false);
        PVEPanel.SetActive(true);
    }
    public void PVPButton()
    {
        SceneManager.LoadScene(3);
    }

    public void OnClickBackButton()
    {
        LobbyPanel.SetActive(true);
        PVEPanel.SetActive(false);
    }

    public void SoloPlayButton()
    {
        SceneManager.LoadScene(2);
    }
}
