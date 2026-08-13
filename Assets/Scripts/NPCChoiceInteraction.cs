using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Conversation Options")]
    [Tooltip("Leave ON for normal NPCs with Option 1 / Option 2. Turn OFF for report-only conversations.")]
    [SerializeField] private bool hasChoices = true;

    [Header("No-Helmet Vehicle Violation")]
    [Tooltip("Turn this ON for the rider NPC so the player can only interact after the bike enters the violation trigger.")]
    [SerializeField] private bool onlyInteractDuringNoHelmetViolation = false;

    [Header("Helmet")]
    [Tooltip("Optional. Assign this only for NPCs that should put on/remove a helmet based on the dialogue choice.")]
    [SerializeField] private HelmetController helmetController;

    [Header("Clue Phone / Doctor Trigger")]
    [Tooltip("Assign the Doctor AI script to activate the Doctor when this dialogue finishes.")]
    [SerializeField] private DoctorAI doctorAI;

    [Header("NPC Highlight")]
    [SerializeField] private GameObject outlineVisual;

    [Header("Objects To Destroy After Dialogue")]
    [SerializeField] private GameObject[] objectsToDestroy;

    [Header("NPC Movement")]
    [SerializeField] private NPCNavMeshWalk npcMovement;

    [Header("Incident Progression")]
    [Tooltip("Optional. Assign the IncidentManager when finishing this NPC conversation should start the next incident.")]
    [SerializeField] private IncidentManager incidentManager;

    [Tooltip("Turn this on if ending this conversation should complete the current incident and activate the next one.")]
    [SerializeField] private bool completeIncidentAfterConversation = false;

    [Header("Interaction Sound")]
    [SerializeField] private AudioSource interactionAudioSource;
    [SerializeField] private AudioClip policeWhistleSound;

    [Header("Player UI")]
    [SerializeField] private GameObject defaultHUDPanel;
    [SerializeField] private GameObject conversationPanel;
    [SerializeField] private GameObject optionPanel;

    [Tooltip("Optional. Assign this when multiple NPCs share the same Conversation Panel/Next button. Leave None to keep using an existing Inspector OnClick setup.")]
    [SerializeField] private Button nextButton;

    [Tooltip("Assign the NoHelmetZone trigger.")]
    [SerializeField] private NoHelmetViolationTrigger noHelmetViolationTrigger;

    [Tooltip("Optional. Assign this when multiple NPCs share the same Option 1 button. Leave None to keep using an existing Inspector OnClick setup.")]
    [SerializeField] private Button option1Button;

    [Tooltip("Optional. Assign this when multiple NPCs share the same Option 2 button. Leave None to keep using an existing Inspector OnClick setup.")]
    [SerializeField] private Button option2Button;

    [Header("Option Button Text")]
    [Tooltip("Assign the TextMeshProUGUI text object inside Option 1 Button.")]
    [SerializeField] private TextMeshProUGUI option1ButtonText;

    [Tooltip("Assign the TextMeshProUGUI text object inside Option 2 Button.")]
    [SerializeField] private TextMeshProUGUI option2ButtonText;

    [Tooltip("Text displayed on Option 1 for this NPC.")]
    [SerializeField] private string option1Text = "Politely inform them";

    [Tooltip("Text displayed on Option 2 for this NPC.")]
    [SerializeField] private string option2Text = "Scold them";

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

    [Header("Score Settings")]
    [Tooltip("Turn OFF if this NPC should not change Hazard Avoided.")]
    [SerializeField] private bool changeHazardAvoided = true;

    [Tooltip("Turn OFF if this NPC should not change Community Trust yet.")]
    [SerializeField] private bool changeCommunityTrust = true;

    [Header("Score On Conversation End")]
    [Tooltip("Turn this ON for report/result scenes where points should be awarded only after the dialogue finishes.")]
    [SerializeField] private bool applyScoreOnConversationEnd = false;

    [Tooltip("Hazard Avoided points applied when the conversation ends.")]
    [SerializeField] private int endConversationHazardChange = 0;

    [Tooltip("Community Trust bars/points applied when the conversation ends.")]
    [SerializeField] private int endConversationTrustChange = 0;

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

    [Tooltip("Default scene loaded after Option 1 or Option 2 when Separate Choice Scenes is OFF.")]
    [SerializeField] private string nextSceneName = "Day2";

    [Header("Separate Choice Scenes")]
    [Tooltip("Turn this ON only for NPCs such as the motorist that need different scenes for Option 1 and Option 2.")]
    [SerializeField] private bool useSeparateChoiceScenes = false;

    [Tooltip("Scene loaded after Option 1 when Separate Choice Scenes is ON.")]
    [SerializeField] private string option1SceneName = "";

    [Tooltip("Scene loaded after Option 2 when Separate Choice Scenes is ON.")]
    [SerializeField] private string option2SceneName = "";

    [Tooltip("Turn this ON if failing to stop this NPC should go to the same scene as Option 2.")]
    [SerializeField] private bool accidentUsesOption2Scene = false;

    [Tooltip("Scene loaded when the player fails to stop the NPC, unless Accident Uses Option 2 Scene is ON.")]
    [SerializeField] private string accidentSceneName = "Day1AccidentScene";

    private bool isTransitioning = false;

    private ConversationStage currentStage = ConversationStage.None;

    private DialogueLine[] currentDialogue;
    private int currentDialogueIndex;

    private bool interactionCompleted;
    private bool rewardApplied;

    private bool choseYes = false;

    // Stores which point sound should play after the result dialogue ends.
    //  1 = point increase sound
    // -1 = point decrease sound
    //  0 = no pending sound
    private int pendingPointSound = 0;

    // Prevents report/result scoring from being applied more than once.
    private bool endConversationScoreApplied = false;

    public bool CanInteract
    {
        get
        {
            if (currentStage != ConversationStage.None || interactionCompleted)
                return false;

            if (onlyInteractDuringNoHelmetViolation)
            {
                NoHelmetVehicleViolation vehicleViolation =
                    GetComponentInParent<NoHelmetVehicleViolation>();

                return vehicleViolation != null &&
                       vehicleViolation.WarningActive;
            }

            return true;
        }
    }

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

        NoHelmetVehicleViolation vehicleViolation =
            GetComponentInParent<NoHelmetVehicleViolation>();

        if (vehicleViolation != null && vehicleViolation.WarningActive)
        {
            return "Press E to stop";
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

        // If this interaction belongs to a no-helmet rider and the warning
        // is active, pressing E stops the vehicle before the dialogue begins.
        NoHelmetVehicleViolation vehicleViolation =
            GetComponentInParent<NoHelmetVehicleViolation>();

        if (vehicleViolation != null && vehicleViolation.WarningActive)
        {
            // Stop the motorist/bike when the player interacts in time.
            vehicleViolation.StopVehicleInTime();

            // Stop the no-helmet warning/alarm sound immediately
            // when the player successfully interacts with the motorist.
            if (noHelmetViolationTrigger != null)
            {
                noHelmetViolationTrigger.StopWarningSound();
            }
        }

        // OPTIONAL SHARED NEXT BUTTON:
        // If this NPC has a Next Button assigned, connect only this NPC's
        // NextDialogue() while its conversation is active.
        //
        // If nextButton is left as None, nothing changes. This keeps older
        // scenes (for example Day 1) compatible with their existing Inspector setup.
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(NextDialogue);
            nextButton.onClick.AddListener(NextDialogue);
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

        // REPORT / RESULT-ONLY NPC:
        // End after the opening dialogue instead of showing Option 1 / Option 2.
        if (!hasChoices)
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

        // Update the text shown on the shared option buttons.
        // Each NPC can have different option wording in the Inspector.
        if (option1ButtonText != null)
        {
            option1ButtonText.text = option1Text;
        }

        if (option2ButtonText != null)
        {
            option2ButtonText.text = option2Text;
        }

        // OPTIONAL SHARED OPTION BUTTONS:
        // When assigned, connect the shared buttons only to the NPC
        // whose conversation is currently active.
        if (option1Button != null)
        {
            option1Button.onClick.RemoveListener(ChooseOption1);
            option1Button.onClick.AddListener(ChooseOption1);
        }

        if (option2Button != null)
        {
            option2Button.onClick.RemoveListener(ChooseOption2);
            option2Button.onClick.AddListener(ChooseOption2);
        }
    }

    // Hazard Avoided +1, Community Trust +2
    public void ChooseOption1()
    {
        choseYes = true;

        // The choice has been made, so this NPC no longer needs
        // to listen to the shared option buttons.
        DisconnectOptionButtons();

        // OPTIONAL HELMET:
        // Only NPCs with a HelmetController assigned will use this.
        // For the no-helmet rider, Option 1 (politely inform) makes
        // the rider put the helmet on.
        if (helmetController != null)
        {
            helmetController.ShowHelmet();
        }

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

        // The choice has been made, so this NPC no longer needs
        // to listen to the shared option buttons.
        DisconnectOptionButtons();

        // OPTIONAL HELMET:
        // If this NPC has a HelmetController, keep the helmet off
        // for Option 2.
        if (helmetController != null)
        {
            helmetController.HideHelmet();
        }

        ApplyChoice(
            hazardChange: 0,
            trustChange: -1,
            responseDialogue: option2Response
        );
    }

    private void DisconnectOptionButtons()
    {
        if (option1Button != null)
        {
            option1Button.onClick.RemoveListener(ChooseOption1);
        }

        if (option2Button != null)
        {
            option2Button.onClick.RemoveListener(ChooseOption2);
        }
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

        // Apply the point changes now, but do NOT play the sound yet.
        if (playerHUD != null)
        {
            // Only change Hazard Avoided if enabled for this NPC.
            if (changeHazardAvoided)
            {
                playerHUD.AddHazardAvoided(hazardChange);
            }

            // Only change Community Trust if enabled for this NPC.
            if (changeCommunityTrust)
            {
                playerHUD.ChangeCommunityTrust(trustChange);
            }
        }
        else
        {
            Debug.LogError(
                "NPCChoiceInteraction: PlayerHUD is not assigned."
            );
        }

        // Save one sound to play only after the response dialogue finishes.
        if (choseYes)
        {
            // Option 1 = polite / positive choice
            pendingPointSound = 1;
        }
        else
        {
            // Option 2 = scolding / negative choice
            pendingPointSound = -1;
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

    private void ApplyEndConversationScore()
    {
        if (!applyScoreOnConversationEnd || endConversationScoreApplied)
            return;

        if (playerHUD == null)
        {
            Debug.LogError(
                "NPCChoiceInteraction: PlayerHUD is not assigned for end-of-conversation scoring."
            );
            return;
        }

        endConversationScoreApplied = true;

        if (changeHazardAvoided && endConversationHazardChange != 0)
        {
            playerHUD.AddHazardAvoided(endConversationHazardChange);
        }

        if (changeCommunityTrust && endConversationTrustChange != 0)
        {
            playerHUD.ChangeCommunityTrust(endConversationTrustChange);
        }

        // Play the matching point sound for this report outcome.
        if (endConversationHazardChange > 0 || endConversationTrustChange > 0)
        {
            PlayPointSound(1);
        }
        else if (endConversationHazardChange < 0 || endConversationTrustChange < 0)
        {
            PlayPointSound(-1);
        }
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
        // Optional report/result scoring.
        // This is useful when the score should only change AFTER the doctor
        // finishes revealing the report in the destination scene.
        ApplyEndConversationScore();

        // Remove this NPC from any optional shared option buttons.
        DisconnectOptionButtons();

        // If this NPC was using the optional shared Next Button,
        // disconnect it when the conversation finishes.
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(NextDialogue);
        }

        DestroyObjectsAfterDialogue();

        if (isPhone)
        {

            // Doctor Appears when phone is destroyed
            if (doctorAI != null)
            {
                doctorAI.OnPhoneDialogueFinished();
            }
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

        // Play the point increase/decrease sound only after
        // the response conversation has completely ended.
        if (pendingPointSound != 0)
        {
            PlayPointSound(pendingPointSound);
            pendingPointSound = 0;
        }

        if (!transitionAfterConversation)
        {
            SetPlayerControls(true);

            // OPTIONAL INCIDENT PROGRESSION:
            // Used for encounters such as the elderly incident in Day 2.
            // When enabled, completing this conversation tells the
            // IncidentManager to disable the current incident and activate
            // the next one.
            if (completeIncidentAfterConversation)
            {
                if (incidentManager != null)
                {
                    incidentManager.CompleteCurrentIncident();
                }
                else
                {
                    Debug.LogWarning(
                        "NPCChoiceInteraction: Complete Incident After Conversation is ON, " +
                        "but Incident Manager is not assigned."
                    );
                }
            }

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

            string option1SceneToLoad =
                useSeparateChoiceScenes
                ? option1SceneName
                : nextSceneName;

            StartSceneTransition(option1SceneToLoad);
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

        string option2SceneToLoad =
            useSeparateChoiceScenes
            ? option2SceneName
            : nextSceneName;

        StartSceneTransition(option2SceneToLoad);
    }

    /// <summary>
    /// Can also be called by another script if you want the same
    /// black-screen transition to a different scene.
    /// </summary>
    // Call this when the player fails to stop the jaywalker in time.
    // This uses the exact same black-screen fade before loading the accident scene.
    public void FadeToAccidentScene()
    {
        string sceneToLoad = accidentSceneName;

        if (useSeparateChoiceScenes && accidentUsesOption2Scene)
        {
            sceneToLoad = option2SceneName;
        }

        StartSceneTransition(sceneToLoad);
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

    private void OnDisable()
    {
        // Safety cleanup so this NPC cannot stay connected to shared
        // UI buttons if the object is disabled before EndConversation().
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(NextDialogue);
        }

        DisconnectOptionButtons();
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