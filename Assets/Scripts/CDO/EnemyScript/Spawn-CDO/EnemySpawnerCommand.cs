using Photon.Pun;
using UnityEngine;

//invoker
//여기서 프리팹 다 넣어서 정리
public class EnemySpawnerCommand : MonoBehaviour
{
    public EnemyBase burnduriEnemyPrefab;
    public EnemyBase pewpewEnemyPrefab;
    public EnemyBase shookshookEnemyPrefab;
    private GameController gameController;

    // 적 생성 및 커맨드 할당
    public EnemyBase SpawnEnemy(string enemyType, Vector3 direction, float speed)
    {
        EnemyBase enemyObject = null;
        ICommand command = null;

        // 적의 종류에 따라 프리팹과 커맨드 설정
        if (enemyType == "Burnduri")
        {
            enemyObject = PhotonNetwork.Instantiate("BurnduriTest", direction, Quaternion.identity).GetComponent<EnemyBase>();
            command = new MoveCommand(enemyObject, direction, speed);
            command.Execute();
        }
        else if (enemyType == "Pewpew")
        {
            enemyObject = PhotonNetwork.Instantiate("PewPew", direction, Quaternion.identity).GetComponent<EnemyBase>();
            command = new MoveCommand(enemyObject, direction, speed);
            command.Execute();
        }
        else if(enemyType == "Shookshook")
        {
            enemyObject = PhotonNetwork.Instantiate("ShookShookTest", direction, Quaternion.identity).GetComponent<EnemyBase>();
            command = new MoveCommand(enemyObject, direction, speed);
            command.Execute();
        }




        if (enemyObject == null)
        {
            Debug.Log("SpawnEnemy null뜸");
            return null;
        }

        return enemyObject;
        // 생성된 적에 커맨드 실행
        //gameController.SetCommand(command);
        //gameController.ExecuteCommand();
    }






}
