using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskPatrol : Node
{
    private Transform ownTransform;
    private Animator animator;
    private Transform[] wayPoints;

    private int currentWaypointIndex = 0;

    private float waitTime = 1f;
    private float waitCounter = 0f;
    private bool waiting = false;

    private GuardBT guardBT;

    public TaskPatrol(Transform _transform, Animator _animator, Transform[] _wayPoints, GuardBT _guardBT)
    {
        ownTransform = _transform;
        animator = _animator;
        wayPoints = _wayPoints;
        guardBT = _guardBT;
    }

    public override NodeState Evaluate()
    {
        if (!guardBT.isAttacking || guardBT.currentWeapon != null)
        {
            if (guardBT.target != null)
            {
                if (guardBT.target.GetComponent<Player>())
                {
                    guardBT.target.GetComponent<Player>().beingAttackedBy = null;
                    guardBT.target.GetComponent<Player>().isBeingAttacked = false;
                    guardBT.target = null;
                }
            }

            if (waiting)
            {
                guardBT.agent.speed = guardBT.walkSpeed;
                guardBT.agent.SetDestination(ownTransform.position);

                guardBT.currentDecision = "Waiting"; // Set Decision

                waitCounter += Time.deltaTime;

                if (waitCounter >= waitTime)
                {
                    waiting = false;
                    animator.SetBool("Walking", true);
                    animator.SetBool("Running", false);
                }
            }
            else
            {
                guardBT.agent.stoppingDistance = 0;
                guardBT.agent.isStopped = false;

                Transform wp = wayPoints[currentWaypointIndex];

                if (Vector3.Distance(ownTransform.position, wp.position) < 0.01f)
                {
                    waitCounter = 0f;
                    waiting = true;

                    currentWaypointIndex = (currentWaypointIndex + 1) % wayPoints.Length;
                    animator.SetBool("Walking", false);
                }
                else
                {
                    guardBT.currentDecision = "Patrolling"; // Set Decision

                    guardBT.agent.speed = guardBT.walkSpeed;
                    guardBT.agent.SetDestination(wp.position);

                    animator.SetBool("Walking", true);
                    animator.SetBool("Running", false);
                }
            }

            if (guardBT.agent.remainingDistance > 0.1f)
            {
                Vector3 destination = new Vector3(guardBT.agent.steeringTarget.x, ownTransform.position.y, guardBT.agent.steeringTarget.z);
                Quaternion targetRotation = Quaternion.LookRotation(destination - ownTransform.position);
                ownTransform.rotation = Quaternion.Slerp(ownTransform.rotation, targetRotation, Time.deltaTime * 10);
            }

            state = NodeState.RUNNING;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
