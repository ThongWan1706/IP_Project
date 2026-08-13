using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialDialogue : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        [TextArea(3, 6)]
        public string sentence;

        [Header("Optional Gameplay Images")]
        public bool showImages;

        public Sprite gameplayImage1;
        public Sprite gameplayImage2;
        public Sprite gameplayImage3;
    }

    [Header("Conversation UI")]
    [SerializeField] private GameObject conversationBox;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Gameplay Image UI")]
    [SerializeField] private GameObject gameplayImageContainer;
    [SerializeField] private Image gameplayImageDisplay1;
    [SerializeField] private Image gameplayImageDisplay2;
    [SerializeField] private Image gameplayImageDisplay3;

    [Header("Gameplay Image Size")]
    [Tooltip("The maximum size used by each tutorial gameplay image.")]
    [SerializeField] private Vector2 gameplayImageSize = new Vector2(600f, 350f);

    [Header("Tutorial Content")]
    [SerializeField] private TutorialStep[] tutorialSteps;

    [Header("Ending Transition")]
    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private float fadeToBlackDuration = 1.5f;
    [SerializeField] private string nextSceneName = "GameScene";

    private int currentStepIndex;
    private bool tutorialFinished;
    private bool isTransitioning;

    private void Start()
    {
        if (conversationBox == null || dialogueText == null)
        {
            Debug.LogError(
                "TutorialDialogue: Conversation Box or Dialogue Text is missing."
            );
            return;
        }

        if (tutorialSteps == null || tutorialSteps.Length == 0)
        {
            Debug.LogError(
                "TutorialDialogue: No tutorial steps have been added."
            );
            return;
        }

        currentStepIndex = 0;
        tutorialFinished = false;
        isTransitioning = false;

        // Conversation box appears from the start
        conversationBox.SetActive(true);

        // Keep the gameplay image container from stretching the images.
        if (gameplayImageContainer != null)
        {
            gameplayImageContainer.transform.localScale = Vector3.one;
            gameplayImageContainer.SetActive(false);
        }

        ShowCurrentStep();
    }

    public void NextSentence()
    {
        if (tutorialFinished || isTransitioning)
        {
            return;
        }

        currentStepIndex++;

        if (currentStepIndex < tutorialSteps.Length)
        {
            ShowCurrentStep();
        }
        else
        {
            isTransitioning = true;
            StartCoroutine(FadeToBlackAndLoadScene());
        }
    }

    private void ShowCurrentStep()
    {
        TutorialStep currentStep = tutorialSteps[currentStepIndex];

        dialogueText.text = currentStep.sentence;

        bool hasImages =
            currentStep.showImages &&
            (
                currentStep.gameplayImage1 != null ||
                currentStep.gameplayImage2 != null ||
                currentStep.gameplayImage3 != null
            );

        if (hasImages)
        {
            SetImage(
                gameplayImageDisplay1,
                currentStep.gameplayImage1
            );

            SetImage(
                gameplayImageDisplay2,
                currentStep.gameplayImage2
            );

            SetImage(
                gameplayImageDisplay3,
                currentStep.gameplayImage3
            );

            if (gameplayImageContainer != null)
            {
                gameplayImageContainer.SetActive(true);
            }
        }
        else
        {
            if (gameplayImageContainer != null)
            {
                gameplayImageContainer.SetActive(false);
            }
        }
    }

    private void SetImage(Image imageDisplay, Sprite sprite)
    {
        if (imageDisplay == null)
        {
            return;
        }

        if (sprite != null)
        {
            imageDisplay.sprite = sprite;

            // Keep the image from being stretched or squeezed.
            imageDisplay.preserveAspect = true;

            RectTransform rect = imageDisplay.rectTransform;

            // UI images should be resized with Width/Height, not uneven Scale values.
            rect.localScale = Vector3.one;

            // Give every tutorial gameplay image the same container size.
            rect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                gameplayImageSize.x
            );

            rect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                gameplayImageSize.y
            );

            imageDisplay.gameObject.SetActive(true);
        }
        else
        {
            imageDisplay.gameObject.SetActive(false);
        }
    }

    private IEnumerator FadeToBlackAndLoadScene()
    {
        tutorialFinished = true;

        // Hide the tutorial UI before fading
        conversationBox.SetActive(false);

        if (gameplayImageContainer != null)
        {
            gameplayImageContainer.SetActive(false);
        }

        if (blackScreen == null)
        {
            Debug.LogError(
                "TutorialDialogue: Black Screen is not assigned."
            );

            SceneManager.LoadScene(nextSceneName);
            yield break;
        }

        // Make sure the black panel is above the conversation UI
        blackScreen.transform.SetAsLastSibling();

        blackScreen.gameObject.SetActive(true);
        blackScreen.alpha = 0f;
        blackScreen.interactable = false;
        blackScreen.blocksRaycasts = true;

        float timer = 0f;
        float duration = Mathf.Max(0.01f, fadeToBlackDuration);

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            blackScreen.alpha =
                Mathf.Clamp01(timer / duration);

            yield return null;
        }

        blackScreen.alpha = 1f;

        yield return SceneManager.LoadSceneAsync(nextSceneName);
    }
}