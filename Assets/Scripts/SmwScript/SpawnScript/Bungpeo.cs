using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bungpeo : MonoBehaviour
{
    float BombTimer;

    [SerializeField] Animator animator;
    [SerializeField] string stateName;
    [SerializeField] AnimationClip clip;      //해당 스테이트에 연결된 클립


    // stateName 상태 애니메이션을 duration초에 딱 맞춰 재생
    public void PlayTimed(float duration)
    {
        // 1) 클립 길이 구하고
        float clipLength = clip.length;
        // 2) 재생 속도 계산 (속도가 높을수록 빨라짐)
        float speed = clipLength / duration;
        // 3) Animator 전체 속도에 적용
        animator.speed = speed;
        // 4) 스테이트 재생 (0번 레이어, 0프레임에서)
        animator.Play(stateName, 0, 0f);
    }



    IEnumerator Bomb()
    {


        



        yield return null;
    }

}
