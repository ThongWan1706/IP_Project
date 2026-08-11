using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AccidentSequenceController : MonoBehaviour
{
    [Header("Crash Car")]
    [Tooltip("The car that will drive into the pedestrian.")]
    public Transform crashCar;

    [Tooltip("Where the car should drive to before the impact happens.")]
    public Transform impactPoint;

    [Tooltip("Optional point the car continues toward after the impact.")]
    public Transform carEndPoint;

    public float carSpeed = 8f;
    public float impactDistance = 0.8f;

    [Header("Crash Audio")]
    public AudioSource crashAudioSource;
    public AudioClip crashSound;

    [Header("Victim")]
    [Tooltip("The jaywalking NPC that gets hit.")]
    public GameObject victim;

    [Tooltip("Rigidbody on the ROOT of the victim.")]
    public Rigidbody victimRigidbody;

    [Tooltip("Animator on the victim.")]
    public Animator victimAnimator;

    [Tooltip("Optional NavMeshAgent on the victim.")]
    public NavMeshAgent victimAgent;

    [Header("Flying Effect")]
    [Tooltip("Forward force from the car.")]
    public float flyForce = 12f;

    [Tooltip("Upward force so the victim flies into the air.")]
    public float upwardForce = 7f;

    [Tooltip("How much the victim spins after impact.")]
    public float spinForce = 7f;

    [Header("Replay Panel")]
    public GameObject replayPanel;

    [Tooltip("How many seconds after the impact before the replay panel appears.")]
    public float replayPanelDelay = 2.5f;

    [Header("Scenes")]
    [Tooltip("The gameplay scene containing the jaywalking attempt.")]
    public string restartSceneName = "Day1";

    private bool impactTriggered = false;
    private bool sequenceFinished = false;

    private void Start()
    {
        if (replayPanel != null)
        {
            replayPanel.SetActive(false);
        }

        // Keep the victim controlled by animation until impact.
        if (victimRigidbody != null)
        {
            victimRigidbody.isKinematic = true;
            victimRigidbody.useGravity = false;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (sequenceFinished)
            return;

        if (!impactTriggered)
        {
            MoveCarTowardImpact();
        }
        else
        {
            MoveCarAfterImpact();
        }
    }

    private void MoveCarTowardImpact()
    {
        if (crashCar == null || impactPoint == null)
            return;

        Vector3 direction = impactPoint.position - crashCar.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            crashCar.rotation = Quaternion.Slerp(
                crashCar.rotation,
                Quaternion.LookRotation(direction.normalized),
                8f * Time.deltaTime
            );
        }

        crashCar.position = Vector3.MoveTowards(
            crashCar.position,
            impactPoint.position,
            carSpeed * Time.deltaTime
        );

        float distance =
            Vector3.Distance(
                crashCar.position,
                impactPoint.position
            );

        if (distance <= impactDistance)
        {
            TriggerImpact();
        }
    }

    private void TriggerImpact()
    {
        if (impactTriggered)
            return;

        impactTriggered = true;

        // Play car crash sound
        if (crashAudioSource != null && crashSound != null)
        {
            crashAudioSource.PlayOneShot(crashSound);
        }

        // Stop all normal NPC movement/animation control.
        if (victimAgent != null)
        {
            if (victimAgent.isOnNavMesh)
            {
                victimAgent.isStopped = true;
                victimAgent.ResetPath();
            }

            victimAgent.enabled = false;
        }

        if (victimAnimator != null)
        {
            victimAnimator.enabled = false;
        }

        // Turn the NPC into a physics object.
        if (victimRigidbody != null)
        {
            victimRigidbody.isKinematic = false;
            victimRigidbody.useGravity = true;

            Vector3 launchDirection =
                crashCar != null
                ? crashCar.forward
                : Vector3.forward;

            Vector3 launchForce =
                launchDirection * flyForce +
                Vector3.up * upwardForce;

            victimRigidbody.AddForce(
                launchForce,
                ForceMode.Impulse
            );

            victimRigidbody.AddTorque(
                new Vector3(
                    spinForce,
                    spinForce * 0.5f,
                    spinForce
                ),
                ForceMode.Impulse
            );
        }

        StartCoroutine(ShowReplayPanelAfterDelay());
    }

    private void MoveCarAfterImpact()
    {
        if (crashCar == null || carEndPoint == null)
            return;

        crashCar.position = Vector3.MoveTowards(
            crashCar.position,
            carEndPoint.position,
            carSpeed * Time.deltaTime
        );
    }

    private IEnumerator ShowReplayPanelAfterDelay()
    {
        yield return new WaitForSeconds(replayPanelDelay);

        sequenceFinished = true;

        if (replayPanel != null)
        {
            replayPanel.SetActive(true);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Connect the Restart button to this.
    public void RestartJaywalkScene()
    {
        Time.timeScale = 1f;

        if (string.IsNullOrWhiteSpace(restartSceneName))
        {
            Debug.LogError("Restart Scene Name is empty.");
            return;
        }

        SceneManager.LoadScene(restartSceneName);
    }

    // Connect the Quit button to this.
    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}