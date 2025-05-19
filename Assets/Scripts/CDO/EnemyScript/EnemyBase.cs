using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


//한 번에 여러 개의 명령을 동시에 사용할 수 없음 (이동 중 공격 등)
public abstract class EnemyBase : MonoBehaviour
{
    public float health = 100f;

    public abstract void Move(Vector3 direction);
}

