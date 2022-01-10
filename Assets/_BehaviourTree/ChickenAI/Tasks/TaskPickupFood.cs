using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskPickupFood : Node
{
    private Animator animator;

    private ChickenBT chickenBT;

    private float pickUpTime = 1.2f;
    private float pickUpCounter = 0f;
    private bool pickingUp = false;

    public TaskPickupFood(Animator _animator, ChickenBT _chickenBT)
    {
        animator = _animator;
        chickenBT = _chickenBT;
    }

    public override NodeState Evaluate()
    {
        if (!chickenBT.agent.isOnNavMesh)
        {
            state = NodeState.FAILURE;
            return state;
        }

        Transform target = (Transform)GetData("Pickupable");

        if (target != null)
        {
            if (pickingUp)
            {
                chickenBT.resetWanderTimer = true;

                pickUpCounter += Time.deltaTime;

                if (pickUpCounter >= pickUpTime)
                {
                    pickUpCounter = 0;
                    pickingUp = false;
                    chickenBT.isPickingup = false;
                    chickenBT.foundFood = false;

                    target.gameObject.SetActive(false);

                    EventSystemNew<GameObject>.RaiseEvent(Event_Type.FRUIT_EATEN, target.gameObject);

                    if (target.GetComponent<Pickupable>() != null)
                    {
                        Pickupable pickupable = target.GetComponent<Pickupable>();

                        if (pickupable.pickupType == pickupableTypes.Useable)
                            chickenBT.needFood = false;

                        pickupable.owner = null;

                    }

                    ClearData("Pickupable");

                    state = NodeState.FAILURE;
                    return state;
                }

                state = NodeState.RUNNING;
                return state;
            }

            if (!chickenBT.isHiding && !pickingUp)
            {
                if (chickenBT.agent.remainingDistance < 0.2f)
                {
                    chickenBT.decision = "E"; // Set Decision
                    animator.SetTrigger("Eat");
                    animator.SetBool("Running", false);
                    animator.SetBool("Walking", false);
                    pickingUp = true;
                    chickenBT.isPickingup = true;
                }

                state = NodeState.RUNNING;
                return state;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}
