//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.InputSystem; // 신형 Input System 네임스페이스  ..테스트용 < 삭제할거임


//public class DashCooldown : MonoBehaviour
//{
//    [Header("쿨타임 설정")]
//    public float cooldownTime = 5f;

//    [Header("UI 연결")]
//    public Image cooldownImage;      // 덮개 이미지 (fillAmount 조절용)
//    public Image dashIconImage;      // 색상 변경용 아이콘 이미지

//    [Header("쿨타임 색상")]
//    public Color cooldownColor = Color.gray;

//    private float timer = 0f;
//    private bool isCooldown = false;
//    private Color originalColor;

//    // 신형 InputSystem 액션
//    private InputAction dashAction;

//    void Start()
//    {
//        // 원래 색상 기억
//        originalColor = dashIconImage.color;

//        // 테스트용 액션 설정 (Shift 키) << 삭제할거임
//        dashAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/C");
//        dashAction.performed += ctx =>
//        {
//            if (!isCooldown)
//                StartCooldown();
//        };
//        dashAction.Enable();
//    }

//    void Update()
//    {
//        if (isCooldown)
//        {
//            timer -= Time.deltaTime;
//            cooldownImage.fillAmount = timer / cooldownTime;

//            if (timer <= 0f)
//            {
//                EndCooldown();
//            }
//        }
//    }

//    public void StartCooldown()
//    {
//        isCooldown = true;
//        timer = cooldownTime;

//        dashIconImage.color = cooldownColor;
//        cooldownImage.fillAmount = 1f;
//    }

//    private void EndCooldown()
//    {
//        isCooldown = false;
//        cooldownImage.fillAmount = 0f;
//        dashIconImage.color = originalColor;
//    }
//}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // 신형 Input System 네임스페이스

public class DashCooldown : MonoBehaviour
{
    [Header("쿨타임 설정")]
    public float cooldownTime = 5f; // 쿨타임 지속 시간

    [Header("UI 연결")]
    public Image cooldownImage;      // 덮개 이미지 (fillAmount 조절용)
    public Image dashIconImage;      // Dash 아이콘 이미지 (색상 변경용)

    [Header("쿨타임 색상")]
    public Color cooldownColor = new Color32(165, 165, 165, 165); // 쿨타임 중 색
    private Color readyColor = new Color32(255, 255, 255, 255);   // 쿨타임 완료 색

    private float timer = 0f;
    private bool isCooldown = false;

    // 신형 Input System 액션 (C 키로 테스트)
    private InputAction dashAction;

    void Start()
    {
        // 초기 상태: Dash 사용 가능
        dashIconImage.color = readyColor;
        cooldownImage.fillAmount = 1f;

        // C 키 입력 시 StartCooldown() 실행
        dashAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/c");
        dashAction.performed += ctx =>
        {
            if (!isCooldown)
                StartCooldown();
        };
        dashAction.Enable();
    }

    void Update()
    {
        if (isCooldown)
        {
            // 타이머 누적
            timer += Time.deltaTime;

            // 덮개 이미지 점점 채워짐 (왼→오)
            cooldownImage.fillAmount = timer / cooldownTime;

            // 쿨타임 완료 시 처리
            if (timer >= cooldownTime)
            {
                EndCooldown();
            }
        }
    }

    public void StartCooldown()
    {
        isCooldown = true;
        timer = 0f;

        // 색을 흐리게, 덮개는 가려진 상태로 시작
        dashIconImage.color = cooldownColor;
        cooldownImage.fillAmount = 0f;
    }

    private void EndCooldown()
    {
        isCooldown = false;

        // 완전히 보이는 상태로 복구
        dashIconImage.color = readyColor;
        cooldownImage.fillAmount = 1f;
    }
}
