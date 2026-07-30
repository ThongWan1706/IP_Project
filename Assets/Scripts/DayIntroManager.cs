using System.Collections;
using UnityEngine;

public class DayIntroController : MonoBehaviour
{
    [Header("Day Intro UI")]
    [SerializeField] private GameObject dayIntroPanel;
    [SerializeField] private CanvasGroup blackScreen;

    [Header("Player Controls")]
    [Tooltip("Add the player movement and mouse-look scripts here.")]
    [SerializeField] private Behaviour[] playerControlScripts;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float dayIntroDuration = 2.5f;

    private void Awake()
    {
        // Show DAY 1 first
        if (dayIntroPanel != null)
        {
            dayIntroPanel.SetActive(true);
        }

        // Stop player movement, but keep the player camera active
        SetPlayerControls(false);

        if (blackScreen != null)
        {
            blackScreen.transform.SetAsLastSibling();
            blackScreen.gameObject.SetActive(true);
            blackScreen.alpha = 1f;
            blackScreen.interactable = false;
            blackScreen.blocksRaycasts = true;
        }
    }

    private IEnumerator Start()
    {
        if (blackScreen == null)
        {
            Debug.LogError(
                "DayIntroController: Black Screen is not assigned."
            );

            yield break;
        }

        // Reveal DAY 1
        yield return FadeBlackScreen(1f, 0f);

        // Keep DAY 1 visible
        yield return new WaitForSecondsRealtime(dayIntroDuration);

        // Fade DAY 1 back to black
        yield return FadeBlackScreen(0f, 1f);

        // Hide the DAY 1 panel
        if (dayIntroPanel != null)
        {
            dayIntroPanel.SetActive(false);
        }

        // Allow the player to move
        SetPlayerControls(true);

        yield return null;

        // Reveal gameplay
        yield return FadeBlackScreen(1f, 0f);

        blackScreen.blocksRaycasts = false;
        blackScreen.gameObject.SetActive(false);
    }

    private void SetPlayerControls(bool enabled)
    {
        foreach (Behaviour controlScript in playerControlScripts)
        {
            if (controlScript != null)
            {
                controlScript.enabled = enabled;
            }
        }
    }

    private IEnumerator FadeBlackScreen(
        float startAlpha,
        float endAlpha
    )
    {
        float timer = 0f;
        float duration = Mathf.Max(0.01f, fadeDuration);

        blackScreen.alpha = startAlpha;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            blackScreen.alpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                timer / duration
            );

            yield return null;
        }

        blackScreen.alpha = endAlpha;
    }
}