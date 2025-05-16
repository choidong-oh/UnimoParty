using UnityEngine;
using UnityEngine.UI;

public class CanvasToggleButton : MonoBehaviour
{
    /// <summary>
    /// 여기는 그냥 캔버스 비활성화 하는 클래스 
    /// </summary>
    [Header("끄고 싶은 캔버스 오브젝트")]
    public GameObject targetCanvas;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ToggleCanvasOff);
    }

    void ToggleCanvasOff()
    {
        if (targetCanvas != null)
            targetCanvas.SetActive(false);
    }


}
