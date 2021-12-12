using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckForItem : Node
{
    private Transform ownTransform;
    private Transform headTransform;
    private LayerMask itemLayer;
    private LayerMask obstructionLayer;
    private GuardBT guardBT;
    private float itemFindRange = 0;
    private float fovAngle = 90f;

    public CheckForItem(Transform _transform, Transform _headTransform, LayerMask _itemLayer, LayerMask _obstructionLayer, GuardBT _guardBT, float _itemFindRange)
    {
        ownTransform = _transform;
        headTransform = _headTransform;
        itemLayer = _itemLayer;
        obstructionLayer = _obstructionLayer;
        guardBT = _guardBT;
        itemFindRange = _itemFindRange;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("Pickupable");

        if (target == null)
        {
            Collider[] colliders = Physics.OverlapSphere(ownTransform.position, itemFindRange, itemLayer);

            float bestDistance = 99999.0f;
            Collider closestItem = null;

            if (colliders.Length > 0)
            {
                foreach (Collider item in colliders)
                {
                    float distance = Vector3.Distance(ownTransform.position, item.transform.position);

                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        closestItem = item;
                    }
                }

                if (closestItem.GetComponent<Pickupable>()?.owner == null)
                {
                    Vector3 directionToTarget = (colliders[0].transform.position - ownTransform.position).normalized;

                    if (Vector3.Angle(ownTransform.forward, directionToTarget) < fovAngle / 2)
                    {
                        float distanceToTarget = Vector3.Distance(ownTransform.position, colliders[0].transform.position);

                        if (!Physics.Raycast(headTransform.position, directionToTarget, distanceToTarget, obstructionLayer))
                        {
                            if (closestItem.GetComponent<Pickupable>()?.pickupType == pickupableTypes.Useable)
                            {
                                if (guardBT.hasWeapon || guardBT.foundWeapon)
                                {
                                    state = NodeState.SUCCESS;
                                    return state;
                                }
                            }
                            else if (closestItem.GetComponent<Pickupable>()?.pickupType == pickupableTypes.Useable)
                            {
                                if (guardBT.hasItem || guardBT.foundItem)
                                {
                                    state = NodeState.SUCCESS;
                                    return state;
                                }
                            }

                            if (closestItem.GetComponent<Pickupable>()?.pickupType == pickupableTypes.Weapon)
                            {
                                guardBT.foundWeapon = true;
                            }
                            else if (closestItem.GetComponent<Pickupable>()?.pickupType == pickupableTypes.Useable)
                            {
                                guardBT.foundItem = true;
                            }

                            closestItem.GetComponent<Pickupable>().owner = ownTransform.gameObject;
                            parent.parent.SetData("Pickupable", closestItem.transform);

                            state = NodeState.SUCCESS;
                            return state;
                        }
                    }
                }
            }
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
