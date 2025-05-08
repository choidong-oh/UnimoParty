using UnityEngine;
using UnityEngine.UI;
using Oculus.Platform;

public class UserManager : MonoBehaviour
{
    public static string UserID { get; private set; } = "Unknown";

    public delegate void OnUserReadyDelegate(string userID);
    public static event OnUserReadyDelegate OnUserReady;

    [Header("Editor 테스트용 가짜 ID")]
    public string mockUserID = "TEST_USER_01";

    [Header("UI 표시용 (선택사항)")]
    public Text displayNameText;
    void Start()
    {
        if (!Core.IsInitialized())
            Core.Initialize();

        Entitlements.IsUserEntitledToApplication().OnComplete(entitlementMsg =>
        {
            if (entitlementMsg.IsError)
            {
                Debug.LogError("[UserManager] 정품 인증 실패");
                UserID = "Unauthorized";
                if (displayNameText != null)
                    displayNameText.text = "Not entitled";
                OnUserReady?.Invoke(UserID);
                return;
            }

            Debug.Log("[UserManager] 정품 인증 성공");

            Users.GetLoggedInUser().OnComplete(userMsg =>
            {
                if (userMsg.IsError)
                {
                    Debug.LogError("[UserManager] 사용자 정보 로딩 실패: " + userMsg.GetError().Message);
                    UserID = "UserLoadFailed";
                }
                else
                {
                    UserID = userMsg.Data.OculusID;
                    Debug.Log("[UserManager] Oculus ID: " + UserID);
                }

                if (displayNameText != null)
                    displayNameText.text = "Hello, " + UserID + "!";

                OnUserReady?.Invoke(UserID);
            });
        });
    }

}
