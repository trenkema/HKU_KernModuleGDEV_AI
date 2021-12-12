using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;

public class AllyBT : BehaviourTree.Tree
{
    #region Variables
    public NavMeshAgent agent;
    public Player master;
    public GrenadeManager grenadeManager;

    [SerializeField] Transform headTransform;

    [SerializeField] Animator animator;

    [SerializeField] LayerMask enemyLayer;

    [SerializeField] LayerMask obstructionLayer;

    [SerializeField] LayerMask hideableLayers;

    [SerializeField] float setWalkSpeed = 2f;
    public float walkSpeed { get { return setWalkSpeed; } }

    [SerializeField] float setRunSpeed = 3.5f;
    public float runSpeed { get { return setRunSpeed; } }

    [SerializeField] float setFovRange = 6f;
    public float fovRange { get { return setFovRange; } }

    [SerializeField] float setFollowDistance = 4f;
    public float followDistance { get { return setFollowDistance; } }

    [SerializeField] float setAttackRange = 2f;
    public float attackRange { get { return setAttackRange; } }

    [SerializeField] float setAttackDuration = 1.5f;
    public float attackDuration { get { return setAttackDuration; } }

    public string currentDecision = "Idle";
    public string getDecision { get { return currentDecision; } }

    public bool canAttack = true;
    public bool isAttacking = false;
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
                new CheckCanAttackEnemy(this),
                new CheckMasterBeingAttacked(master),
                new TaskFindCover(transform, animator, this, master, hideableLayers),
                new TaskAttackEnemy(transform, animator, this, master),
            }),
            new TaskFollowMaster(transform, animator, this, master),
        });
        
        return root;
    }
}
