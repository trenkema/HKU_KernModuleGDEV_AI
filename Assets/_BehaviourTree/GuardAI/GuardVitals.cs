using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardVitals : MonoBehaviour, IDamageable
{
    [SerializeField] float maxHealth;
    float currentHealth;
    GameObject attacker;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float _damage, GameObject _attacker)
    {
        currentHealth -= _damage;
        attacker = _attacker;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeFullDamage()
    {
        currentHealth = 0;
        Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
