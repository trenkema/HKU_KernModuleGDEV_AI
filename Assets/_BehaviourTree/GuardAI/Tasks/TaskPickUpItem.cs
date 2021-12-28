using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskPickUpItem : Node
{
    private Transform ownTransform;

    private Animator animator;

    private float pickUpTime = 4f;
    private float pickUpCounter = 0f;
    private bool pickingUp = false;

    private GuardBT guardBT;
    private AttackManager attackManager;

    public TaskPickUpItem(Transform _ownTransform, Animator _animator, GuardBT _guardBT, AttackManager _attackManager)
    {
        ownTransform = _ownTransform;
        animator = _animator;
        guardBT = _guardBT;
        attackManager = _attackManager;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("Pickupable");

        if (target != null && !guardBT.isAttacking)
        {
            if (target.GetComponent<Pickupable>()?.pickupType == pickupableTypes.Weapon)
            {
                if (guardBT.hasWeapon)
                {
                    ClearData("Pickupable");
                    guardBT.foundWeapon = false;
                    state = NodeState.SUCCESS;
                    return state;
                }
            }
            else if (target.GetComponent<Pickupable>()?.pickupType == pickupableTypes.Useable)
            {
                if (guardBT.hasItem)
                {
                    ClearData("Pickupable");
                    guardBT.foundItem = false;
                    state = NodeState.SUCCESS;
                    return state;
                }
            }

            if (pickingUp)
            {
                pickUpCounter += Time.deltaTime;

                if (pickUpCounter >= pickUpTime)
                {
                    pickUpCounter = 0;
                    pickingUp = false;
                    guardBT.isPickingup = false;

                    target.gameObject.SetActive(false);

                    if (target.GetComponent<Pickupable>()?.pickupType == pickupableTypes.Weapon)
                    {
                        guardBT.hasWeapon = true;
                        guardBT.weaponObjects[target.gameObject.GetComponent<Pickupable>().itemIndex].SetActive(true);
                        guardBT.currentWeapon = guardBT.weaponObjects[target.gameObject.GetComponent<Pickupable>().itemIndex].GetComponent<BaseWeapon>();
                        attackManager.currentWeapon = guardBT.currentWeapon;
                    }
                    else if (target.GetComponent<Pickupable>()?.pickupType == pickupableTypes.Useable)
                    {
                        guardBT.hasItem = true;
                        target.GetComponent<Pickupable>()?.Use();
                    }

                    ClearData("Pickupable");

                    state = NodeState.SUCCESS;
                    return state;
                }
            }

            if (Vector3.Distance(target.transform.position, ownTransform.position) < 1f && !pickingUp)
            {
                guardBT.decision = "Picking Up"; // Set Decision
                animator.SetTrigger("PickUp");
                animator.SetBool("Running", false);
                animator.SetBool("Walking", false);
                pickingUp = true;
                guardBT.isPickingup = true;
            }

            state = NodeState.RUNNING;
            return state;
        }

        Debug.Log("Test4");
        
        state = NodeState.SUCCESS;
        return state;
    }
}
