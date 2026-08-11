using System.Collections;
using UnityEngine;

public class DayIntroManager : MonoBehaviour
{
    [Header("Day Intro UI")]
    [SerializeField] private Canvas dayIntroCanvas;
    [SerializeField] private GameObject dayIntroPanel;
    [SerializeField] private CanvasGroup blackScreen;

    [Header("Player Controls")]
    [Tooltip("Add only movement or mouse-look scripts. Do not add the Camera.")]
    [SerializeField] private Behaviour[] playerControlScripts;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float dayIntroDuration = 2.5f;

    private void Awake()
    {
    Debug.Log("Day 1 intro started.");

    if (dayIntroCanvas != null)
    {
        dayIntroCanvas.gameObject.SetActive(true);
        dayIntroCanvas.overrideSorting = true;
        dayIntroCanvas.sortingOrder = 100;
    }
    else
    {
        Debug.LogError(
            "DayIntroController: Day Intro Canvas is missing."
        );
    }

    if (dayIntroPanel != null)
    {
        dayIntroPanel.SetActive(true);
    }
    else
    {
        Debug.LogError(
            "DayIntroController: Day Intro Panel is missing."
        );
    }

    SetPlayerControls(false);

    if (blackScreen != null)
    {
        blackScreen.gameObject.SetActive(true);
        blackScreen.transform.SetAsLastSibling();
        blackScreen.alpha = 1f;
        blackScreen.interactable = false;
        blackScreen.blocksRaycasts = true;
    }
    else
    {
        Debug.LogError(
            "DayIntroController: Black Screen is missing."
        );
    }
    }

    private IEnumerator Start()
    {
        if (blackScreen == null)
        {
            SetPlayerControls(true);
            yield break;
        }

        // Black -> DAY 1
        yield return FadeBlackScreen(1f, 0f);

        // Keep DAY 1 visible.
        yield return new WaitForSecondsRealtime(dayIntroDuration);

        // DAY 1 -> black
        yield return FadeBlackScreen(0f, 1f);

        if (dayIntroPanel != null)
        {
            dayIntroPanel.SetActive(false);
        }

        // Turn the player controls back on while the screen is black.
        SetPlayerControls(true);

        yield return null;

        // Black -> gameplay
        yield return FadeBlackScreen(1f, 0f);

        blackScreen.blocksRaycasts = false;
        blackScreen.gameObject.SetActive(false);

        Debug.Log("Day 1 intro completed.");
    }

    private void SetPlayerControls(bool controlsEnabled)
    {
        if (playerControlScripts == null)
        {
            return;
        }

        foreach (Behaviour controlScript in playerControlScripts)
        {
            if (controlScript != null)
            {
                controlScript.enabled = controlsEnabled;
            }
        }
    }

    private IEnumerator FadeBlackScreen(
        float startAlpha,
        float endAlpha)
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