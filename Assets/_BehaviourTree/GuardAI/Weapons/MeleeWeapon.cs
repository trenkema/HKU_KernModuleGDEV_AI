using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : BaseWeapon
{
    [SerializeField] Transform attackPoint;
    [SerializeField] float hitRange;
    [SerializeField] LayerMask attackLayerMask;
    [SerializeField] float attackDamage;

    public override void Attack()
    {
        animator.SetTrigger(animationString);
    }

    public override void AttackImpact()
    {
        Collider[] colliders = Physics.OverlapSphere(attackPoint.position, hitRange, attackLayerMask, QueryTriggerInteraction.Ignore);

        foreach (var collider in colliders)
        {
            Debug.Log("Dealth Damage");
            collider.GetComponent<IDamageable>()?.TakeDamage(attackDamage, gameObject);
        }
    }
}
