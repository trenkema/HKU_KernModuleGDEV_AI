using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pepper : Pickupable
{
    public override void Use()
    {
        if (owner.GetComponent<GuardBT>() != null)
        {
            owner.GetComponent<GuardBT>().hasSpeedUpgrade = true;
            owner.GetComponent<GuardBT>().speedUpgradeEffect.Play();
        }
    }
}
