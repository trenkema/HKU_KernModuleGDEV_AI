using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float _damage, GameObject _attacker);

    void TakeFullDamage();
}
