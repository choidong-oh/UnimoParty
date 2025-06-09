using UnityEngine;
using TMPro;

public class GoalCollectUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI row1;
    [SerializeField] private TextMeshProUGUI row2;
    [SerializeField] private TextMeshProUGUI row3;

    private void Start()
    {
        // 게임시작 초기값
        //UpdateCollectUI(Manager.Instance.observer.UserPlayer.gamedata);

        // 채집 수가 바뀔 때마다 UI 업데이트
        Manager.Instance.observer.OnGameDataChange += UpdateCollectUI;
    }

    private void OnDestroy()
    {
        // 구독 해제
        Manager.Instance.observer.OnGameDataChange -= UpdateCollectUI;
    }

    // 채집 수 / 목표 수 갱신
    private void UpdateCollectUI(DataCenter data)
    {
        // 목표량
        int goal1 = Manager.Instance.tempFairyValue_1;
        int goal2 = Manager.Instance.tempFairyValue_2;
        int goal3 = Manager.Instance.tempFairyValue_3;

        // 플레이어가 오브젝트에 반납한 페어리 총 수량 (UI에 표시될 채집 수)
        // DeliveryFairy 함수 호출 시에만 증가
        int current1 = Manager.Instance.goalCount.GoalFairyValue_1;
        int current2 = Manager.Instance.goalCount.GoalFairyValue_2;
        int current3 = Manager.Instance.goalCount.GoalFairyValue_3;


        row1.text = $"{current1} / {goal1}";
        row2.text = $"{current2} / {goal2}";
        row3.text = $"{current3} / {goal3}";
    }

}
