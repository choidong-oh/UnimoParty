using Photon.Realtime;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class OptionManager : MonoBehaviour
{
    [SerializeField] Slider[] sliders;
    [SerializeField] TextMeshProUGUI[] sliderTexts;
    [SerializeField] TunnelingVignetteController vignette;

    [SerializeField] Toggle[] operationToggles;
    [SerializeField] Toggle[] angleToggles;

    [SerializeField] ActionBasedControllerManager ABCM;
    [SerializeField] ActionBasedSnapTurnProvider snapTurn;
    [SerializeField] ActionBasedContinuousTurnProvider smoothTurn;
    float vigentteSize;

    private void Start()
    {
        ABCM = GameObject.Find("Camera Offset").transform.GetChild(5).GetComponent<ActionBasedControllerManager>();


        OptionLoad();
        UpdateTurnAmount();
        OperationChange();
    }
    
    public void ValueChange()
    {
        for(int i=0;i<sliders.Length;i++)
        {
            sliderTexts[i].text = sliders[i].value.ToString();
        }
        UpdateVignetteSize();
        UpdateNomalTurn();

        
    }
    public void OperationChange()
    {
        // smooth 회전
        if (operationToggles[0].isOn)
        {
            operationToggles[0].interactable = false;
            operationToggles[1].interactable = true;

            ABCM.smoothTurnEnabled = true;
            sliders[3].interactable = true;



            for (int i = 0; i < angleToggles.Length;i++)
            {
                angleToggles[i].interactable = false;
            }

        }
        //스냅 회전
        else if(operationToggles[1].isOn)
        {
            operationToggles[0].interactable = true;
            operationToggles[1].interactable = false;

            ABCM.smoothTurnEnabled = false;
            sliders[3].interactable = false;


            for (int i = 0; i < angleToggles.Length; i++)
            {
                angleToggles[i].interactable = true;
            }

        }
    }

    public void OptionSave()
    {
        Debug.Log("Save");
        for (int i = 0; i < sliders.Length;i++)
        {
            PlayerPrefs.SetFloat("Slider" + i , sliders[i].value);
        }

        if (operationToggles[0].isOn)
        {
            PlayerPrefs.SetInt("SmoothTurn", 1);
        }
        else if (operationToggles[1].isOn)
        {
            PlayerPrefs.SetInt("SmoothTurn", 0);
        }
        
       
        PlayerPrefs.Save();
    }

    public void OptionLoad()
    {
        Debug.Log("Load");

        for (int i = 0;i < sliders.Length;i++)
        {
            sliders[i].value = PlayerPrefs.GetFloat("Slider" + i, 50);
        }

        PlayerPrefs.GetInt("SmoothTurn", 1);
        //조작 토글
        if (PlayerPrefs.GetInt("SmoothTurn") == 1)
        {
            operationToggles[0].isOn = true;
            operationToggles[1].isOn = false;
        }
        else if (PlayerPrefs.GetInt("SmoothTurn") == 0)
        {
            operationToggles[0].isOn = false;
            operationToggles[1].isOn = true;
        }
        //스냅턴 조작 기억
        if(operationToggles[1].isOn)
        {
            if(PlayerPrefs.GetInt("TurnAmount")==0)
            {
                angleToggles[0].isOn = true;
            }
            else if(PlayerPrefs.GetInt("TurnAmount")==1)
            {
                angleToggles[1].isOn = true;
            }
            else if(PlayerPrefs.GetInt("TurnAmount")==2)
            {
                angleToggles[2].isOn = true;
            }
        }
       

    }
    public void UpdateVignetteSize()
    {
        //0일때 0.9f
        //100일때 0.7f

        float normalized = sliders[0].value / 100f;
        float qhrks = Mathf.Lerp(0.9f, 0.7f, normalized);
        vignette.defaultParameters.apertureSize = qhrks;

        //여기서 optionData저장 vignette.defaultParameters.apertureSize
    }
    public void UpdateBGMSound()
    {
        //여기서 optionData저장 
        //배경음 업데이트 및 저장
    }
    public void UpdateSFXSound()
    {
        //여기서 optionData저장 
        //효과음 소리 업데이트 및 저장
    }
    public void UpdateNomalTurn()
    {
        //0일때 50
        //1당 2
        //100일 때 250
        smoothTurn.turnSpeed = sliders[3].value * 2f + 50f;
        //여기서 optionData저장 smoothTurn.turnSpeed
    }
    public void UpdateTurnAmount()
    {
        if (angleToggles[0].isOn)
        {
            snapTurn.turnAmount = 30;
            PlayerPrefs.SetInt("TurnAmount", 0);
        }
        else if(angleToggles[1].isOn)
        {
            snapTurn.turnAmount = 60;
            PlayerPrefs.SetInt("TurnAmount", 1);
        }
        else if(angleToggles[2].isOn)
        {
            snapTurn.turnAmount = 90;
            PlayerPrefs.SetInt("TurnAmount", 2);
        }
    }

}
