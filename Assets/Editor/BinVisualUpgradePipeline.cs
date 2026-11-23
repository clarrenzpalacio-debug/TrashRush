using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TrashRush.Core;
using TrashRush.Recycling;
using TrashRush.Art;

namespace TrashRush.Editor
{
    public static class BinVisualUpgradePipeline
    {
        private const string TEXTURE_PATH = "Assets/Art/Textures";
        private const string MATERIAL_PATH = "Assets/Art/Materials";
        private const string PREFAB_PATH = "Assets/Prefabs/Recycling";
        private const string SCENE_PATH = "Assets/Scenes/GameScene.unity";

        // Required Color Palette:
        // PAPER: Blue (#1E88E5)
        // PLASTIC: Yellow (#FDD835)
        // METAL: Red (#E53935)
        // ORGANIC: Green (#43A047)
        private static readonly Color ColorPaper = new Color(0.12f, 0.53f, 0.90f, 1f);     // Blue
        private static readonly Color ColorPlastic = new Color(0.99f, 0.85f, 0.21f, 1f);   // Yellow
        private static readonly Color ColorMetal = new Color(0.90f, 0.22f, 0.21f, 1f);     // Red
        private static readonly Color ColorOrganic = new Color(0.26f, 0.63f, 0.28f, 1f);   // Green

        [MenuItem("TrashRush/Run Bin Visual Upgrade")]
        public static void RunUpgrade()
        {
            Debug.Log("[BinVisualUpgradePipeline] Starting Recycling Bin visual upgrade...");

            EnsureDirectories();
            UpdateBinMaterials();
            GenerateAllBinTextures();
            BuildAllBinPrefabs();
            UpdateSceneRecyclingStation();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[BinVisualUpgradePipeline] Recycling Bin visual upgrade completed successfully!");
        }

        private static void EnsureDirectories()
        {
            if (!Directory.Exists(TEXTURE_PATH)) Directory.CreateDirectory(TEXTURE_PATH);
            if (!Directory.Exists(MATERIAL_PATH)) Directory.CreateDirectory(MATERIAL_PATH);
            if (!Directory.Exists(PREFAB_PATH)) Directory.CreateDirectory(PREFAB_PATH);
        }

        private static void UpdateBinMaterials()
        {
            // Bin Body Materials
            SetMaterialColor($"{MATERIAL_PATH}/M_Bin_Paper.mat", ColorPaper, "Paper Body");
            SetMaterialColor($"{MATERIAL_PATH}/M_Bin_Plastic.mat", ColorPlastic, "Plastic Body");
            SetMaterialColor($"{MATERIAL_PATH}/M_Bin_Metal.mat", ColorMetal, "Metal Body");
            SetMaterialColor($"{MATERIAL_PATH}/M_Bin_Organic.mat", ColorOrganic, "Organic Body");

            // Bin Lid Material (Dark/Neutral Plastic)
            SetMaterialColor($"{MATERIAL_PATH}/M_Bin_Lid.mat", new Color(0.2f, 0.22f, 0.25f, 1f), "Bin Lid");
            // Plaque backing material
            SetMaterialColor($"{MATERIAL_PATH}/M_Bin_Plaque.mat", new Color(0.12f, 0.12f, 0.14f, 1f), "Bin Plaque");
        }

        private static void SetMaterialColor(string path, Color col, string label)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                Shader standard = Shader.Find("Standard");
                mat = new Material(standard);
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.color = col;
            mat.SetFloat("_Glossiness", 0.4f);
            mat.SetFloat("_Metallic", 0.1f);
            EditorUtility.SetDirty(mat);
            Debug.Log($"[BinVisualUpgradePipeline] Configured Material: {path} ({label}) -> {col}");
        }

        private static void GenerateAllBinTextures()
        {
            GenerateCategoryTextures(TrashCategory.Paper, "PAPER", ColorPaper, "M_Label_PAPER", "M_Floating_PAPER");
            GenerateCategoryTextures(TrashCategory.Plastic, "PLASTIC", ColorPlastic, "M_Label_PLASTIC", "M_Floating_PLASTIC");
            GenerateCategoryTextures(TrashCategory.Metal, "METAL", ColorMetal, "M_Label_METAL", "M_Floating_METAL");
            GenerateCategoryTextures(TrashCategory.Organic, "ORGANIC", ColorOrganic, "M_Label_ORGANIC", "M_Floating_ORGANIC");
        }

        private static void GenerateCategoryTextures(TrashCategory cat, string name, Color themeColor, string frontMatName, string floatMatName)
        {
            // 1. Enhanced Front/Back Label Texture (512x256)
            Texture2D labelTex = ProceduralTextureGenerator.CreateEnhancedBinLabelTexture(cat);
            string labelPngPath = $"{TEXTURE_PATH}/T_Label_{name}.png";
            File.WriteAllBytes(labelPngPath, labelTex.EncodeToPNG());
            AssetDatabase.ImportAsset(labelPngPath, ImportAssetOptions.ForceSynchronousImport);

            TextureImporter labelImporter = AssetImporter.GetAtPath(labelPngPath) as TextureImporter;
            if (labelImporter != null)
            {
                labelImporter.textureType = TextureImporterType.Default;
                labelImporter.sRGBTexture = true;
                labelImporter.wrapMode = TextureWrapMode.Clamp;
                labelImporter.filterMode = FilterMode.Bilinear;
                labelImporter.SaveAndReimport();
            }

            Texture2D importedLabelTex = AssetDatabase.LoadAssetAtPath<Texture2D>(labelPngPath);

            // Front label material
            string frontMatPath = $"{MATERIAL_PATH}/{frontMatName}.mat";
            Material frontMat = AssetDatabase.LoadAssetAtPath<Material>(frontMatPath);
            if (frontMat == null)
            {
                frontMat = new Material(Shader.Find("Standard"));
                AssetDatabase.CreateAsset(frontMat, frontMatPath);
            }
            frontMat.mainTexture = importedLabelTex;
            frontMat.color = Color.white;
            frontMat.SetFloat("_Glossiness", 0.2f);
            EditorUtility.SetDirty(frontMat);

            // 2. Floating Billboard Texture (256x256 Circular / Rounded Icon)
            Texture2D floatTex = ProceduralTextureGenerator.CreateFloatingLabelTexture(cat);
            string floatPngPath = $"{TEXTURE_PATH}/T_Floating_{name}.png";
            File.WriteAllBytes(floatPngPath, floatTex.EncodeToPNG());
            AssetDatabase.ImportAsset(floatPngPath, ImportAssetOptions.ForceSynchronousImport);

            TextureImporter floatImporter = AssetImporter.GetAtPath(floatPngPath) as TextureImporter;
            if (floatImporter != null)
            {
                floatImporter.textureType = TextureImporterType.Default;
                floatImporter.sRGBTexture = true;
                floatImporter.alphaIsTransparency = true;
                floatImporter.wrapMode = TextureWrapMode.Clamp;
                floatImporter.filterMode = FilterMode.Bilinear;
                floatImporter.SaveAndReimport();
            }

            Texture2D importedFloatTex = AssetDatabase.LoadAssetAtPath<Texture2D>(floatPngPath);

            // Floating billboard material (Cutout or Transparent Standard)
            string floatMatPath = $"{MATERIAL_PATH}/{floatMatName}.mat";
            Material floatMat = AssetDatabase.LoadAssetAtPath<Material>(floatMatPath);
            if (floatMat == null)
            {
                // Unlit/Transparent or Standard Transparent
                Shader s = Shader.Find("Unlit/Transparent");
                if (s == null) s = Shader.Find("Standard");
                floatMat = new Material(s);
                AssetDatabase.CreateAsset(floatMat, floatMatPath);
            }
            else
            {
                Shader s = Shader.Find("Unlit/Transparent");
                if (s != null && floatMat.shader != s) floatMat.shader = s;
            }
            floatMat.mainTexture = importedFloatTex;
            floatMat.color = Color.white;
            EditorUtility.SetDirty(floatMat);

            Debug.Log($"[BinVisualUpgradePipeline] Textures & Materials generated for {name}");
        }

        private static void BuildAllBinPrefabs()
        {
            BuildBinPrefab(TrashCategory.Paper, "Bin_Paper", $"{MATERIAL_PATH}/M_Bin_Paper.mat", $"{MATERIAL_PATH}/M_Label_PAPER.mat", $"{MATERIAL_PATH}/M_Floating_PAPER.mat", ColorPaper);
            BuildBinPrefab(TrashCategory.Plastic, "Bin_Plastic", $"{MATERIAL_PATH}/M_Bin_Plastic.mat", $"{MATERIAL_PATH}/M_Label_PLASTIC.mat", $"{MATERIAL_PATH}/M_Floating_PLASTIC.mat", ColorPlastic);
            BuildBinPrefab(TrashCategory.Metal, "Bin_Metal", $"{MATERIAL_PATH}/M_Bin_Metal.mat", $"{MATERIAL_PATH}/M_Label_METAL.mat", $"{MATERIAL_PATH}/M_Floating_METAL.mat", ColorMetal);
            BuildBinPrefab(TrashCategory.Organic, "Bin_Organic", $"{MATERIAL_PATH}/M_Bin_Organic.mat", $"{MATERIAL_PATH}/M_Label_ORGANIC.mat", $"{MATERIAL_PATH}/M_Floating_ORGANIC.mat", ColorOrganic);
        }

        private static void BuildBinPrefab(TrashCategory cat, string prefabName, string bodyMatPath, string labelMatPath, string floatMatPath, Color themeColor)
        {
            GameObject root = new GameObject(prefabName);
            root.tag = "RecyclingBin";

            Material bodyMat = AssetDatabase.LoadAssetAtPath<Material>(bodyMatPath);
            Material lidMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIAL_PATH}/M_Bin_Lid.mat");
            Material plaqueMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIAL_PATH}/M_Bin_Plaque.mat");
            Material labelMat = AssetDatabase.LoadAssetAtPath<Material>(labelMatPath);
            Material floatMat = AssetDatabase.LoadAssetAtPath<Material>(floatMatPath);

            // 1. Bin Body (Cube base)
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.65f, 0f);
            body.transform.localScale = new Vector3(0.9f, 1.3f, 0.9f);
            if (bodyMat != null) body.GetComponent<MeshRenderer>().sharedMaterial = bodyMat;
            // Remove primitive collider on child; root will have the trigger / physics collider
            UnityEngine.Object.DestroyImmediate(body.GetComponent<Collider>());

            // 2. Bin Lid (Beveled top)
            GameObject lid = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lid.name = "Lid";
            lid.transform.SetParent(root.transform, false);
            lid.transform.localPosition = new Vector3(0f, 1.35f, 0f);
            lid.transform.localScale = new Vector3(0.98f, 0.16f, 0.98f);
            if (lidMat != null) lid.GetComponent<MeshRenderer>().sharedMaterial = lidMat;
            UnityEngine.Object.DestroyImmediate(lid.GetComponent<Collider>());

            // 3. Front Plaque Backplate & Label Quad
            // Plaque backplate slightly extruding
            GameObject frontPlaque = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frontPlaque.name = "FrontPlaqueBacking";
            frontPlaque.transform.SetParent(root.transform, false);
            frontPlaque.transform.localPosition = new Vector3(0f, 0.75f, 0.455f);
            frontPlaque.transform.localScale = new Vector3(0.72f, 0.38f, 0.02f);
            if (plaqueMat != null) frontPlaque.GetComponent<MeshRenderer>().sharedMaterial = plaqueMat;
            UnityEngine.Object.DestroyImmediate(frontPlaque.GetComponent<Collider>());

            // Front Label Quad (512x256 aspect ratio: 0.68m wide x 0.34m high)
            GameObject frontQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            frontQuad.name = "FrontLabel";
            frontQuad.transform.SetParent(root.transform, false);
            frontQuad.transform.localPosition = new Vector3(0f, 0.75f, 0.468f);
            frontQuad.transform.localScale = new Vector3(0.68f, 0.34f, 1f);
            frontQuad.transform.localRotation = Quaternion.identity;
            if (labelMat != null) frontQuad.GetComponent<MeshRenderer>().sharedMaterial = labelMat;
            UnityEngine.Object.DestroyImmediate(frontQuad.GetComponent<Collider>());

            // 4. Back Plaque Backplate & Label Quad (for 360-degree visibility in the park)
            GameObject backPlaque = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backPlaque.name = "BackPlaqueBacking";
            backPlaque.transform.SetParent(root.transform, false);
            backPlaque.transform.localPosition = new Vector3(0f, 0.75f, -0.455f);
            backPlaque.transform.localScale = new Vector3(0.72f, 0.38f, 0.02f);
            if (plaqueMat != null) backPlaque.GetComponent<MeshRenderer>().sharedMaterial = plaqueMat;
            UnityEngine.Object.DestroyImmediate(backPlaque.GetComponent<Collider>());

            GameObject backQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            backQuad.name = "BackLabel";
            backQuad.transform.SetParent(root.transform, false);
            backQuad.transform.localPosition = new Vector3(0f, 0.75f, -0.468f);
            backQuad.transform.localScale = new Vector3(0.68f, 0.34f, 1f);
            backQuad.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            if (labelMat != null) backQuad.GetComponent<MeshRenderer>().sharedMaterial = labelMat;
            UnityEngine.Object.DestroyImmediate(backQuad.GetComponent<Collider>());

            // 5. Floating Category Billboard above the bin (y = 1.9m)
            GameObject floatObj = new GameObject("FloatingCategoryLabel");
            floatObj.transform.SetParent(root.transform, false);
            floatObj.transform.localPosition = new Vector3(0f, 1.9f, 0f);

            GameObject floatQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            floatQuad.name = "BillboardQuad";
            floatQuad.transform.SetParent(floatObj.transform, false);
            floatQuad.transform.localPosition = Vector3.zero;
            floatQuad.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            floatQuad.transform.localRotation = Quaternion.identity;
            if (floatMat != null) floatQuad.GetComponent<MeshRenderer>().sharedMaterial = floatMat;
            UnityEngine.Object.DestroyImmediate(floatQuad.GetComponent<Collider>());

            // 6. Highlight Light (matching bin color, disabled by default)
            GameObject lightObj = new GameObject("HighlightLight");
            lightObj.transform.SetParent(root.transform, false);
            lightObj.transform.localPosition = new Vector3(0f, 1.4f, 0f);
            Light pLight = lightObj.AddComponent<Light>();
            pLight.type = LightType.Point;
            pLight.color = themeColor;
            pLight.range = 3.5f;
            pLight.intensity = 2.0f;
            pLight.enabled = false;

            // 7. Root Physics & Trigger Collider
            // Physical BoxCollider for solid obstacles
            BoxCollider solidCol = root.AddComponent<BoxCollider>();
            solidCol.center = new Vector3(0f, 0.7f, 0f);
            solidCol.size = new Vector3(0.95f, 1.4f, 0.95f);
            solidCol.isTrigger = false;

            // Trigger SphereCollider for interaction zone
            SphereCollider trigCol = root.AddComponent<SphereCollider>();
            trigCol.center = new Vector3(0f, 0.7f, 0f);
            trigCol.radius = 1.8f;
            trigCol.isTrigger = true;

            // 8. RecyclingBin Component
            RecyclingBin binComp = root.AddComponent<RecyclingBin>();
            binComp.Configure(cat, body.GetComponent<MeshRenderer>());
            binComp.SetFloatingLabelTransform(floatObj.transform);
            binComp.SetHighlightLight(pLight);

            // Save Prefab
            string prefabFilePath = $"{PREFAB_PATH}/{prefabName}.prefab";
            PrefabUtility.SaveAsPrefabAsset(root, prefabFilePath);
            UnityEngine.Object.DestroyImmediate(root);

            Debug.Log($"[BinVisualUpgradePipeline] Prefab created: {prefabFilePath}");
        }

        private static void UpdateSceneRecyclingStation()
        {
            var scene = EditorSceneManager.OpenScene(SCENE_PATH, OpenSceneMode.Single);

            // Find existing RecyclingStation
            GameObject station = GameObject.Find("RecyclingStation");
            if (station == null)
            {
                station = new GameObject("RecyclingStation");
                station.transform.position = new Vector3(0f, 0f, 0f);
            }

            // Remove existing children under RecyclingStation
            int childCount = station.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(station.transform.GetChild(i).gameObject);
            }

            // Also check for any loose bins in the scene root and remove them
            RecyclingBin[] looseBins = UnityEngine.Object.FindObjectsOfType<RecyclingBin>();
            foreach (var lb in looseBins)
            {
                UnityEngine.Object.DestroyImmediate(lb.gameObject);
            }

            // Positions for the 4 bins in a line along the park walkway
            // Center is (0, 0, -2), spaced horizontally by 1.6m
            // 1. PAPER (Blue) at (-2.4, 0, -2)
            // 2. PLASTIC (Yellow) at (-0.8, 0, -2)
            // 3. METAL (Red) at (0.8, 0, -2)
            // 4. ORGANIC (Green) at (2.4, 0, -2)

            SpawnBinToStation(station.transform, "Bin_Paper", new Vector3(-2.4f, 0f, -2f), Quaternion.identity);
            SpawnBinToStation(station.transform, "Bin_Plastic", new Vector3(-0.8f, 0f, -2f), Quaternion.identity);
            SpawnBinToStation(station.transform, "Bin_Metal", new Vector3(0.8f, 0f, -2f), Quaternion.identity);
            SpawnBinToStation(station.transform, "Bin_Organic", new Vector3(2.4f, 0f, -2f), Quaternion.identity);

            // Re-cache bins in PlayerInteraction if present
            var playerInter = UnityEngine.Object.FindObjectOfType<TrashRush.Player.PlayerInteraction>();
            if (playerInter != null)
            {
                playerInter.RefreshBinsCache();
                EditorUtility.SetDirty(playerInter);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[BinVisualUpgradePipeline] Scene RecyclingStation updated with 4 upgraded bins and saved.");
        }

        private static void SpawnBinToStation(Transform stationParent, string prefabName, Vector3 localPos, Quaternion localRot)
        {
            string path = $"{PREFAB_PATH}/{prefabName}.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"[BinVisualUpgradePipeline] Could not load prefab: {path}");
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, stationParent);
            instance.transform.localPosition = localPos;
            instance.transform.localRotation = localRot;
            Debug.Log($"[BinVisualUpgradePipeline] Instantiated {prefabName} at {localPos}");
        }
    }
}
