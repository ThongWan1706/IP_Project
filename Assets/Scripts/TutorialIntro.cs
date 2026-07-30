using System.Collections;
using UnityEngine;

public class TutorialIntro : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private GameObject tutorialPanel;

    [Header("Fade Settings")]
    [SerializeField] private float blackScreenDuration = 1f;
    [SerializeField] private float fadeDuration = 1.5f;

    private void Awake()
    {
        if (blackScreen == null)
        {
            Debug.LogError("TutorialIntro: Black Screen is not assigned.");
            return;
        }

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        // Put the black screen above every other UI object
        blackScreen.transform.SetAsLastSibling();

        // Start fully black before the first frame appears
        blackScreen.gameObject.SetActive(true);
        blackScreen.alpha = 1f;
        blackScreen.interactable = false;
        blackScreen.blocksRaycasts = true;
    }

    private IEnumerator Start()
    {
        if (blackScreen == null)
        {
            yield break;
        }

        // Remain black briefly
        yield return new WaitForSecondsRealtime(blackScreenDuration);

        float timer = 0f;
        float duration = Mathf.Max(0.01f, fadeDuration);

        // Fade from black to the tutorial
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            blackScreen.alpha =
                1f - Mathf.Clamp01(timer / duration);

            yield return null;
        }

        blackScreen.alpha = 0f;
        blackScreen.blocksRaycasts = false;
        blackScreen.gameObject.SetActive(false);
    }
}