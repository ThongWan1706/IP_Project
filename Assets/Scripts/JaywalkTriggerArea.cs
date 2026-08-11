using UnityEngine;

[RequireComponent(typeof(Collider))]
public class JaywalkTriggerArea : MonoBehaviour
{
    [Header("Jaywalking Sound")]
    public AudioSource audioSource;
    public AudioClip warningSound;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        JaywalkingNPCController jaywalker =
            other.GetComponentInParent<JaywalkingNPCController>();

        if (jaywalker == null)
            return;

        // Remember this trigger so the NPC controller can
        // stop the sound later.
        jaywalker.SetJaywalkTrigger(this);

        // Start jaywalking warning.
        jaywalker.BeginJaywalkWarning();

        // Play the warning sound.
        if (audioSource != null && warningSound != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(warningSound);
        }
    }

    public void StopWarningSound()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}