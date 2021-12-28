using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskAttackEnemy : Node
{
    Transform ownTransform;
    Animator animator;
    AllyBT allyBT;
    Player master;

    public TaskAttackEnemy(Transform _transform, Animator _animator, AllyBT _allyBT, Player _master)
    {
        ownTransform = _transform;
        animator = _animator;
        allyBT = _allyBT;
        master = _master;
    }

    public override NodeState Evaluate()
    {
        if (!master.gameObject.activeInHierarchy || master.beingAttackedBy == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (master.isBeingAttacked)
        {
            if (allyBT.canAttack)
            {
                allyBT.grenadeManager.targetObject = master.beingAttackedBy;
                allyBT.grenadeManager.playerObject = ownTransform.gameObject;

                Debug.Log("Attacked");

                ownTransform.LookAt(master.beingAttackedBy.transform.position);

                allyBT.canAttack = false;
                allyBT.isAttacking = true;
                allyBT.decision = "Attacking"; // Set Decision

                animator.SetTrigger("AllyAttack");

                state = NodeState.RUNNING;
                return state;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}
