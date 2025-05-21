using UnityEngine;

public class MonsterShadowCtrl : MonoBehaviour
{
    [SerializeField]
    
    private GameObject shadowObj;

    private void Update()
    {
        if (PlaySystemRefStorage.mapSetter.IsInMap(transform.position) != shadowObj.activeSelf)
        {
            shadowObj.SetActive(PlaySystemRefStorage.mapSetter.IsInMap(transform.position));
        }
    }
}