using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    static Manager instance;
    public GoalFairyCount goalCount;
    private bool isMaster = false;

    // 인스펙터로 테스트를 한 후 해당 값을 Sprit의 private 변수로 변경하여 노출을 막아야함.
    public int tempFairyValue_1 = 10;
    public int tempFairyValue_2 = 10;
    public int tempFairyValue_3 = 10;

    public static Manager Instance
    {
        get { return instance; }
    }

    public IngameObserver observer = new IngameObserver();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        observer.Setting();
    }
}
