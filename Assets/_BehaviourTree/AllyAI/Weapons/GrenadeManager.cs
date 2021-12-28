using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeManager : MonoBehaviour
{
    public Transform throwPoint;
    public GameObject grenadePrefab;
    private Rigidbody rb;
    public GameObject targetObject;
    public GameObject playerObject;
    public float enemyDistance;
    public float minOffset, maxOffset;

    public float forwardForce, upwardForce;
    public float newForwardForce, newUpwardForce;

    public void ThrowGrenade()
    {
        float distance = Vector3.Distance(playerObject.transform.position, targetObject.transform.position);

        newForwardForce = forwardForce * distance;
        newUpwardForce = upwardForce * distance;


        GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, Quaternion.identity);
        rb = grenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            float randomOffset = Random.Range(minOffset, maxOffset);
            newForwardForce += randomOffset;
            newUpwardForce += randomOffset;

            rb.AddForce(transform.forward * newForwardForce, ForceMode.Impulse);
            rb.AddForce(transform.up * newUpwardForce, ForceMode.Impulse);
        }
    }
}
