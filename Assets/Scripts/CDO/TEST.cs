using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class TEST : MonoBehaviour
{
    public ActionBasedController RightController; //진짜모델
    private Renderer[] handRenderers;


    private void Start()
    {
        handRenderers = RightController.GetComponentsInChildren<Renderer>();

        StartCoroutine(WAIT());
    }
  
    void SetHandVisible(bool visible)
    {
        foreach (var renderer in handRenderers)
        {
            renderer.enabled = visible;
        }

    }
    //[ContextMenu("얼음")]
    IEnumerator WAIT()
    {
        yield return new WaitForSeconds(2);

        RightController.model.gameObject.SetActive(false);
        SetHandVisible(false);
    }



    

    //public void OnSelectEnter()
    //{
    //    LController.SetActive(true);
    //    SetHandVisible(false);

    //    leftController.model.gameObject.SetActive(false);
    //}
    //public void OnSelectExit()
    //{
    //    LController.SetActive(false);
    //    SetHandVisible(true);
    //    leftController.model.gameObject.SetActive(true);
    //}


    
}


