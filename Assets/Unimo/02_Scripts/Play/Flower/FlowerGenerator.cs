using System.Collections;

using System.Collections.Generic;

using UnityEngine;

public class FlowerGenerator : MonoBehaviour
{
    [HideInInspector]
    
    public List<FlowerController> AllFlowers;

    [SerializeField]
    
    protected List<GameObject> flowerObjs;

    [SerializeField]
    
    protected List<float> appearRatios;

    protected List<float> appearAccProb;

    protected int gatheredFlowers = 0;

    protected virtual void Awake()
    {
        if (flowerObjs.Count != appearRatios.Count)
        {
            Debug.Log("Wrong set flower generator.");
        }

        float appearAcc = 0f;

        appearAccProb = new List<float>();

        foreach (float ratio in appearRatios)
        {
            appearAcc += ratio;
        }

        float prob = 0f;

        foreach (float ratio in appearRatios)
        {
            prob += ratio / appearAcc;

            appearAccProb.Add(prob);
        }

        appearAccProb[^1] = 1.4f;
    }

    protected virtual void Start()
    {
        AllFlowers = new();

        StartCoroutine(GenerateCoroutine());
    }
    
    public virtual void GatherFlower()
    {
        ++gatheredFlowers;
    }
    
    protected virtual void generateFlower()
    {
        float rand = Random.Range(0f, 1f);

        int idx = 0;

        while (rand > appearAccProb[idx])
        {
            ++idx;
        }

        var flower = Instantiate(flowerObjs[idx], FindPosition(), SetRotation()).GetComponent<FlowerController>();

        flower.InitFlower(this);
    }

    protected virtual Vector3 FindPosition()
    {
        return Vector3.zero;
    }

    protected virtual Quaternion SetRotation()
    {
        return Quaternion.identity;
    }

    protected virtual IEnumerator GenerateCoroutine()
    {
        yield break;
    }
}