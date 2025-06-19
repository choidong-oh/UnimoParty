using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectStatusUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI row1;
    [SerializeField] private TextMeshProUGUI row2;
    [SerializeField] private TextMeshProUGUI row3;

    private void Start()
    {
        Manager.Instance.observer.OnGameDataChange += UpdateHaveUI;
    }

    private void OnDestroy()
    {
        Manager.Instance.observer.OnGameDataChange -= UpdateHaveUI;
    }

    public void UpdateHaveUI(DataCenter data)
    {
        int have1 = data.playerFairyType.FairyDataType_1;
        int have2 = data.playerFairyType.FairyDataType_2;
        int have3 = data.playerFairyType.FairyDataType_3;

        int goal1 = Manager.Instance.tempFairyValue_1;
        int goal2 = Manager.Instance.tempFairyValue_2;
        int goal3 = Manager.Instance.tempFairyValue_3;

        row1.text = $"{have1:00} / {goal1:00}";
        row2.text = $"{have2:00} / {goal2:00}";
        row3.text = $"{have3:00} / {goal3:00}";
    }
}

