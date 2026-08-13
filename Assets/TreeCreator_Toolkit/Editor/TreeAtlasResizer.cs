using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class TreeAtlasRemapper : EditorWindow
{
    private GameObject treePrefab;
    private Texture2D barkTexture;
    private Texture2D leafTexture;
    private Texture2D barkNormal;
    private Texture2D leafNormal;

    [MenuItem("Tools/Game Dev/Tree Custom Atlas Packer")]
    public static void ShowWindow()
    {
        GetWindow<TreeAtlasRemapper>("Tree Atlas Packer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Custom 2048 Atlas Generator for Trees", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        treePrefab = (GameObject)EditorGUILayout.ObjectField("Tree Prefab/GameObject", treePrefab, typeof(GameObject), true);

        EditorGUILayout.LabelField("Source Textures (Originals, not the baked atlas)", EditorStyles.miniBoldLabel);
        barkTexture = (Texture2D)EditorGUILayout.ObjectField("Bark Diffuse (Tane)", barkTexture, typeof(Texture2D), false);
        barkNormal = (Texture2D)EditorGUILayout.ObjectField("Bark Normal", barkNormal, typeof(Texture2D), false);
        leafTexture = (Texture2D)EditorGUILayout.ObjectField("Leaf Diffuse (Barg)", leafTexture, typeof(Texture2D), false);
        leafNormal = (Texture2D)EditorGUILayout.ObjectField("Leaf Normal", leafNormal, typeof(Texture2D), false);

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate 2048 Atlas & Remap UVs"))
        {
            if (treePrefab == null || barkTexture == null || leafTexture == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign the Tree and at least Diffuse Textures.", "OK");
                return;
            }
            PackAndRemap();
        }
    }

    private void PackAndRemap()
    {
        try
        {
            // ۱. تنظیم اساسی پروپرتی Readable برای تکسچرها
            SetTextureReadable(barkTexture);
            SetTextureReadable(leafTexture);
            if (barkNormal) SetTextureReadable(barkNormal);
            if (leafNormal) SetTextureReadable(leafNormal);

            // ۲. ساخت پوشه با استفاده از AssetDatabase (امن‌ترین روش در ادیتور)
            string folderName = "BakedTrees";
            if (!AssetDatabase.IsValidFolder("Assets/" + folderName))
            {
                AssetDatabase.CreateFolder("Assets", folderName);
                Debug.Log("Folder 'Assets/BakedTrees' created successfully.");
            }

            string saveDirectory = "Assets/" + folderName;

            // ۳. ساخت تکسچر اطلس جدید با سایز ۲۰۴۸
            Texture2D diffuseAtlas = new Texture2D(2048, 2048, TextureFormat.RGBA32, true);
            Texture2D[] texturesToPack = new Texture2D[] { barkTexture, leafTexture };

            // پک کردن تکسچرها و دریافت مختصات جدید
            Rect[] uvs = diffuseAtlas.PackTextures(texturesToPack, 2, 2048);

            // ذخیره اطلس Diffuse
            string diffusePath = Path.Combine(saveDirectory, treePrefab.name + "_DiffuseAtlas_2048.png");
            byte[] diffuseBytes = diffuseAtlas.EncodeToPNG();
            File.WriteAllBytes(Path.GetFullPath(diffusePath), diffuseBytes);
            AssetDatabase.ImportAsset(diffusePath);

            // ساخت اطلس نرمال در صورت وجود
            string normalPath = "";
            if (barkNormal != null && leafNormal != null)
            {
                Texture2D normalAtlas = new Texture2D(2048, 2048, TextureFormat.RGBA32, true);
                Texture2D[] normalsToPack = new Texture2D[] { barkNormal, leafNormal };
                normalAtlas.PackTextures(normalsToPack, 2, 2048);

                normalPath = Path.Combine(saveDirectory, treePrefab.name + "_NormalAtlas_2048.png");
                byte[] normalBytes = normalAtlas.EncodeToPNG();
                File.WriteAllBytes(Path.GetFullPath(normalPath), normalBytes);
                AssetDatabase.ImportAsset(normalPath);
            }

            // ۴. اعمال تغییرات روی مش درخت و جابجایی UVها
            MeshFilter meshFilter = treePrefab.GetComponentInChildren<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                EditorUtility.DisplayDialog("Error", "No MeshFilter found on the tree hierarchy.", "OK");
                return;
            }

            Mesh originalMesh = meshFilter.sharedMesh;
            Mesh newMesh = Instantiate(originalMesh);
            newMesh.name = originalMesh.name + "_2048";

            Vector2[] meshUVs = originalMesh.uv;
            if (meshUVs == null || meshUVs.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "The target mesh doesn't have UV coordinates.", "OK");
                return;
            }

            for (int subMeshIndex = 0; subMeshIndex < originalMesh.subMeshCount; subMeshIndex++)
            {
                int[] triangles = originalMesh.GetTriangles(subMeshIndex);
                Rect targetRect = uvs[Mathf.Clamp(subMeshIndex, 0, uvs.Length - 1)];

                foreach (int index in triangles)
                {
                    Vector2 originalUV = meshUVs[index];
                    meshUVs[index] = new Vector2(
                        Mathf.Lerp(targetRect.xMin, targetRect.xMax, originalUV.x),
                        Mathf.Lerp(targetRect.yMin, targetRect.yMax, originalUV.y)
                    );
                }
            }

            newMesh.uv = meshUVs;
            newMesh.RecalculateBounds();
            newMesh.RecalculateTangents();

            // ذخیره مش جدید در پروژه
            string meshPath = Path.Combine(saveDirectory, treePrefab.name + "_Mesh_2048.asset");
            AssetDatabase.CreateAsset(newMesh, meshPath);

            // ۵. ساخت متریال جدید
            Material newMat = new Material(Shader.Find("Nature/Tree Creator Leaves"));
            if (newMat.shader == null)
            {
                // فال‌بک در صورتی که شیدر قدیمی در پایپلاین جاری وجود نداشته باشد
                newMat = new Material(Shader.Find("Standard"));
            }

            newMat.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath<Texture2D>(diffusePath));
            if (!string.IsNullOrEmpty(normalPath))
            {
                newMat.SetTexture("_BumpSpecMap", AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath));
                newMat.SetTexture("_BumpMap", AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath));
                newMat.EnableKeyword("_NORMALMAP");
            }

            string matPath = Path.Combine(saveDirectory, treePrefab.name + "_Material_2048.mat");
            AssetDatabase.CreateAsset(newMat, matPath);

            // ۶. اعمال مش و متریال به یک نسخه کپی شده از پرفاب
            GameObject newTreeObj = Instantiate(treePrefab);
            newTreeObj.name = treePrefab.name + "_HighRes";

            MeshFilter targetFilter = newTreeObj.GetComponentInChildren<MeshFilter>();
            MeshRenderer targetRenderer = newTreeObj.GetComponentInChildren<MeshRenderer>();

            if (targetFilter != null && targetRenderer != null)
            {
                targetFilter.sharedMesh = newMesh;
                targetRenderer.sharedMaterial = newMat;
            }

            // ذخیره به عنوان پرفاب جدید
            string prefabPath = Path.Combine(saveDirectory, newTreeObj.name + ".prefab");
            PrefabUtility.SaveAsPrefabAsset(newTreeObj, prefabPath);
            DestroyImmediate(newTreeObj);

            // اعمال نهایی و رفرش پروژه
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Success", "2048 Atlas generated and saved to: " + saveDirectory, "OK");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Atlas Packer: {ex.Message}\n{ex.StackTrace}");
            EditorUtility.DisplayDialog("Execution Failed", "An error occurred. Check Console for details.", "OK");
        }
    }

    private void SetTextureReadable(Texture2D tex)
    {
        string path = AssetDatabase.GetAssetPath(tex);
        if (string.IsNullOrEmpty(path)) return;

        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null && !importer.isReadable)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
        }
    }
}
