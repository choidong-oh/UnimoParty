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
    [SerializeField] int SpiritVisagePoint = 0;


    [SerializeField] private InputActionReference activateAction;
    Flower flower;

    //콜백은 OnEnable 안댐
    //player은 안사라지니깐 awake, start에 넣으면 댈듯 
    //콜백 쓸만한건없긴함
    void OnEnable()
    {
        activateAction.action.performed += OnTriggerPressed;
        activateAction.action.canceled += OnTriggerReleased;
    }

    void OnDisable()
    {
        activateAction.action.performed -= OnTriggerPressed;
        activateAction.action.canceled -= OnTriggerReleased;
    }

    //안전코드 써야댐 flower가없을수있음
    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        flower = null;

        if (RayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {

            if (!hit.collider.TryGetComponent<Flower>(out flower))
            {
                return;
            }

            //더 안전한 코드
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
        if (this.gameObject.activeSelf == true && flower !=null)
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

    //채집 결과
    private void CompleteHarvest()
    {
        harvestingRoutine = null;
        currentProgress = 0f;

        //나중에 rpc바꿔야댐
        flowerUi.gameObject.SetActive(false);

        SpiritVisagePoint++;

        Debug.Log("채집 했음");
    }

}
