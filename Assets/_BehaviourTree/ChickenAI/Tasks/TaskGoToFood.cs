using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;

public class TaskGoToFood : Node
{
    private Transform ownTransform;
    private Animator animator;
    private ChickenBT chickenBT;

    private float runSpeed = 0f;

    public TaskGoToFood(Transform _transform, Animator _animator, ChickenBT _chickenBT, float _runSpeed)
    {
        ownTransform = _transform;
        animator = _animator;
        chickenBT = _chickenBT;
        runSpeed = _runSpeed;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("Pickupable");

        if (target != null && !chickenBT.isHiding)
        {
            chickenBT.resetWanderTimer = true;

            if (target.GetComponent<Pickupable>()?.pickupType == pickupableTypes.Useable)
            {
                if (!chickenBT.needFood)
                {
                    ClearData("Pickupable");
                    chickenBT.foundFood = false;
                    state = NodeState.SUCCESS;
                    return state;
                }
            }

            if (chickenBT.agent.remainingDistance > 0.2f)
            {
                chickenBT.decision = "F"; // Set Decision
                animator.SetBool("Running", true);
                animator.SetBool("Walking", false);

                chickenBT.agent.speed = runSpeed;

                chickenBT.agent.SetDestination(target.position);

                if (chickenBT.agent.remainingDistance > 0.1f)
                {
                    Vector3 destination = new Vector3(chickenBT.agent.steeringTarget.x, ownTransform.position.y, chickenBT.agent.steeringTarget.z);
                    Quaternion targetRotation = Quaternion.LookRotation(destination - ownTransform.position);
                    ownTransform.rotation = Quaternion.Slerp(ownTransform.rotation, targetRotation, Time.deltaTime * 10);
                }

                state = NodeState.RUNNING;
                return state;
            }
        }
        else if (target != null)
        {
            target.GetComponent<Pickupable>().owner = null;
            ClearData("Pickupable");
            chickenBT.foundFood = false;
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
