using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public Animator animator;

    public string animationString;

    public float attackRange;

    public float attackDuration;

    public abstract void Attack();

    public abstract void AttackImpact();
}
