using UnityEngine;

public class HorizonView : MonoBehaviour
{
    public Transform target; // 바라볼 중심점
    public float height = 5f;

    void Start()
    {
        transform.position = new Vector3(0, height, -10);
        transform.LookAt(target);
    }
}