using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckCanAttackEnemy : Node
{
    private AllyBT allyBT;

    private float currentAttackTime = 2f;
    private float attackTimeOffset = 5f;
    private float attackCooldownTimer = 0f;

    private float isAttackingTimer;

    public CheckCanAttackEnemy(AllyBT _allyBT)
    {
        allyBT = _allyBT;
    }

    public override NodeState Evaluate()
    {
        float calculatedAttackTime = allyBT.attackDuration + attackTimeOffset;

        if (calculatedAttackTime != currentAttackTime)
        {
            currentAttackTime = calculatedAttackTime;
        }

        if (!allyBT.canAttack)
        {
            attackCooldownTimer += Time.deltaTime;

            if (attackCooldownTimer >= currentAttackTime)
            {
                attackCooldownTimer = 0;
                allyBT.canAttack = true;
            }
        }

        if (allyBT.isAttacking)
        {
            allyBT.decision = "Attacking";

            isAttackingTimer += Time.deltaTime;

            if (isAttackingTimer >= allyBT.attackDuration)
            {
                isAttackingTimer = 0;
                allyBT.isAttacking = false;
                allyBT.agent.isStopped = false;
            }
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
