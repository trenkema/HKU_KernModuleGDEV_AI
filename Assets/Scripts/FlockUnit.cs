using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockUnit : MonoBehaviour
{
    [SerializeField] float FOV;
    [SerializeField] float smoothDamp;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] Vector3[] directionsToCheckWhenAvoidingObstacles;

    List<FlockUnit> cohesionNeighbours = new List<FlockUnit>();
    List<FlockUnit> avoidanceNeighbours = new List<FlockUnit>();
    List<FlockUnit> alignmentNeighbours = new List<FlockUnit>();

    Flock assignedFlock;

    Vector3 currentVelocity;
    Vector3 currentObstacleAvoidanceVector;
    float speed;

    public Transform myTransform { get; private set; }

    private void Awake()
    {
        myTransform = transform;
    }

    public void AssignFlock(Flock _flock)
    {
        assignedFlock = _flock;
    }

    public void InitializeSpeed(float _speed)
    {
        speed = _speed;
    }

    public void MoveUnit()
    {
        FindNeighbours();
        CalculateSpeed();

        var cohesionVector = CalculateCohesionVector() * assignedFlock.cohesionWeight;
        var avoidanceVector = CalculateAvoidanceVector() * assignedFlock.avoidanceWeight;
        var alignmentVector = CalculateAlignmentVector() * assignedFlock.alignmentWeight;
        var boundsVector = CalculateBoundsVector() * assignedFlock.boundsWeight;
        var obstacleVector = CalculateObstacleVector() * assignedFlock.obstacleWeight;

        var moveVector = cohesionVector + avoidanceVector + alignmentVector + boundsVector + obstacleVector;
        moveVector = Vector3.SmoothDamp(myTransform.forward, moveVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized * speed;
        myTransform.forward = moveVector;
        myTransform.position += moveVector * Time.deltaTime;
    }

    private void CalculateSpeed()
    {
        if (cohesionNeighbours.Count == 0)
            return;

        speed = 0;

        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            speed += cohesionNeighbours[i].speed;
        }

        speed /= cohesionNeighbours.Count;
        speed = Mathf.Clamp(speed, assignedFlock.minSpeed, assignedFlock.maxSpeed);
    }

    private void FindNeighbours()
    {
        cohesionNeighbours.Clear();
        avoidanceNeighbours.Clear();
        alignmentNeighbours.Clear();

        var allUnits = assignedFlock.allUnits;

        for (int i = 0; i < allUnits.Count; i++)
        {
            var currentUnit = allUnits[i];

            if (currentUnit != this)
            {
                float currentNeighbourDistanceSqr = Vector3.SqrMagnitude(currentUnit.transform.position - myTransform.position);

                if (currentNeighbourDistanceSqr <= assignedFlock.cohesionDistance * assignedFlock.cohesionDistance)
                {
                    cohesionNeighbours.Add(currentUnit);
                }

                if (currentNeighbourDistanceSqr <= assignedFlock.avoidanceDistance * assignedFlock.avoidanceDistance)
                {
                    avoidanceNeighbours.Add(currentUnit);
                }

                if (currentNeighbourDistanceSqr <= assignedFlock.alignmentDistance * assignedFlock.alignmentDistance)
                {
                    alignmentNeighbours.Add(currentUnit);
                }
            }
        }
    }

    private Vector3 CalculateCohesionVector()
    {
        var cohesionVector = Vector3.zero;

        if (cohesionNeighbours.Count == 0)
            return cohesionVector;

        int neighboursInFOV = 0;

        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            if (IsInFOV(cohesionNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                cohesionVector += cohesionNeighbours[i].myTransform.position;
            }
        }

        cohesionVector /= neighboursInFOV;
        cohesionVector -= myTransform.position;
        cohesionVector = cohesionVector.normalized;

        return cohesionVector;
    }

    private Vector3 CalculateAvoidanceVector()
    {
        var avoidanceVector = Vector3.zero;

        if (avoidanceNeighbours.Count == 0)
            return Vector3.zero; ;

        int neighboursInFOV = 0;

        for (int i = 0; i < avoidanceNeighbours.Count; i++)
        {
            if (IsInFOV(avoidanceNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                avoidanceVector += (myTransform.position - avoidanceNeighbours[i].myTransform.position);
            }
        }

        avoidanceVector /= neighboursInFOV;
        avoidanceVector -= avoidanceVector.normalized;
        return avoidanceVector;
    }

    private Vector3 CalculateAlignmentVector()
    {
        var alignmentVector = myTransform.forward;

        if (alignmentNeighbours.Count == 0)
            return alignmentVector;

        int neighboursInFOV = 0;

        for (int i = 0; i < alignmentNeighbours.Count; i++)
        {
            if (IsInFOV(alignmentNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                alignmentVector += alignmentNeighbours[i].myTransform.forward;
            }
        }

        alignmentVector /= neighboursInFOV;
        alignmentVector -= alignmentVector.normalized;
        return alignmentVector;
    }

    private Vector3 CalculateBoundsVector()
    {
        var offsetToCenter = assignedFlock.transform.position - myTransform.position;
        bool isNearCenter = (offsetToCenter.magnitude >= assignedFlock.boundsDistance * 0.9);
        
        return isNearCenter ? offsetToCenter.normalized : Vector3.zero;
    }

    private Vector3 CalculateObstacleVector()
    {
        var obstacleVector = Vector3.zero;
        RaycastHit hit;

        if (Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.obstacleDistance, obstacleLayer))
        {
            obstacleVector = FindBestDirectionToAvoidObstacle();
        }
        else
        {
            currentObstacleAvoidanceVector = Vector3.zero;
        }

        return obstacleVector;
    }

    private Vector3 FindBestDirectionToAvoidObstacle()
    {
        if (currentObstacleAvoidanceVector != Vector3.zero)
        {
            RaycastHit hit;

            if (!Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.obstacleDistance, obstacleLayer))
            {
                return currentObstacleAvoidanceVector;
            }
        }

        float maxDistance = int.MinValue;
        var selectedDirection = Vector3.zero;

        for (int i = 0; i < directionsToCheckWhenAvoidingObstacles.Length; i++)
        {
            RaycastHit hit;
            var currentDirection = myTransform.TransformDirection(directionsToCheckWhenAvoidingObstacles[i].normalized);

            if (Physics.Raycast(myTransform.position, currentDirection, out hit, assignedFlock.obstacleDistance, obstacleLayer))
            {
                float currentDistance = (hit.point - myTransform.position).sqrMagnitude;

                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    selectedDirection = currentDirection;
                }
                else
                {
                    selectedDirection = currentDirection;
                    currentObstacleAvoidanceVector = currentDirection.normalized;
                    return selectedDirection.normalized;
                }
            }
        }

        return selectedDirection.normalized;
    }

    private bool IsInFOV(Vector3 _position)
    {
        return Vector3.Angle(myTransform.forward, _position - myTransform.position) <= FOV;
    }
}
