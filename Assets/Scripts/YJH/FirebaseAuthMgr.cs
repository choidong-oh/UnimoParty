// FirebaseAuthMgr.cs
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class FirebaseAuthMgr : MonoBehaviour
{
    public static FirebaseAuthMgr Instance { get; private set; }

    [SerializeField] private Button startButton;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button signUpButton;
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField nicknameField;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private TextMeshProUGUI confirmText;

    public static FirebaseUser user;
    public static DatabaseReference dbRef;
    public static bool HasUser => user != null;

    public FirebaseAuth auth;

    private bool test = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.LogError("Firebase dependency issue.");
            }
        });
    }

    private void Start()
    {
        if (startButton != null) startButton.interactable = false;
        if (warningText != null) warningText.text = "";
        if (confirmText != null) confirmText.text = "";

        loginButton.onClick.AddListener(Login);
        signUpButton.onClick.AddListener(Register);
    }

    private void OnDisable()
    {
        loginButton.onClick.RemoveListener(Login);
        signUpButton.onClick.RemoveListener(Register);
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

    private IEnumerator LoginCor(string email, string password)
    {
        Task<AuthResult> loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            var errorCode = (AuthError)((FirebaseException)loginTask.Exception.GetBaseException()).ErrorCode;
            warningText.text = errorCode.ToString();
        }
        else
        {
            user = loginTask.Result.User;
            warningText.text = "";
            nicknameField.text = user.DisplayName;
            confirmText.text = "nickname: " + user.DisplayName;
            SetButtonInteractable();

            if (test) SceneManager.LoadScene(2);
        }
    }

    private IEnumerator RegisterCor(string email, string password, string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            warningText.text = "Nickname is missing";
            yield break;
        }

        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            var errorCode = (AuthError)((FirebaseException)registerTask.Exception.GetBaseException()).ErrorCode;
            warningText.text = errorCode.ToString();
        }
        else
        {
            user = registerTask.Result.User;
            UserProfile profile = new UserProfile { DisplayName = username };
            Task profileTask = user.UpdateUserProfileAsync(profile);
            yield return new WaitUntil(() => profileTask.IsCompleted);

            if (profileTask.Exception != null)
            {
                warningText.text = "Failed to set nickname";
            }
            else
            {
                warningText.text = "";
                confirmText.text = "nickname: " + user.DisplayName;
                SetButtonInteractable();
                yield return StartCoroutine(InitPlayerCurrency());
            }
        }
    }

    private IEnumerator InitPlayerCurrency()
    {
        yield return SaveUserData(user.DisplayName, "rewardIngameCurrency", 0);
        yield return SaveUserData(user.DisplayName, "rewardMetaCurrency", 0);
    }

    public IEnumerator SaveUserData<T>(string userId, string dataName, T value)
    {
        var task = dbRef.Child("users").Child(userId).Child(dataName).SetValueAsync(value);
        yield return new WaitUntil(() => task.IsCompleted);
    }

    public void StartButton()
    {
        SceneManager.LoadScene(1);
    }

    public void AdminButton()
    {
        emailField.text = "111";
        passwordField.text = "111111";
        test = true;
        Login();
    }

    
}
