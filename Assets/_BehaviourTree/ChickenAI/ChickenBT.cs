using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;

public class ChickenBT : BehaviourTree.Tree
{
    #region Variables
    public NavMeshAgent agent;

    [Space(10)]

    [Header("Bool Settings")]
    public bool needFood = false;
    /* [HideInInspector] */ public bool isEating = false;
    /* [HideInInspector] */ public bool foundFood = false;
    /* [HideInInspector] */ public bool isPickingup = false;
    /* [HideInInspector] */ public bool isHiding = false;
    /* [HideInInspector] */ public bool resetWanderTimer = false;

    [Space(10)]

    [Header("References")]
    public DisplayDecision displayDecision;
    [SerializeField] Animator animator;
    [SerializeField] Transform headTransform;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask obstructionLayer;
    [SerializeField] LayerMask hideableLayers;
    [SerializeField] LayerMask eatLayer;

    [Space(10)]

    [Header("Slider Settings")]
    [Range(0, 5)]
    [SerializeField] int amountOfTimesRequiredToFeed = 3;

    [Range(0, 60f)]
    [SerializeField] float needFoodCooldown = 10f;

    [Range(0, 60f)]
    [SerializeField] float hungryDeathTimeMin = 30f;
    [Range(0, 60f)]
    [SerializeField] float hungryDeathTimeMax = 30f;
    private float hungryDeathTime = 0;

    [Space(5)]

    [Range(0, 15f)]
    [SerializeField] float minEnemyDistance = 4f;

    [Range(0, 15f)]
    [SerializeField] float minCoverEnemyDistance = 8f;

    [Range(0, 50f)]
    [SerializeField] float findCoverRange = 40f;

    [Space(5)]

    [Range(0, 15f)]
    [SerializeField] float foodFindRange = 7.5f;

    [Range(0, 15f)]
    [SerializeField] float fovRange = 7.5f;

    [Space(5)]

    [Range(0, 5f)]
    [SerializeField] float wanderTimeMin = 3.5f;

    [Range(0, 10f)]
    [SerializeField] float wanderTimeMax = 6.5f;

    [Range(0, 15f)]
    [SerializeField] float wanderRange = 5f;

    [Space(5)]

    [Range(0, 5f)]
    [SerializeField] float setWalkSpeed = 2f;
    public float walkSpeed { get { return setWalkSpeed; } }

    [Range(0, 5f)]
    [SerializeField] float setRunSpeed = 3.5f;
    public float runSpeed { get { return setRunSpeed; } }

    public bool beenFedCompletely = false;

    public bool hasStarted = false;
    #endregion

    private void Awake()
    {
        agent.updateRotation = false;

        hungryDeathTime = Random.Range(hungryDeathTimeMin, hungryDeathTimeMax);

        EventSystemNew.Subscribe(Event_Type.START_GAME, StartGame);
        EventSystemNew.Subscribe(Event_Type.GAME_LOST, EndGame);
        EventSystemNew.Subscribe(Event_Type.GAME_WON, EndGame);
    }

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckEnemyInFOVRange(transform, headTransform, enemyLayer, obstructionLayer, fovRange),
                new CheckForHunger(this, needFoodCooldown, hungryDeathTime),
                new CheckForFood(transform, headTransform, eatLayer, obstructionLayer, this, foodFindRange),
                new TaskHide(transform, animator, this, hideableLayers, findCoverRange, minCoverEnemyDistance, minEnemyDistance),
                new TaskGoToFood(transform, animator, this),
                new TaskPickupFood(animator, this, amountOfTimesRequiredToFeed),
            }),
            new TaskWander(transform, animator, this, wanderTimeMin, wanderTimeMax, wanderRange),
        });

        return root;
    }

    private void StartGame()
    {
        hasStarted = true;
    }

    private void EndGame()
    {
        hasStarted = false;
    }

    public void ChickenDied()
    {
        EventSystemNew.RaiseEvent(Event_Type.CHICKEN_DIED);
        Destroy(gameObject);
    }
}
