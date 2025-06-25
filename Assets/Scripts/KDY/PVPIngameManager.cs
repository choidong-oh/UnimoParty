using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class PVPIngameManager : MonoBehaviour
{
    public static PVPIngameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // PVPIngame 씬이 아닐 경우 자기 삭제
        if (SceneManager.GetActiveScene().name != "PvpIngame")
        {
            Destroy(gameObject);
        }
    }

    public void HandlePlayAgain(string roomCode)
    {
        RoomOptions options = new RoomOptions { MaxPlayers = 8 };
        PhotonNetwork.JoinOrCreateRoom(roomCode, options, TypedLobby.Default);
    }

    public void HandleGoToLobby()
    {
        SceneManager.LoadScene("Lobby 1");
    }
}
