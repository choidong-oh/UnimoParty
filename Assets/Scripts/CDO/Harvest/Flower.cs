using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Flower : MonoBehaviourPun
{
    private InGameDataController controller;
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
    HandHarvest handHarvest;
    public float HarvestTime => harvestTime;


    [Header("FairyDataType Value값")]
    [SerializeField] int FairyDataType1;
    [SerializeField] int FairyDataType2;
    [SerializeField] int FairyDataType3;

    FairyType fairyType;

    private LaycockSP LaycockSpawner;





    private void Start()
    {
        controller = GameObject.Find("InGameDataController").GetComponent<InGameDataController>();
        //체크포인트
        checkPoints.Add(harvestTime / 3f);
        checkPoints.Add(harvestTime / 3f * 2f);

        ChangeValueFairyDataType(FairyDataType1, FairyDataType2, FairyDataType3);
    }


    void ChangeValueFairyDataType(int value1, int value2, int value3)
    {
        fairyType.FairyDataType_1 = value1;
        fairyType.FairyDataType_2 = value2;
        fairyType.FairyDataType_3 = value3;
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
            float progressChange = decreaseSpeed * 0.01f;
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
        LaycockSpawner.SpawnLaycock(gameObject.transform);
        harvestingRoutine = null;
        currentProgress = 0f;

        if (handHarvest != null)
        {
            handHarvest.SpiritPoint++;
            Debug.Log("spiritPoint 증가!");
        }
        else
        {
            Debug.LogWarning("handHarvest가 설정되지 않았습니다.");
        }

        if (controller.IsTestMode == false)
        {
            //매니저랑 상호작용
            Manager.Instance.observer.GetFairy(fairyType);
        }

        this.gameObject.SetActive(false);

        photonView.RPC("FlowerSetAcive", RpcTarget.Others,false);

        Debug.Log("채집 완료!");
    }

    public void Init(HandHarvest handHarvest)
    {
        this.handHarvest = handHarvest;
    }

    [PunRPC]
    void FlowerSetAcive(bool isTrue)
    {
        this.gameObject.SetActive(isTrue);
    }

    private void ResetFlower()
    {
        currentProgress = 0;
    }

}








