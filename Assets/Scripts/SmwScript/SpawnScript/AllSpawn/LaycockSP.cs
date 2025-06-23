using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class LaycockSP : MonoBehaviour
{
    [SerializeField] private string monsterPrefab = "Laycock";
    [SerializeField] private int maxCount = 10;
    private int Count = 0;
    List<GameObject> Monsters = new List<GameObject>();

    public void SpawnLaycock(Transform position)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Vector3 vector3 = new Vector3(position.position.x, position.position.y, position.position.z);
        GameObject go = PoolManager.Instance.Spawn(monsterPrefab, vector3, Quaternion.identity);
        Monsters.Add(go);

        Debug.Log(Count);

        Count++;
        if (Count >= maxCount)
        {
            for (int i = 0; i < Monsters.Count; i++)
            {
                var m = Monsters[i];
                var lay = m.GetComponent<Laycock>();
                if (lay != null)
                {
                    lay.ShootLazer();
                }
            }

            Count = 0;
            Monsters.Clear();
        }

    }
    
    public void DisCountLaycock()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Count--;
    }


}




