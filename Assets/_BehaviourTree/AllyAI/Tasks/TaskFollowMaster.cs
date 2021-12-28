using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;

public class TaskFollowMaster : Node
{
    private Transform ownTransform;
    private Animator animator;

    private AllyBT allyBT;
    private Player master;

    public TaskFollowMaster(Transform _transform, Animator _animator, AllyBT _allyBT, Player _master)
    {
        ownTransform = _transform;
        animator = _animator;
        allyBT = _allyBT;
        master = _master;
    }

    public override NodeState Evaluate()
    {
        if (!master.gameObject.activeInHierarchy)
        {
            allyBT.decision = "Master Died"; // Set Decision
            allyBT.agent.isStopped = true;
            animator.SetBool("Walking", false);
            animator.SetBool("Running", false);
            state = NodeState.FAILURE;
            return state;
        }

        if (!allyBT.isAttacking && !master.isBeingAttacked)
        {
            Debug.Log("Following Master");

            allyBT.agent.stoppingDistance = allyBT.followDistance;

            allyBT.agent.speed = allyBT.walkSpeed;

            allyBT.decision = "Following Master"; // Set Decision
            allyBT.agent.SetDestination(master.gameObject.transform.position);


            if (allyBT.agent.velocity == Vector3.zero)
            {
                animator.SetBool("Walking", false);
                animator.SetBool("Running", false);
            }
            else
            {
                animator.SetBool("Walking", true);
                animator.SetBool("Running", false);
            }

            if (allyBT.agent.remainingDistance > 0.1f)
            {
                Vector3 destination = new Vector3(allyBT.agent.steeringTarget.x, ownTransform.position.y, allyBT.agent.steeringTarget.z);
                Quaternion targetRotation = Quaternion.LookRotation(destination - ownTransform.position);
                ownTransform.rotation = Quaternion.Slerp(ownTransform.rotation, targetRotation, Time.deltaTime * 10);
            }

            state = NodeState.RUNNING;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
