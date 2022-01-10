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
    [HideInInspector] public bool needFood = false;
    [HideInInspector] public bool isEating = false;
    [HideInInspector] public bool foundFood = false;
    [HideInInspector] public bool isPickingup = false;
    [HideInInspector] public bool isHiding = false;
    [HideInInspector] public bool resetWanderTimer = false;
    [HideInInspector] public bool hasStarted = false;

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
    [Range(0, 60f)]
    [SerializeField] float needFoodCooldown = 10f;

    [Range(0, 120f)]
    [SerializeField] float hungerDeathTimeMin = 30f;
    [Range(0, 120f)]
    [SerializeField] float hungerDeathTimeMax = 30f;
    private float hungerDeathTime = 0;

    [Space(5)]

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
    [SerializeField] float walkSpeed = 2f;

    [Range(0, 5f)]
    [SerializeField] float runSpeed = 3.5f;
    #endregion

    private void Awake()
    {
        agent.updateRotation = false;

        hungerDeathTime = Random.Range(hungerDeathTimeMin, hungerDeathTimeMax);

        EventSystemNew.Subscribe(Event_Type.START_CHICKENS, StartGame);
        EventSystemNew.Subscribe(Event_Type.GAME_LOST, EndGame);
        EventSystemNew.Subscribe(Event_Type.GAME_WON, EndGame);
    }

    private void OnDisable()
    {
        EventSystemNew.Unsubscribe(Event_Type.START_CHICKENS, StartGame);
        EventSystemNew.Unsubscribe(Event_Type.GAME_LOST, EndGame);
        EventSystemNew.Unsubscribe(Event_Type.GAME_WON, EndGame);
    }

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckEnemyInFOVRange(transform, headTransform, enemyLayer, obstructionLayer, fovRange),
                new CheckForHunger(this, needFoodCooldown, hungerDeathTime),
                new CheckForFood(transform, headTransform, eatLayer, obstructionLayer, this, foodFindRange),
                new TaskHide(transform, animator, this, runSpeed, hideableLayers, findCoverRange, minCoverEnemyDistance, fovRange),
                new TaskGoToFood(transform, animator, this, runSpeed),
                new TaskPickupFood(animator, this),
            }),
            new TaskWander(transform, animator, this, walkSpeed, wanderTimeMin, wanderTimeMax, wanderRange),
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
