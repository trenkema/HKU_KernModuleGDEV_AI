using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckSightBlocked : Node
{
    private Transform ownTransform;
    private Animator animator;
    private LayerMask blockedSightLayer;
    private GuardBT guardBT;

    public CheckSightBlocked(Transform _transform, Animator _animator, LayerMask _blockedSightLayer, GuardBT _guardBT)
    {
        ownTransform = _transform;
        animator = _animator;
        blockedSightLayer = _blockedSightLayer;
        guardBT = _guardBT;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("Target");

        if (target != null)
        {
            if (!target.gameObject.activeInHierarchy)
                ClearData("Target");
        }

        if (target != null && !guardBT.isPickingup)
        {
            Collider[] colliders = Physics.OverlapSphere(ownTransform.position, 1f, blockedSightLayer, QueryTriggerInteraction.Collide);

            if (colliders.Length > 0)
            {
                if (target.GetComponent<Player>())
                {
                    target.GetComponent<Player>().isBeingAttacked = false;
                }

                Debug.Log("IM BLINDED");
                animator.SetBool("Walking", false);
                animator.SetBool("Running", false);
                guardBT.isAttacking = false;
                guardBT.sightBlocked = true;
                guardBT.agent.isStopped = true;
                guardBT.decision = "Blinded"; // Set Decision
            }
            else
            {
                Debug.Log("IM NOT BLINDED");
                guardBT.sightBlocked = false;
                guardBT.agent.isStopped = false;

                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.RUNNING;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
