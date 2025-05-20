using UnityEngine;

using UnityEngine.UI;

public class TimeGaugeController : MonoBehaviour
{
    private readonly Color32 fullGaugeColor = new(154, 228, 80, 255);

    private readonly Color32 midGaugeColor = new(255, 216, 78, 255);

    private readonly Color32 lowGaugeColor = new(255, 78, 78, 255);

    private readonly float midStandard = 0.75f;

    private readonly float lowStandard = 0.25f;

    private readonly float exLowStandard = 0.2f;

    [SerializeField]
    
    private Image gaugeImg;

    [SerializeField]
    
    private RectTransform timerRect;

    [SerializeField]
    
    private Image clockImg;

    [SerializeField]
    
    private Animator clockAnim;

    private AudioSource cautionAudio;

    private bool canCaution = true;

    private float maxWidth = 450;

    private float height = 48;

    private void Start()
    {
        cautionAudio = GetComponent<AudioSource>();

        maxWidth = timerRect.transform.parent.GetComponent<RectTransform>().rect.width;

        height = timerRect.rect.height;

        SetGauge(1f);

        PlaySystemRefStorage.playProcessController.SubscribePauseAction(StopCaution);

        PlaySystemRefStorage.playProcessController.SubscribeGameoverAction(StopCaution);

        PlaySystemRefStorage.playProcessController.SubscribeResumeAction(StartCaution);
    }

    public void SetGauge(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);

        CheckClockShake(ratio);

        SetImageColors(ratio);

        timerRect.sizeDelta = new Vector2(ratio * maxWidth, height);
    }

    private void CheckClockShake(float ratio)
    {
        if (!canCaution) 
        {
            clockAnim.SetBool("isshake", false);

            cautionAudio.enabled = false;

            return;
        }

        bool isshaking = clockAnim.GetBool("isshake");

        bool willshake = ratio <= exLowStandard;

        if (isshaking != willshake) 
        { 
            clockAnim.SetBool("isshake", willshake);

            cautionAudio.enabled = willshake;
        }
    }

    private void StopCaution()
    {
        canCaution = false;

        CheckClockShake(1f);
    }

    private void StartCaution()
    {
        canCaution = true;
    }

    private void SetImageColors(float ratio)
    {
        Color32 newcolor;

        if (ratio > midStandard)
        {
            float actratio = (ratio - midStandard) / (1f - midStandard);

            newcolor = midGaugeColor.HSVInterp(fullGaugeColor, actratio);
        }

        else if (ratio > lowStandard)
        {
            float actratio = (ratio - lowStandard) / (midStandard - lowStandard);

            newcolor = lowGaugeColor.HSVInterp(midGaugeColor, actratio);
        }

        else
        {
            newcolor = lowGaugeColor;
        }

        gaugeImg.color = newcolor;

        clockImg.color = newcolor;
    }
}