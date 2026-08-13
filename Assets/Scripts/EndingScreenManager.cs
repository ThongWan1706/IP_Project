using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingScreenManager : MonoBehaviour
{
    [Header("Final Score UI")]
    [SerializeField] private TextMeshProUGUI hazardAvoidedText;
    [SerializeField] private TextMeshProUGUI communityTrustText;

    [Tooltip("Optional. Assign the 5 Community Trust bar Images from left to right.")]
    [SerializeField] private Image[] communityTrustBars;

    [SerializeField] private Color filledTrustColour = Color.green;
    [SerializeField] private Color emptyTrustColour = Color.white;

    [Header("Ending Result")]
    [Tooltip("Community Trust at or above this value uses the good-ending BGM.")]
    [SerializeField] private int goodEndingTrustThreshold = 3;

    [SerializeField] private TextMeshProUGUI endingResultText;
    [SerializeField] private string goodEndingMessage = "Good Ending";
    [SerializeField] private string badEndingMessage = "Bad Ending";

    [Header("Ending BGM")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioClip goodEndingBGM;
    [SerializeField] private AudioClip badEndingBGM;

    [Header("Replay")]
    [Tooltip("Exact scene name for Day 1.")]
    [SerializeField] private string day1SceneName = "Day1";

    [Header("Optional Fade")]
    [Tooltip("Optional full-screen black CanvasGroup used when replaying.")]
    [SerializeField] private CanvasGroup blackScreen;

    [SerializeField] private float fadeDuration = 1.0f;

    private bool isChangingScene = false;

    private void Start()
    {
        ShowFinalScore();
        PlayEndingBGM();

        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            blackScreen.alpha = 0f;
            blackScreen.interactable = false;
            blackScreen.blocksRaycasts = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ShowFinalScore()
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "EndingScreenManager: GameProgressManager.Instance was not found. " +
                "Start the game from Day 1 so the persistent score manager exists."
            );

            if (hazardAvoidedText != null)
                hazardAvoidedText.text = "Hazard Avoided: 0";

            if (communityTrustText != null)
                communityTrustText.text = "Community Trust: 0";

            UpdateTrustBars(0);
            return;
        }

        int hazards = GameProgressManager.Instance.hazardsAvoided;
        int trust = GameProgressManager.Instance.communityTrust;

        if (hazardAvoidedText != null)
        {
            hazardAvoidedText.text = "Hazard Avoided: " + hazards;
        }

        if (communityTrustText != null)
        {
            communityTrustText.text = "Community Trust: ";
        }

        UpdateTrustBars(trust);

        if (endingResultText != null)
        {
            endingResultText.text =
                trust >= goodEndingTrustThreshold
                ? goodEndingMessage
                : badEndingMessage;
        }
    }

    private void UpdateTrustBars(int trust)
    {
        if (communityTrustBars == null)
            return;

        for (int i = 0; i < communityTrustBars.Length; i++)
        {
            if (communityTrustBars[i] == null)
                continue;

            communityTrustBars[i].color =
                i < trust
                ? filledTrustColour
                : emptyTrustColour;
        }
    }

    private void PlayEndingBGM()
    {
        if (bgmAudioSource == null)
        {
            Debug.LogWarning(
                "EndingScreenManager: BGM AudioSource is not assigned."
            );
            return;
        }

        int trust = 0;

        if (GameProgressManager.Instance != null)
        {
            trust = GameProgressManager.Instance.communityTrust;
        }

        AudioClip selectedBGM =
            trust >= goodEndingTrustThreshold
            ? goodEndingBGM
            : badEndingBGM;

        if (selectedBGM == null)
        {
            Debug.LogWarning(
                "EndingScreenManager: The selected ending BGM is not assigned."
            );
            return;
        }

        bgmAudioSource.Stop();
        bgmAudioSource.clip = selectedBGM;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    // Connect the Replay button to this method.
    public void ReplayGame()
    {
        if (isChangingScene)
            return;

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.ResetProgress();
        }

        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop();
        }

        if (!Application.CanStreamedLevelBeLoaded(day1SceneName))
        {
            Debug.LogError(
                "EndingScreenManager: Scene '" + day1SceneName +
                "' cannot be loaded. Add it to the active Build Profile / Scene List " +
                "and make sure the name matches exactly."
            );
            return;
        }

        if (blackScreen != null)
        {
            StartCoroutine(FadeAndLoadDay1());
        }
        else
        {
            SceneManager.LoadScene(day1SceneName);
        }
    }

    private System.Collections.IEnumerator FadeAndLoadDay1()
    {
        isChangingScene = true;

        blackScreen.gameObject.SetActive(true);
        blackScreen.blocksRaycasts = true;
        blackScreen.interactable = true;

        float timer = 0f;
        float startAlpha = blackScreen.alpha;
        float duration = Mathf.Max(0.01f, fadeDuration);

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            blackScreen.alpha = Mathf.Lerp(
                startAlpha,
                1f,
                timer / duration
            );

            yield return null;
        }

        blackScreen.alpha = 1f;

        yield return SceneManager.LoadSceneAsync(day1SceneName);
    }

    // Connect the Quit button to this method.
    public void QuitGame()
    {
        Debug.Log("Quitting game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}