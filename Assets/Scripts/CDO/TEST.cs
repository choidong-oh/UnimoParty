using System.Collections;
using UnityEngine;

public class TEST : EnemyBase
{

    private void Start()
    {
        Move(transform.position);
    }
    public override void Move(Vector3 direction)
    {
        StartCoroutine(moveCor());
    }
    [ContextMenu("¾óÀ½")]
    public void djfdma()
    {
        StopAllCoroutines();
        var CurrentTransform = transform.position;

        StartCoroutine(djfdmaCor());
    }

    IEnumerator djfdmaCor()
    {

        yield return new WaitForSeconds(2);
        StartCoroutine(moveCor());
    }


    IEnumerator moveCor()
    {
        while (true)
        {
            var dsds = new Vector3(1, 0, 1) * 10 * Time.deltaTime;
            transform.Translate(dsds);
            yield return null;
        }

    }

    public override void CsvEnemyInfo()
    {
        throw new System.NotImplementedException();
    }
}


