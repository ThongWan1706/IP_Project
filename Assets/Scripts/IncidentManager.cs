using UnityEngine;

public class IncidentManager : MonoBehaviour
{
    [Header("Incidents In Order")]
    [SerializeField] private GameObject[] incidents;

    private int currentIncidentIndex = 0;

    private void Start()
    {
        // Turn all incidents off first
        for (int i = 0; i < incidents.Length; i++)
        {
            if (incidents[i] != null)
                incidents[i].SetActive(false);
        }

        // Start the first incident
        StartCurrentIncident();
    }

    private void StartCurrentIncident()
    {
        if (currentIncidentIndex >= incidents.Length)
        {
            Debug.Log("All incidents completed.");
            return;
        }

        if (incidents[currentIncidentIndex] != null)
        {
            incidents[currentIncidentIndex].SetActive(true);

            Debug.Log(
                "Starting incident " +
                (currentIncidentIndex + 1)
            );
        }
    }

    public void CompleteCurrentIncident()
    {
        if (currentIncidentIndex >= incidents.Length)
            return;

        // Turn off completed incident
        if (incidents[currentIncidentIndex] != null)
        {
            incidents[currentIncidentIndex].SetActive(false);
        }

        currentIncidentIndex++;

        // Start next incident
        StartCurrentIncident();
    }
}