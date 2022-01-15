using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;

public class TaskHide : Node
{
    private Transform ownTransform;
    private Animator animator;
    private ChickenBT chickenBT;
    private LayerMask hideableLayers;

    private float findCoverRadius = 0f;
    private float minCoverEnemyDistance = 0f;
    private float minEnemyDistance = 0f;

    private Collider[] colliders = new Collider[10];

    private Vector3 coverLocation = Vector3.zero;

    private bool foundCover = false;

    private float runSpeed = 0f;

    public TaskHide(Transform _transform, Animator _animator, ChickenBT _chickenBT, float _runSpeed, LayerMask _hideableLayers, float _findCoverRange, float _minCoverEnemyDistance, float _minEnemyDistance)
    {
        ownTransform = _transform;
        animator = _animator;
        chickenBT = _chickenBT;
        runSpeed = _runSpeed;
        hideableLayers = _hideableLayers;
        findCoverRadius = _findCoverRange;
        minCoverEnemyDistance = _minCoverEnemyDistance;
        minEnemyDistance = _minEnemyDistance;
    }

    public override NodeState Evaluate()
    {
        if (!chickenBT.agent.isOnNavMesh)
        {
            state = NodeState.FAILURE;
            return state;
        }

        Transform pickupable = (Transform)GetData("Pickupable");
        Transform target = (Transform)GetData("Target");

        if (target != null && !chickenBT.isPickingup)
        {
            if (pickupable != null)
            {
                pickupable.GetComponent<Pickupable>().owner = null;
                ClearData("Pickupable");
                chickenBT.foundFood = false;
            }

            float distanceToEnemy = Vector3.Distance(target.position, ownTransform.position);

            if (distanceToEnemy <= minEnemyDistance && !foundCover)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i] = null;
                }

                int hits = Physics.OverlapSphereNonAlloc(ownTransform.position, findCoverRadius, colliders, hideableLayers);

                int hitReduction = 0;

                for (int i = 0; i < hits; i++)
                {
                    if (Vector3.Distance(target.position, colliders[i].transform.position) < minCoverEnemyDistance)
                    {
                        colliders[i] = null;
                        hitReduction++;
                    }
                }

                hits -= hitReduction;

                System.Array.Sort(colliders, ColliderArraySortComparer);

                for (int i = 0; i < hits; i++)
                {
                    bool isThereCover = RandomPoint(colliders[i].transform.position, 2f, target, out coverLocation);

                    if (isThereCover)
                    {
                        NavMeshHit hit = new NavMeshHit();

                        if (NavMesh.SamplePosition(coverLocation, out hit, 1f, chickenBT.agent.areaMask))
                        {
                            chickenBT.agent.speed = runSpeed;
                            
                            chickenBT.agent.SetDestination(hit.position);

                            chickenBT.isHiding = true;
                            foundCover = true;
                            animator.SetBool("Walking", false);
                            animator.SetBool("Running", true);
                            break;
                        }
                    }
                }
            }
        }

        if (chickenBT.isHiding)
        {
            if (chickenBT.agent.pathPending)
            {
                state = NodeState.RUNNING;
                return state;
            }

            animator.SetBool("Walking", false);
            animator.SetBool("Running", true);
            chickenBT.decision = "Covering";

            Vector3 destination = new Vector3(chickenBT.agent.steeringTarget.x, ownTransform.position.y, chickenBT.agent.steeringTarget.z);
            Quaternion targetRotation = Quaternion.LookRotation(destination - ownTransform.position);
            ownTransform.localRotation = Quaternion.Slerp(ownTransform.localRotation, targetRotation, Time.deltaTime * 10);

            if (chickenBT.agent.remainingDistance < 0.15f)
            {
                chickenBT.isHiding = false;
                foundCover = false;
                chickenBT.resetWanderTimer = true;

                animator.SetBool("Walking", false);
                animator.SetBool("Running", false);

                ownTransform.localRotation = Quaternion.Slerp(ownTransform.localRotation, targetRotation, Time.deltaTime * 10);
            }

            state = NodeState.RUNNING;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }

    bool RandomPoint(Vector3 _center, float _rangeFromCenter, Transform _targetTransform, out Vector3 _resultCover)
    {
        Vector3 randomPosition = Vector3.zero;

        for (int i = 0; i < 10; i++)
        {
            randomPosition = _center + Random.insideUnitSphere * _rangeFromCenter;
            Vector3 directionToEnemy = _targetTransform.position - randomPosition;

            RaycastHit hitCheckCover;
            if (Physics.Raycast(randomPosition, directionToEnemy.normalized, out hitCheckCover, _rangeFromCenter, hideableLayers))
            {
                _resultCover = randomPosition;
                return true;
            }

        }

        _resultCover = Vector3.zero;
        return false;
    }

    private int ColliderArraySortComparer(Collider _a, Collider _b)
    {
        if (_a == null && _b != null)
        {
            return 1;
        }
        else if (_a != null && _b == null)
        {
            return -1;
        }
        else if (_a == null && _b == null)
        {
            return 0;
        }
        else
        {
            return Vector3.Distance(chickenBT.agent.transform.position, _a.transform.position).CompareTo(Vector3.Distance(chickenBT.agent.transform.position, _b.transform.position));
        }
    }
}
