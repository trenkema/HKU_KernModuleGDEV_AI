using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenade : MonoBehaviour
{
    [SerializeField] private ParticleSystem topParticles;
    [SerializeField] private GameObject explodeParticles;

    [SerializeField] private float explosionTime = 1f;
    private float explosionCounter;

    private bool isTriggered = false;
    private bool exploded = false;

    private bool hasEmitted = false;

    private ParticleSystem explosionParticles;

    private void Update()
    {
        if (isTriggered && !exploded)
        {
            explosionCounter += Time.deltaTime;

            if (explosionCounter >= explosionTime)
            {
                exploded = true;
                topParticles.Stop();
                explosionParticles = Instantiate(explodeParticles, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            }
        }

        if (explosionParticles != null)
        {
            if (explosionParticles.particleCount > 0 && !hasEmitted)
            {
                hasEmitted = true;
            }

            if (explosionParticles.particleCount == 0 && hasEmitted)
            {
                Destroy(explosionParticles.gameObject);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered)
        {
            isTriggered = true;
        }
    }
}
