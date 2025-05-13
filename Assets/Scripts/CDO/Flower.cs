using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Flower : MonoBehaviour
{
    [SerializeField] XRGrabInteractable grabInteractable;

    [SerializeField] FlowerUi flowerUi;

    //코루틴변수
    Coroutine harvestingRoutine = null; //시작코루틴
    Coroutine decreaseRoutine = null; //감소코루틴

    //쿨타임 게이지
    [Header("게이지 속도 관련")]
    [SerializeField] float currentProgress = 0f; // 현재게이지
    [SerializeField] float harvestTime = 2f; // 게이지 쿨타임
    [SerializeField] float decreaseSpeed = 0.5f; // 줄어드는 속도
    List<float> checkPoints = new List<float>(); // 체크포인트 목록

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        //체크포인트
        checkPoints.Add(harvestTime / 3f);
        checkPoints.Add(harvestTime / 3f * 2f);
    }



    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("잡혔다!");
        StartHarvest();
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        Debug.Log("놓았다!");
        if (this.gameObject.activeSelf == true)
        {
            StopHarvest();
        }
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
    private void CompleteHarvest()
    {
        harvestingRoutine = null;
        currentProgress = 0f;

        this.gameObject.SetActive(false);

        Debug.Log("채집 완료!");
    }
}

    






