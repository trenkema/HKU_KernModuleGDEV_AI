using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckEnemyInAttackRange : Node
{
    private Transform ownTransform;
    private GuardBT guardBT;

    public CheckEnemyInAttackRange(Transform _transform, GuardBT _guardBT)
    {
        ownTransform = _transform;
        guardBT = _guardBT;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("Target");

        if (target == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (guardBT.currentWeapon != null)
        {
            if (Vector3.Distance(ownTransform.position, target.position) <= guardBT.currentWeapon.attackRange)
            {
                state = NodeState.SUCCESS;
                return state;
            }
        }
        else
        {
            if (Vector3.Distance(ownTransform.position, target.position) <= guardBT.defaultAttackRange)
            {
                state = NodeState.SUCCESS;
                return state;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}
