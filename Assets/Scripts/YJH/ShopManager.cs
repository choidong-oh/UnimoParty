using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] GameObject[] scrollVeiw;

    [SerializeField] Toggle[] viewToggles;

    private void Start()
    {
        for (int i = 0; i < viewToggles.Length; i++)
        {
            if (viewToggles[i].isOn)
            {
                ViewActive(i);
                break;
            }
        }
    }

    public void ViewActive(int activeIndex)
    {
        for (int i = 0; i < scrollVeiw.Length; i++)
        {
            scrollVeiw[i].SetActive(i == activeIndex);
        }
    }

}
