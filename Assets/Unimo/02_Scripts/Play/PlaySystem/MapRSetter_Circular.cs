using UnityEngine;

public class MapRSetter_Circular : MapRangeSetter
{
    [SerializeField]
    
    private float mapRadius = 17.5f;

    protected override void Start()
    {
        base.Start();

        //Should set cam range in Map setter to control execution order
        var cammover = FindAnyObjectByType<CamMover_ST001>();

        if (cammover != null)
        {
            cammover.SetMaximumRange(MaxRange);
        }
    }

    protected override void SetMaxRanges()
    {
        MaxRange = mapRadius;
    }

    public override bool IsInMap(Vector3 point)
    {
        Vector2 pos = new(point.x, point.z);

        return pos.magnitude <= MaxRange;
    }

    public override Vector3 FindNearestPoint(Vector3 point)
    {
        Vector2 pos = new(point.x, point.z);

        float angle = pos.AngleInXZ();

        Vector3 newpoint = new(mapRadius * Mathf.Cos(angle), 0f, mapRadius * Mathf.Sin(angle));

        return newpoint;
    }
}