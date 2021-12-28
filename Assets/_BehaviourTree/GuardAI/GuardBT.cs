using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;

public class GuardBT : BehaviourTree.Tree
{
    #region Variables
    public NavMeshAgent agent;
    public BaseWeapon currentWeapon;
    public KickAttack defaultAttack;

    [SerializeField] AttackManager attackManager;

    [SerializeField] Transform headTransform;

    [SerializeField] Transform[] waypoints;

    [SerializeField] Animator animator;

    [SerializeField] LayerMask enemyLayer;

    [SerializeField] LayerMask obstructionLayer;

    [SerializeField] LayerMask pickupableLayer;

    [SerializeField] LayerMask blockedSightLayer;

    [SerializeField] float setWalkSpeed = 2f;
    public float walkSpeed { get { return setWalkSpeed; } }

    [SerializeField] float setRunSpeed = 3.5f;
    public float runSpeed { get { return setRunSpeed; } }

    [SerializeField] float setUpgradedRunSpeed = 4.5f;
    public float upgradedRunSpeed { get { return setUpgradedRunSpeed; } }

    [SerializeField] float setFovRange = 6f;
    public float fovRange { get { return setFovRange; } }

    [SerializeField] float setChaseRange = 4f;
    public float chaseRange { get { return setChaseRange; } }

    [SerializeField] float setAttackRange = 2f;
    public float defaultAttackRange { get { return setAttackRange; } }

    [SerializeField] float setDefaultAttackDuration = 1.5f;
    public float defaultAttackDuration { get { return setDefaultAttackDuration; } }

    [SerializeField] float pickupableFindRange = 10f;

    [SerializeField] GameObject[] setWeaponObjects;
    public GameObject[] weaponObjects { get { return setWeaponObjects; } }

    public bool hasWeapon = false;
    public bool foundWeapon = false;

    public bool hasItem = false;
    public bool foundItem = false;

    public bool canAttack = true;
    public bool isAttacking = false;
    public bool isPickingup = false;
    public bool sightBlocked = false;

    public bool hasSpeedUpgrade = false;
    public ParticleSystem speedUpgradeEffect;
    #endregion

    private void Awake()
    {
        agent.updateRotation = false;
    }

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckSightBlocked(transform, animator, blockedSightLayer, this),
                new CheckCanAttack(this),
                new CheckEnemyInAttackRange(transform, this),
                new TaskAttack(transform, animator, this),
            }),
            new Sequence(new List<Node>
            {
                new CheckSightBlocked(transform, animator, blockedSightLayer, this),
                new CheckEnemyInFOVRange(transform, headTransform, enemyLayer, obstructionLayer, fovRange),
                new CheckForItem(transform, headTransform, pickupableLayer, obstructionLayer, this, pickupableFindRange),
                new TaskGoToItem(transform, animator, this),
                new TaskPickUpItem(transform, animator, this, attackManager),
                new TaskGoToTarget(transform, headTransform, animator, this),
            }),
            new TaskPatrol(transform, animator, waypoints, this),
        });
        
        return root;
    }
}
