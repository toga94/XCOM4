using System;
using UnityEngine;

public interface IDamageable
{
    event Action<bool, GameObject> OnDie;

    void TakeDamage(float value, bool isCritical);
    void Heal(float value);
}
