using UnityEngine;

public class BurnduriSpawn : MonoBehaviour
{
    public EnemySpawnerCommand enemySpawnerCommand;
    public GameObject testPrefab;
    Vector3 spawnPosition;

    [SerializeField] Transform player;
    [SerializeField] Transform donutTransform;
    [SerializeField] float distanceGap; 
    float angle; // 각도
    int r = 15; //반지름

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        InvokeRepeating("SpawnEnemy", 1f, 2f);
    }


    void SpawnEnemy()
    {

        //enemySpawnerCommand.SpawnEnemy("Burnduri", isPlayerHere(), 5);
        enemySpawnerCommand.SpawnEnemy("Burnduri", IsDoWhile(), 5);
        enemySpawnerCommand.SpawnEnemy("Burnduri", IsDoWhile(), 5);
        enemySpawnerCommand.SpawnEnemy("Burnduri", IsDoWhile(), 5);
        enemySpawnerCommand.SpawnEnemy("Burnduri", IsDoWhile(), 5);
        enemySpawnerCommand.SpawnEnemy("Burnduri", IsDoWhile(), 5);

    }

    Vector3 DonutPostion()
    {
        angle = UnityEngine.Random.Range(0, 360);
        float x = Mathf.Cos(angle) * r;
        float y = donutTransform.transform.position.y;
        float z = Mathf.Sin(angle) * r;
        donutTransform.position = new Vector3(x, y, z);

        return donutTransform.position;
    }

    Vector3 isPlayerHere()
    {
        for (int i = 0; i < 5; i++)
        {
            //var dd = donutTransform.position; //테스트버전
            var dd = DonutPostion(); //실제

            if (Vector3.Distance(dd, player.position) >= distanceGap)
            {
                Debug.Log("플레이어가 근처 아님ㅋ");
                return dd;
            }
            else
            {
                Debug.Log("플레이어가 근처임");
            }
        }
        //나중에 player근처 아닐때까지 돌리면될듯
        Debug.Log("i번 돌렷는데 플레이어가 근처임");

        var aa = DonutPostion();
        return aa;
    }


    Vector3 IsDoWhile()
    {
        Vector3 dd;
        int roof = 0; // loop
        do 
        { dd = DonutPostion();roof++; } 
        while ((Vector3.Distance(dd, player.position) >= distanceGap)&&roof>=10);

        if (roof >= 10) { Debug.Log("무한루프될뻔"); }

        return dd;
    }







}
