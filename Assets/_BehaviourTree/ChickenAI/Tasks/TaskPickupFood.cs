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

    private int amountOfTimesEaten = 0;
    private int amountOfTimesRequiredToFeed;

    public TaskPickupFood(Animator _animator, ChickenBT _chickenBT, int _amountOfTimesRequiredToFeed)
    {
        animator = _animator;
        chickenBT = _chickenBT;
        amountOfTimesRequiredToFeed = _amountOfTimesRequiredToFeed;
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

                    if (target.GetComponent<Pickupable>()?.pickupType == pickupableTypes.Useable)
                    {
                        chickenBT.needFood = false;

                        amountOfTimesEaten++;

                        if (amountOfTimesEaten >= amountOfTimesRequiredToFeed)
                        {
                            chickenBT.beenFedCompletely = true;

                            chickenBT.displayDecision.ChangeColorState(2);

                            EventSystemNew.RaiseEvent(Event_Type.CHICKEN_SUCCESSFULLY_FED);
                        }
                        else
                        {
                            chickenBT.displayDecision.ChangeColorState(1);
                        }
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
                if (chickenBT.agent.remainingDistance < 0.4f)
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
