using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] FlockUnit flockUnityPrefab;
    [SerializeField] int flockSize;
    [SerializeField] Vector3 spawnBounds;

    [Header("Speed Settings")]
    [Range(0, 10)]
    [SerializeField] float _minSpeed;
    public float minSpeed { get { return _minSpeed; } }
    [Range(0, 10)]
    [SerializeField] float _maxSpeed;
    public float maxSpeed { get { return _maxSpeed; } }

    [Header("Detection Distances")]
    // Cohesion
    [Range(0, 10)]
    [SerializeField] float _cohesionDistance;
    public float cohesionDistance { get { return _cohesionDistance; } }

    // Avoidance
    [Range(0, 10)]
    [SerializeField] float _avoidanceDistance;
    public float avoidanceDistance { get { return _avoidanceDistance; } }

    // Alignment
    [Range(0, 10)]
    [SerializeField] float _alignmentDistance;
    public float alignmentDistance { get { return _alignmentDistance; } }

    // Obstacle
    [Range(0, 10)]
    [SerializeField] float _obstacleDistance;
    public float obstacleDistance { get { return _obstacleDistance; } }

    // Bounds
    [Range(0, 100)]
    [SerializeField] float _boundsDistance;
    public float boundsDistance { get { return _boundsDistance; } }

    [Header("Behaviour Weights")]
    // Cohesion
    [Range(0, 10)]
    [SerializeField] float _cohesionWeight;
    public float cohesionWeight { get { return _cohesionWeight; } }

    // Avoidance
    [Range(0, 10)]
    [SerializeField] float _avoidanceWeight;
    public float avoidanceWeight { get { return _avoidanceWeight; } }

    // Alignment
    [Range(0, 10)]
    [SerializeField] float _alignmentWeight;
    public float alignmentWeight { get { return _alignmentWeight; } }

    // Obstacle
    [Range(0, 100)]
    [SerializeField] float _obstacleWeight;
    public float obstacleWeight { get { return _obstacleWeight; } }

    // Bounds
    [Range(0, 10)]
    [SerializeField] float _boundsWeight;
    public float boundsWeight { get { return _boundsWeight; } }

    public List<FlockUnit> allUnits { get; private set; }

    private void Start()
    {
        GenerateUnits();
    }

    private void Update()
    {
        for (int i = 0; i < allUnits.Count; i++)
        {
            allUnits[i].MoveUnit();
        }
    }

    private void GenerateUnits()
    {
        allUnits = new List<FlockUnit>();

        for (int i = 0; i < flockSize; i++)
        {
            var randomVector = Random.insideUnitSphere;
            randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);
            var spawnPosition = transform.position + randomVector;
            var spawnRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            FlockUnit newFlock = Instantiate(flockUnityPrefab, spawnPosition, spawnRotation);
            allUnits.Add(newFlock);

            newFlock.AssignFlock(this);
            newFlock.InitializeSpeed(Random.Range(minSpeed, maxSpeed));
        }
    }
}
