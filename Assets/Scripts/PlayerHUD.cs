using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Hazards Avoided")]
    [SerializeField] private TextMeshProUGUI hazardAvoidedText;
    [SerializeField] private int startingHazardsAvoided = 0;

    [Header("Community Trust")]
    [SerializeField] private Image[] trustBars;

    [Range(0, 5)]
    [SerializeField] private int startingCommunityTrust = 3;

    [Header("Trust Bar Colours")]
    [SerializeField] private Color filledBarColour =
        new Color(0.2f, 0.8f, 0.3f, 1f);

    [SerializeField] private Color emptyBarColour =
        new Color(0.25f, 0.25f, 0.25f, 0.7f);

    private int hazardsAvoided;
    private int communityTrust;

    public int HazardsAvoided => hazardsAvoided;
    public int CommunityTrust => communityTrust;

    private void Awake()
    {
        hazardsAvoided = Mathf.Max(0, startingHazardsAvoided);

        communityTrust = Mathf.Clamp(
            startingCommunityTrust,
            0,
            trustBars.Length
        );

        UpdateHUD();
    }

    // Call this after the player successfully avoids a hazard
    // or completes a mission.
    public void AddHazardAvoided(int amount = 1)
    {
        if (amount <= 0)
        {
            return;
        }

        hazardsAvoided += amount;
        UpdateHazardText();
    }

    // Use a positive value to increase trust.
    // Use a negative value to decrease trust.
    public void ChangeCommunityTrust(int amount)
    {
        communityTrust = Mathf.Clamp(
            communityTrust + amount,
            0,
            trustBars.Length
        );

        UpdateTrustBars();
    }

    public void SetCommunityTrust(int value)
    {
        communityTrust = Mathf.Clamp(
            value,
            0,
            trustBars.Length
        );

        UpdateTrustBars();
    }

    private void UpdateHUD()
    {
        UpdateHazardText();
        UpdateTrustBars();
    }

    private void UpdateHazardText()
    {
        if (hazardAvoidedText != null)
        {
            hazardAvoidedText.text =
                "Hazard Avoided: " + hazardsAvoided;
        }
    }

    private void UpdateTrustBars()
    {
        for (int i = 0; i < trustBars.Length; i++)
        {
            if (trustBars[i] == null)
            {
                continue;
            }

            if (i < communityTrust)
            {
                trustBars[i].color = filledBarColour;
            }
            else
            {
                trustBars[i].color = emptyBarColour;
            }
        }
    }
}