using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject homepagePanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Scene Settings")]
    [SerializeField] private string tutorialSceneName = "TutorialView";

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private float fadeDuration = 1.5f;

    private bool isTransitioning;

    private void Awake()
    {
        if (blackScreen == null)
        {
            Debug.LogError("Black Screen Canvas Group is not assigned.");
            return;
        }

        blackScreen.gameObject.SetActive(true);
        blackScreen.alpha = 0f;
        blackScreen.interactable = false;
        blackScreen.blocksRaycasts = false;
    }

    public void PlayGame()
    {
        if (!isTransitioning)
        {
            StartCoroutine(FadeToTutorial());
        }
    }

    private IEnumerator FadeToTutorial()
    {
        isTransitioning = true;

        blackScreen.blocksRaycasts = true;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            blackScreen.alpha = Mathf.Lerp(
                0f,
                1f,
                timer / fadeDuration
            );

            yield return null;
        }

        blackScreen.alpha = 1f;

        yield return SceneManager.LoadSceneAsync(tutorialSceneName);
    }

    public void OpenSettings()
    {
        homepagePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OpenHomepage()
    {
        settingsPanel.SetActive(false);
        homepagePanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}