
public interface IDamageable
{
    void TakeDamage(float dmg) { }
}

public interface ICommand
{
    float DelayTime { get; }
    void Execute();
}