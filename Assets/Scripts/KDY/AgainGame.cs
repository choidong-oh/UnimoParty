using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections;

public class AgainGame : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // 씬 넘어가도 안 죽게 함
    }
    // "한 번 더 하기" 클릭 시 호출됨
    public void OnClickPlayAgain()
    {
        // 1. 랜덤 방 코드 생성 및 Manager에 저장
        string roomCode = Random.Range(10000, 99999).ToString();
        Manager.Instance.PlayAgainRoomCode = roomCode;
        Manager.Instance.IsPlayAgainPending = true;

        Debug.Log($"[AgainGame] 다시하기 선택 - 방 코드: {roomCode}");

        // 2. 방 나가기
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);
    }

}
