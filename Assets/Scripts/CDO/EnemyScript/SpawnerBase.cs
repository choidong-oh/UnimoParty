//생성 리스폰 추상클래스
using UnityEngine;

public abstract class SpawnerBase : MonoBehaviour,ISpawnable
{
    public virtual void Spawn()
    {
        //여기는 enemy기본 메서드
    }
}

//생성함수 인터페이스
public interface ISpawnable
{
    public void Spawn();

}







//사용 예시
public class EnemySpawner1 : SpawnerBase
{
    void Start()
    {
        base.Spawn(); //상속자의 메서드
        Spawn();      //내 코드의 메서드
    }
    public override void Spawn()
    {
        //여기는 enemy1의 메서드
    }
}