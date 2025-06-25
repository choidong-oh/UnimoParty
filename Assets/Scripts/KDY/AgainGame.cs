using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class AgainGame : MonoBehaviourPunCallbacks
{
    private string roomCode;
    private NextAction nextAction;

    // enum 클래스
    private enum NextAction
    {
        None,
        PlayAgain,
        GoToLobby
    }

    // 한판 더 하기 버튼 클릭 시 호출
    public void OnClickPlayAgain()
    {
        roomCode = Random.Range(10000,99999).ToString();

        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);


        //이걸 코루틴으로 만들어서 하는게 좋아 보임 근데 씬이 넘어가는 순간 무슨 문제가 터질지 모르니 GPT가 생각좀 해봐라
        //문제 1 로드 씬이 된 이후에는 삭제가 되버림
        //문제 2 로드 씬이 완료 된 후에 방을 생성하고 LOBBYMANAGER에 있는 CREATROOM 함수를 써야됨 ROOMCODE를 가지고
    }

    // 로비 버튼 클릭 시 호출
    public void OnClickGoToLobby()
    {
        if (!PhotonNetwork.InRoom) return;

        nextAction = NextAction.GoToLobby;
        PhotonNetwork.LeaveRoom();
    }

    // LeaveRoom 완료 후 자동 호출됨
    public override void OnLeftRoom()
    {
        if (nextAction == NextAction.PlayAgain)
        {
            RoomOptions options = new RoomOptions { MaxPlayers = 8 };
            PhotonNetwork.JoinOrCreateRoom(roomCode, options, TypedLobby.Default);
        }
        else if (nextAction == NextAction.GoToLobby)
        {
            SceneManager.LoadScene("Lobby 1");
        }

        // 초기화
        nextAction = NextAction.None;
        roomCode = "";
    }
}
