using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using TMPro;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] Transform Camera;

    [SerializeField] Color defaultOutlineColor;
    [SerializeField] Color pickupableOutlineColor;

    [Space(10)]

    public bool isBeingAttacked = false;
    public GameObject beingAttackedBy = null;

    [Space(10)]

    [SerializeField] float rotationSpeed = 180f;
    [SerializeField] float moveSpeed = 3;
    [SerializeField] float deathForce = 1000;
    [SerializeField] GameObject ragdoll;

    [SerializeField] LayerMask pickupableLayerMask;
    [SerializeField] GameObject[] pickupablesSprites;
    [SerializeField] GameObject[] pickupableDrops;

    [SerializeField] GameObject dropText;

    [SerializeField] Transform dropTransform;

    [Space(10)]

    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] float maxHealth;
    private float currentHealth;

    private Rigidbody rb;
    private Animator animator;
    private float vert = 0;
    private float hor = 0;
    private Vector3 moveDirection;
    private Collider mainCollider;
    GameObject attacker;

    private bool hasFood = false;
    private int pickupableIndex = 0;

    public bool hasStarted = false;

    private GameObject pickedupFruit;

    void Start()
    {
        if (healthText != null)
        {
            currentHealth = maxHealth;

            healthText.text = "+ " + currentHealth;
        }

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

        dropText.SetActive(false);
    }

    private void OnEnable()
    {
        EventSystemNew.Subscribe(Event_Type.START_GAME, StartGame);
        EventSystemNew.Subscribe(Event_Type.GAME_WON, EndGame);
        EventSystemNew.Subscribe(Event_Type.GAME_LOST, EndGame);
    }

    private void OnDisable()
    {
        EventSystemNew.Unsubscribe(Event_Type.START_GAME, StartGame);
        EventSystemNew.Unsubscribe(Event_Type.GAME_WON, EndGame);
        EventSystemNew.Unsubscribe(Event_Type.GAME_LOST, EndGame);
    }

    void Update()
    {
        if (hasStarted)
        {
            vert = Input.GetAxisRaw("Vertical");
            hor = Input.GetAxisRaw("Horizontal");
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

            if (Input.GetKeyDown(KeyCode.Q) && hasFood)
            {
                hasFood = false;

                pickedupFruit.transform.position = dropTransform.position;
                pickedupFruit.transform.rotation = Quaternion.identity;

                pickedupFruit.SetActive(true);

                pickedupFruit = null;

                pickupableIndex = 0;

                foreach (var sprite in pickupablesSprites)
                {
                    sprite.SetActive(false);
                }

                dropText.SetActive(false);
            }
        }
    }

    private void StartGame()
    {
        hasStarted = true;
    }

    private void EndGame()
    {
        hasStarted = false;

        animator.SetBool("Walking", false);
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

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKey(KeyCode.E))
        {
            if (IsInLayerMask(other.gameObject, pickupableLayerMask) && !hasFood)
            {
                if (other.GetComponent<Pickupable>() != null)
                {
                    Pickupable pickupable = other.GetComponent<Pickupable>();

                    if (pickupable.owner == null)
                    {
                        hasFood = true;

                        foreach (var sprite in pickupablesSprites)
                        {
                            sprite.SetActive(false);
                        }

                        pickupableIndex = pickupable.itemIndex;
                        pickupablesSprites[pickupableIndex].SetActive(true);

                        dropText.SetActive(true);

                        pickedupFruit = other.gameObject;

                        other.gameObject.SetActive(false);
                    }
                }
            }
        }

        if (IsInLayerMask(other.gameObject, pickupableLayerMask))
        {
            if (!hasFood)
            {
                if (other.GetComponent<Pickupable>() != null)
                {
                    Pickupable pickupable = other.GetComponent<Pickupable>();

                    if (pickupable.owner == null)
                    {
                        other.GetComponent<OutlineGameObject>().OutlineColor = pickupableOutlineColor;
                    }
                    else
                    {
                        other.GetComponent<OutlineGameObject>().OutlineColor = defaultOutlineColor;
                    }
                }
            }
            else
            {
                other.GetComponent<OutlineGameObject>().OutlineColor = defaultOutlineColor;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsInLayerMask(other.gameObject, pickupableLayerMask))
        {
            if (other.GetComponent<Pickupable>() != null)
            {
                other.GetComponent<OutlineGameObject>().OutlineColor = defaultOutlineColor;
            }
        }
    }

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }
}
