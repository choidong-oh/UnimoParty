using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//flower은 풀링구현
//나는 상호작용할때 flower1 담고 게이지를 추가함
//다른플레이어가 flower1을 담고 비활성화댐
//나는 어쩌다가 flower1을 상호작용하면 의도처럼 안됨
public class PlayerHarvestData : MonoBehaviour
{
    // Flower 채집 진행도
    private Dictionary<Flower, float> flowerProgress = new Dictionary<Flower, float>();

    public float GetProgress(Flower flower)
    {
        if (flowerProgress.TryGetValue(flower, out float value))
            return value;
        return 0f;
    }

    public void SetProgress(Flower flower, float progress)
    {
        flowerProgress[flower] = progress;
    }
}
