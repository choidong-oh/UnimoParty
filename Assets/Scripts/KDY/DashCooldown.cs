using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // 신형 Input System 네임스페이스  ..테스트용 < 삭제할거임


public class DashCooldown : MonoBehaviour
{
    [Header("쿨타임 설정")]
    public float cooldownTime = 5f;

    [Header("UI 연결")]
    public Image cooldownImage;      // 덮개 이미지 (fillAmount 조절용)
    public Image dashIconImage;      // 색상 변경용 아이콘 이미지

    [Header("쿨타임 색상")]
    public Color cooldownColor = Color.gray;

    private float timer = 0f;
    private bool isCooldown = false;
    private Color originalColor;

    // 신형 InputSystem 액션
    private InputAction dashAction;

    void Start()
    {
        // 원래 색상 기억
        originalColor = dashIconImage.color;

        // 테스트용 액션 설정 (Shift 키) << 삭제할거임
        dashAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/C");
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
            timer -= Time.deltaTime;
            cooldownImage.fillAmount = timer / cooldownTime;

            if (timer <= 0f)
            {
                EndCooldown();
            }
        }
    }

    public void StartCooldown()
    {
        isCooldown = true;
        timer = cooldownTime;

        dashIconImage.color = cooldownColor;
        cooldownImage.fillAmount = 1f;
    }

    private void EndCooldown()
    {
        isCooldown = false;
        cooldownImage.fillAmount = 0f;
        dashIconImage.color = originalColor;
    }
}
