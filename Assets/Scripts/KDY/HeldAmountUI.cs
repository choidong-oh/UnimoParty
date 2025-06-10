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
        Manager.Instance.observer.OnGameDataChange += UpdateHaveUI;
    }

    private void OnDestroy()
    {
        // 구독 해제
        Manager.Instance.observer.OnGameDataChange -= UpdateHaveUI;
    }

    public void UpdateHaveUI(DataCenter data)
    {
        int have1 = data.playerFairyType.FairyDataType_1;
        int have2 = data.playerFairyType.FairyDataType_2;
        int have3 = data.playerFairyType.FairyDataType_3;

        row1.text = $"{have1:00} / {MAX_AMOUNT}";
        row2.text = $"{have2:00} / {MAX_AMOUNT}";
        row3.text = $"{have3:00} / {MAX_AMOUNT}";
    }
}
