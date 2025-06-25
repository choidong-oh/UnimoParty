using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class AgainGame : MonoBehaviourPunCallbacks
{
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
    }

    // "로비로 가기" 클릭 시 호출됨
    public void OnClickGoToLobby()
    {
        Manager.Instance.IsPlayAgainPending = false;
        Manager.Instance.PlayAgainRoomCode = "";

        Debug.Log("[AgainGame] 로비로 가기 선택");
        PhotonNetwork.LeaveRoom();
    }

    // LeaveRoom 완료 후 콜백
    public override void OnLeftRoom()
    {
        Debug.Log("[AgainGame] 방 나가기 완료 → 로비로 이동");
        SceneManager.LoadScene("Lobby 1"); // 공통 처리
    }
}
