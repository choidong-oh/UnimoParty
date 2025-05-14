using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlowerUi : MonoBehaviour
{
    //[SerializeField] TextMeshProUGUI GaugeText;


    //public void UpdateGauge(float Gauge)
    //{
    //    GaugeText.text = Gauge.ToString();
    //}
    //위에꺼는 동오씨 코드 

    //이거는 민우코드
    [SerializeField] private Image GatheringImage; // Radial Fill 이미지 (Filled로 설정)
    [SerializeField] private Flower flower;
    public void UpdateGauge(float currentGauge)
    {
        float fillAmount = Mathf.Clamp01(currentGauge / flower.HarvestTime);
        GatheringImage.fillAmount = fillAmount;
    }


}
