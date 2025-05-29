using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemybase : EnemyBase
{
    public override void CsvEnemyInfo() { }

    public override void Move(Vector3 direction)
    {
        Debug.Log("move호출");
    }

    public override void Freeze(Vector3 direction)
    {
        Debug.Log("Freeze호출");
    }


}
