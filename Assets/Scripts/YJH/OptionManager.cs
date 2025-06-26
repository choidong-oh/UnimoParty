using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class OptionManager : MonoBehaviourPunCallbacks
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
        UpdateBGMSound();
        UpdateSFXSound();

    }
    public void OperationChange()
    {
        // smooth 회전
        if (operationToggles[0].isOn)
        {
            OptionData.isSmooth = true;
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
            OptionData.isSmooth = false;
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

        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 50f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 50f);

        sliders[1].value = bgmVolume;
        sliders[2].value = sfxVolume;

        AudioManager.Instance?.SetBGMVolume(bgmVolume / 100f);
        AudioManager.Instance?.SetSFXVolume(sfxVolume / 100f);


    }
    public void UpdateVignetteSize()
    {
        //0일때 0.9f
        //100일때 0.7f

        float normalized = sliders[0].value / 100f;
        float qhrks = Mathf.Lerp(0.9f, 0.7f, normalized);
        vignette.defaultParameters.apertureSize = qhrks;

        OptionData.dataValues[0] = qhrks;
        //여기서 optionData저장 vignette.defaultParameters.apertureSize
    }
    public void UpdateBGMSound()
    {
        //여기서 optionData저장 
        //배경음 업데이트 및 저장
        float value = sliders[1].value;
        OptionData.dataValues[1] = value;

        PlayerPrefs.SetFloat("BGMVolume", value);
        PlayerPrefs.Save();

        AudioManager.Instance?.SetBGMVolume(value / 100f);
    }
    public void UpdateSFXSound()
    {
        //여기서 optionData저장 
        //효과음 소리 업데이트 및 저장
        float value = sliders[2].value;
        OptionData.dataValues[2] = value;

        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();

        AudioManager.Instance?.SetSFXVolume(value / 100f);
    }
    public void UpdateNomalTurn()
    {
        //0일때 50
        //1당 2
        //100일 때 250
        float _turnSpeed = sliders[3].value * 2f + 50f;
        smoothTurn.turnSpeed = _turnSpeed;
        OptionData.dataValues[3] = _turnSpeed;
        //여기서 optionData저장 smoothTurn.turnSpeed
    }
    public void UpdateTurnAmount()
    {
        if (angleToggles[0].isOn)
        {
            snapTurn.turnAmount = 30;
            PlayerPrefs.SetInt("TurnAmount", 0);
            OptionData.amount = 30;
        }
        else if(angleToggles[1].isOn)
        {
            snapTurn.turnAmount = 60;
            PlayerPrefs.SetInt("TurnAmount", 1);
            OptionData.amount = 60;
        }
        else if(angleToggles[2].isOn)
        {
            snapTurn.turnAmount = 90;
            PlayerPrefs.SetInt("TurnAmount", 2);
            OptionData.amount = 90;
        }
    }

    public void ExitButton()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);
    }
}
