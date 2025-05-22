using UnityEngine;

public class CustomAudioListener : MonoBehaviour
{
    private void Awake()
    {
        Custom3DAudio.SetListenerPos(transform);
    }
}