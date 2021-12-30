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
    }

    public override NodeState Evaluate()
    {
        if (!chickenBT.hasStarted)
        {
            ClearData("Target");
            ClearData("Pickupable");

            chickenBT.isPickingup = false;
            chickenBT.isEating = false;
            chickenBT.isHiding = false;
            chickenBT.foundFood = false;

            state = NodeState.FAILURE;
            return state;
        }

        if (!chickenBT.needFood)
        {
            needFoodTimer += Time.deltaTime;

            if (needFoodTimer >= needFoodCooldownTime)
            {
                chickenBT.needFood = true;
                hungryTimer = 0f;
                needFoodTimer = 0f;
            }
        }

        if (chickenBT.needFood && !chickenBT.beenFedCompletely)
        {
            hungryTimer += Time.deltaTime;

            if (hungryTimer >= hungryDeathTime)
            {
                chickenBT.ChickenDied();
            }
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
