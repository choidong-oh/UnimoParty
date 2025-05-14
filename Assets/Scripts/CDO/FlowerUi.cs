using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlowerUi : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI GaugeText;

    
    public void UpdateGauge(float Gauge)
    {
        GaugeText.text = Gauge.ToString();
    }
   
}
