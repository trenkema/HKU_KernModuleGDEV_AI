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
    private float fovAngle = 90f;
    private float fovRange = 0f;

    public CheckEnemyInFOVRange(Transform _transform, Transform _headTransform, LayerMask _enemyLayer, LayerMask _obstructionLayer, float _fovRange)
    {
        ownTransform = _transform;
        headTransform = _headTransform;
        enemyLayer = _enemyLayer;
        obstructionLayer = _obstructionLayer;
        fovRange = _fovRange;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("Target");

        Debug.Log("Test");

        if (target == null)
        {
            Collider[] colliders = Physics.OverlapSphere(ownTransform.position, fovRange, enemyLayer, QueryTriggerInteraction.Ignore);

            if (colliders.Length > 0)
            {
                Vector3 directionToTarget = (colliders[0].transform.position - ownTransform.position).normalized;
                
                if (Vector3.Angle(ownTransform.forward, directionToTarget) < fovAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(ownTransform.position, colliders[0].transform.position);

                    if (!Physics.Raycast(headTransform.position, directionToTarget, distanceToTarget, obstructionLayer) && distanceToTarget < fovRange)
                    {
                        parent.parent.SetData("Target", colliders[0].transform);

                        state = NodeState.SUCCESS;
                        return state;
                    }
                }
            }
        }
        else if (target.gameObject.activeInHierarchy)
        {
            Vector3 directionToTarget2 = (target.position - ownTransform.position).normalized;

            if (Vector3.Angle(ownTransform.forward, directionToTarget2) < fovAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(ownTransform.position, target.position);

                if (!Physics.Raycast(headTransform.position, directionToTarget2, distanceToTarget, obstructionLayer) && distanceToTarget < fovRange)
                {
                    Debug.Log("Target Visible");
                    state = NodeState.SUCCESS;
                    return state;
                }
                else
                {
                    Debug.Log("Target Not Visible");
                    ClearData("Target");
                    state = NodeState.SUCCESS;
                    return state;
                }
            }
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
