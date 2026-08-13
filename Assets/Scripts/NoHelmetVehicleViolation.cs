using UnityEngine;
using UnityEngine.AI;

public class NoHelmetVehicleViolation : MonoBehaviour
{
    [Header("Vehicle References")]
    [Tooltip("The NavMeshAgent that moves the bike/vehicle.")]
    [SerializeField] private NavMeshAgent agent;

    [Tooltip("The normal vehicle movement script.")]
    [SerializeField] private CarNavMeshTraffic vehicleTraffic;

    [Tooltip("The red outline script. Put this on the rider if you only want the rider outlined.")]
    [SerializeField] private NPCViolationOutline violationOutline;

    [Header("Stop Window")]
    [Tooltip("How many seconds the player has to stop the rider after entering the trigger zone.")]
    [SerializeField] private float interactionTimeLimit = 4f;

    public bool WarningActive { get; private set; }
    public bool WasStopped { get; private set; }
    public bool FailedToStop { get; private set; }

    private float warningTimer;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (vehicleTraffic == null)
            vehicleTraffic = GetComponentInChildren<CarNavMeshTraffic>();

        if (violationOutline == null)
            violationOutline = GetComponentInChildren<NPCViolationOutline>();
    }

    private void Start()
    {
        // Make sure the rider starts without the red outline.
        if (violationOutline != null)
            violationOutline.StopViolationWarning();
    }

    private void Update()
    {
        if (!WarningActive || WasStopped || FailedToStop)
            return;

        warningTimer += Time.deltaTime;

        if (warningTimer >= interactionTimeLimit)
        {
            FailToStopInTime();
        }
    }

    public void BeginViolationWarning()
    {
        if (WarningActive || WasStopped || FailedToStop)
            return;

        WarningActive = true;
        warningTimer = 0f;

        // The bike keeps travelling normally while the warning is active.
        if (violationOutline != null)
            violationOutline.AboutToViolateRule();

        Debug.Log(gameObject.name +
                  " entered the no-helmet violation zone. Player can stop the rider now.");
    }

    public void StopVehicleInTime()
    {
        if (!WarningActive || WasStopped || FailedToStop)
            return;

        WasStopped = true;
        WarningActive = false;

        // Disable the normal traffic controller first, otherwise it may
        // set NavMeshAgent.isStopped back to false on the next frame.
        if (vehicleTraffic != null)
            vehicleTraffic.enabled = false;

        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        if (violationOutline != null)
            violationOutline.StopViolationWarning();

        Debug.Log(gameObject.name + " was stopped in time for riding without a helmet.");
    }

    private void FailToStopInTime()
    {
        FailedToStop = true;
        WarningActive = false;

        // The rider was missed, so remove the warning outline
        // and allow normal vehicle movement to continue.
        if (violationOutline != null)
            violationOutline.StopViolationWarning();

        Debug.Log(gameObject.name +
                  " was not stopped in time. The bike continues its normal route.");
    }
}