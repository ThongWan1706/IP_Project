using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Hazards Avoided")]
    [SerializeField] private TextMeshProUGUI hazardAvoidedText;

    [Header("Community Trust")]
    [SerializeField] private Image[] trustBars;

    [Header("Trust Bar Colours")]
    [SerializeField] private Color filledBarColour =
        new Color(0.2f, 0.8f, 0.3f, 1f);

    [SerializeField] private Color emptyBarColour =
        new Color(0.25f, 0.25f, 0.25f, 0.7f);

    public int HazardsAvoided
    {
        get
        {
            if (GameProgressManager.Instance != null)
            {
                return GameProgressManager.Instance.hazardsAvoided;
            }

            return 0;
        }
    }

    public int CommunityTrust
    {
        get
        {
            if (GameProgressManager.Instance != null)
            {
                return GameProgressManager.Instance.communityTrust;
            }

            return 0;
        }
    }

    private void Start()
    {
        UpdateHUD();
    }

    public void AddHazardAvoided(int amount = 1)
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogError(
                "GameProgressManager does not exist."
            );

            return;
        }

        GameProgressManager.Instance.AddHazardAvoided(amount);

        UpdateHazardText();
    }

    public void ChangeCommunityTrust(int amount)
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogError(
                "GameProgressManager does not exist."
            );

            return;
        }

        GameProgressManager.Instance.ChangeCommunityTrust(amount);

        UpdateTrustBars();
    }

    public void SetCommunityTrust(int value)
    {
        if (GameProgressManager.Instance == null)
        {
            return;
        }

        GameProgressManager.Instance.communityTrust =
            Mathf.Clamp(value, 0, trustBars.Length);

        UpdateTrustBars();
    }

    public void ApplyChoiceResult(
        int hazardPoints,
        int trustChange
    )
    {
        AddHazardAvoided(hazardPoints);
        ChangeCommunityTrust(trustChange);
    }

    private void UpdateHUD()
    {
        UpdateHazardText();
        UpdateTrustBars();
    }

    private void UpdateHazardText()
    {
        if (hazardAvoidedText != null &&
            GameProgressManager.Instance != null)
        {
            hazardAvoidedText.text =
                "Hazard Avoided: " +
                GameProgressManager.Instance.hazardsAvoided;
        }
    }

    private void UpdateTrustBars()
    {
        if (GameProgressManager.Instance == null ||
            trustBars == null)
        {
            return;
        }

        int trust =
            GameProgressManager.Instance.communityTrust;

        for (int i = 0; i < trustBars.Length; i++)
        {
            if (trustBars[i] == null)
            {
                continue;
            }

            if (i < trust)
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