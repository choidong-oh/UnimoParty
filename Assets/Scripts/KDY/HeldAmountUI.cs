using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeldAmountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI row1;
    [SerializeField] private TextMeshProUGUI row2;
    [SerializeField] private TextMeshProUGUI row3;

    // 최대 보유 수량 고정값
    private int MAX_AMOUNT = 99;

    private void Start()
    {
        UpdateHaveUI();
    }

    public void UpdateHaveUI()
    {
        int have1 = Manager.Instance.goalCount.GoalFairyValue_1;
        int have2 = Manager.Instance.goalCount.GoalFairyValue_2;
        int have3 = Manager.Instance.goalCount.GoalFairyValue_3;

        row1.text = $"{have1:00} / {MAX_AMOUNT}";
        row2.text = $"{have2:00} / {MAX_AMOUNT}";
        row3.text = $"{have3:00} / {MAX_AMOUNT}";
    }
}
