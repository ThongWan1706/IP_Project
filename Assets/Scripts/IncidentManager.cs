using UnityEngine;

public class IncidentManager : MonoBehaviour
{
    [Header("Incidents In Order")]
    [SerializeField] private GameObject[] incidents;

    private int currentIncidentIndex = 0;

    private void Start()
    {
        // Turn all incidents off
        for (int i = 0; i < incidents.Length; i++)
        {
            if (incidents[i] != null)
            {
                incidents[i].SetActive(false);
                Debug.Log("Disabled incident: " + incidents[i].name);
            }
        }

        StartCurrentIncident();
    }

    private void StartCurrentIncident()
    {
        if (currentIncidentIndex >= incidents.Length)
        {
            Debug.Log("All incidents completed.");
            return;
        }

        GameObject currentIncident = incidents[currentIncidentIndex];

        if (currentIncident != null)
        {
            currentIncident.SetActive(true);

            Debug.Log(
                "STARTING INCIDENT " +
                (currentIncidentIndex + 1) +
                ": " +
                currentIncident.name
            );
        }
    }

    public void CompleteCurrentIncident()
    {
        if (currentIncidentIndex >= incidents.Length)
            return;

        GameObject currentIncident = incidents[currentIncidentIndex];

        if (currentIncident != null)
        {
            Debug.Log(
                "COMPLETED INCIDENT " +
                (currentIncidentIndex + 1) +
                ": " +
                currentIncident.name
            );

            currentIncident.SetActive(false);
        }

        currentIncidentIndex++;

        StartCurrentIncident();
    }
}