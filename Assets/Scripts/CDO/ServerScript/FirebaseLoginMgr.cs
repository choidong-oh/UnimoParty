using System.Collections;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;

public class FirebaseLoginMgr : MonoBehaviour
{
    //유저 
    static public FirebaseUser user;
    static public FirebaseAuth auth;


    //로그인용(TMP로 최신화 했음)
    [Header("로그인용")]
    [SerializeField] private TMP_InputField LoginIdInputField;
    [SerializeField] private TMP_InputField LoginPasswordInputField;
    [SerializeField] TextMeshProUGUI LoginwarningText;

    //회원가입용
    [Header("회원가입용")]
    [SerializeField] private TMP_InputField CreateIdInputField;
    [SerializeField] private TMP_InputField CreatePasswordInputField;
    [SerializeField] TextMeshProUGUI CreatewarningText;

    //닉네임 설정용
    [Header("닉네임 설정용")]
    [SerializeField] private TMP_InputField NickNameInputField;
    [SerializeField] TextMeshProUGUI NickNamewarningText;

    [Header("큰테두리Ui")]
    //[SerializeField] private GameObject SceneChanege;
    [SerializeField] private GameObject LoginUiPanel;
    [SerializeField] private GameObject CreateUiIdPanel;
    [SerializeField] private GameObject NickNameUiPanel;

    private void Awake()
    {
        //안전코드 auth연결
        //비동기식 ContinueWith
        //파이어베이스 초기화
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            DependencyStatus dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            }
        });


        CreatewarningText.text = "";
        LoginwarningText.text = "";

        CreateUiIdPanel.gameObject.SetActive(false);
        LoginUiPanel.gameObject.SetActive(true);
        NickNameUiPanel.gameObject.SetActive(false);
    }

    //회원가입패널로 넘어감
    //나중에는 인자값으로 받고 하면 될듯
    public void CreateIdPanel()
    {
        //회원가입패널
        CreateUiIdPanel.gameObject.SetActive(true);
        LoginUiPanel.gameObject.SetActive(false);
        NickNameUiPanel.gameObject.SetActive(false);

    }

    public void CreateIdPanelFalse()
    {
        LoginUiPanel.gameObject.SetActive(true);
        CreateUiIdPanel.gameObject.SetActive(false);
    }

    //닉네임패널로 넘어감
    public void NickNamePanel()
    {
        //회원가입패널
        CreateUiIdPanel.gameObject.SetActive(false);
        LoginUiPanel.gameObject.SetActive(false);
        NickNameUiPanel.gameObject.SetActive(true);

    }

    //로그인패널로 넘어감
    public void LoginPanel()
    {
        //회원가입패널
        CreateUiIdPanel.gameObject.SetActive(false);
        LoginUiPanel.gameObject.SetActive(true);
        NickNameUiPanel.gameObject.SetActive(false);

    }



    //회원가입
    public void CreateId()
    {
        StartCoroutine(CreateIdCor(CreateIdInputField.text, CreatePasswordInputField.text));
    }

    //로그인
    public void Login()
    {
        StartCoroutine(LoginCor(LoginIdInputField.text, LoginPasswordInputField.text));
    }

    //로그아웃
    public void Logout()
    {
        auth.SignOut();
    }

    //public void CreateNickName()
    //{
    //    StartCoroutine(CreateNickNameCor(NickNameInputField.text));
    //}

    //IEnumerator CreateNickNameCor(string NickName)
    //{
    //    if (user != null)
    //    {
    //        UserProfile profile = new UserProfile { DisplayName = NickName };

    //        Task profileTask = user.UpdateUserProfileAsync(profile);
    //        while (profileTask.IsCompleted ==false)
    //        {
    //            NickNamewarningText.text += "1";
    //            yield return null;
    //        }

    //        yield return new WaitUntil(() => profileTask.IsCompleted);


    //        if (profileTask.Exception != null)
    //        {
    //            Debug.LogWarning("닉네임 설정 실패: " + profileTask.Exception);
    //            FirebaseException firebaseEx = profileTask.Exception.GetBaseException() as FirebaseException;
    //            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
    //            NickNamewarningText.text = "닉네임 설정 실패";
    //        }
    //        else
    //        {
    //            int delay = 0;
    //            while (user.DisplayName==null || user.DisplayName != NickName)
    //            {
    //                yield return new WaitForSeconds(0.2f);
    //                delay++;
    //                //NickNamewarningText.text = $"닉네임 저장... {delay}";
    //            }

    //            //yield return new WaitUntil(() => XRGeneralSettings.Instance.Manager.isInitializationComplete);
    //            SceneManager.LoadScene("Lobby");
    //        }
    //    }
    //}

    //IEnumerator CreateNickNameCor(string nickName)
    //{
    //    if (user != null)
    //    {
    //        UserProfile profile = new UserProfile { DisplayName = nickName };
    //        var updateTask = user.UpdateUserProfileAsync(profile);
    //        yield return new WaitUntil(() => updateTask.IsCompleted);

    //        if (updateTask.Exception != null)
    //        {
    //            NickNamewarningText.text = "닉네임 설정 실패";
    //            yield break;
    //        }

    //        // 최대 5초(10회)까지 닉네임 반영 기다리기
    //        int retry = 0;
    //        while (retry < 10)
    //        {
    //            var reloadTask = user.ReloadAsync();
    //            yield return new WaitUntil(() => reloadTask.IsCompleted);
    //            yield return new WaitForSeconds(0.3f);

    //            if (!string.IsNullOrEmpty(user.DisplayName) && user.DisplayName == nickName)
    //                break;

    //            retry++;
    //        }

    //        if (user.DisplayName != nickName)
    //        {
    //            NickNamewarningText.text = "닉네임 적용 지연. 다시 시도해주세요";
    //            yield break;
    //        }

    //        // VR용 XR 초기화 대기
    //        yield return new WaitUntil(() => XRGeneralSettings.Instance.Manager.isInitializationComplete);

    //        // 닉네임 정상 반영 후 씬 이동
    //        SceneManager.LoadScene("Lobby");
    //    }
    //}

    //동기식 회원가입 코루틴
    IEnumerator CreateIdCor(string ID, string password)
    {
        var createIdTask = auth.CreateUserWithEmailAndPasswordAsync(ID + "@unimo.com", password);
        //회원가입 성공할때 까지
        yield return new WaitUntil(predicate: () => createIdTask.IsCompleted);
        if (createIdTask.Exception != null)
        {
            Debug.LogWarning(message: "다음과 같은 이유로 회원가입 실패:" + createIdTask.Exception);
            FirebaseException firebaseEx = createIdTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = " 회원가입 실패";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "아이디를 입력해주세요";
                    break;
                case AuthError.MissingPassword:
                    message = "패스워드를 입력해주세요";
                    break;
                case AuthError.WeakPassword:
                    message = "최소 6자리 이상으로 만들어주세요";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "중복된 아이디 입니다";
                    break;
                default:
                    message = "관리자에게 문의 바랍니다";
                    break;
            }
            CreatewarningText.text = message;
        }
        else
        {
            Debug.Log("회원가입 완료");
            user = createIdTask.Result.User;
            CreatewarningText.text = "";
            LoginUiPanel.gameObject.SetActive(true);
            CreateUiIdPanel.gameObject.SetActive(false);
        }

    }

    //동기식 로그인 코루틴 
    IEnumerator LoginCor(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email + "@unimo.com", password);
        //로그인 성공할때 까지
        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);
        if (loginTask.Exception != null)
        {
            Debug.LogWarning(message: "다음과 같은 이유로 로그인 실패:" + loginTask.Exception);
            FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = "로그인 실패";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "아이디를 입력해주세요";
                    break;
                case AuthError.MissingPassword:
                    message = "패스워드를 입력해주세요";
                    break;
                case AuthError.WrongPassword:
                    message = "패스워드가 들렸습니다";
                    break;
                case AuthError.UserNotFound:
                    message = "아이디를 찾을 수 없습니다";
                    break;
                default:
                    message = "관리자에게 문의 바랍니다";
                    break;
            }
            LoginwarningText.text = message;
        }
        else
        {
            user = loginTask.Result.User;
            LoginwarningText.text = "";
            LoginUiPanel.gameObject.SetActive(false);

            SceneManager.LoadScene("Lobby");
        }
    }
}
