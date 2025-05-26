using UnityEngine;
using TMPro;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private float gameDuration = 120f;

   
    private float currentTime;

   
    [SerializeField] private TextMeshProUGUI timeText;

    private void Start()
    {
        currentTime = gameDuration;
    }

    private void Update()
    {
        if (currentTime <= 0f)
        {
            currentTime = 0f;

            //게임 종료 호출  isGameOver 처리 포함됨 (팀장님이 반영함)
            Manager.Instance.observer.EndGame();

            //타이머 비활성화로 정지
            enabled = false;
            return;
        }

        currentTime -= Time.deltaTime;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timeText.text = $"{minutes:00}:{seconds:00}";
    }
}
