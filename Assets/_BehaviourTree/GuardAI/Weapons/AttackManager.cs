using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public BaseWeapon currentWeapon;
    public KickAttack defaultAttack;
    [SerializeField] GuardBT guardBT;

    public void MeleeImpact()
    {
        currentWeapon.AttackImpact();
    }

    public void ShootImpact()
    {
        currentWeapon.AttackImpact();
    }

    public void KickImpact()
    {
        defaultAttack.AttackImpact();
    }
}
