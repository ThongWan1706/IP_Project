using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNPCInteractor : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 4f;
    [SerializeField] private LayerMask npcLayer;

    [Header("Prompt")]
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private TextMeshProUGUI interactPromptText;

    private NPCChoiceInteraction currentNPC;

    private void Start()
    {
        HidePrompt();
    }

    private void Update()
    {
        NPCChoiceInteraction detectedNPC = DetectNPC();

        if (detectedNPC != null && !detectedNPC.CanInteract)
        {
            detectedNPC = null;
        }

        ChangeHighlightedNPC(detectedNPC);

        if (currentNPC != null &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            NPCChoiceInteraction selectedNPC = currentNPC;

            ChangeHighlightedNPC(null);

            // =========================================
            // CHECK IF THIS NPC IS THE JAYWALKER
            // =========================================
            JaywalkingNPCController jaywalker =
                selectedNPC.GetComponentInParent<JaywalkingNPCController>();

            // Only stop the jaywalker while the warning is active
            if (jaywalker != null && jaywalker.WarningActive)
            {
                jaywalker.StopJaywalkerInTime();

                // IMPORTANT:
                // Do NOT call BeginConversation here.
                // JaywalkingNPCController will call it
                // through "On Stopped In Time".
                return;
            }

            // =========================================
            // NORMAL NPC
            // =========================================
            selectedNPC.BeginConversation();
        }
    }

    private NPCChoiceInteraction DetectNPC()
    {
        if (playerCamera == null)
        {
            return null;
        }

        Vector3 rayStart =
            playerCamera.transform.position;

        Vector3 rayDirection =
            playerCamera.transform.forward;

        bool didHit = Physics.Raycast(
            rayStart,
            rayDirection,
            out RaycastHit hit,
            interactionDistance,
            npcLayer,
            QueryTriggerInteraction.Ignore
        );

        Debug.DrawRay(
            rayStart,
            rayDirection * interactionDistance,
            didHit ? Color.green : Color.red,
            0f,
            false
        );

        if (didHit)
        {
            return hit.collider
                .GetComponentInParent<NPCChoiceInteraction>();
        }

        return null;
    }

    private void ChangeHighlightedNPC(
        NPCChoiceInteraction newNPC)
    {
        if (currentNPC == newNPC)
        {
            return;
        }

        if (currentNPC != null)
        {
            currentNPC.SetHighlighted(false);
        }

        currentNPC = newNPC;

        if (currentNPC != null)
        {
            currentNPC.SetHighlighted(true);
            ShowPrompt();
        }
        else
        {
            HidePrompt();
        }
    }

    private void ShowPrompt()
    {
        if (interactPromptText != null)
        {
            JaywalkingNPCController jaywalker =
                currentNPC != null
                ? currentNPC.GetComponentInParent<JaywalkingNPCController>()
                : null;

            if (jaywalker != null &&
                jaywalker.WarningActive)
            {
                interactPromptText.text =
                    "Press E to stop";
            }
            else
            {
                interactPromptText.text =
                    "Press E to talk";
            }
        }

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(true);
        }
    }

    private void HidePrompt()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void OnDisable()
    {
        ChangeHighlightedNPC(null);
    }
}