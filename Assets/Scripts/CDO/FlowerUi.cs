using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlowerUi : MonoBehaviour
{
    [SerializeField] private Image GatheringImage; // Radial Fill 이미지 (Filled로 설정)
    [SerializeField] private Flower flower;
    public void UpdateGauge(float currentGauge)
    {
        float fillAmount = Mathf.Clamp01(currentGauge / flower.HarvestTime);
        GatheringImage.fillAmount = fillAmount;
    }
}
