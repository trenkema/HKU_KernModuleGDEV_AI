using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckMasterBeingAttacked : Node
{
    Player master;

    public CheckMasterBeingAttacked(Player _master)
    {
        master = _master;
    }

    public override NodeState Evaluate()
    {
        if (!master.gameObject.activeInHierarchy)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (master.isBeingAttacked)
        {
            Debug.Log("Master Being Attacked");
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
