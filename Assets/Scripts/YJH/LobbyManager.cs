using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase;
using Oculus.Platform.Models;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("판넬들")]
    [SerializeField] GameObject LobbyCanvas;
    [SerializeField] GameObject PVECanvas;
    [SerializeField] GameObject NickNamePanel;

    [Space]
    [Header("닉네임 적는칸")]
    [SerializeField] InputField nickInputField;

    [Space]
    [Header("닉네임 경고창")]
    [SerializeField] TextMeshProUGUI NickNamewarningText;



    [Space]
    public Transform contentParent;
    public GameObject userButtonPrefab;

    private Dictionary<string, GameObject> playerButtons = new Dictionary<string, GameObject>();

    IEnumerator Start()
    {
        LobbyCanvas.SetActive(true);
        PVECanvas.SetActive(false);

        PhotonNetwork.ConnectUsingSettings();

        yield return new WaitUntil(() => PhotonNetwork.IsConnected);

        PhotonNetwork.NickName = "Player" + Random.Range(1000, 9999);
        //PhotonNetwork.NickName = FirebaseLoginMgr.user.DisplayName;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions options = new RoomOptions { MaxPlayers = 20 };
        PhotonNetwork.CreateRoom("LobbyRoom", options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            AddPlayerButton(p);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerButton(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerButton(otherPlayer);
    }

    void AddPlayerButton(Player p)
    {
        if (playerButtons.ContainsKey(p.NickName))
            return;

        GameObject button = Instantiate(userButtonPrefab, contentParent);
        button.GetComponentInChildren<TextMeshProUGUI>().text = p.NickName;
        playerButtons.Add(p.NickName, button);

        if (p.IsLocal)
        {
            button.transform.SetAsFirstSibling();
        }
    }

    void RemovePlayerButton(Player p)
    {
        if (playerButtons.TryGetValue(p.NickName, out GameObject btn))
        {
            Destroy(btn);
            playerButtons.Remove(p.NickName);
        }
    }

    public void OnClickPVESceneButton()
    {
        LobbyCanvas.SetActive(false);
        PVECanvas.SetActive(true);
    }

    public void OnClickBackButton()
    {
        LobbyCanvas.SetActive(true);
        PVECanvas.SetActive(false);
    }

    public void GameStartButton()
    {
        SceneManager.LoadScene(2);
    }

    public void CreateNickName()
    {
        StartCoroutine(CreateNickNameCor(nickInputField.text));
    }

    IEnumerator CreateNickNameCor(string NickName)
    {
        if (FirebaseLoginMgr.user != null)
        {
            UserProfile profile = new UserProfile { DisplayName = NickName };

            Task profileTask = FirebaseLoginMgr.user.UpdateUserProfileAsync(profile);
            while (profileTask.IsCompleted == false)
            {
                NickNamewarningText.text += "1";
                yield return null;
            }

            yield return new WaitUntil(() => profileTask.IsCompleted);


            if (profileTask.Exception != null)
            {
                Debug.LogWarning("닉네임 설정 실패: " + profileTask.Exception);
                FirebaseException firebaseEx = profileTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                NickNamewarningText.text = "닉네임 설정 실패";
            }
            else
            {
                int delay = 0;
                while (FirebaseLoginMgr.user.DisplayName == null || FirebaseLoginMgr.user.DisplayName != NickName)
                {
                    yield return new WaitForSeconds(0.2f);
                    delay++;
                    //NickNamewarningText.text = $"닉네임 저장... {delay}";
                }
                
                //yield return new WaitUntil(() => XRGeneralSettings.Instance.Manager.isInitializationComplete);
            }
        }
    }
}
