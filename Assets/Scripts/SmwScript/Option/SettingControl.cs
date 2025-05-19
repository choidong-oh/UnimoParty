using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingControl : MonoBehaviour
{
    /// <summary>
    /// 여기는 버튼을 누르면 값이 바뀌게겠끔 한 클래스 만약 설정 더 추가할꺼면 switch로 되있는부분 2군데 추가하고 사용하면됨
    /// </summary>
    [Header("UI 연결")]
    public TextMeshProUGUI settingNameText;
    public TextMeshProUGUI settingValueText;
    public Button increaseButton;
    public Button decreaseButton;

    [Header("설정 키와 범위")]
    [SerializeField] private string settingKey = "Option1";
    private int minValue = 0;
    private int maxValue = 100;

    void Start()
    {
        settingNameText.text = settingKey;

        increaseButton.onClick.AddListener(() => ChangeValue(+1));
        decreaseButton.onClick.AddListener(() => ChangeValue(-1));

        UpdateValueText();
    }

    void ChangeValue(int delta)
    {
        int current = GetSettingValue();
        int next = Mathf.Clamp(current + delta, minValue, maxValue);
        SetSettingValue(next);
        UpdateValueText();
    }

    void UpdateValueText()
    {
        settingValueText.text = GetSettingValue().ToString();
    }

    int GetSettingValue()
    {
        switch (settingKey)
        {
            //여기 스위치 추가하면됨1
            case "Option1": return SettingValues.Instance.Option1;
            case "Option2": return SettingValues.Instance.Option2;
            case "Option3": return SettingValues.Instance.Option3;
            default: return 0;
        }
    }

    void SetSettingValue(int value)
    {
        switch (settingKey)
        {
            //여기 스위치 추가하면됨2
            case "Option1": SettingValues.Instance.Option1 = value; break;
            case "Option2": SettingValues.Instance.Option2 = value; break;
            case "Option3": SettingValues.Instance.Option3 = value; break;
        }
    }
}
