using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bungpeo : MonoBehaviour
{
    public float explosionForce = 500f;      // 폭발 힘
    public float explosionRadius = 5f;       // 폭발 반경
    public float upwardsModifier = 1f;       // 위로 튀어오르게 하는 힘
    public LayerMask explosionMask;          // 폭발 대상만 골라서 적용 가능

    public void Explode()
    {
        // 폭발 중심 위치
        Vector3 explosionPosition = transform.position;

        // 해당 반경 안의 콜라이더 검색
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius, explosionMask);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }
        }
    }

}
