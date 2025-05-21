using System.Collections;

using UnityEngine;

public class MonsterGenerator : MonoBehaviour
{
    [SerializeField]
    
    protected MonGenCalculator monGenCalculator;

    [SerializeField]
    
    protected GameObject monsterPrefab;

    protected float timetoNextGen;

    protected float timetoNextBigWave;

    protected float currentBigWaveDuration;

    protected bool isBigWave;

    protected float bigWaveLapseTime = 0f;

    protected Transform playerTransform;

    protected bool isPaused = false;

    protected bool isExtreme = false;

    protected Coroutine specialPatternCoroutine;

    protected void OnEnable()
    {
        playerTransform = GameObject.Find("Player").transform;

        monGenCalculator.InitTime();

        isPaused = true;

        timetoNextGen = 0f;

        timetoNextBigWave = monGenCalculator.CalculateBigWaveTime();

        currentBigWaveDuration = monGenCalculator.CalculateBigWaveDuration();

        specialPatternCoroutine = StartCoroutine(StartPatternCoroutine());
    }

    protected void Update()
    {
        if (isPaused)
        {
            return;
        }

        float timediff = Time.deltaTime;

        monGenCalculator.AddLapseTime(timediff);

        float ratio = Mathf.Clamp01(bigWaveLapseTime / currentBigWaveDuration);

        timetoNextGen -= timediff / monGenCalculator.CalculateBigWaveWeight(ratio);

        timetoNextBigWave -= timediff;

        if (timetoNextGen < 0f)
        {
            generateEnemy();

            timetoNextGen = monGenCalculator.CalculateBasicGenTime();
        }

        if (timetoNextBigWave < 0f && !isBigWave)
        {
            StartCoroutine(callBigWaveCoroutine());
        }
    }

    public void PauseGenerate(bool isstopEx)
    {
        isPaused = true;

        if (isstopEx)
        {
            if (isExtreme || specialPatternCoroutine != null)
            {
                StopCoroutine(specialPatternCoroutine);
            }
        }
    }

    public void ResumeGenerator()
    {
        isPaused = false;
    }

    public void TriggerExPattern()
    {
        PauseGenerate(false);

        specialPatternCoroutine = StartCoroutine(ExPatternCoroutine());
    }

    protected virtual MonsterController generateEnemy()
    {
        var pos = findGenPosition();

        var quat = setGenRotation(pos);

        MonsterController controller = Instantiate(monsterPrefab, pos, quat).GetComponent<MonsterController>();

        controller.InitEnemy(playerTransform);

        return controller;
    }

    protected virtual Vector3 findGenPosition()
    {
        return Vector3.zero;
    }

    protected virtual Quaternion setGenRotation(Vector3 genPos)
    {
        var rot = Quaternion.identity;

        return rot;
    }

    protected IEnumerator callBigWaveCoroutine()
    {
        isBigWave = true;

        bigWaveLapseTime = 0f;

        while (bigWaveLapseTime < currentBigWaveDuration)
        {
            bigWaveLapseTime += Time.deltaTime;

            yield return null;
        }

        timetoNextBigWave = monGenCalculator.CalculateBigWaveTime();

        currentBigWaveDuration = monGenCalculator.CalculateBigWaveDuration();

        isBigWave = false;

        yield break;
    }

    protected virtual IEnumerator StartPatternCoroutine()
    {
        isPaused = false;
        yield break;
    }

    protected virtual IEnumerator ExPatternCoroutine()
    {
        isPaused = false;

        isExtreme = false;

        yield break;
    }
}