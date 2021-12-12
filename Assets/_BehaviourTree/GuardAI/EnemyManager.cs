using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private int healthPoints;

    private void Awake()
    {
        healthPoints = 30;
    }

    public bool TakeHit()
    {
        healthPoints -= 10;
        bool isDead = healthPoints <= 0;
        if (isDead) Die();
        return isDead;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
