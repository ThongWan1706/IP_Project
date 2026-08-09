using UnityEngine;
using System.Collections.Generic;

public class NPCViolationOutline : MonoBehaviour
{
    [Header("Outline")]
    public Material outlineMaterial;

    [Header("Testing")]
    public bool aboutToViolate = true;

    private List<Renderer> outlineRenderers = new List<Renderer>();
    private bool previousState;

    void Start()
    {
        CreateOutline();

        // IMPORTANT:
        // Show or hide based on the checkbox
        SetOutline(aboutToViolate);

        previousState = aboutToViolate;
    }

    void Update()
    {
        // Detect checkbox change during Play Mode
        if (aboutToViolate != previousState)
        {
            SetOutline(aboutToViolate);
            previousState = aboutToViolate;
        }
    }

    void CreateOutline()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer originalRenderer in renderers)
        {
            if (originalRenderer.gameObject.name.Contains("_Outline"))
                continue;

            GameObject outlineObject =
                new GameObject(originalRenderer.gameObject.name + "_Outline");

            // Put outline directly under original renderer
            outlineObject.transform.SetParent(originalRenderer.transform);

            outlineObject.transform.localPosition = Vector3.zero;
            outlineObject.transform.localRotation = Quaternion.identity;
            outlineObject.transform.localScale = Vector3.one;

            // =========================
            // SKINNED MESH
            // =========================

            if (originalRenderer is SkinnedMeshRenderer originalSkinned)
            {
                SkinnedMeshRenderer outline =
                    outlineObject.AddComponent<SkinnedMeshRenderer>();

                outline.sharedMesh = originalSkinned.sharedMesh;

                outline.rootBone = originalSkinned.rootBone;
                outline.bones = originalSkinned.bones;

                outline.localBounds = originalSkinned.localBounds;

                outline.updateWhenOffscreen = true;

                Material[] materials =
                    new Material[originalSkinned.sharedMaterials.Length];

                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = outlineMaterial;
                }

                outline.sharedMaterials = materials;

                outlineRenderers.Add(outline);
            }

            // =========================
            // NORMAL MESH
            // =========================

            else if (originalRenderer is MeshRenderer)
            {
                MeshFilter originalFilter =
                    originalRenderer.GetComponent<MeshFilter>();

                if (originalFilter != null)
                {
                    MeshFilter outlineFilter =
                        outlineObject.AddComponent<MeshFilter>();

                    outlineFilter.sharedMesh =
                        originalFilter.sharedMesh;

                    MeshRenderer outline =
                        outlineObject.AddComponent<MeshRenderer>();

                    Material[] materials =
                        new Material[originalRenderer.sharedMaterials.Length];

                    for (int i = 0; i < materials.Length; i++)
                    {
                        materials[i] = outlineMaterial;
                    }

                    outline.sharedMaterials = materials;

                    outlineRenderers.Add(outline);
                }
            }
        }
    }

    public void SetOutline(bool active)
    {
        foreach (Renderer renderer in outlineRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = active;
            }
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