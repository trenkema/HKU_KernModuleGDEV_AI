using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskGoToItem : Node
{
    private Transform ownTransform;
    private Animator animator;
    private GuardBT guardBT;

    public TaskGoToItem(Transform _transform, Animator _animator, GuardBT _guardBT)
    {
        ownTransform = _transform;
        animator = _animator; 
        guardBT = _guardBT;
    }

    public override NodeState Evaluate()
    {
        if (!guardBT.isAttacking && !guardBT.sightBlocked)
        {
            guardBT.agent.stoppingDistance = 0;
            guardBT.agent.isStopped = false;

            Transform pickupable = (Transform)GetData("Pickupable");
            Transform target = (Transform)GetData("Target");

            if (pickupable != null)
            {
                if (target != null)
                {
                    if (target.GetComponent<Player>())
                    {
                        target.GetComponent<Player>().beingAttackedBy = null;
                        target.GetComponent<Player>().isBeingAttacked = false;
                    }
                }

                if (pickupable.GetComponent<Pickupable>()?.pickupType == pickupableTypes.Weapon)
                {
                    if (guardBT.hasWeapon)
                    {
                        state = NodeState.SUCCESS;
                        return state;
                    }
                }
                else if (pickupable.GetComponent<Pickupable>()?.pickupType == pickupableTypes.Useable)
                {
                    if (guardBT.hasItem)
                    {
                        state = NodeState.SUCCESS;
                        return state;
                    }
                }

                if (guardBT.agent.remainingDistance > 1.2f)
                {
                    guardBT.decision = "Going To Item"; // Set Decision
                    animator.SetBool("Running", true);
                    animator.SetBool("Walking", false);

                    if (guardBT.hasSpeedUpgrade)
                        guardBT.agent.speed = guardBT.upgradedRunSpeed;
                    else
                        guardBT.agent.speed = guardBT.runSpeed;

                    guardBT.agent.SetDestination(pickupable.position);

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
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
