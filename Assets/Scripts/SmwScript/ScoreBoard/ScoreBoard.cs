using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [Header("텍스트")]
    [SerializeField] private TextMeshProUGUI[] texts = new TextMeshProUGUI[4];

    // 목표값은 GoalFairyCount를 참조 (Manager에서 가져온다고 가정)
    private GoalFairyCount goalCount
    {
        get { return Manager.Instance.goalCount; }
    }

    private void Start()
    {
        Manager.Instance.observer.OnGameDataChange += HpPoint;
        Manager.Instance.observer.OnGameDataChange += FairyType1Score;
        Manager.Instance.observer.OnGameDataChange += FairyType2Score;
        Manager.Instance.observer.OnGameDataChange += FairyType3Score;
    }

    private void OnDestroy()//이거는 안전코드
    {
        Manager.Instance.observer.OnGameDataChange -= HpPoint;
        Manager.Instance.observer.OnGameDataChange -= FairyType1Score;
        Manager.Instance.observer.OnGameDataChange -= FairyType2Score;
        Manager.Instance.observer.OnGameDataChange -= FairyType3Score;
    }

    // 체력 표시 (texts[0])
    public void HpPoint(DataCenter data)
    {
        texts[0].text = $"HP: {data.life}";
    }

    // 각 FairyType별 점수 표시 (목표값은 goalCount에서 바로 가져옴)
    public void FairyType1Score(DataCenter data)
    {
        texts[1].text = $"{data.playerFairyType.FairyDataType_1} / {goalCount.GoalFairyValue_1}";
    }
    public void FairyType2Score(DataCenter data)
    {
        texts[2].text = $"{data.playerFairyType.FairyDataType_2} / {goalCount.GoalFairyValue_2}";
    }
    public void FairyType3Score(DataCenter data)
    {
        texts[3].text = $"{data.playerFairyType.FairyDataType_3} / {goalCount.GoalFairyValue_3}";
    }

    public void ResetScore()
    {
        for (int i = 0; i < texts.Length; i++)
            texts[i].text = $"0 / 0";
    }
}
