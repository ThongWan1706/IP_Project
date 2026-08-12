using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CarNavMeshTraffic : MonoBehaviour
{
    [Header("NavMesh")]
    public NavMeshAgent agent;

    [Header("Traffic Light")]
    public Light greenLight;

    [Header("Stop Line")]
    public Transform stopPoint;
    public float stopCheckDistance = 2.5f;

    [Header("Bus Stop")]
    public bool isBus = false;

    [Tooltip("Only assign this if the vehicle is a bus.")]
    public Transform busStopPoint;

    public float busStopDuration = 10f;
    public float busStopReachDistance = 1f;

    [Header("Random Direction")]
    public Transform leftDestination;
    public Transform straightDestination;

    [Header("Respawn")]
    public Transform respawnPoint;
    public float respawnDelay = 1f;

    [Header("Car Settings")]
    public float speed = 5f;
    public float acceleration = 8f;
    public float angularSpeed = 120f;

    [Header("Vehicle Detection")]
    public LayerMask vehicleLayer;

    [Tooltip("Optional. Recommended for buses. Place at the front bumper.")]
    public Transform frontSensorPoint;

    public float detectionDistance = 5f;

    public Vector3 detectionBoxHalfSize =
        new Vector3(0.8f, 0.6f, 0.5f);

    private Vector3 startPosition;
    private Quaternion startRotation;

    private bool passedTrafficLight = false;
    private bool directionChosen = false;
    private bool goingToFinalDestination = false;
    private bool isRespawning = false;

    // Bus states
    private bool goingToBusStop = false;
    private bool waitingAtBusStop = false;
    private bool busStopFinished = false;

    // Reasons for stopping
    private bool stopForTrafficLight = false;
    private bool stopForCar = false;
    private bool stopForBusStop = false;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError(gameObject.name + " has no NavMeshAgent!");
            enabled = false;
            return;
        }

        agent.speed = speed;
        agent.acceleration = acceleration;
        agent.angularSpeed = angularSpeed;

        // Save respawn location
        if (respawnPoint != null)
        {
            startPosition = respawnPoint.position;
            startRotation = respawnPoint.rotation;
        }
        else
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(gameObject.name + " is NOT on the NavMesh!");
            return;
        }

        StartNewLoop();
    }

    void Update()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        if (isRespawning)
            return;

        // Check for vehicles in front
        CheckForCarInFront();

        // Bus is travelling toward bus stop
        if (isBus && goingToBusStop)
        {
            CheckBusStop();
        }

        // Before crossing traffic light
        if (!passedTrafficLight)
        {
            CheckTrafficLight();
        }

        // Check if final destination was reached
        if (goingToFinalDestination)
        {
            CheckReachedDestination();
        }

        UpdateCarStopState();
    }

    // Check if another vehicle is in front
    void CheckForCarInFront()
    {
        Vector3 detectionCenter;
        Quaternion detectionRotation;

        // Use FrontSensorPoint if one has been assigned
        if (frontSensorPoint != null)
        {
            detectionCenter =
                frontSensorPoint.position +
                frontSensorPoint.forward *
                (detectionDistance * 0.5f);

            detectionRotation =
                frontSensorPoint.rotation;
        }
        else
        {
            // Normal cars can continue using their existing setup
            detectionCenter =
                transform.position +
                transform.forward *
                (detectionDistance * 0.5f + 1f) +
                transform.up * 0.6f;

            detectionRotation =
                transform.rotation;
        }

        Vector3 halfSize = new Vector3(
            detectionBoxHalfSize.x,
            detectionBoxHalfSize.y,
            detectionDistance * 0.5f
        );

        Collider[] hits = Physics.OverlapBox(
            detectionCenter,
            halfSize,
            detectionRotation,
            vehicleLayer,
            QueryTriggerInteraction.Ignore
        );

        stopForCar = false;

        foreach (Collider hit in hits)
        {
            if (hit == null)
                continue;

            // Ignore collider on this object
            if (hit.transform == transform)
                continue;

            // Ignore colliders belonging to children of this vehicle
            if (hit.transform.IsChildOf(transform))
                continue;

            // Another vehicle was detected
            stopForCar = true;
            break;
        }
    }

    // Check traffic light
    void CheckTrafficLight()
    {
        if (stopPoint == null)
            return;

        float distanceToStop =
            Vector3.Distance(
                transform.position,
                stopPoint.position
            );

        if (distanceToStop > stopCheckDistance)
            return;

        bool isGreen =
            greenLight != null &&
            greenLight.enabled;

        // Red or yellow
        if (!isGreen)
        {
            stopForTrafficLight = true;
            return;
        }

        // Green
        stopForTrafficLight = false;

        // Still wait if another vehicle is in front
        if (stopForCar)
            return;

        if (directionChosen)
            return;

        directionChosen = true;
        passedTrafficLight = true;

        // Bus always continues straight toward bus stop
        if (isBus)
        {
            if (busStopPoint != null)
            {
                goingToBusStop = true;

                agent.SetDestination(
                    busStopPoint.position
                );

                Debug.Log(
                    gameObject.name +
                    " passed green light and is going to bus stop."
                );
            }
            else
            {
                GoStraight();
            }
        }
        else
        {
            // Normal cars choose randomly
            ChooseRandomDirection();
        }
    }

    // Check if bus reached bus stop
    void CheckBusStop()
    {
        if (busStopPoint == null)
            return;

        if (agent.pathPending)
            return;

        float distanceToBusStop =
            Vector3.Distance(
                transform.position,
                busStopPoint.position
            );

        if (distanceToBusStop <= busStopReachDistance &&
            !waitingAtBusStop &&
            !busStopFinished)
        {
            StartCoroutine(
                WaitAtBusStop()
            );
        }
    }

    // Wait at bus stop then continue straight
    IEnumerator WaitAtBusStop()
    {
        waitingAtBusStop = true;
        stopForBusStop = true;

        UpdateCarStopState();

        Debug.Log(
            gameObject.name +
            " stopped at the bus stop."
        );

        yield return new WaitForSeconds(
            busStopDuration
        );

        stopForBusStop = false;
        waitingAtBusStop = false;
        busStopFinished = true;
        goingToBusStop = false;

        Debug.Log(
            gameObject.name +
            " is leaving the bus stop."
        );

        // Continue to end of tunnel
        GoStraight();

        UpdateCarStopState();
    }

    // Bus continues straight
    void GoStraight()
    {
        if (straightDestination == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " has no Straight Destination!"
            );

            return;
        }

        agent.SetDestination(
            straightDestination.position
        );

        goingToFinalDestination = true;
    }

    // Decide whether vehicle should stop
    void UpdateCarStopState()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        agent.isStopped =
            stopForTrafficLight ||
            stopForCar ||
            stopForBusStop;
    }

    // Normal cars choose left, straight or right
    void ChooseRandomDirection()
    {
        Transform[] possibleDirections =
        {
            leftDestination,
            straightDestination,
        };

        for (int i = 0; i < 10; i++)
        {
            int randomDirection =
                Random.Range(
                    0,
                    possibleDirections.Length
                );

            Transform chosenDestination =
                possibleDirections[randomDirection];

            if (chosenDestination != null)
            {
                agent.SetDestination(
                    chosenDestination.position
                );

                goingToFinalDestination = true;

                if (chosenDestination == leftDestination)
                {
                    Debug.Log(
                        gameObject.name +
                        " chose LEFT"
                    );
                }
                else if (chosenDestination == straightDestination)
                {
                    Debug.Log(
                        gameObject.name +
                        " chose STRAIGHT"
                    );
                }
                return;
            }
        }

        Debug.LogWarning(
            gameObject.name +
            " has no valid destination assigned!"
        );
    }

    // Check if final destination was reached
    void CheckReachedDestination()
    {
        if (agent.pathPending)
            return;

        if (agent.remainingDistance <=
            agent.stoppingDistance + 0.3f)
        {
            if (!agent.hasPath ||
                agent.velocity.sqrMagnitude < 0.01f)
            {
                goingToFinalDestination = false;

                StartCoroutine(
                    RespawnCar()
                );
            }
        }
    }

    // Respawn the vehicle
    IEnumerator RespawnCar()
    {
        isRespawning = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(
            respawnDelay
        );

        agent.ResetPath();

        bool warped =
            agent.Warp(startPosition);

        if (!warped)
        {
            Debug.LogError(
                gameObject.name +
                " could not respawn because the respawn point is not on the NavMesh!"
            );

            isRespawning = false;
            yield break;
        }

        transform.rotation =
            startRotation;

        passedTrafficLight = false;
        directionChosen = false;
        goingToFinalDestination = false;

        goingToBusStop = false;
        waitingAtBusStop = false;
        busStopFinished = false;

        stopForTrafficLight = false;
        stopForBusStop = false;

        agent.updateRotation = true;

        // Check immediately after respawning
        CheckForCarInFront();

        isRespawning = false;

        StartNewLoop();
    }

    // Start route again
    void StartNewLoop()
    {
        passedTrafficLight = false;
        directionChosen = false;
        goingToFinalDestination = false;

        goingToBusStop = false;
        waitingAtBusStop = false;
        busStopFinished = false;

        stopForTrafficLight = false;
        stopForBusStop = false;

        agent.updateRotation = true;

        // Check for car already in front
        CheckForCarInFront();

        // Every vehicle starts by going toward traffic light
        if (stopPoint != null)
        {
            agent.SetDestination(
                stopPoint.position
            );
        }

        UpdateCarStopState();
    }

    // Show front detection box
    void OnDrawGizmosSelected()
    {
        Vector3 detectionCenter;
        Quaternion detectionRotation;

        if (frontSensorPoint != null)
        {
            detectionCenter =
                frontSensorPoint.position +
                frontSensorPoint.forward *
                (detectionDistance * 0.5f);

            detectionRotation =
                frontSensorPoint.rotation;
        }
        else
        {
            detectionCenter =
                transform.position +
                transform.forward *
                (detectionDistance * 0.5f + 1f) +
                transform.up * 0.6f;

            detectionRotation =
                transform.rotation;
        }

        Vector3 boxSize = new Vector3(
            detectionBoxHalfSize.x * 2f,
            detectionBoxHalfSize.y * 2f,
            detectionDistance
        );

        Gizmos.color =
            Color.yellow;

        Gizmos.matrix =
            Matrix4x4.TRS(
                detectionCenter,
                detectionRotation,
                Vector3.one
            );

        Gizmos.DrawWireCube(
            Vector3.zero,
            boxSize
        );

        Gizmos.matrix =
            Matrix4x4.identity;
    }
}