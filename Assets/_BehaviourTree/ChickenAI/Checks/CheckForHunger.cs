using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckForHunger : Node
{
    private ChickenBT chickenBT;

    private float needFoodCooldownTime = 20f;
    private float hungryDeathTime = 30f;

    private float needFoodTimer = 0f;
    private float hungryTimer = 0f;

    public CheckForHunger(ChickenBT _chickenBT, float _needFoodCooldown, float _hungryDeathTime)
    {
        chickenBT = _chickenBT;

        needFoodCooldownTime = _needFoodCooldown;

        hungryDeathTime = _hungryDeathTime;

        hungryTimer = hungryDeathTime;
    }

    public override NodeState Evaluate()
    {
        if (!chickenBT.hasStarted)
        {
            ClearData("Target");
            ClearData("Pickupable");

            chickenBT.isPickingup = false;
            chickenBT.isHiding = false;
            chickenBT.foundFood = false;

            state = NodeState.FAILURE;
            return state;
        }

        if (!chickenBT.needFood)
        {
            chickenBT.displayDecision.ColorFromGradient(1f);

            hungryTimer = hungryDeathTime;

            needFoodTimer += Time.deltaTime;

            if (needFoodTimer >= needFoodCooldownTime)
            {
                chickenBT.needFood = true;

                needFoodTimer = 0f;
            }
        }

        if (chickenBT.needFood)
        {
            hungryTimer -= Time.deltaTime;

            float gradientTime = hungryTimer / hungryDeathTime;

            Debug.Log("Gradient Time: " + gradientTime);

            chickenBT.displayDecision.ColorFromGradient(gradientTime);

            if (hungryTimer <= 0)
            {
                Transform pickupable = (Transform)GetData("Pickupable");

                if (pickupable != null)
                {
                    pickupable.GetComponent<Pickupable>().owner = null;
                    ClearData("Pickupable");
                    chickenBT.foundFood = false;
                }

                chickenBT.ChickenDied();
            }
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
