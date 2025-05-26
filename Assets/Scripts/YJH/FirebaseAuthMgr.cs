using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class FirebaseAuthMgr : MonoBehaviour
{
    public static FirebaseAuthMgr Instance { get; private set; }

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private Button loginButton;

    [SerializeField]
    private Button signUpButton;

    public static FirebaseUser user; // 인증된 유저 정보

    public static DatabaseReference dbRef; // DB 추가

    private static bool hasUser;

    bool test = false;
    public static bool HasUser
    {
        get
        {
            return hasUser;
        }
    }

    public static bool IsFirebaseReady { get; private set; } = false;

    public FirebaseAuth auth; // 인증 진행을 위한 정보

    [SerializeField]
    private TMP_InputField emailField;

    [SerializeField]
    private TMP_InputField passwordField;

    [SerializeField]
    private TMP_InputField nicknameField;

    [SerializeField]
    private TextMeshProUGUI warningText;

    [SerializeField]
    private TextMeshProUGUI confirmText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);

            return;
        }
        else
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            DependencyStatus dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;

                dbRef = FirebaseDatabase.DefaultInstance.RootReference;

                IsFirebaseReady = true;
            }

            else
            {
                Debug.LogError("파이어베이스 오류");
            }
        });

        if (user == null)
        {
            hasUser = false;
        }
        else
        {
            hasUser = true;
        }
    }

    private void Start()
    {
        if (startButton != null) startButton.interactable = false;

        if (warningText != null) warningText.text = "";

        if (confirmText != null) confirmText.text = "";

        loginButton.onClick.AddListener(() => Login());

        signUpButton.onClick.AddListener(() => Register());
    }

    private void OnDisable()
    {
        loginButton.onClick.RemoveListener(() => Login());

        signUpButton.onClick.RemoveListener(() => Register());
    }

    public void Login()
    {
        StartCoroutine(LoginCor(emailField.text + "@unimo.com", passwordField.text));
    }

    public void Register()
    {
        StartCoroutine(RegisterCor(emailField.text + "@unimo.com", passwordField.text, nicknameField.text));
    }

    private void SetButtonInteractable()
    {
        startButton.interactable = !startButton.interactable;

        loginButton.interactable = !loginButton.interactable;

        signUpButton.interactable = !signUpButton.interactable;
    }

    #region 로그인 코루틴

    private IEnumerator LoginCor(string email, string password)
    {
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        // 로그인 계정에 문제가 있다면
        if (LoginTask.Exception != null)
        {
            Debug.LogWarning(message: "다음과 같은 이유로 로그인 실패: " + LoginTask.Exception);

            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;

            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "";

            switch (errorCode)
            {
                case AuthError.MissingEmail:

                    message = "Missing email";

                    break;

                case AuthError.MissingPassword:

                    message = "Missing password";

                    break;

                case AuthError.WrongPassword:

                    message = "Wrong password";

                    break;

                case AuthError.InvalidEmail:

                    message = "Invalid email format";

                    break;

                case AuthError.UserNotFound:

                    message = "User not found";

                    break;

                default:

                    message = "Please contact the administrator";

                    break;
            }

            warningText.text = message;
        }

        // 로그인 계정에 문제가 없다면
        else
        {
            // 로그인
            user = LoginTask.Result.User; // 유저 정보 기억

            hasUser = true;

            warningText.text = "";

            nicknameField.text = user.DisplayName;

            confirmText.text = "nickname: " + user.DisplayName;

            SetButtonInteractable();

            if (test)
            {
                SceneManager.LoadScene(2);
            }
        }
    }

    #endregion

    #region 회원가입 코루틴

    private IEnumerator RegisterCor(string email, string password, string username)
    {
        if (username == "")
        {
            warningText.text = "Nickname is missing";
        }
        else
        {
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            // 회원가입 문제가 있다면
            if (RegisterTask.Exception != null)
            {
                Debug.LogWarning(message: "실패 사유" + RegisterTask.Exception);

                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;

                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Registration failed";

                switch (errorCode)
                {
                    case AuthError.MissingEmail:

                        message = "Missing email";

                        break;

                    case AuthError.MissingPassword:

                        message = "Missing password";

                        break;

                    case AuthError.WeakPassword:

                        message = "Weak password";

                        break;

                    case AuthError.EmailAlreadyInUse:

                        message = "Email already in use";

                        break;

                    default:

                        message = "Other reason. Please contact the administrator";

                        break;
                }

                warningText.text = message;
            }

            // 회원가입 문제가 없다면
            else
            {
                // 바로 로그인
                user = RegisterTask.Result.User;

                hasUser = true;

                if (user != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = username };

                    Task ProfileTask = user.UpdateUserProfileAsync(profile);

                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    yield return StartCoroutine(InitPlayerCurrency());

                    if (ProfileTask.Exception != null)
                    {
                        Debug.LogWarning(message: "닉네임 설정 실패" + ProfileTask.Exception);

                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;

                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                        warningText.text = "Failed to set nickname";
                    }
                    else
                    {
                        warningText.text = "";

                        confirmText.text = "nickname: " + user.DisplayName;

                        SetButtonInteractable();
                    }
                }
            }
        }
    }

    private IEnumerator InitPlayerCurrency() // 회원가입 시 재화 초기값 설정
    {
        // 초기 인게임 재화 생성
        var DBTask = dbRef.Child("users").Child(user.UserId).Child(user.DisplayName).Child("rewardIngameCurrency").SetValueAsync(0);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        // 초기 메타 재화 생성
        DBTask = dbRef.Child("users").Child(user.UserId).Child(user.DisplayName).Child("rewardMetaCurrency").SetValueAsync(0);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
    }
    #endregion

    public void StartButton()
    {
        SceneManager.LoadScene(1);
    }
    public void AdminButton()
    {
        nicknameField.text = "111";
        passwordField.text = "111111";
        test = true;
        Login();
    }

}
