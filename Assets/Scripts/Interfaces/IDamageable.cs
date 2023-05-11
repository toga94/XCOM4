using System;

public interface IDamageable
{
    event Action<bool> OnDie;

    void TakeDamage(float value);
    void Heal(float value);
}
