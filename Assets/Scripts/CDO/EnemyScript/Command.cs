
public interface IDamageable
{
    void TakeDamage(int dmg);
}

public interface ICommand
{
    float DelayTime { get; }
    void Execute();
}