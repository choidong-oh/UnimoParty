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
using Random = UnityEngine.Random;

public class FirebaseDataMgr : MonoBehaviour
{
    public static FirebaseDataMgr Instance { get; private set; }
    private DatabaseReference dbReference;
    public int userMoney = -1;
   
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

    //private void Start()
    //{
    //    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(async task =>
    //    {
    //        FirebaseApp app = FirebaseApp.DefaultInstance;
    //        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    //        if (FirebaseAuthMgr.user != null)
    //        {
    //            //TODO:초기 저장 바꾸기 12000
    //            if (userMoney == -1)
    //            {
    //                //StartCoroutine(SaveUserData(FirebaseAuthMgr.user.DisplayName, "gold", 10000));
    //                userMoney = 10000;
    //            }

    //            // 친구 목록 불러오기
    //            //await LoadFriends(FirebaseAuthMgr.user.DisplayName);

    //        }
    //        else
    //        {
    //            Debug.LogError("파이어베이스 문제");
    //        }
    //    });
    //}

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
}
