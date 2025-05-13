using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class HandHarvest : MonoBehaviour
{
    [SerializeField] XRRayInteractor RayInteractor;
    FlowerUi flowerUi; //채집물

    //코루틴변수
    Coroutine harvestingRoutine = null; //시작코루틴
    Coroutine decreaseRoutine = null; //감소코루틴

    //쿨타임 게이지
    [Header("게이지 관련")]
    public  float currentProgress = 0f; // 현재게이지
    [SerializeField] float harvestTime = 3f; // 게이지 쿨타임
    [SerializeField] float decreaseSpeed = 0.5f; // 줄어드는 속도
    List<float> checkPoints = new List<float>(); // 체크포인트 목록

    [Header("테스트 채집성공 포인트")]
    [SerializeField] int harvestPoint = 0;


    [SerializeField] private InputActionReference activateAction;
    Flower flower;

    //콜백은 OnEnable 안댐
    //player은 안사라지니깐 awake, start에 넣으면 댈듯 
    //콜백 쓸만한건없긴함
    void OnEnable()
    {
        //RayInteractor.selectEntered.AddListener(OnGrabbed);
        //RayInteractor.selectExited.AddListener(OnReleased);
        
        activateAction.action.performed += OnTriggerPressed;
        activateAction.action.canceled += OnTriggerReleased;
    }

    void OnDisable()
    {
        //RayInteractor.selectEntered.RemoveListener(OnGrabbed);
        //RayInteractor.selectExited.RemoveListener(OnReleased);

         activateAction.action.performed -= OnTriggerPressed;
        activateAction.action.canceled -= OnTriggerReleased;
    }

    //안전코드 써야댐 flower가없을수있음
    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        flower = null;
        if (RayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            flower = hit.collider.GetComponent<Flower>();
            if (flower != null)
            {
                flower.StartHarvest();
                Debug.Log("Flower 수확 시작!");
            }
        }
    }

  
   

    private void OnTriggerReleased(InputAction.CallbackContext context)
    {
        Debug.Log("Trigger 뗌");
        if (this.gameObject.activeSelf == true)
        {
            flower.StopHarvest();
        }
    }


    private void Start()
    {
        //체크포인트
        checkPoints.Add(harvestTime / 3f);
        checkPoints.Add(harvestTime / 3f * 2f);
    }


    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("플레이어 손으로 잡음");
        flowerUi = args.interactableObject.transform.GetComponent<FlowerUi>();
        StartHarvest();

    }

    private void OnReleased(SelectExitEventArgs args)
    {
        Debug.Log("플레이어 손에서 놓음");
        StopHarvest();

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
    //델타타임 써는데 상대방의게이지가 안보여서 신경안써도 댈듯
    //정확히할려면 바꿔야대고
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

        flowerUi.gameObject.SetActive(false);

        harvestPoint++;

        Debug.Log("채집 했음");
    }

}
