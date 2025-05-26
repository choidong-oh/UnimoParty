using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FirebaseDataMgr : MonoBehaviour
{
    public static FirebaseDataMgr Instance { get; private set; }
    private DatabaseReference dbReference;
    public int userMoney = -1;

    [SerializeField] GameObject NickNamePanel;

    [Space]
    [Header("닉네임 적는칸")]
    [SerializeField] TMP_InputField nickInputField;

    [Space]
    [Header("닉네임 경고창")]
    [SerializeField] TextMeshProUGUI NickNamewarningText;

    // 추가: 친구 목록 리스트
    public List<string> friendList = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {


        yield return new WaitUntil(() => FirebaseApp.DefaultInstance != null);

        yield return new WaitUntil(() => FirebaseLoginMgr.user != null);

        if (string.IsNullOrWhiteSpace(FirebaseLoginMgr.user.DisplayName))
        {
            NickNamePanel.SetActive(true);
            yield return new WaitUntil(() => !string.IsNullOrEmpty(FirebaseLoginMgr.user.DisplayName));
        }


        NickNamePanel.SetActive(false);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(async task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            if (FirebaseLoginMgr.user != null)
            {
                //TODO:초기 저장 바꾸기 12000
                userMoney = await LoadUserDataAsync(FirebaseLoginMgr.user.DisplayName, "gold", userMoney);
                if (userMoney == -1)
                {
                    StartCoroutine(SaveUserData(FirebaseLoginMgr.user.DisplayName, "gold", 10000));
                    userMoney = 10000;
                }

                // 친구 목록 불러오기
                //await LoadFriends(FirebaseLoginMgr.user.DisplayName);

            }
            else
            {
                Debug.LogError("파이어베이스 문제");
            }
        });
    }

    public void CreateNickName()
    {
        StartCoroutine(CreateNickNameCor());
    }
    IEnumerator CreateNickNameCor()
    {
        if (FirebaseLoginMgr.user != null)
        {
            UserProfile profile = new UserProfile
            {
                DisplayName = nickInputField.text
            };

            Task profileTask = FirebaseLoginMgr.user.UpdateUserProfileAsync(profile);

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
                if (!string.IsNullOrEmpty(FirebaseLoginMgr.user.DisplayName))
                {
                    NickNamewarningText.text = "닉네임 설정 완료!";
                }
                else
                {
                    NickNamewarningText.text = "닉네임 정보 로드 실패";
                }
            }
        }
    }

    //데이터 저장 함수
    //SaveUserData(id,"level",5);
    //id의 레벨은 5 추가?
    //ContinueWithOnMainThread 메인쓰레드에서 함
    public IEnumerator SaveUserData<T>(string userId, string dataName, T value)
    {
        var task = dbReference.Child("users").Child(userId).Child(dataName).SetValueAsync(value);

        yield return new WaitUntil(()=> task.IsCompleted);
    }

    //데이터 불러오기 함수
    //함수쓸때 앞에 await 붙여야댐
    //playerLevel = await LoadUserDataAsync(id, "level", useLevel);
    //id의 레벨 불러오고 playerLevel 변수에 담음
    //playerLevel =  데이터value; 이런식
    //await할때까지 기달림
    //https://ljhyunstory.tistory.com/284 
    public async Task<T> LoadUserDataAsync<T>(string userId, string dataName, T type)
    {
        // 비동기적으로 데이터 불러오기
        DataSnapshot snapshot = await dbReference.Child("users").Child(userId).Child(dataName).GetValueAsync();
        T Tvalue;

        Tvalue = type;
        if (snapshot.Exists)
        {
            //타입을 바꿔서 집어넣음
            Tvalue = (T)Convert.ChangeType(snapshot.Value, typeof(T));
            Debug.Log(userId + "의 " + dataName + "불러옴");
            Debug.Log("Tvalue : " + Tvalue);
        }
        else
        {
            Debug.Log("저장된 데이터 없음");
        }
        return Tvalue;
    }

    //// 친구 추가 함수
    //public void AddFriend(string friendNickname)
    //{
    //    if (!friendList.Contains(friendNickname))
    //    {
    //        friendList.Add(friendNickname);
    //        SaveFriends(FirebaseLoginMgr.user.DisplayName);
    //    }
    //}

    //// 친구 저장 함수 (리스트를 전부 저장)
    //public void SaveFriends(string userId)
    //{
    //    for (int i = 0; i < friendList.Count; i++)
    //    {
    //        dbReference.Child("users").Child(userId).Child("friends").Child(i.ToString())
    //            .SetValueAsync(friendList[i]);
    //    }
    //}

    //// 친구 목록 불러오기 함수 (시작 시 불러옴)
    //public async Task LoadFriends(string userId)
    //{
    //    friendList.Clear();
    //    DataSnapshot snapshot = await dbReference.Child("users").Child(userId).Child("friends").GetValueAsync();
    //    if (snapshot.Exists)
    //    {
    //        foreach (DataSnapshot child in snapshot.Children)
    //        {
    //            friendList.Add(child.Value.ToString());
    //        }
    //        Debug.Log(userId + "의 친구 목록 불러옴: " + string.Join(", ", friendList));
    //    }
    //    else
    //    {
    //        Debug.Log(userId + "의 친구 목록 없음");
    //    }
    //}

    //// 특정 닉네임이 친구인지 확인
    //public bool IsFriend(string nickname)
    //{
    //    return friendList.Contains(nickname);
    //}

}
