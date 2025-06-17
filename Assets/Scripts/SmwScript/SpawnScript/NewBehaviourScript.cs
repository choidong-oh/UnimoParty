using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class NewBehaviourScript : MonoBehaviour
{
    private LaycockSP LaycockSpawner;

    public void Start()
    {
        if (LaycockSpawner == null)
        {
            LaycockSpawner = GameObject.Find("LaycockSp").GetComponent<LaycockSP>();
        }
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        while (true) 
        {
            LaycockSpawner.SpawnLaycock(gameObject.transform);
            yield return new WaitForSeconds(5);
        }
    }
}
