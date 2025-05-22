using System.Collections;

using UnityEngine;

public class FlowerHarvestFX : MonoBehaviour
{
    private static Transform targetTransform;

    private void Start()
    {
        targetTransform = GameObject.Find("Player").transform;

        //if (targetTransform == null) { targetTransform = PlaySystemRefStorage.playerStatManager.transform; }

        StartCoroutine(FollowCoroutine());
    }

    private IEnumerator FollowCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            transform.position = targetTransform.position + 0.5f * Vector3.up;

            yield return null;
        }
    }
}