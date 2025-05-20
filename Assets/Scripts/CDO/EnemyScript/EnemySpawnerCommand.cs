using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//invoke
//여기서 프리팹 다 넣어서 정리
public class EnemySpawnerCommand : MonoBehaviour
{
    public EnemyBase burnduriEnemyPrefab;
    public EnemyBase pupuEnemyPrefab;
    private GameController gameController;

    // 적 생성 및 커맨드 할당
    public void SpawnEnemy(string enemyType, Vector3 direction, float speed )
    {
        EnemyBase enemyObject = null;
        ICommand command = null;

        // 적의 종류에 따라 프리팹과 커맨드 설정
        if (enemyType == "Burnduri")
        {
            enemyObject = Instantiate(burnduriEnemyPrefab, transform.position, Quaternion.identity);
            command = new MoveCommand(enemyObject, direction, speed);
            command.Execute();
        }
        else if (enemyType == "pupu")
        {
            enemyObject = Instantiate(pupuEnemyPrefab, transform.position, Quaternion.identity);
            command = new MoveCommand(enemyObject, direction, speed);
            command.Execute();
        }

        // 생성된 적에 커맨드 실행
        //gameController.SetCommand(command);
        //gameController.ExecuteCommand();
    }






}
