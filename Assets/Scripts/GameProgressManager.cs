using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    [Header("Current Progress")]
    public int hazardsAvoided = 0;
    public int communityTrust = 3;

    [Header("Trust Settings")]
    public int maxCommunityTrust = 5;

    private void Awake()
    {
        // If another GameProgressManager already exists,
        // destroy this duplicate.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep this object when changing scenes.
        DontDestroyOnLoad(gameObject);
    }

    public void AddHazardAvoided(int amount)
    {
        hazardsAvoided += amount;
    }

    public void ChangeCommunityTrust(int amount)
    {
        communityTrust = Mathf.Clamp(
            communityTrust + amount,
            0,
            maxCommunityTrust
        );
    }

    public void ResetProgress()
    {
        hazardsAvoided = 0;
        communityTrust = 3;
    }
}