using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickAttack : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string animationString;
    [SerializeField] Transform attackPoint;
    [SerializeField] float hitRange;
    [SerializeField] LayerMask attackLayerMask;
    [SerializeField] float attackDamage;

    public void Attack()
    {
        animator.SetTrigger(animationString);
    }

    public void AttackImpact()
    {
        Collider[] colliders = Physics.OverlapSphere(attackPoint.position, hitRange, attackLayerMask, QueryTriggerInteraction.Ignore);

        foreach (var collider in colliders)
        {
            Debug.Log("Dealth Damage");
            collider.GetComponent<IDamageable>()?.TakeDamage(attackDamage, gameObject);
        }
    }
}
