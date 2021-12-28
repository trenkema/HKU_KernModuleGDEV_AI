using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;

public class TaskWander : Node
{
    private Transform ownTransform;
    private Animator animator;
    private ChickenBT chickenBT;

    private float wanderRange;
    private float wanderTimeMin;
    private float wanderTimeMax;
    private float wanderTimer;

    private float timer = 3f;

    public TaskWander(Transform _transform, Animator _animator, ChickenBT _chickenBT, float _wanderTimeMin, float _wanderTimeMax, float _wanderRange)
    {
        ownTransform = _transform;
        animator = _animator;
        chickenBT = _chickenBT;
        wanderTimeMin = _wanderTimeMin;
        wanderTimeMax = _wanderTimeMax;
        wanderTimer = Random.Range(wanderTimeMin, wanderTimeMax);
        wanderRange = _wanderRange;
    }

    public override NodeState Evaluate()
    {
        if (!chickenBT.agent.isOnNavMesh)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (!chickenBT.foundFood && !chickenBT.isHiding)
        {
            chickenBT.isHiding = false;

            if (chickenBT.resetWanderTimer)
            {
                Debug.Log("Skip Timer");
                timer = wanderTimer;
                chickenBT.resetWanderTimer = false;
            }

            Debug.Log("Wandering");
            chickenBT.agent.stoppingDistance = 0;
            chickenBT.agent.speed = chickenBT.walkSpeed;

            timer += Time.deltaTime;

            if (timer >= wanderTimer)
            {
                wanderTimer = Random.Range(wanderTimeMin, wanderTimeMax);
                Vector3 newPos = RandomNavSphere(ownTransform.position, wanderRange, -1);
                chickenBT.agent.SetDestination(newPos);
                timer = 0;
            }

            if (chickenBT.agent.remainingDistance > 0.1f)
            {
                chickenBT.decision = "W"; // Set Decision
                animator.SetBool("Running", false);
                animator.SetBool("Walking", true);

                Vector3 destination = new Vector3(chickenBT.agent.steeringTarget.x, ownTransform.position.y, chickenBT.agent.steeringTarget.z);
                Quaternion targetRotation = Quaternion.LookRotation(destination - ownTransform.position);
                ownTransform.rotation = Quaternion.Slerp(ownTransform.rotation, targetRotation, Time.deltaTime * 10);
            }
            else
            {
                chickenBT.decision = "..."; // Set Decision
                animator.SetBool("Running", false);
                animator.SetBool("Walking", false);
            }

            state = NodeState.RUNNING;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }

    private Vector3 RandomNavSphere(Vector3 _origin, float _dist, int _layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * _dist;

        randomDirection += _origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, _dist, _layermask);

        return navHit.position;
    }
}
