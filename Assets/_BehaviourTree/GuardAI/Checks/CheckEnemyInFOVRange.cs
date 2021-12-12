using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckEnemyInFOVRange : Node
{
    private Transform ownTransform;
    private Transform headTransform;
    private LayerMask enemyLayer;
    private LayerMask obstructionLayer;
    private GuardBT guardBT;
    private float fovAngle = 90f;

    public CheckEnemyInFOVRange(Transform _transform, Transform _headTransform, LayerMask _enemyLayer, LayerMask _obstructionLayer, GuardBT _guardBT)
    {
        ownTransform = _transform;
        headTransform = _headTransform;
        enemyLayer = _enemyLayer;
        obstructionLayer = _obstructionLayer;
        guardBT = _guardBT;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("Target");

        if (target == null)
        {
            Collider[] colliders = Physics.OverlapSphere(ownTransform.position, guardBT.fovRange, enemyLayer, QueryTriggerInteraction.Ignore);

            if (colliders.Length > 0)
            {
                Vector3 directionToTarget = (colliders[0].transform.position - ownTransform.position).normalized;

                if (Vector3.Angle(ownTransform.forward, directionToTarget) < fovAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(ownTransform.position, colliders[0].transform.position);

                    if (!Physics.Raycast(headTransform.position, directionToTarget, distanceToTarget, obstructionLayer))
                    {
                        parent.parent.SetData("Target", colliders[0].transform);
                        guardBT.target = colliders[0].gameObject;

                        state = NodeState.SUCCESS;
                        return state;
                    }
                    else
                    {
                        state = NodeState.FAILURE;
                        return state;
                    }
                }
                else
                {
                    state = NodeState.FAILURE;
                    return state;
                }
            }

            state = NodeState.FAILURE;
            return state;
        }
        else if (target.gameObject.activeInHierarchy)
        {
            Vector3 directionToTarget2 = (target.position - ownTransform.position).normalized;

            if (Vector3.Angle(ownTransform.forward, directionToTarget2) < fovAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(ownTransform.position, target.position);

                if (!Physics.Raycast(headTransform.position, directionToTarget2, distanceToTarget, obstructionLayer))
                {
                    Debug.Log("Target Visible");
                    state = NodeState.SUCCESS;
                    return state;
                }
                else
                {
                    Debug.Log("Target Not Visible");
                    ClearData("Target");
                    state = NodeState.FAILURE;
                    return state;
                }
            }
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
