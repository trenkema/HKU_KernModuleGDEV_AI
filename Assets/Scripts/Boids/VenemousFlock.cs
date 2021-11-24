using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenemousFlock : MonoBehaviour
{
    [SerializeField] LayerMask enemyLayers;

    private void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject, enemyLayers))
        {
            other.GetComponent<IDamageable>()?.TakeFullDamage();
        }
    }

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }
}
