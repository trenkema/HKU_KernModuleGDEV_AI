using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootWeapon : BaseWeapon
{
    [SerializeField] Transform attackPoint;
    [SerializeField] GameObject bulletPrefab;

    [SerializeField] float shootForceForward, shootForceUp;

    public override void Attack()
    {
        animator.SetTrigger(animationString);
    }

    public override void AttackImpact()
    {
        GameObject projectile = Instantiate(bulletPrefab, attackPoint.position, Quaternion.identity);
        projectile.GetComponent<Bullet>().SetVariables(gameObject);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * shootForceForward, ForceMode.Impulse);
        rb.AddForce(transform.up * shootForceUp, ForceMode.Impulse);
    }
}
