using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;

public class TaskFindCover : Node
{
    private Transform ownTransform;
    private Animator animator;
    private AllyBT allyBT;
    private Player master;
    private LayerMask hideableLayers;
    private float findCoverRadius = 40f;
    private Collider[] colliders = new Collider[10];
    private float hideSensitivity = 0f;
    private float minCoverEnemyDistance = 5f;
    private float minEnemyDistance = 4f;
    private float maxObstacleHeight = 1.25f;

    private float findFrequency = 0.25f;
    private float frequencyCounter = 0f;
    private bool canFind = true;
    private bool foundCover = false;

    public TaskFindCover(Transform _transform, Animator _animator, AllyBT _allyBT, Player _master, LayerMask _hideableLayers)
    {
        ownTransform = _transform;
        animator = _animator;
        allyBT = _allyBT;
        master = _master;
        hideableLayers = _hideableLayers;
    }

    public override NodeState Evaluate()
    {
        if (!master.gameObject.activeInHierarchy || master.beingAttackedBy == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (canFind == false)
        {
            if (!master.isBeingAttacked)
                canFind = true;
            else
            {
                frequencyCounter += Time.deltaTime;

                if (frequencyCounter >= findFrequency)
                {
                    frequencyCounter = 0;
                    canFind = true;
                }
            }
        }

        if (master.isBeingAttacked)
        {
            allyBT.agent.stoppingDistance = 0;

            if (canFind)
            {
                float distanceToEnemy = Vector3.Distance(master.beingAttackedBy.transform.position, ownTransform.position);

                if (distanceToEnemy <= minEnemyDistance || !foundCover)
                {
                    foundCover = false;
                    canFind = false;

                    if (!allyBT.isAttacking)
                        allyBT.currentDecision = "Finding Cover";

                    for (int i = 0; i < colliders.Length; i++)
                    {
                        colliders[i] = null;
                    }

                    int hits = Physics.OverlapSphereNonAlloc(allyBT.agent.transform.position, findCoverRadius, colliders, hideableLayers);

                    int hitReduction = 0;

                    for (int i = 0; i < hits; i++)
                    {
                        if (Vector3.Distance(colliders[i].transform.position, master.beingAttackedBy.transform.position) < minCoverEnemyDistance || colliders[i].bounds.size.y > maxObstacleHeight)
                        {
                            colliders[i] = null;
                            hitReduction++;
                        }
                    }

                    hits -= hitReduction;

                    System.Array.Sort(colliders, ColliderArraySortComparer);

                    for (int i = 0; i < hits; i++)
                    {
                        if (NavMesh.SamplePosition(colliders[i].transform.position, out NavMeshHit hit, 2f, allyBT.agent.areaMask))
                        {
                            if (!NavMesh.FindClosestEdge(hit.position, out hit, allyBT.agent.areaMask))
                            {
                                Debug.LogError($"Unable to find edge close to {hit.position}");
                            }

                            if (Vector3.Dot(hit.normal, (master.beingAttackedBy.transform.position - hit.position).normalized) < hideSensitivity)
                            {
                                allyBT.agent.SetDestination(hit.position);
                                animator.SetBool("Walking", false);
                                animator.SetBool("Running", true);
                                break;
                            }
                            else
                            {
                                // Since previous spot wasn't facing away from the target, we'll try the other side of the object
                                if (NavMesh.SamplePosition(colliders[i].transform.position - (master.beingAttackedBy.transform.position - hit.position).normalized * 2, out NavMeshHit hit2, 2f, allyBT.agent.areaMask))
                                {
                                    if (!NavMesh.FindClosestEdge(hit2.position, out hit2, allyBT.agent.areaMask))
                                    {
                                        Debug.LogError($"Unable to find edge close to {hit2.position} (second attempt)");
                                    }

                                    if (Vector3.Dot(hit2.normal, (master.beingAttackedBy.transform.position - hit2.position).normalized) < hideSensitivity)
                                    {
                                        allyBT.agent.SetDestination(hit2.position);
                                        animator.SetBool("Walking", false);
                                        animator.SetBool("Running", true);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError($"Unable to find NavMesh near object {colliders[i].name} at {colliders[i].transform.position}");

                            animator.SetBool("Walking", false);
                            animator.SetBool("Running", false);
                            state = NodeState.FAILURE;
                            return state;
                        }
                    }
                }
            }

            if (allyBT.agent.remainingDistance > 0.1f)
            {
                animator.SetBool("Walking", false);
                animator.SetBool("Running", true);
                Vector3 destination = new Vector3(allyBT.agent.steeringTarget.x, ownTransform.position.y, allyBT.agent.steeringTarget.z);
                Quaternion targetRotation = Quaternion.LookRotation(destination - ownTransform.position);
                ownTransform.rotation = Quaternion.Slerp(ownTransform.rotation, targetRotation, Time.deltaTime * 10);
            }
            else
            {
                animator.SetBool("Walking", false);
                animator.SetBool("Running", false);
                foundCover = true;
                allyBT.currentDecision = "Found Cover";
                Vector3 destination = new Vector3(master.beingAttackedBy.transform.position.x, ownTransform.position.y, master.beingAttackedBy.transform.position.z);
                Quaternion targetRotation = Quaternion.LookRotation(destination - ownTransform.position);
                ownTransform.rotation = Quaternion.Slerp(ownTransform.rotation, targetRotation, Time.deltaTime * 10);
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.RUNNING;
            return state;
        }

        animator.SetBool("Walking", false);
        animator.SetBool("Running", false);

        state = NodeState.FAILURE;
        return state;
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
            return Vector3.Distance(allyBT.agent.transform.position, _a.transform.position).CompareTo(Vector3.Distance(allyBT.agent.transform.position, _b.transform.position));
        }
    }
}
