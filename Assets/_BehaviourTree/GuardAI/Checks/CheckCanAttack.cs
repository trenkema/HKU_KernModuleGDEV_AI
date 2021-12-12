using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckCanAttack : Node
{
    private GuardBT guardBT;

    private float currentAttackTime = 2f;
    private float attackTimeOffset = 1f;
    private float attackCooldownTimer = 2f;

    private float isAttackingTimer;

    public CheckCanAttack(GuardBT _guardBT)
    {
        guardBT = _guardBT;
    }

    public override NodeState Evaluate()
    {
        if (guardBT.currentWeapon != null)
        {
            float calculatedAttackTime = guardBT.currentWeapon.attackDuration + attackTimeOffset;

            if (calculatedAttackTime != currentAttackTime)
            {
                currentAttackTime = calculatedAttackTime;
            }
        }
        else
        {
            float calculatedAttackTime = guardBT.defaultAttackDuration + attackTimeOffset;

            if (calculatedAttackTime != currentAttackTime)
            {
                currentAttackTime = calculatedAttackTime;
            }
        }

        if (!guardBT.canAttack)
        {
            attackCooldownTimer += Time.deltaTime;

            if (attackCooldownTimer >= currentAttackTime)
            {
                attackCooldownTimer = 0;
                guardBT.canAttack = true;
            }
        }

        if (guardBT.isAttacking)
        {
            isAttackingTimer += Time.deltaTime;

            if (guardBT.currentWeapon != null)
            {
                if (isAttackingTimer >= guardBT.currentWeapon.attackDuration)
                {
                    isAttackingTimer = 0;
                    guardBT.isAttacking = false;
                    guardBT.agent.isStopped = false;
                }
            }
            else
            {
                if (isAttackingTimer >= guardBT.defaultAttackDuration)
                {
                    isAttackingTimer = 0;
                    guardBT.isAttacking = false;
                    guardBT.agent.isStopped = false;
                }
            }
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
