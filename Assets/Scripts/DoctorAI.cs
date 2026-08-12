// ========================================================================
// File: DoctorAI.cs
// Description: Controls Doctor NPC navigation. Walks slowly behind the player 
//              when far away, stands still when near, and highlights orange.
// ========================================================================

using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

/// <summary>
/// Manages slow player tracking, distance-based stopping, and proximity orange highlighting.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class DoctorAI : MonoBehaviour
{
    [Header("Follow Settings")]
    [Tooltip("Target player transform. Auto-detected via 'Player' tag if unassigned.")]
    [SerializeField] private Transform playerTarget;

    [Tooltip("Movement speed of the Doctor (lower values = slower walk).")]
    [SerializeField] private float walkSpeed = 1.5f;

    [Tooltip("Doctor only starts walking if player gets farther than this distance.")]
    [SerializeField] private float startFollowDistance = 4f;

    [Tooltip("Doctor stops moving and stands still once within this distance.")]
    [SerializeField] private float stopFollowDistance = 2f;

    [Tooltip("Interval in seconds between path calculations.")]
    [SerializeField] private float updateInterval = 0.2f;

    [Header("Orange Proximity Highlight")]
    [SerializeField] private Material orangeOutlineMaterial;
    [SerializeField] private float highlightDistance = 3f;

    private NavMeshAgent agent;
    private float navTimer;
    private List<Renderer> outlineRenderers = new List<Renderer>();

    private bool isFollowing = false;
    private bool hasInteracted = false;
    private bool isHighlighted = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Apply slow walking speed and stopping distance to NavMeshAgent
        if (agent != null)
        {
            agent.speed = walkSpeed;
            agent.stoppingDistance = stopFollowDistance;
        }

        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) playerTarget = playerObj.transform;
        }

        CreateOutline();
        SetOutline(false);

        // Hide doctor initially until phone dialogue completes
        gameObject.SetActive(false); 
    }

    private void Update()
    {
        if (playerTarget == null || !isFollowing || hasInteracted) return;

        HandleFollowBehavior();
        HandleProximityHighlight();
    }

    /// <summary>
    /// Called by NPCChoiceInteraction when the clue phone dialogue ends.
    /// </summary>
    public void OnPhoneDialogueFinished()
    {
        gameObject.SetActive(true);
        isFollowing = true;
        
        if (agent != null)
        {
            agent.speed = walkSpeed;
            agent.isStopped = false;
        }
    }

    /// <summary>
    /// Call when player directly interacts with Doctor to freeze movement permanently.
    /// </summary>
    public void OnPlayerInteractWithDoctor()
    {
        hasInteracted = true;
        isFollowing = false;

        if (agent != null) agent.isStopped = true;
        SetOutline(false);
    }

    /// <summary>
    /// Controls slow follow logic and forces Doctor to freeze when player is close.
    /// </summary>
    private void HandleFollowBehavior()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        // 1. Stand completely still when player is close (never moves away)
        if (distanceToPlayer <= stopFollowDistance)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            return;
        }

        // 2. Start following again only if player moves too far
        if (distanceToPlayer >= startFollowDistance)
        {
            agent.isStopped = false;
        }

        // 3. Update navigation path periodically while moving
        if (!agent.isStopped)
        {
            navTimer += Time.deltaTime;
            if (navTimer >= updateInterval)
            {
                agent.SetDestination(playerTarget.position);
                navTimer = 0f;
            }
        }
    }

    private void HandleProximityHighlight()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        bool shouldHighlight = distanceToPlayer <= highlightDistance && !hasInteracted;

        if (shouldHighlight != isHighlighted)
        {
            isHighlighted = shouldHighlight;
            SetOutline(isHighlighted);
        }
    }

    private void CreateOutline()
    {
        if (orangeOutlineMaterial == null) return;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer originalRenderer in renderers)
        {
            if (originalRenderer.gameObject.name.Contains("_Outline")) continue;

            GameObject outlineObject = new GameObject(originalRenderer.gameObject.name + "_Outline");
            outlineObject.transform.SetParent(originalRenderer.transform.parent);
            outlineObject.transform.localPosition = originalRenderer.transform.localPosition;
            outlineObject.transform.localRotation = originalRenderer.transform.localRotation;
            outlineObject.transform.localScale = originalRenderer.transform.localScale;

            Renderer outlineRenderer = null;

            if (originalRenderer is SkinnedMeshRenderer originalSkinned)
            {
                SkinnedMeshRenderer outline = outlineObject.AddComponent<SkinnedMeshRenderer>();
                outline.sharedMesh = originalSkinned.sharedMesh;
                outline.rootBone = originalSkinned.rootBone;
                outline.bones = originalSkinned.bones;
                outlineRenderer = outline;
            }
            else if (originalRenderer is MeshRenderer)
            {
                MeshFilter originalFilter = originalRenderer.GetComponent<MeshFilter>();
                if (originalFilter != null)
                {
                    MeshFilter newFilter = outlineObject.AddComponent<MeshFilter>();
                    newFilter.sharedMesh = originalFilter.sharedMesh;
                    outlineRenderer = outlineObject.AddComponent<MeshRenderer>();
                }
            }

            if (outlineRenderer != null)
            {
                Material[] materials = new Material[originalRenderer.sharedMaterials.Length];
                for (int i = 0; i < materials.Length; i++) materials[i] = orangeOutlineMaterial;
                outlineRenderer.materials = materials;
                outlineRenderers.Add(outlineRenderer);
            }
        }
    }

    public void SetOutline(bool active)
    {
        foreach (Renderer renderer in outlineRenderers)
        {
            if (renderer != null) renderer.enabled = active;
        }
    }
}