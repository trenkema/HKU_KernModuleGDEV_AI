using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskAttack : Node
{
    private Transform ownTransform;
    private Transform lastTarget;
    private Animator animator;

    private GuardBT guardBT;

    public TaskAttack(Transform _ownTransform, Animator _animator, GuardBT _guardBT)
    {
        ownTransform = _ownTransform;
        animator = _animator;
        guardBT = _guardBT;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("Target");

        if (!target.gameObject.activeInHierarchy)
        {
            animator.SetBool("Running", false);
            animator.SetBool("Walking", false);
            ClearData("Target");
        }

        if (target != null && !guardBT.isPickingup && !guardBT.sightBlocked)
        {
            if (target != lastTarget)
            {
                lastTarget = target;
            }

            if (guardBT.canAttack)
            {
                ownTransform.LookAt(target.position);

                guardBT.canAttack = false;
                guardBT.isAttacking = true;
                guardBT.decision = "Attacking Enemy"; // Set Decision

                if (guardBT.hasWeapon)
                    guardBT.currentWeapon.Attack();
                else
                {
                    guardBT.agent.isStopped = true;
                    guardBT.defaultAttack.Attack();
                }

                animator.SetBool("Running", false);
                animator.SetBool("Walking", false);
            }

            state = NodeState.RUNNING;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
