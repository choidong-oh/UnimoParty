using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Flower : MonoBehaviour
{

    [SerializeField] FlowerUi flowerUi;

    //코루틴변수
    Coroutine harvestingRoutine = null; //시작코루틴
    Coroutine decreaseRoutine = null; //감소코루틴

    //쿨타임 게이지
    [Header("게이지 속도 관련")]
    [SerializeField] float currentProgress = 0f; // 현재게이지
    [SerializeField] float harvestTime = 3f; // 게이지 쿨타임
    [SerializeField] float decreaseSpeed = 0.5f; // 줄어드는 속도
    List<float> checkPoints = new List<float>(); // 체크포인트 목록

    //인풋액션
    [Header("InputSystem")]
    [SerializeField] private InputActionReference activateAction;

    public int SpiritVisagePoint;

    public float HarvestTime => harvestTime;//이거는 FlowerUi에 필요한 프로퍼티 (게이지쿨타임 받아와야해서 ㅠㅠ)

    private void Start()
    {
        //체크포인트
        checkPoints.Add(harvestTime / 3f);
        checkPoints.Add(harvestTime / 3f * 2f);
    }

    
    public void StartHarvest()
    {
        if (decreaseRoutine != null)
        {
            StopCoroutine(decreaseRoutine);
            decreaseRoutine = null;
        }

        if (harvestingRoutine == null)
        {
            harvestingRoutine = StartCoroutine(HarvestCoroutine());
        }
    }

    public void StopHarvest()
    {
        if (harvestingRoutine != null)
        {
            StopCoroutine(harvestingRoutine);
            harvestingRoutine = null;
        }

        if (decreaseRoutine == null)
        {
            decreaseRoutine = StartCoroutine(DecreaseCoroutine());
        }
    }

    //수확 시작
    private IEnumerator HarvestCoroutine()
    {
        while (currentProgress < harvestTime)
        {
            currentProgress += Time.deltaTime;
            flowerUi.UpdateGauge(currentProgress);
            yield return null;
        }

        CompleteHarvest();
    }
 

    //수확 감소(중간 놓았을때)
    private IEnumerator DecreaseCoroutine()
    {
        while (currentProgress > 0f)
        {
            // 진행도 감소
            float progressChange = Time.deltaTime * decreaseSpeed;
            currentProgress = Mathf.Max(0f, currentProgress - progressChange);

            foreach (float checkPoint in checkPoints)
            {
                if (Mathf.Abs(currentProgress - checkPoint) < 0.01f)
                {
                    currentProgress = checkPoint;
                    break;
                }
            }

            // UI 갱신
            flowerUi.UpdateGauge(currentProgress);

            yield return null;
        }
        decreaseRoutine = null;

    }


    //채집 결과
    void CompleteHarvest()
    {
        harvestingRoutine = null;
        currentProgress = 0f;
        
        //player
        SpiritVisagePoint++;

        this.gameObject.SetActive(false);

        Debug.Log("채집 완료!");
    }

    private void ResetFlower()
    {
        currentProgress = 0;
    }

}

    






