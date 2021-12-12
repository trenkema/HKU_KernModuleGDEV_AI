using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] LayerMask damageableLayers;
    GameObject attacker;
    bool hasCollided = false;

    public void SetVariables(GameObject _attacker)
    {
        attacker = _attacker;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject, damageableLayers) && !hasCollided)
        {
            hasCollided = true;
            other.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage, attacker);
            Destroy(gameObject);
        }
    }

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }
}
