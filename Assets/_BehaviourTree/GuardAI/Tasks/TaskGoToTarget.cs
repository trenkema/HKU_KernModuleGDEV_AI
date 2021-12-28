using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskGoToTarget : Node
{
    private Transform ownTransform;
    private Transform headTransform;
    private Animator animator;
    private GuardBT guardBT;

    public TaskGoToTarget(Transform _transform, Transform _headTransform, Animator _animator, GuardBT _guardBT)
    {
        ownTransform = _transform;
        headTransform = _headTransform;
        animator = _animator;
        guardBT = _guardBT;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("Target");

        if (target != null)
        {
            if (!target.gameObject.activeInHierarchy)
            {
                animator.SetBool("Running", false);
                ClearData("Target");
            }
        }

        if (target != null)
        {
            if (!guardBT.isAttacking || guardBT.currentWeapon != null)
            {
                float attackRange = guardBT.currentWeapon != null ? guardBT.currentWeapon.attackRange : guardBT.defaultAttackRange;
                if (Vector3.Distance(ownTransform.position, target.position) > attackRange && Vector3.Distance(ownTransform.position, target.position) < guardBT.chaseRange)
                {
                    if (target.GetComponent<Player>())
                    {
                        target.GetComponent<Player>().beingAttackedBy = ownTransform.gameObject;
                        target.GetComponent<Player>().isBeingAttacked = true;
                    }

                    guardBT.agent.stoppingDistance = attackRange;
                    guardBT.decision = "Chasing Enemy"; // Set Decision

                    animator.SetBool("Walking", false);
                    animator.SetBool("Running", true);

                    if (guardBT.hasSpeedUpgrade)
                        guardBT.agent.speed = guardBT.upgradedRunSpeed;
                    else
                        guardBT.agent.speed = guardBT.runSpeed;

                    guardBT.agent.SetDestination(target.position);
                }
                else if (Vector3.Distance(ownTransform.position, target.position) > guardBT.chaseRange)
                {
                    if (target.GetComponent<Player>())
                    {
                        if (target.GetComponent<Player>().beingAttackedBy == ownTransform.gameObject)
                            target.GetComponent<Player>().beingAttackedBy = null;

                        target.GetComponent<Player>().isBeingAttacked = false;
                    }

                    ClearData("Target");

                    guardBT.agent.stoppingDistance = 0;

                    animator.SetBool("Running", false);

                    state = NodeState.FAILURE;
                    return state;
                }

                if (guardBT.agent.remainingDistance > 0.1f)
                {
                    Vector3 destination = new Vector3(guardBT.agent.steeringTarget.x, ownTransform.position.y, guardBT.agent.steeringTarget.z);
                    Quaternion targetRotation = Quaternion.LookRotation(destination - ownTransform.position);
                    ownTransform.rotation = Quaternion.Slerp(ownTransform.rotation, targetRotation, Time.deltaTime * 10);
                }

                state = NodeState.RUNNING;
                return state;
            }
        }

        guardBT.agent.stoppingDistance = 0;

        state = NodeState.FAILURE;
        return state;
    }
}
