using UnityEngine;
using System.Collections.Generic;

public class NPCViolationOutline : MonoBehaviour
{
    [Header("Outline")]
    public Material outlineMaterial;

    [Header("Violation Point")]
    [Tooltip("Drag the Empty GameObject that represents the rule violation point here.")]
    public Transform violationPoint;

    [Tooltip("Red outline appears when NPC is this close to the violation point.")]
    public float warningDistance = 2f;

    [Tooltip("Red outline disappears when NPC reaches this distance.")]
    public float reachedDistance = 0.5f;

    [Header("Testing")]
    public bool aboutToViolate = false;

    private List<Renderer> outlineRenderers = new List<Renderer>();
    private bool previousState;

    private bool warningStarted = false;
    private bool violationFinished = false;

    void Start()
    {
        CreateOutline();

        // Start with outline hidden
        SetOutline(false);

        previousState = aboutToViolate;
    }

    void Update()
    {
        // ============================
        // MANUAL TESTING
        // ============================

        if (aboutToViolate != previousState)
        {
            SetOutline(aboutToViolate);
            previousState = aboutToViolate;
        }

        // ============================
        // VIOLATION POINT
        // ============================

        if (violationPoint == null)
            return;

        if (violationFinished)
            return;

        float distanceToViolation =
            Vector3.Distance(
                transform.position,
                violationPoint.position
            );

        // NPC is getting close to violation point
        if (distanceToViolation <= warningDistance &&
            distanceToViolation > reachedDistance &&
            !warningStarted)
        {
            AboutToViolateRule();

            warningStarted = true;
        }

    }

    void CreateOutline()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer originalRenderer in renderers)
        {
            // Ignore already-created outlines
            if (originalRenderer.gameObject.name.Contains("_Outline"))
                continue;

            GameObject outlineObject =
                new GameObject(originalRenderer.gameObject.name + "_Outline");

            outlineObject.transform.SetParent(originalRenderer.transform.parent);

            outlineObject.transform.localPosition =
                originalRenderer.transform.localPosition;

            outlineObject.transform.localRotation =
                originalRenderer.transform.localRotation;

            outlineObject.transform.localScale =
                originalRenderer.transform.localScale;

            // ============================
            // SKINNED MESH
            // ============================

            if (originalRenderer is SkinnedMeshRenderer originalSkinned)
            {
                SkinnedMeshRenderer outline =
                    outlineObject.AddComponent<SkinnedMeshRenderer>();

                outline.sharedMesh = originalSkinned.sharedMesh;
                outline.rootBone = originalSkinned.rootBone;
                outline.bones = originalSkinned.bones;

                Material[] materials =
                    new Material[originalSkinned.sharedMaterials.Length];

                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = outlineMaterial;
                }

                outline.materials = materials;

                outlineRenderers.Add(outline);
            }

            // ============================
            // NORMAL MESH
            // ============================

            else if (originalRenderer is MeshRenderer)
            {
                MeshFilter originalFilter =
                    originalRenderer.GetComponent<MeshFilter>();

                if (originalFilter != null)
                {
                    MeshFilter newFilter =
                        outlineObject.AddComponent<MeshFilter>();

                    newFilter.sharedMesh =
                        originalFilter.sharedMesh;

                    MeshRenderer outline =
                        outlineObject.AddComponent<MeshRenderer>();

                    Material[] materials =
                        new Material[originalRenderer.sharedMaterials.Length];

                    for (int i = 0; i < materials.Length; i++)
                    {
                        materials[i] = outlineMaterial;
                    }

                    outline.materials = materials;

                    outlineRenderers.Add(outline);
                }
            }
        }
    }

    public void SetOutline(bool active)
    {
        foreach (Renderer renderer in outlineRenderers)
        {
            renderer.enabled = active;
        }
    }

    public void AboutToViolateRule()
    {
        aboutToViolate = true;
        SetOutline(true);
    }

    public void StopViolationWarning()
    {
        aboutToViolate = false;
        SetOutline(false);
    }
}