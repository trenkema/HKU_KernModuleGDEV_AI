using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum pickupableTypes { Weapon, Useable }

public abstract class Pickupable : MonoBehaviour
{
    public GameObject owner = null;
    public int itemIndex = 0;
    public pickupableTypes pickupType;
    public bool pickupableByPlayer = false;

    public abstract void Use();
}
