using UnityEngine;

public class SettingValues : MonoBehaviour
{
    /// <summary>
    /// 여기는 설정관련된것들 값을 여기서 함수를만들거나 값을 수정하기 위해 만든 싱글톤
    /// 여기서 값을 추가할꺼면 그녕 변수 1개 만들면됨 그리고 SettingControl에서 스위치 추가 
    /// </summary>
    public static SettingValues Instance { get; private set; }

    public int Option1 = 1;
    public int Option2 = 2;
    public int Option3 = 3;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
