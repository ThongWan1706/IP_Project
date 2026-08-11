using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCChoiceInteraction : MonoBehaviour
{
    private enum ConversationStage
    {
        None,
        Introduction,
        Result
    }

    public enum Speaker
    {
        Resident,
        PoliceOfficer
    }

    [Serializable]
    public class DialogueLine
    {
        public Speaker speaker;

        [TextArea(2, 5)]
        public string sentence;
    }

    [Header("Interaction Type")]
    [SerializeField] private bool isPhone = false;
    [SerializeField] private bool transitionAfterConversation = true;

    [Header("NPC Highlight")]
    [SerializeField] private GameObject outlineVisual;

    [Header("Objects To Destroy After Dialogue")]
    [SerializeField] private GameObject[] objectsToDestroy;

    [Header("NPC Movement")]
    [SerializeField] private NPCNavMeshWalk npcMovement;

    [Header("Interaction Sound")]
    [SerializeField] private AudioSource interactionAudioSource;
    [SerializeField] private AudioClip policeWhistleSound;

    [Header("Player UI")]
    [SerializeField] private GameObject defaultHUDPanel;
    [SerializeField] private GameObject conversationPanel;
    [SerializeField] private GameObject optionPanel;

    [Header("Conversation Text")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI conversationText;

    [Header("Speaker Names")]
    [SerializeField] private string residentName = "Resident";
    [SerializeField] private string policeName = "Police Officer";

    [Header("Speaker Name Colours")]
    [SerializeField] private Color residentNameColour = Color.yellow;
    [SerializeField] private Color policeNameColour = Color.cyan;

    [Header("Opening Conversation")]
    [SerializeField] private DialogueLine[] openingDialogue;

    [Header("Option 1 Response")]
    [SerializeField] private DialogueLine[] option1Response;

    [Header("Option 2 Response")]
    [SerializeField] private DialogueLine[] option2Response;

    [Header("Player HUD")]
    [SerializeField] private PlayerHUD playerHUD;

    [Header("Point Sounds")]
    [SerializeField] private AudioSource pointAudioSource;

    [SerializeField] private AudioClip pointIncreaseSound;
    [SerializeField] private AudioClip pointDecreaseSound;

    [Header("Player Controls")]
    [Tooltip("Assign the First Person Controller component.")]
    [SerializeField] private Behaviour[] playerControlScripts;

    [Header("Scene Transition")]
    [Tooltip("Assign a full-screen black Image that has a CanvasGroup.")]
    [SerializeField] private CanvasGroup blackScreen;

    [Tooltip("How long the screen takes to fade to black.")]
    [SerializeField] private float fadeDuration = 1.5f;

    [Tooltip("Scene loaded after Option 1 or Option 2.")]
    [SerializeField] private string nextSceneName = "Day2";

    [Tooltip("Scene loaded when the player fails to stop the jaywalker in time.")]
    [SerializeField] private string accidentSceneName = "Day1AccidentScene";

    private bool isTransitioning = false;

    private ConversationStage currentStage = ConversationStage.None;

    private DialogueLine[] currentDialogue;
    private int currentDialogueIndex;

    private bool interactionCompleted;
    private bool rewardApplied;

    private bool choseYes = false;

    public bool CanInteract =>
        currentStage == ConversationStage.None &&
        !interactionCompleted;

    private void Awake()
    {
        SetHighlighted(false);

        if (conversationPanel != null)
        {
            conversationPanel.SetActive(false);
        }

        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
        }

        // Start with the transition screen invisible.
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            blackScreen.alpha = 0f;
            blackScreen.interactable = false;
            blackScreen.blocksRaycasts = false;
        }
    }

    public string GetInteractionPrompt()
    {
        if (isPhone)
        {
            return "Press E to view clue";
        }

        JaywalkingNPCController jaywalker =
        GetComponentInParent<JaywalkingNPCController>();

        if (jaywalker != null && jaywalker.WarningActive)
        {
            return "Press E to stop";
        }

        return "Press E to talk";
    }

    public void SetHighlighted(bool highlighted)
    {
        if (outlineVisual != null)
        {
            outlineVisual.SetActive(highlighted);
        }
    }

    // Called by PlayerNPCInteractor after the player presses E.
    public void BeginConversation()
    {
        if (!CanInteract)
        {
            return;
        }

        // Play police whistle when player starts talking to NPC
        if (interactionAudioSource != null && policeWhistleSound != null)
        {
            interactionAudioSource.PlayOneShot(policeWhistleSound);
        }

        if (openingDialogue == null || openingDialogue.Length == 0)
        {
            Debug.LogError(
                "NPCChoiceInteraction: Opening Dialogue is empty."
            );
            return;
        }

        SetHighlighted(false);

        currentStage = ConversationStage.Introduction;
        currentDialogue = openingDialogue;
        currentDialogueIndex = 0;
        rewardApplied = false;

        if (defaultHUDPanel != null)
        {
            defaultHUDPanel.SetActive(false);
        }

        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
        }

        ShowConversationPanel();
        ShowCurrentDialogue();

        SetPlayerControls(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Connect your existing NextButton to this function.
public void NextDialogue()
{
    if (currentStage == ConversationStage.None ||
        currentDialogue == null)
    {
        return;
    }

    // Move to the next dialogue line
    currentDialogueIndex++;

    // There are still dialogue lines remaining
    if (currentDialogueIndex < currentDialogue.Length)
    {
        ShowCurrentDialogue();
        return;
    }

    // Opening dialogue has finished
    if (currentStage == ConversationStage.Introduction)
    {
        // PHONE:
        // Finish the phone conversation.
        if (isPhone)
        {
            EndConversation();
            return;
        }

        // NPC:
        // Show the normal NPC choices.
        ShowOptions();
        return;
    }

    // Result dialogue finished
    if (currentStage == ConversationStage.Result)
    {
        EndConversation();
    }
}

    private void ShowCurrentDialogue()
    {
        if (currentDialogue == null ||
            currentDialogueIndex < 0 ||
            currentDialogueIndex >= currentDialogue.Length)
        {
            return;
        }

        DialogueLine line = currentDialogue[currentDialogueIndex];

        if (characterNameText != null)
        {
            switch (line.speaker)
            {
                case Speaker.Resident:
                    characterNameText.text = residentName;
                    characterNameText.color = residentNameColour;
                    break;

                case Speaker.PoliceOfficer:
                    characterNameText.text = policeName;
                    characterNameText.color = policeNameColour;
                    break;
            }
        }

        if (conversationText != null)
        {
            conversationText.text = line.sentence;
        }
    }

    private void ShowConversationPanel()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
        }

        if (conversationPanel != null)
        {
            conversationPanel.SetActive(true);
            conversationPanel.transform.SetAsLastSibling();
        }
    }

    private void ShowOptions()
    {
        if (conversationPanel != null)
        {
            conversationPanel.SetActive(false);
        }

        if (optionPanel != null)
        {
            optionPanel.SetActive(true);
            optionPanel.transform.SetAsLastSibling();
        }
    }

    // Hazard Avoided +1, Community Trust +2
    public void ChooseOption1()
    {
        choseYes = true;

        ApplyChoice(
            hazardChange: 1,
            trustChange: 2,
            responseDialogue: option1Response
        );
    }

    public void ChooseOption2()
    {
        // Remember that the player chose NO
        choseYes = false;

        ApplyChoice(
            hazardChange: 0,
            trustChange: -1,
            responseDialogue: option2Response
        );
    }

    private void ApplyChoice(
        int hazardChange,
        int trustChange,
        DialogueLine[] responseDialogue)
    {
        if (currentStage != ConversationStage.Introduction ||
            rewardApplied)
        {
            return;
        }

        rewardApplied = true;

        if (playerHUD != null)
        {
            playerHUD.AddHazardAvoided(hazardChange);
            PlayPointSound(hazardChange);

            playerHUD.ChangeCommunityTrust(trustChange);
            
            if (trustChange != 0)
                {
                    PlayPointSound(trustChange);
            }
        }
        else
        {
            Debug.LogError(
                "NPCChoiceInteraction: PlayerHUD is not assigned."
            );
        }

        if (responseDialogue == null ||
            responseDialogue.Length == 0)
        {
            Debug.LogWarning(
                "NPCChoiceInteraction: Selected option has no response dialogue."
            );

            EndConversation();
            return;
        }

        currentStage = ConversationStage.Result;
        currentDialogue = responseDialogue;
        currentDialogueIndex = 0;

        ShowConversationPanel();
        ShowCurrentDialogue();
    }

    private void DestroyObjectsAfterDialogue()
    {
        if (objectsToDestroy == null)
        return;

        foreach (GameObject obj in objectsToDestroy)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

    }

    private void EndConversation()
    {
        if (isPhone)
        {
            DestroyObjectsAfterDialogue();
        }

        interactionCompleted = true;
        currentStage = ConversationStage.None;
        currentDialogue = null;

        if (conversationPanel != null)
        {
            conversationPanel.SetActive(false);
        }

        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
        }

        if (defaultHUDPanel != null)
        {
            defaultHUDPanel.SetActive(true);
        }

        // Keep controls disabled while the screen is fading.
        SetPlayerControls(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!transitionAfterConversation)
        {
            SetPlayerControls(true);
            return;
        }

        // =====================================
        // OPTION 1 = POLITELY INFORM NPC
        // =====================================
        if (choseYes)
        {
            Debug.Log("Option 1 selected. Fading to next scene.");

            if (npcMovement != null)
            {
                npcMovement.ConditionYes();
            }

            StartSceneTransition(nextSceneName);
            return;
        }

        // =====================================
        // OPTION 2 = SCOLD NPC
        // =====================================
        Debug.Log("Option 2 selected. Fading to next scene.");

        if (npcMovement != null)
        {
            npcMovement.ConditionNo();
        }

        StartSceneTransition(nextSceneName);
    }

    /// <summary>
    /// Can also be called by another script if you want the same
    /// black-screen transition to a different scene.
    /// </summary>
    // Call this when the player fails to stop the jaywalker in time.
    // This uses the exact same black-screen fade before loading the accident scene.
    public void FadeToAccidentScene()
    {
        StartSceneTransition(accidentSceneName);
    }

    public void StartSceneTransition(string sceneName)
    {
        if (isTransitioning)
            return;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("NPCChoiceInteraction: Scene name is empty.");
            return;
        }

        StartCoroutine(FadeToScene(sceneName));
    }

    private IEnumerator FadeToScene(string sceneName)
    {
        isTransitioning = true;

        // If no black screen was assigned, still load the scene.
        if (blackScreen == null)
        {
            Debug.LogWarning(
                "NPCChoiceInteraction: Black Screen is not assigned. Loading scene without fade."
            );

            yield return SceneManager.LoadSceneAsync(sceneName);
            yield break;
        }

        blackScreen.gameObject.SetActive(true);
        blackScreen.blocksRaycasts = true;
        blackScreen.interactable = true;

        float timer = 0f;
        float startAlpha = blackScreen.alpha;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            blackScreen.alpha = Mathf.Lerp(
                startAlpha,
                1f,
                timer / fadeDuration
            );

            yield return null;
        }

        blackScreen.alpha = 1f;

        yield return SceneManager.LoadSceneAsync(sceneName);
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

    private void PlayPointSound(int change)
    {
        if (pointAudioSource == null)
            return;

        // Point increased
        if (change > 0)
        {
                if (pointIncreaseSound != null)
            {
                pointAudioSource.PlayOneShot(pointIncreaseSound);
            }
        }

        // Point decreased
        else if (change < 0)
        {
            if (pointDecreaseSound != null)
            {
                pointAudioSource.PlayOneShot(pointDecreaseSound);
            }
        }
    }
}