using System;
using UnityEngine;

public interface IDamageable
{
    event Action<bool, GameObject> OnDie;

    void TakeDamage(float value);
    void Heal(float value);
}
