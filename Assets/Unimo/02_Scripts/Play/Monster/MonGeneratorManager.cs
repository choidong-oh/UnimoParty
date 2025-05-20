using System.Collections.Generic;

using UnityEngine;

public class MonGeneratorManager : MonoBehaviour
{
    public static List<MonsterController> AllMonsterListSTATIC;

    [SerializeField]
    
    private List<MonsterGenerator> generators;

    [SerializeField]
    
    private List<float> activateTime;

    [SerializeField]
    
    private float extreamTime;

    [SerializeField]
    
    private float extreamCoolTime;

    private float lapseTime = 0f;

    private float currentExCoolTime;

    private int newGenIdx = 0;

    private void Awake()
    {
        AllMonsterListSTATIC = new List<MonsterController>();

        foreach(var gen in generators)
        {
            gen.gameObject.SetActive(false);
        }

        currentExCoolTime = 0f;
    }

    private void Update()
    {
        lapseTime += Time.deltaTime;

        if (newGenIdx < generators.Count)
        {
            CheckNewGenerator();
        }

        if (lapseTime > extreamTime)
        {
            currentExCoolTime -= Time.deltaTime;

            if (currentExCoolTime < 0f)
            {
                TriggerRndExPattern();

                SetNextExCoolTime();
            }
        }
    }

    public void StopGenerateAllMonster()
    {
        foreach(var gen in generators)
        {
            gen.PauseGenerate(true);
        }
    }
    public void PauseGenerateAllMonster()
    {
        foreach (var gen in generators)
        {
            gen.PauseGenerate(false);
        }
    }

    public void ResumeGenerateAllMonster()
    {
        foreach (var gen in generators)
        {
            gen.ResumeGenerator();
        }
    }

    private void CheckNewGenerator()
    {
        if (newGenIdx >= generators.Count)
        {
            return;
        }

        if (lapseTime > activateTime[newGenIdx])
        {
            generators[newGenIdx].gameObject.SetActive(true);

            ++newGenIdx;
        }
    }

    private void SetNextExCoolTime()
    {
        currentExCoolTime = Random.Range(0.8f, 1.1f) * extreamCoolTime;
    }

    private void TriggerRndExPattern()
    {
        int idx = Random.Range(0, generators.Count);

        generators[idx].TriggerExPattern();
    }
}