using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public class Laycock : EnemyBase
{
    public List<Transform> players = new List<Transform>();//플레이어 여기에 등록함

    Vector3 myPos;
    Vector3 Target;

    Transform nearestPlayer;

    [SerializeField] ParticleSystem ChargeParticles;
    [SerializeField] ParticleSystem ShootParticles;
    [SerializeField] GameObject DieParticles;

    Terrain terrain;

    float LazerLoopTime = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            damage = 1;
            Manager.Instance.observer.HitPlayer(damage);
            Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);

            Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게

            Vector3 normal = (hitPoint - transform.position).normalized;// 방향계산
            Quaternion rot = Quaternion.LookRotation(normal);// 방향계산

            GameObject inst = Instantiate(DieParticles, hitPoint, rot);

            gameObject.SetActive(false);
        }

    }


    public override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Distance());
        //ShootLazer();

    }


    public override void OnDisable()
    {
        base.OnDisable();


    }

    IEnumerator Distance()
    {
        terrain = Terrain.activeTerrain;
        float terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f;
        transform.position = new Vector3(transform.position.x, terrainY, transform.position.z);
        if (players.Count == 0)
        {
            var objs = GameObject.FindGameObjectsWithTag("Player");
            foreach (var obj in objs)
            {
                players.Add(obj.transform);
            }
        }
        myPos = transform.position;
        float minDistance = Mathf.Infinity;// 일단 제일 큰값으로함 

        foreach (var player in players)
        {
            float Distance = Vector3.Distance(myPos, player.position);


            if (Distance < minDistance)
            {
                minDistance = Distance;
                nearestPlayer = player;
            }
        }

        if (nearestPlayer != null)
        {
            Target = nearestPlayer.position;

            transform.LookAt(Target);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        yield return null;
    }
    [PunRPC]
    public void ShootLazer()
    {
        StartCoroutine(Lazer());
    }

   
    IEnumerator Lazer()
    {
        ChargeParticles.gameObject.SetActive(true);

        yield return new WaitUntil(() => !ChargeParticles.IsAlive(true));

        StartCoroutine(LoopLazer());
    }

    IEnumerator LoopLazer()
    {
        ShootParticles.gameObject.SetActive(true);
        yield return new WaitForSeconds(LazerLoopTime);
        ShootParticles.gameObject.SetActive(false);
    }


    public override void CsvEnemyInfo()
    {
        throw new System.NotImplementedException();
    }

    public override void Freeze(Vector3 direction, bool isFreeze)
    {
        throw new System.NotImplementedException();
    }

    public override void Move(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }
}
