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
    [SerializeField] GameObject LobbyCanvas;
    [SerializeField] GameObject PVECanvas;

    public void PVEButton()
    {
        LobbyCanvas.SetActive(false);
        PVECanvas.SetActive(true);
    }
    public void PVPButton()
    {
        SceneManager.LoadScene(3);
    }

    public void OnClickBackButton()
    {
        LobbyCanvas.SetActive(true);
        PVECanvas.SetActive(false);
    }

    public void SoloPlayButton()
    {
        SceneManager.LoadScene(2);
    }
}
