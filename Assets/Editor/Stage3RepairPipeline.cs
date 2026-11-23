using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TrashRush.Art;
using TrashRush.Core;
using TrashRush.Player;
using TrashRush.Recycling;
using TrashRush.Trash;
using TrashRush.UI;

namespace TrashRush.Editor
{
    public static class Stage3RepairPipeline
    {
        [MenuItem("TrashRush/Stage 3 Repair — Fix All Issues")]
        public static void RunRepair()
        {
            Debug.Log("[Stage3Repair] Beginning Stage 3 Repair Pipeline...");

            // 1. Regenerate Category Label Textures with updated scheme:
            // PLASTIC = Blue, PAPER = Yellow, METAL = Gray, ORGANIC = Green
            UpdateCategoryColorsAndTextures();

            // 2. Rebuild Player Prefab with complete mesh, materials, and carry point
            RepairPlayerPrefab();

            // 3. Rebuild Bin Prefabs with updated colors and icons
            RepairRecyclingBins();

            // 4. Update GameScene with repaired Player, Bins, and Enhanced UI
            RepairGameScene();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Stage3Repair] Stage 3 Repair Pipeline completed successfully!");
        }

        private static void UpdateCategoryColorsAndTextures()
        {
            Debug.Log("[Stage3Repair] Updating category colors & label textures...");

            // Scheme:
            // PLASTIC = Blue (#0099FF)
            // PAPER = Yellow (#FFD700)
            // METAL = Gray (#757575)
            // ORGANIC = Green (#388E3C)

            Color plasticBlue = new Color(0.0f, 0.6f, 1.0f);
            Color paperYellow = new Color(1.0f, 0.84f, 0.0f);
            Color metalGray = new Color(0.48f, 0.48f, 0.48f);
            Color organicGreen = new Color(0.22f, 0.58f, 0.24f);

            SetMatColor("Assets/Art/Materials/M_Bin_Plastic.mat", plasticBlue);
            SetMatColor("Assets/Art/Materials/M_Bin_Paper.mat", paperYellow);
            SetMatColor("Assets/Art/Materials/M_Bin_Metal.mat", metalGray, 0.6f);
            SetMatColor("Assets/Art/Materials/M_Bin_Organic.mat", organicGreen);

            // Generate textures with high-contrast icons
            Texture2D texPlastic = ProceduralTextureGenerator.CreateBinLabelTexture("PLASTIC", plasticBlue, Color.white, "1");
            ProceduralTextureGenerator.SaveTextureAsPNG(texPlastic, "Assets/Art/Textures/Tex_Bin_Plastic.png");

            Texture2D texPaper = ProceduralTextureGenerator.CreateBinLabelTexture("PAPER", paperYellow, Color.black, "2");
            ProceduralTextureGenerator.SaveTextureAsPNG(texPaper, "Assets/Art/Textures/Tex_Bin_Paper.png");

            Texture2D texMetal = ProceduralTextureGenerator.CreateBinLabelTexture("METAL", metalGray, Color.white, "3");
            ProceduralTextureGenerator.SaveTextureAsPNG(texMetal, "Assets/Art/Textures/Tex_Bin_Metal.png");

            Texture2D texOrganic = ProceduralTextureGenerator.CreateBinLabelTexture("ORGANIC", organicGreen, Color.white, "4");
            ProceduralTextureGenerator.SaveTextureAsPNG(texOrganic, "Assets/Art/Textures/Tex_Bin_Organic.png");

            AssetDatabase.Refresh();

            // Update Label Materials
            AssignTextureToMat("Assets/Art/Materials/M_Label_PLASTIC.mat", "Assets/Art/Textures/Tex_Bin_Plastic.png");
            AssignTextureToMat("Assets/Art/Materials/M_Label_PAPER.mat", "Assets/Art/Textures/Tex_Bin_Paper.png");
            AssignTextureToMat("Assets/Art/Materials/M_Label_METAL.mat", "Assets/Art/Textures/Tex_Bin_Metal.png");
            AssignTextureToMat("Assets/Art/Materials/M_Label_ORGANIC.mat", "Assets/Art/Textures/Tex_Bin_Organic.png");
        }

        private static void SetMatColor(string path, Color c, float metallic = 0f)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Standard"));
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.color = c;
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Glossiness", 0.3f);
            EditorUtility.SetDirty(mat);
        }

        private static void AssignTextureToMat(string matPath, string texPath)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Standard"));
                AssetDatabase.CreateAsset(mat, matPath);
            }
            if (tex != null)
            {
                mat.mainTexture = tex;
                mat.color = Color.white;
            }
            EditorUtility.SetDirty(mat);
        }

        private static void RepairPlayerPrefab()
        {
            Debug.Log("[Stage3Repair] Repairing Player Prefab...");

            GameObject player = new GameObject("Player");
            player.tag = "Player";

            CharacterController cc = player.AddComponent<CharacterController>();
            cc.center = new Vector3(0, 0.95f, 0);
            cc.height = 1.9f;
            cc.radius = 0.35f;
            cc.stepOffset = 0.35f;
            cc.slopeLimit = 45f;
            cc.minMoveDistance = 0.001f;

            // VisualRoot
            GameObject visualRoot = new GameObject("VisualRoot");
            visualRoot.transform.SetParent(player.transform, false);

            // Load character mesh
            Mesh playerMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Models/Mesh_Player.asset");
            if (playerMesh == null)
            {
                playerMesh = LowPolyMeshBuilder.CreatePlayerMesh();
                AssetDatabase.CreateAsset(playerMesh, "Assets/Art/Models/Mesh_Player.asset");
            }

            MeshFilter mf = visualRoot.AddComponent<MeshFilter>();
            mf.sharedMesh = playerMesh;

            MeshRenderer mr = visualRoot.AddComponent<MeshRenderer>();
            Material bodyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Player_Body.mat");
            mr.sharedMaterial = bodyMat;
            mr.enabled = true;

            // Dedicated CarryPoint positioned right in front of chest/hands
            GameObject carryPoint = new GameObject("CarryPoint");
            carryPoint.transform.SetParent(player.transform, false);
            carryPoint.transform.localPosition = new Vector3(0.0f, 1.05f, 0.55f);

            PlayerController pc = player.AddComponent<PlayerController>();
            SerializedObject soPC = new SerializedObject(pc);
            soPC.FindProperty("visualRoot").objectReferenceValue = visualRoot.transform;
            soPC.ApplyModifiedProperties();

            PlayerInteraction pi = player.AddComponent<PlayerInteraction>();
            SerializedObject soPI = new SerializedObject(pi);
            soPI.FindProperty("carryPoint").objectReferenceValue = carryPoint.transform;
            soPI.ApplyModifiedProperties();

            PrefabUtility.SaveAsPrefabAsset(player, "Assets/Prefabs/Player/Player.prefab");
            GameObject.DestroyImmediate(player);
        }

        private static void RepairRecyclingBins()
        {
            Debug.Log("[Stage3Repair] Repairing Bin Prefabs...");

            RebuildBinPrefab("Assets/Prefabs/RecyclingBins/Bin_Plastic.prefab", TrashCategory.Plastic, "PLASTIC", "M_Bin_Plastic", "M_Label_PLASTIC");
            RebuildBinPrefab("Assets/Prefabs/RecyclingBins/Bin_Paper.prefab", TrashCategory.Paper, "PAPER", "M_Bin_Paper", "M_Label_PAPER");
            RebuildBinPrefab("Assets/Prefabs/RecyclingBins/Bin_Metal.prefab", TrashCategory.Metal, "METAL", "M_Bin_Metal", "M_Label_METAL");
            RebuildBinPrefab("Assets/Prefabs/RecyclingBins/Bin_Organic.prefab", TrashCategory.Organic, "ORGANIC", "M_Bin_Organic", "M_Label_ORGANIC");
        }

        private static void RebuildBinPrefab(string prefabPath, TrashCategory category, string labelText, string binMatName, string labelMatName)
        {
            GameObject binRoot = new GameObject($"Bin_{labelText}");

            // Body
            GameObject bodyObj = new GameObject("BinBody");
            bodyObj.transform.SetParent(binRoot.transform, false);
            MeshFilter mfBody = bodyObj.AddComponent<MeshFilter>();
            mfBody.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Models/Mesh_BinBody.asset");
            MeshRenderer mrBody = bodyObj.AddComponent<MeshRenderer>();
            mrBody.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Art/Materials/{binMatName}.mat");

            // Lid Hinge & Mesh
            GameObject lidPivot = new GameObject("LidPivot");
            lidPivot.transform.SetParent(binRoot.transform, false);
            lidPivot.transform.localPosition = new Vector3(0, 1.15f, -0.45f);

            GameObject lidObj = new GameObject("BinLid");
            lidObj.transform.SetParent(lidPivot.transform, false);
            lidObj.transform.localPosition = new Vector3(0, 0, 0.45f);
            MeshFilter mfLid = lidObj.AddComponent<MeshFilter>();
            mfLid.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Models/Mesh_BinLid.asset");
            MeshRenderer mrLid = lidObj.AddComponent<MeshRenderer>();
            mrLid.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Bin_Lid.mat");

            // Front Label Quad
            GameObject labelQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            labelQuad.name = "CategoryLabelFront";
            labelQuad.transform.SetParent(binRoot.transform, false);
            labelQuad.transform.localPosition = new Vector3(0, 0.75f, 0.47f);
            labelQuad.transform.localScale = new Vector3(0.55f, 0.28f, 1f);
            GameObject.DestroyImmediate(labelQuad.GetComponent<Collider>());
            labelQuad.GetComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Art/Materials/{labelMatName}.mat");

            // Back Label Quad
            GameObject labelBack = GameObject.CreatePrimitive(PrimitiveType.Quad);
            labelBack.name = "CategoryLabelBack";
            labelBack.transform.SetParent(binRoot.transform, false);
            labelBack.transform.localPosition = new Vector3(0, 0.75f, -0.47f);
            labelBack.transform.localScale = new Vector3(0.55f, 0.28f, 1f);
            labelBack.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            GameObject.DestroyImmediate(labelBack.GetComponent<Collider>());
            labelBack.GetComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Art/Materials/{labelMatName}.mat");

            // Trigger & Physics
            BoxCollider col = binRoot.AddComponent<BoxCollider>();
            col.center = new Vector3(0, 0.6f, 0);
            col.size = new Vector3(1.3f, 1.5f, 1.3f);
            col.isTrigger = true;

            RecyclingBin binComp = binRoot.AddComponent<RecyclingBin>();
            binComp.SetCategory(category, labelText);

            SerializedObject so = new SerializedObject(binComp);
            so.FindProperty("lidTransform").objectReferenceValue = lidPivot.transform;
            so.ApplyModifiedProperties();

            PrefabUtility.SaveAsPrefabAsset(binRoot, prefabPath);
            GameObject.DestroyImmediate(binRoot);
        }

        private static void RepairGameScene()
        {
            Debug.Log("[Stage3Repair] Re-syncing GameScene with repaired Player, Bins, and UI...");

            var scene = EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");

            // 1. Replace existing Player instance with repaired Prefab
            GameObject oldPlayer = GameObject.Find("Player") ?? GameObject.FindWithTag("Player");
            Vector3 spawnPos = new Vector3(0f, 0.05f, -3.0f);
            if (oldPlayer != null)
            {
                spawnPos = oldPlayer.transform.position;
                GameObject.DestroyImmediate(oldPlayer);
            }

            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/Player.prefab");
            GameObject newPlayer = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
            newPlayer.name = "Player";
            newPlayer.transform.position = spawnPos;

            // 2. Bind Camera
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                ThirdPersonCameraController camCtrl = mainCam.GetComponent<ThirdPersonCameraController>();
                if (camCtrl != null)
                {
                    camCtrl.SetTarget(newPlayer.transform);
                }
            }

            // 3. Re-place Bins in the Recycling Station
            GameObject stationRoot = GameObject.Find("RecyclingStation");
            if (stationRoot != null)
            {
                for (int i = stationRoot.transform.childCount - 1; i >= 0; i--)
                {
                    Transform child = stationRoot.transform.GetChild(i);
                    if (child.name.StartsWith("Bin_"))
                    {
                        GameObject.DestroyImmediate(child.gameObject);
                    }
                }

                string[] binPaths = {
                    "Assets/Prefabs/RecyclingBins/Bin_Plastic.prefab",
                    "Assets/Prefabs/RecyclingBins/Bin_Paper.prefab",
                    "Assets/Prefabs/RecyclingBins/Bin_Metal.prefab",
                    "Assets/Prefabs/RecyclingBins/Bin_Organic.prefab"
                };
                float[] xOffsets = { -3.2f, -1.1f, 1.1f, 3.2f };
                for (int i = 0; i < 4; i++)
                {
                    var p = AssetDatabase.LoadAssetAtPath<GameObject>(binPaths[i]);
                    var bin = PrefabUtility.InstantiatePrefab(p) as GameObject;
                    bin.transform.SetParent(stationRoot.transform, false);
                    bin.transform.localPosition = new Vector3(xOffsets[i], 0.08f, 0);
                }
            }

            // 4. Update UI Canvas with Prompt, Carried Item, and Feedback Toast
            RepairCanvasUI();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void RepairCanvasUI()
        {
            GameObject canvasObj = GameObject.Find("GameCanvas");
            if (canvasObj == null) return;

            // Find or create Carried Item Text
            GameObject carriedObj = GameObject.Find("CarriedItemText");
            if (carriedObj == null)
            {
                carriedObj = new GameObject("CarriedItemText");
                carriedObj.transform.SetParent(canvasObj.transform, false);
                Text t = carriedObj.AddComponent<Text>();
                t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                t.fontSize = 24;
                t.fontStyle = FontStyle.Bold;
                t.color = Color.white;
                t.alignment = TextAnchor.MiddleLeft;
                RectTransform rt = carriedObj.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0f, 0f);
                rt.anchorMax = new Vector2(0f, 0f);
                rt.pivot = new Vector2(0f, 0f);
                rt.anchoredPosition = new Vector2(40f, 40f);
                rt.sizeDelta = new Vector2(600f, 40f);
            }

            // Find or create Toast Feedback Text
            GameObject feedbackObj = GameObject.Find("FeedbackToastText");
            if (feedbackObj == null)
            {
                feedbackObj = new GameObject("FeedbackToastText");
                feedbackObj.transform.SetParent(canvasObj.transform, false);
                Text t = feedbackObj.AddComponent<Text>();
                t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                t.fontSize = 28;
                t.fontStyle = FontStyle.Bold;
                t.color = Color.yellow;
                t.alignment = TextAnchor.MiddleCenter;
                RectTransform rt = feedbackObj.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.35f);
                rt.anchorMax = new Vector2(0.5f, 0.35f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(900f, 50f);
            }

            UIManager uiMgr = canvasObj.GetComponent<UIManager>();
            if (uiMgr != null)
            {
                Text scoreText = GameObject.Find("ScoreText")?.GetComponent<Text>();
                Text timerText = GameObject.Find("TimerText")?.GetComponent<Text>();
                Text tcText = GameObject.Find("TrashCountText")?.GetComponent<Text>();
                Text promptText = GameObject.Find("PromptText")?.GetComponent<Text>();
                Text carriedText = carriedObj.GetComponent<Text>();
                Text fbText = feedbackObj.GetComponent<Text>();

                GameObject pausePanel = GameObject.Find("PausePanel");
                GameObject victoryPanel = GameObject.Find("VictoryPanel");
                GameObject gameOverPanel = GameObject.Find("GameOverPanel");
                Text vScore = GameObject.Find("VictoryPanel/Score")?.GetComponent<Text>();
                Text goScore = GameObject.Find("GameOverPanel/Score")?.GetComponent<Text>();

                uiMgr.BindUI(scoreText, timerText, tcText, promptText, carriedText, fbText, pausePanel, victoryPanel, gameOverPanel, vScore, goScore);
            }
        }
    }
}
