using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [SerializeField] Slider[] sliders;
    [SerializeField] TextMeshProUGUI[] sliderTexts;

    [SerializeField] Toggle[] operationToggles;
    [SerializeField] Toggle[] angleToggles;
    private void Start()
    {
        OperationChange();
    }
    public void ValueChange()
    {
        for(int i=0;i<sliders.Length;i++)
        {
            sliderTexts[i].text = sliders[i].value.ToString();
        }
    }
    public void OperationChange()
    {
        if (operationToggles[0].isOn)
        {
            for (int i = 0; i < angleToggles.Length;i++)
            {
                angleToggles[i].interactable = false;
            }
        }
        else if(operationToggles[1].isOn)
        {
            for (int i = 0; i < angleToggles.Length; i++)
            {
                angleToggles[i].interactable = true;
            }
        }
    }


}
