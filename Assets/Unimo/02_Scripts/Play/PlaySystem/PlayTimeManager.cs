using UnityEngine;

public class PlayTimeManager : MonoBehaviour
{
    public float LapseTime { get; private set; } = 0f;

    [SerializeField]
    
    private bool isInfinite = false;

    [SerializeField]
    
    private float maxTime = 60f;

    [SerializeField]
    
    private float reduceIncTime = 120f;

    private float remainTime = 60f;

    private float minReduce = 1f;

    private bool isPaused = true;

    private ItemGenerator itemGenerator;

    [SerializeField]
    
    private TimeGaugeController timerGauge;

    private void Awake()
    {
        PlaySystemRefStorage.playTimeManager = this;

        itemGenerator = FindAnyObjectByType<ItemGenerator>();

        remainTime = maxTime;

        timerGauge.SetGauge(remainTime / maxTime);
    }

    private void Update()
    {
        if (isPaused == true)
        {
            return;
        }

        LapseTime += Time.deltaTime;

        float rate = CalcReduceRate(LapseTime);

        itemGenerator.DecreaseTick(Time.deltaTime * rate);

        ChangeTimer(-Time.deltaTime * rate);
    }
    
    public void InitTimer()
    {
        remainTime = maxTime;

        isPaused = true;

        StartCoroutine(CoroutineExtensions.DelayedActionCall(() => { ToggleTimer(); }, PlayProcessController.InitTimeSTATIC));
    }

    public void ToggleTimer()
    {
        isPaused = !isPaused;
    }

    public void ChangeTimer(float tchange)
    {
        if (isInfinite)
        {
            return;
        }

        remainTime += tchange;

        if (remainTime > maxTime) 
        {
            remainTime = maxTime; 
        }

        if (remainTime < 0f) 
        { 
            remainTime = 0f;

            TimeUp();
        }

        timerGauge.SetGauge(remainTime / maxTime);
    }

    public float GetMaxTime()
    {
        return maxTime;
    }

    public float GetRemainTimeRatio()
    {
        return remainTime / maxTime;
    }

    private float CalcReduceRate(float lapse)
    {
        float ratio = lapse / reduceIncTime;

        float rate = minReduce * ((0.7f * ratio * ratio + 0.3f * Mathf.Pow(ratio,3.2f)) + (1f + Mathf.Exp(0.78f*ratio)) / 2f);

        return rate;
    }

    private void TimeUp()
    {
        isPaused = true;

        PlaySystemRefStorage.playProcessController.TimeUp();
    }
}