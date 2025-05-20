using UnityEngine;

public class MapRangeSetter : MonoBehaviour
{
    public float MaxRange { get; protected set; } = 10f;

    protected void Awake()
    {
        PlaySystemRefStorage.mapSetter = this;
    }

    protected virtual void Start()
    {
        SetMaxRanges();
    }

    protected virtual void SetMaxRanges()
    {

    }

    public virtual bool IsInMap(Vector3 point)
    {
        return true;
    }
    
    public virtual Vector3 FindNearestPoint(Vector3 point)
    {
        return Vector3.zero;
    }
}