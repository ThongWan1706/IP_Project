using System;
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

    [Header("NPC Highlight")]
    [SerializeField] private GameObject outlineVisual;

    [Header("NPC Movement")]
    [SerializeField] private NPCNavMeshWalk npcMovement;

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

    [Header("Player Controls")]
    [Tooltip("Assign the First Person Controller component.")]
    [SerializeField] private Behaviour[] playerControlScripts;

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

        currentDialogueIndex++;

        if (currentDialogueIndex < currentDialogue.Length)
        {
            ShowCurrentDialogue();
            return;
        }

        // Opening conversation completed.
        if (currentStage == ConversationStage.Introduction)
        {
            ShowOptions();
            return;
        }

        // Result conversation completed.
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
        // Remember that the player chose NO
        choseYes = false;

        ApplyChoice(
            hazardChange: 1,
            trustChange: -1,
            responseDialogue: option1Response
        );
    }

    public void ChooseOption2()
    {
        choseYes = true;

        ApplyChoice(
            hazardChange: 1,
            trustChange: 2,
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
            playerHUD.ChangeCommunityTrust(trustChange);
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

    private void EndConversation()
    {
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

        SetPlayerControls(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ============================
        // PLAYER CHOSE YES
        // ============================
        if (choseYes)
        {
            Debug.Log("Dialogue finished. NPC continues walking.");

            if (npcMovement != null)
            {
                npcMovement.ConditionYes();

                // Wait for NPC to reach Target 2
                StartCoroutine(WaitForNPCToFinish());
            }

            return;
        }

        // ============================
        // PLAYER CHOSE NO
        // ============================
        Debug.Log("Player chose NO.");

        if (npcMovement != null)
        {
            npcMovement.ConditionNo();
        }

        Destroy(gameObject);
        SceneManager.LoadScene("Day2");
    }

    private System.Collections.IEnumerator WaitForNPCToFinish()
{
    while (!npcMovement.HasReachedFinalTarget)
    {
        yield return null;
    }

    Debug.Log("NPC reached Target 2.");

    Destroy(gameObject);

    SceneManager.LoadScene("Day2");
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
}