using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fairy : MonoBehaviour
{
    public FairyType type;

    private bool IsObjCompleteCollect = false;

    public List<UserPlayer> touchplayer = new List<UserPlayer>();
    [SerializeField] GameObject fairyObjectPrefab;
    [SerializeField] float playerEnterRange;
    [SerializeField] float objRemoveTime;


    void objRemove()
    {
        //폴링을 통해 채집물 관리
        transform.gameObject.SetActive(false);
        Manager.Instance.observer.RetrunFairy();
        Manager.Instance.observer.AddScore(100);
    }
}
