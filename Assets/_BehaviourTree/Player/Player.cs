using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using TMPro;

public class Player : MonoBehaviour, IDamageable
{
    public Transform Camera;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float deathForce = 1000;
    [SerializeField] private GameObject ragdoll;
    Rigidbody rb;
    Animator animator;
    float vert = 0;
    float hor = 0;
    Vector3 moveDirection;
    Collider mainCollider;

    [SerializeField] TextMeshProUGUI healthText;

    [SerializeField] float maxHealth;
    float currentHealth;
    GameObject attacker;
    public bool isBeingAttacked = false;
    public GameObject beingAttackedBy = null;

    void Start()
    {
        currentHealth = maxHealth;

        healthText.text = "+ " + currentHealth;

        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        mainCollider = GetComponent<Collider>();
        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rib in rigidBodies)
        {
            rib.isKinematic = true;
            rib.useGravity = false;
        }

        var cols = GetComponentsInChildren<Collider>();
        foreach (Collider col in cols)
        {
            if (col.isTrigger) { continue; }
            col.enabled = false;
        }
        mainCollider.enabled = true;
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    void Update()
    {
        vert = Input.GetAxis("Vertical");
        hor = Input.GetAxis("Horizontal");
        Vector3 forwardDirection = Vector3.Scale(new Vector3(1, 0, 1), Camera.transform.forward);
        Vector3 rightDirection = Vector3.Cross(Vector3.up, forwardDirection.normalized);
        moveDirection = forwardDirection.normalized * vert + rightDirection.normalized * hor;
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDirection.normalized, Vector3.up), rotationSpeed * Time.deltaTime);
        }
        transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;

        bool isMoving = hor != 0 || vert != 0;

        if (isMoving)
        {
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }
    }

    public void TakeDamage(float _damage, GameObject _attacker)
    {
        currentHealth -= _damage;
        attacker = _attacker;

        if (currentHealth <= 0)
        {
            Die();
        }

        healthText.text = "+ " + currentHealth;
    }

    public void TakeFullDamage()
    {
        currentHealth = 0;
        Die();

        healthText.text = "+ " + currentHealth;
    }

    private void Die()
    {
        animator.enabled = false;
        var cols = GetComponentsInChildren<Collider>();
        foreach (Collider col in cols)
        {
            col.enabled = true;
        }
        mainCollider.enabled = false;

        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rib in rigidBodies)
        {
            rib.isKinematic = false;
            rib.useGravity = true;
            rib.AddForce(Vector3.Scale(new Vector3(1, 0.5f, 1), (transform.position - attacker.transform.position).normalized * deathForce));
        }
        ragdoll.transform.SetParent(null);

        gameObject.SetActive(false);
    }
}
