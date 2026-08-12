using UnityEngine;

public class HelmetController : MonoBehaviour
{
    [Header("Helmet")]
    [SerializeField] private GameObject helmet;

    [Header("Condition")]
    [SerializeField] private bool conditionMet = false;

    [Header("Objects Based On Condition")]
    [Tooltip("This object appears when the condition IS met.")]
    [SerializeField] private GameObject ifConditionMet;

    [Tooltip("This object appears when the condition is NOT met.")]
    [SerializeField] private GameObject ifConditionNotMet;

    private void Start()
    {
        UpdateHelmet();
    }

    private void UpdateHelmet()
    {
        // Helmet
        if (helmet != null)
        {
            helmet.SetActive(conditionMet);
        }

        // Object shown when condition IS met
        if (ifConditionMet != null)
        {
            ifConditionMet.SetActive(conditionMet);
        }

        // Object shown when condition is NOT met
        if (ifConditionNotMet != null)
        {
            ifConditionNotMet.SetActive(!conditionMet);
        }
    }

    // Call this from another script
    public void SetCondition(bool value)
    {
        conditionMet = value;
        UpdateHelmet();
    }

    public void ShowHelmet()
    {
        SetCondition(true);
    }

    public void HideHelmet()
    {
        SetCondition(false);
    }
}