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
}
