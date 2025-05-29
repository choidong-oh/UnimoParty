
public interface IDamageable
{
    void TakeDamage(int dmg);
}

public interface ICommand
{
    float DelayTime { get; }
    void Execute();
}

public interface IFreeze 
{
    void Freeze(bool IsFreeze);
}
