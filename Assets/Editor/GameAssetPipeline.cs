using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TrashRush.Art;
using TrashRush.Audio;
using TrashRush.Core;
using TrashRush.Player;
using TrashRush.Recycling;
using TrashRush.Trash;
using TrashRush.UI;

namespace TrashRush.Editor
{
    public static class GameAssetPipeline
    {
        [MenuItem("TrashRush/Generate All Assets and Build Scene")]
        public static void GenerateAllAndBuild()
        {
            Debug.Log("[GameAssetPipeline] Starting Asset Generation Pipeline...");

            EnsureDirectories();
            GenerateAudioAssets();
            GenerateTexturesAndMaterials();
            GenerateMeshAssets();
            GeneratePrefabs();
            BuildGameScene();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[GameAssetPipeline] Asset Generation and Scene Construction Complete!");
        }

        private static void EnsureDirectories()
        {
            string[] dirs = {
                "Assets/Art/Materials",
                "Assets/Art/Models",
                "Assets/Art/Textures",
                "Assets/Audio/Music",
                "Assets/Audio/SFX",
                "Assets/Prefabs/Environment",
                "Assets/Prefabs/Player",
                "Assets/Prefabs/RecyclingBins",
                "Assets/Prefabs/Trash",
                "Assets/Prefabs/UI",
                "Assets/Scenes"
            };

            foreach (var d in dirs)
            {
                if (!Directory.Exists(d))
                {
                    Directory.CreateDirectory(d);
                }
            }
        }

        private static void GenerateAudioAssets()
        {
            Debug.Log("[GameAssetPipeline] Generating Audio files...");

            // 1. BGM
            ProceduralAudioGenerator.GenerateWav("Assets/Audio/Music/BGM_ParkLoop.wav", ProceduralAudioGenerator.CreateBGM(16f));

            // 2. SFX
            ProceduralAudioGenerator.GenerateWav("Assets/Audio/SFX/SFX_Pickup.wav", ProceduralAudioGenerator.CreatePickupSFX());
            ProceduralAudioGenerator.GenerateWav("Assets/Audio/SFX/SFX_CorrectSort.wav", ProceduralAudioGenerator.CreateCorrectSortSFX());
            ProceduralAudioGenerator.GenerateWav("Assets/Audio/SFX/SFX_IncorrectSort.wav", ProceduralAudioGenerator.CreateIncorrectSortSFX());
            ProceduralAudioGenerator.GenerateWav("Assets/Audio/SFX/SFX_ButtonClick.wav", ProceduralAudioGenerator.CreateButtonClickSFX());
            ProceduralAudioGenerator.GenerateWav("Assets/Audio/SFX/SFX_Victory.wav", ProceduralAudioGenerator.CreateVictoryJingle());
            ProceduralAudioGenerator.GenerateWav("Assets/Audio/SFX/SFX_GameOver.wav", ProceduralAudioGenerator.CreateGameOverSFX());
            ProceduralAudioGenerator.GenerateWav("Assets/Audio/SFX/SFX_Footstep.wav", ProceduralAudioGenerator.CreateFootstepSFX());

            AssetDatabase.Refresh();
        }

        private static void GenerateTexturesAndMaterials()
        {
            Debug.Log("[GameAssetPipeline] Generating Textures and Materials...");

            // Color Palette
            CreateMaterial("Assets/Art/Materials/M_Grass.mat", new Color(0.30f, 0.69f, 0.31f)); // #4CAF50
            CreateMaterial("Assets/Art/Materials/M_Path.mat", new Color(0.82f, 0.71f, 0.55f));  // #D2B48C
            CreateMaterial("Assets/Art/Materials/M_TreeTrunk.mat", new Color(0.47f, 0.33f, 0.28f)); // #795548
            CreateMaterial("Assets/Art/Materials/M_TreeFoliage_Dark.mat", new Color(0.18f, 0.49f, 0.20f)); // #2E7D32
            CreateMaterial("Assets/Art/Materials/M_TreeFoliage_Light.mat", new Color(0.40f, 0.73f, 0.42f)); // #66BB6A
            CreateMaterial("Assets/Art/Materials/M_Bush.mat", new Color(0.26f, 0.63f, 0.28f)); // #43A047
            CreateMaterial("Assets/Art/Materials/M_Wood.mat", new Color(0.55f, 0.43f, 0.39f)); // #8D6E63
            CreateMaterial("Assets/Art/Materials/M_Metal_Light.mat", new Color(0.74f, 0.74f, 0.74f), 0.8f, 0.4f);
            CreateMaterial("Assets/Art/Materials/M_Metal_Dark.mat", new Color(0.38f, 0.38f, 0.38f), 0.8f, 0.3f);
            CreateMaterial("Assets/Art/Materials/M_Plastic_Clear.mat", new Color(0.70f, 0.90f, 0.99f));
            CreateMaterial("Assets/Art/Materials/M_Plastic_White.mat", new Color(0.96f, 0.96f, 0.96f));
            CreateMaterial("Assets/Art/Materials/M_Paper.mat", new Color(1.0f, 0.98f, 0.77f));
            CreateMaterial("Assets/Art/Materials/M_Cardboard.mat", new Color(0.74f, 0.67f, 0.64f));
            CreateMaterial("Assets/Art/Materials/M_Organic_Green.mat", new Color(0.41f, 0.62f, 0.22f));
            CreateMaterial("Assets/Art/Materials/M_Organic_Yellow.mat", new Color(1.0f, 0.84f, 0.31f));
            CreateMaterial("Assets/Art/Materials/M_Organic_Red.mat", new Color(0.90f, 0.22f, 0.21f));
            
            // Bins
            CreateMaterial("Assets/Art/Materials/M_Bin_Plastic.mat", new Color(0.99f, 0.85f, 0.21f)); // Yellow #FDD835
            CreateMaterial("Assets/Art/Materials/M_Bin_Paper.mat", new Color(0.12f, 0.53f, 0.90f));   // Blue #1E88E5
            CreateMaterial("Assets/Art/Materials/M_Bin_Metal.mat", new Color(0.46f, 0.46f, 0.46f), 0.6f, 0.3f); // Gray #757575
            CreateMaterial("Assets/Art/Materials/M_Bin_Organic.mat", new Color(0.26f, 0.63f, 0.28f)); // Green #43A047

            // Player & Environment Props
            CreateMaterial("Assets/Art/Materials/M_Player_Body.mat", new Color(0.16f, 0.45f, 0.85f));
            CreateMaterial("Assets/Art/Materials/M_Player_Skin.mat", new Color(1.0f, 0.80f, 0.50f));
            CreateMaterial("Assets/Art/Materials/M_Stone.mat", new Color(0.62f, 0.62f, 0.62f));
            CreateMaterial("Assets/Art/Materials/M_Water.mat", new Color(0.31f, 0.76f, 0.97f));
            CreateMaterial("Assets/Art/Materials/M_Lamp_Post.mat", new Color(0.26f, 0.26f, 0.26f), 0.8f, 0.3f);
            CreateMaterial("Assets/Art/Materials/M_Lamp_Light.mat", new Color(1.0f, 0.93f, 0.70f), 0f, 0f, true);

            // Bin Textures
            Texture2D texPlastic = ProceduralTextureGenerator.CreateBinLabelTexture("PLASTIC", new Color(0.99f, 0.85f, 0.21f), Color.black, "1");
            ProceduralTextureGenerator.SaveTextureAsPNG(texPlastic, "Assets/Art/Textures/Tex_Bin_Plastic.png");

            Texture2D texPaper = ProceduralTextureGenerator.CreateBinLabelTexture("PAPER", new Color(0.12f, 0.53f, 0.90f), Color.white, "2");
            ProceduralTextureGenerator.SaveTextureAsPNG(texPaper, "Assets/Art/Textures/Tex_Bin_Paper.png");

            Texture2D texMetal = ProceduralTextureGenerator.CreateBinLabelTexture("METAL", new Color(0.46f, 0.46f, 0.46f), Color.white, "3");
            ProceduralTextureGenerator.SaveTextureAsPNG(texMetal, "Assets/Art/Textures/Tex_Bin_Metal.png");

            Texture2D texOrganic = ProceduralTextureGenerator.CreateBinLabelTexture("ORGANIC", new Color(0.26f, 0.63f, 0.28f), Color.white, "4");
            ProceduralTextureGenerator.SaveTextureAsPNG(texOrganic, "Assets/Art/Textures/Tex_Bin_Organic.png");

            AssetDatabase.Refresh();
        }

        private static Material CreateMaterial(string path, Color albedo, float metallic = 0f, float smoothness = 0.2f, bool emissive = false)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                Shader shader = Shader.Find("Standard");
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.color = albedo;
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Glossiness", smoothness);
            if (emissive)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", albedo * 1.5f);
            }
            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static void GenerateMeshAssets()
        {
            Debug.Log("[GameAssetPipeline] Saving Procedural Meshes as assets...");

            SaveMesh(LowPolyMeshBuilder.CreatePlasticBottleMesh(), "Assets/Art/Models/Mesh_PlasticBottle.asset");
            SaveMesh(LowPolyMeshBuilder.CreatePlasticCupMesh(), "Assets/Art/Models/Mesh_PlasticCup.asset");
            SaveMesh(LowPolyMeshBuilder.CreatePlasticContainerMesh(), "Assets/Art/Models/Mesh_PlasticContainer.asset");

            SaveMesh(LowPolyMeshBuilder.CreateNewspaperMesh(), "Assets/Art/Models/Mesh_Newspaper.asset");
            SaveMesh(LowPolyMeshBuilder.CreateCardboardBoxMesh(), "Assets/Art/Models/Mesh_CardboardBox.asset");
            SaveMesh(LowPolyMeshBuilder.CreatePaperBallMesh(), "Assets/Art/Models/Mesh_PaperBall.asset");

            SaveMesh(LowPolyMeshBuilder.CreateSodaCanMesh(), "Assets/Art/Models/Mesh_SodaCan.asset");
            SaveMesh(LowPolyMeshBuilder.CreateFoodCanMesh(), "Assets/Art/Models/Mesh_FoodCan.asset");
            SaveMesh(LowPolyMeshBuilder.CreateMetalLidMesh(), "Assets/Art/Models/Mesh_MetalLid.asset");

            SaveMesh(LowPolyMeshBuilder.CreateBananaPeelMesh(), "Assets/Art/Models/Mesh_BananaPeel.asset");
            SaveMesh(LowPolyMeshBuilder.CreateAppleCoreMesh(), "Assets/Art/Models/Mesh_AppleCore.asset");
            SaveMesh(LowPolyMeshBuilder.CreateLeafPileMesh(), "Assets/Art/Models/Mesh_LeafPile.asset");

            SaveMesh(LowPolyMeshBuilder.CreateRecyclingBinBodyMesh(), "Assets/Art/Models/Mesh_BinBody.asset");
            SaveMesh(LowPolyMeshBuilder.CreateRecyclingBinLidMesh(), "Assets/Art/Models/Mesh_BinLid.asset");

            SaveMesh(LowPolyMeshBuilder.CreatePineTreeMesh(), "Assets/Art/Models/Mesh_PineTree.asset");
            SaveMesh(LowPolyMeshBuilder.CreateRoundTreeMesh(), "Assets/Art/Models/Mesh_RoundTree.asset");
            SaveMesh(LowPolyMeshBuilder.CreateBushMesh(1.0f), "Assets/Art/Models/Mesh_Bush.asset");
            SaveMesh(LowPolyMeshBuilder.CreateParkBenchMesh(), "Assets/Art/Models/Mesh_ParkBench.asset");
            SaveMesh(LowPolyMeshBuilder.CreateStreetLampMesh(), "Assets/Art/Models/Mesh_StreetLamp.asset");
            SaveMesh(LowPolyMeshBuilder.CreateFenceSectionMesh(), "Assets/Art/Models/Mesh_FenceSection.asset");
            SaveMesh(LowPolyMeshBuilder.CreateRockMesh(1.0f), "Assets/Art/Models/Mesh_Rock.asset");
            SaveMesh(LowPolyMeshBuilder.CreateParkSignMesh(), "Assets/Art/Models/Mesh_ParkSign.asset");
            SaveMesh(LowPolyMeshBuilder.CreatePlayerMesh(), "Assets/Art/Models/Mesh_Player.asset");

            AssetDatabase.Refresh();
        }

        private static void SaveMesh(Mesh mesh, string path)
        {
            Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing != null)
            {
                EditorUtility.CopySerialized(mesh, existing);
            }
            else
            {
                AssetDatabase.CreateAsset(mesh, path);
            }
        }

        private static void GeneratePrefabs()
        {
            Debug.Log("[GameAssetPipeline] Assembling Prefabs...");

            // 1. Trash Prefabs
            CreateTrashPrefab("Assets/Prefabs/Trash/Trash_PlasticBottle.prefab", "Mesh_PlasticBottle", "M_Plastic_Clear", TrashCategory.Plastic, "Plastic Bottle");
            CreateTrashPrefab("Assets/Prefabs/Trash/Trash_PlasticCup.prefab", "Mesh_PlasticCup", "M_Plastic_White", TrashCategory.Plastic, "Plastic Cup");
            CreateTrashPrefab("Assets/Prefabs/Trash/Trash_PlasticContainer.prefab", "Mesh_PlasticContainer", "M_Plastic_White", TrashCategory.Plastic, "Food Container");

            CreateTrashPrefab("Assets/Prefabs/Trash/Trash_Newspaper.prefab", "Mesh_Newspaper", "M_Paper", TrashCategory.Paper, "Newspaper");
            CreateTrashPrefab("Assets/Prefabs/Trash/Trash_CardboardBox.prefab", "Mesh_CardboardBox", "M_Cardboard", TrashCategory.Paper, "Cardboard Box");
            CreateTrashPrefab("Assets/Prefabs/Trash/Trash_PaperBall.prefab", "Mesh_PaperBall", "M_Paper", TrashCategory.Paper, "Crumpled Paper");

            CreateTrashPrefab("Assets/Prefabs/Trash/Trash_SodaCan.prefab", "Mesh_SodaCan", "M_Metal_Light", TrashCategory.Metal, "Soda Can");
            CreateTrashPrefab("Assets/Prefabs/Trash/Trash_FoodCan.prefab", "Mesh_FoodCan", "M_Metal_Dark", TrashCategory.Metal, "Food Can");
            CreateTrashPrefab("Assets/Prefabs/Trash/Trash_MetalLid.prefab", "Mesh_MetalLid", "M_Metal_Light", TrashCategory.Metal, "Metal Jar Lid");

            CreateTrashPrefab("Assets/Prefabs/Trash/Trash_BananaPeel.prefab", "Mesh_BananaPeel", "M_Organic_Yellow", TrashCategory.Organic, "Banana Peel");
            CreateTrashPrefab("Assets/Prefabs/Trash/Trash_AppleCore.prefab", "Mesh_AppleCore", "M_Organic_Red", TrashCategory.Organic, "Apple Core");
            CreateTrashPrefab("Assets/Prefabs/Trash/Trash_LeafPile.prefab", "Mesh_LeafPile", "M_Organic_Green", TrashCategory.Organic, "Leaf Pile");

            // 2. Recycling Bins
            CreateBinPrefab("Assets/Prefabs/RecyclingBins/Bin_Plastic.prefab", TrashCategory.Plastic, "Plastic", "M_Bin_Plastic");
            CreateBinPrefab("Assets/Prefabs/RecyclingBins/Bin_Paper.prefab", TrashCategory.Paper, "Paper", "M_Bin_Paper");
            CreateBinPrefab("Assets/Prefabs/RecyclingBins/Bin_Metal.prefab", TrashCategory.Metal, "Metal", "M_Bin_Metal");
            CreateBinPrefab("Assets/Prefabs/RecyclingBins/Bin_Organic.prefab", TrashCategory.Organic, "Organic", "M_Bin_Organic");

            // 3. Environment Prefabs
            CreateSimplePrefab("Assets/Prefabs/Environment/Env_PineTree.prefab", "Mesh_PineTree", "M_TreeFoliage_Dark");
            CreateSimplePrefab("Assets/Prefabs/Environment/Env_RoundTree.prefab", "Mesh_RoundTree", "M_TreeFoliage_Light");
            CreateSimplePrefab("Assets/Prefabs/Environment/Env_Bush.prefab", "Mesh_Bush", "M_Bush");
            CreateSimplePrefab("Assets/Prefabs/Environment/Env_ParkBench.prefab", "Mesh_ParkBench", "M_Wood");
            CreateSimplePrefab("Assets/Prefabs/Environment/Env_StreetLamp.prefab", "Mesh_StreetLamp", "M_Lamp_Post");
            CreateSimplePrefab("Assets/Prefabs/Environment/Env_Fence.prefab", "Mesh_FenceSection", "M_Wood");
            CreateSimplePrefab("Assets/Prefabs/Environment/Env_Rock.prefab", "Mesh_Rock", "M_Stone");
            CreateSimplePrefab("Assets/Prefabs/Environment/Env_ParkSign.prefab", "Mesh_ParkSign", "M_Wood");

            // 4. Player Prefab
            CreatePlayerPrefab("Assets/Prefabs/Player/Player.prefab");

            AssetDatabase.Refresh();
        }

        private static void CreateTrashPrefab(string prefabPath, string meshName, string matName, TrashCategory category, string displayName)
        {
            GameObject go = new GameObject(displayName);
            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>($"Assets/Art/Models/{meshName}.asset");

            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Art/Materials/{matName}.mat");

            BoxCollider col = go.AddComponent<BoxCollider>();
            col.isTrigger = false;

            Rigidbody rb = go.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            TrashItem item = go.AddComponent<TrashItem>();
            item.SetData(category, displayName);

            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            GameObject.DestroyImmediate(go);
        }

        private static void CreateBinPrefab(string prefabPath, TrashCategory category, string displayName, string matName)
        {
            GameObject binRoot = new GameObject($"Bin_{displayName}");
            
            // Body
            GameObject bodyObj = new GameObject("BinBody");
            bodyObj.transform.SetParent(binRoot.transform, false);
            MeshFilter mfBody = bodyObj.AddComponent<MeshFilter>();
            mfBody.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Models/Mesh_BinBody.asset");
            MeshRenderer mrBody = bodyObj.AddComponent<MeshRenderer>();
            mrBody.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Art/Materials/{matName}.mat");

            // Lid
            GameObject lidObj = new GameObject("BinLid");
            lidObj.transform.SetParent(binRoot.transform, false);
            lidObj.transform.localPosition = new Vector3(0, 1.15f, 0);
            MeshFilter mfLid = lidObj.AddComponent<MeshFilter>();
            mfLid.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Models/Mesh_BinLid.asset");
            MeshRenderer mrLid = lidObj.AddComponent<MeshRenderer>();
            mrLid.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Metal_Dark.mat");

            // Label plaque
            GameObject labelQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            labelQuad.name = "CategoryLabel";
            labelQuad.transform.SetParent(binRoot.transform, false);
            labelQuad.transform.localPosition = new Vector3(0, 0.75f, 0.44f);
            labelQuad.transform.localScale = new Vector3(0.5f, 0.25f, 1f);
            labelQuad.transform.localRotation = Quaternion.Euler(0, 0, 0);
            GameObject.DestroyImmediate(labelQuad.GetComponent<Collider>());

            // Trigger & Physics
            BoxCollider col = binRoot.AddComponent<BoxCollider>();
            col.center = new Vector3(0, 0.6f, 0);
            col.size = new Vector3(1.1f, 1.4f, 1.1f);
            col.isTrigger = true;

            RecyclingBin binComp = binRoot.AddComponent<RecyclingBin>();
            binComp.SetCategory(category, displayName);

            // Set lid field through reflection or serialized property
            SerializedObject so = new SerializedObject(binComp);
            so.FindProperty("lidTransform").objectReferenceValue = lidObj.transform;
            so.ApplyModifiedProperties();

            PrefabUtility.SaveAsPrefabAsset(binRoot, prefabPath);
            GameObject.DestroyImmediate(binRoot);
        }

        private static void CreateSimplePrefab(string prefabPath, string meshName, string matName)
        {
            GameObject go = new GameObject(Path.GetFileNameWithoutExtension(prefabPath));
            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>($"Assets/Art/Models/{meshName}.asset");
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Art/Materials/{matName}.mat");
            go.AddComponent<BoxCollider>();

            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            GameObject.DestroyImmediate(go);
        }

        private static void CreatePlayerPrefab(string prefabPath)
        {
            GameObject player = new GameObject("Player");
            CharacterController cc = player.AddComponent<CharacterController>();
            cc.center = new Vector3(0, 0.95f, 0);
            cc.height = 1.9f;
            cc.radius = 0.35f;

            GameObject visualRoot = new GameObject("VisualRoot");
            visualRoot.transform.SetParent(player.transform, false);

            MeshFilter mf = visualRoot.AddComponent<MeshFilter>();
            mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Models/Mesh_Player.asset");
            MeshRenderer mr = visualRoot.AddComponent<MeshRenderer>();
            mr.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Player_Body.mat");

            GameObject carryPoint = new GameObject("CarryPoint");
            carryPoint.transform.SetParent(player.transform, false);
            carryPoint.transform.localPosition = new Vector3(0.35f, 0.9f, 0.45f);

            PlayerController pc = player.AddComponent<PlayerController>();
            SerializedObject soPC = new SerializedObject(pc);
            soPC.FindProperty("visualRoot").objectReferenceValue = visualRoot.transform;
            soPC.ApplyModifiedProperties();

            PlayerInteraction pi = player.AddComponent<PlayerInteraction>();
            SerializedObject soPI = new SerializedObject(pi);
            soPI.FindProperty("carryPoint").objectReferenceValue = carryPoint.transform;
            soPI.ApplyModifiedProperties();

            PrefabUtility.SaveAsPrefabAsset(player, prefabPath);
            GameObject.DestroyImmediate(player);
        }

        public static void BuildGameScene()
        {
            Debug.Log("[GameAssetPipeline] Building Complete Game Scene...");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Lighting & Environment
            GameObject sun = new GameObject("Directional Light");
            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1.0f, 0.96f, 0.88f);
            light.intensity = 1.25f;
            light.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.45f, 0.55f, 0.65f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.75f, 0.85f, 0.92f);
            RenderSettings.fogDensity = 0.008f;

            // 2. Terrain & Park Ground
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "ParkGround";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(6f, 1f, 6f); // 60m x 60m park
            ground.GetComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Grass.mat");

            // Walking Paths
            CreateWalkingPaths();

            // Central Pond
            GameObject pond = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pond.name = "ParkPond";
            pond.transform.position = new Vector3(-14f, 0.05f, 10f);
            pond.transform.localScale = new Vector3(10f, 0.05f, 7f);
            pond.GetComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Water.mat");

            // 3. Recycling Station
            BuildRecyclingStation();

            // 4. Environmental Props & Foliage
            PopulateParkProps();

            // 5. Scatter Trash Items (16 items: 4 of each category)
            ScatterTrashItems();

            // 6. Player & Camera
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/Player.prefab");
            GameObject player = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
            player.transform.position = new Vector3(0, 0.01f, 0);

            GameObject camObj = new GameObject("Main Camera");
            Camera cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
            cam.clearFlags = CameraClearFlags.Skybox;
            camObj.AddComponent<AudioListener>();
            ThirdPersonCameraController camCtrl = camObj.AddComponent<ThirdPersonCameraController>();
            camCtrl.SetTarget(player.transform);

            // 7. Audio Manager
            GameObject audioMgrObj = new GameObject("AudioManager");
            AudioManager audioMgr = audioMgrObj.AddComponent<AudioManager>();
            audioMgr.SetClips(
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Music/BGM_ParkLoop.wav"),
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_Pickup.wav"),
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_CorrectSort.wav"),
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_IncorrectSort.wav"),
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_ButtonClick.wav"),
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_Victory.wav"),
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_GameOver.wav"),
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/SFX_Footstep.wav")
            );

            // 8. Game Manager
            GameObject gmObj = new GameObject("GameManager");
            gmObj.AddComponent<GameManager>();

            // 9. UI Canvas
            BuildGameUI();

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/SampleScene.unity");
            Debug.Log("[GameAssetPipeline] Scene successfully built and saved to Assets/Scenes/SampleScene.unity");
        }

        private static void CreateWalkingPaths()
        {
            GameObject pathsRoot = new GameObject("WalkingPaths");
            Material pathMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Path.mat");

            // Main path crossing park N-S and E-W
            GameObject pathNS = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pathNS.name = "Path_NorthSouth";
            pathNS.transform.SetParent(pathsRoot.transform);
            pathNS.transform.position = new Vector3(0, 0.02f, 0);
            pathNS.transform.localScale = new Vector3(3f, 0.04f, 55f);
            pathNS.GetComponent<MeshRenderer>().sharedMaterial = pathMat;

            GameObject pathEW = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pathEW.name = "Path_EastWest";
            pathEW.transform.SetParent(pathsRoot.transform);
            pathEW.transform.position = new Vector3(0, 0.02f, 0);
            pathEW.transform.localScale = new Vector3(55f, 0.04f, 3f);
            pathEW.GetComponent<MeshRenderer>().sharedMaterial = pathMat;

            // Circular plaza path
            GameObject plaza = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            plaza.name = "Path_Plaza";
            plaza.transform.SetParent(pathsRoot.transform);
            plaza.transform.position = new Vector3(0, 0.03f, 0);
            plaza.transform.localScale = new Vector3(10f, 0.04f, 10f);
            plaza.GetComponent<MeshRenderer>().sharedMaterial = pathMat;
        }

        private static void BuildRecyclingStation()
        {
            GameObject stationRoot = new GameObject("RecyclingStationArea");
            stationRoot.transform.position = new Vector3(0, 0, 7f);

            // Ground marking pad
            GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pad.name = "StationPad";
            pad.transform.SetParent(stationRoot.transform, false);
            pad.transform.localPosition = new Vector3(0, 0.04f, 0);
            pad.transform.localScale = new Vector3(9f, 0.06f, 3.5f);
            pad.GetComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Metal_Dark.mat");

            // Bins
            string[] binPrefabs = {
                "Assets/Prefabs/RecyclingBins/Bin_Plastic.prefab",
                "Assets/Prefabs/RecyclingBins/Bin_Paper.prefab",
                "Assets/Prefabs/RecyclingBins/Bin_Metal.prefab",
                "Assets/Prefabs/RecyclingBins/Bin_Organic.prefab"
            };

            float[] xOffsets = { -3.0f, -1.0f, 1.0f, 3.0f };
            for (int i = 0; i < 4; i++)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(binPrefabs[i]);
                GameObject bin = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                bin.transform.SetParent(stationRoot.transform, false);
                bin.transform.localPosition = new Vector3(xOffsets[i], 0.08f, 0);
            }

            // Station Sign
            GameObject signPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_ParkSign.prefab");
            GameObject sign = PrefabUtility.InstantiatePrefab(signPrefab) as GameObject;
            sign.transform.SetParent(stationRoot.transform, false);
            sign.transform.localPosition = new Vector3(0, 0.08f, 1.4f);
            sign.transform.localRotation = Quaternion.Euler(0, 180f, 0);
        }

        private static void PopulateParkProps()
        {
            GameObject propsRoot = new GameObject("ParkEnvironmentProps");

            GameObject pinePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_PineTree.prefab");
            GameObject roundTreePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_RoundTree.prefab");
            GameObject bushPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_Bush.prefab");
            GameObject benchPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_ParkBench.prefab");
            GameObject lampPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_StreetLamp.prefab");
            GameObject fencePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_Fence.prefab");
            GameObject rockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_Rock.prefab");

            // Trees
            Vector3[] pinePositions = {
                new Vector3(-20, 0, 15), new Vector3(-23, 0, -12), new Vector3(18, 0, 18),
                new Vector3(22, 0, -14), new Vector3(-8, 0, 22), new Vector3(14, 0, -20)
            };
            foreach (var pos in pinePositions)
            {
                GameObject t = PrefabUtility.InstantiatePrefab(pinePrefab) as GameObject;
                t.transform.SetParent(propsRoot.transform);
                t.transform.position = pos;
                t.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            }

            Vector3[] roundPositions = {
                new Vector3(-12, 0, -18), new Vector3(10, 0, 16), new Vector3(24, 0, 6),
                new Vector3(-24, 0, 0), new Vector3(6, 0, -24)
            };
            foreach (var pos in roundPositions)
            {
                GameObject t = PrefabUtility.InstantiatePrefab(roundTreePrefab) as GameObject;
                t.transform.SetParent(propsRoot.transform);
                t.transform.position = pos;
            }

            // Bushes
            Vector3[] bushPositions = {
                new Vector3(-6, 0, 5), new Vector3(6, 0, 5), new Vector3(-8, 0, -6),
                new Vector3(8, 0, -7), new Vector3(-15, 0, 12), new Vector3(16, 0, -10)
            };
            foreach (var pos in bushPositions)
            {
                GameObject b = PrefabUtility.InstantiatePrefab(bushPrefab) as GameObject;
                b.transform.SetParent(propsRoot.transform);
                b.transform.position = pos;
            }

            // Benches along paths
            Vector3[] benchPositions = {
                new Vector3(-3f, 0, 2f), new Vector3(3f, 0, -2f),
                new Vector3(-2f, 0, -8f), new Vector3(2f, 0, 8f)
            };
            float[] benchRotations = { 90f, -90f, 0f, 180f };
            for (int i = 0; i < benchPositions.Length; i++)
            {
                GameObject b = PrefabUtility.InstantiatePrefab(benchPrefab) as GameObject;
                b.transform.SetParent(propsRoot.transform);
                b.transform.position = benchPositions[i];
                b.transform.rotation = Quaternion.Euler(0, benchRotations[i], 0);
            }

            // Lamps
            Vector3[] lampPositions = {
                new Vector3(-2.2f, 0, 4f), new Vector3(2.2f, 0, -4f),
                new Vector3(4f, 0, 2.2f), new Vector3(-4f, 0, -2.2f)
            };
            foreach (var pos in lampPositions)
            {
                GameObject l = PrefabUtility.InstantiatePrefab(lampPrefab) as GameObject;
                l.transform.SetParent(propsRoot.transform);
                l.transform.position = pos;
            }

            // Rocks
            Vector3[] rockPositions = {
                new Vector3(-11, 0, 8), new Vector3(-16, 0, 14), new Vector3(14, 0, 12),
                new Vector3(19, 0, -18)
            };
            foreach (var pos in rockPositions)
            {
                GameObject r = PrefabUtility.InstantiatePrefab(rockPrefab) as GameObject;
                r.transform.SetParent(propsRoot.transform);
                r.transform.position = pos;
            }

            // Boundary Fences (along park edge)
            for (int x = -26; x <= 26; x += 4)
            {
                GameObject fNorth = PrefabUtility.InstantiatePrefab(fencePrefab) as GameObject;
                fNorth.transform.SetParent(propsRoot.transform);
                fNorth.transform.position = new Vector3(x, 0, 28);

                GameObject fSouth = PrefabUtility.InstantiatePrefab(fencePrefab) as GameObject;
                fSouth.transform.SetParent(propsRoot.transform);
                fSouth.transform.position = new Vector3(x, 0, -28);
            }
        }

        private static void ScatterTrashItems()
        {
            GameObject trashRoot = new GameObject("ScatteredTrash");

            string[] trashPrefabs = {
                // Plastic
                "Assets/Prefabs/Trash/Trash_PlasticBottle.prefab",
                "Assets/Prefabs/Trash/Trash_PlasticCup.prefab",
                "Assets/Prefabs/Trash/Trash_PlasticContainer.prefab",
                "Assets/Prefabs/Trash/Trash_PlasticBottle.prefab",
                // Paper
                "Assets/Prefabs/Trash/Trash_Newspaper.prefab",
                "Assets/Prefabs/Trash/Trash_CardboardBox.prefab",
                "Assets/Prefabs/Trash/Trash_PaperBall.prefab",
                "Assets/Prefabs/Trash/Trash_Newspaper.prefab",
                // Metal
                "Assets/Prefabs/Trash/Trash_SodaCan.prefab",
                "Assets/Prefabs/Trash/Trash_FoodCan.prefab",
                "Assets/Prefabs/Trash/Trash_MetalLid.prefab",
                "Assets/Prefabs/Trash/Trash_SodaCan.prefab",
                // Organic
                "Assets/Prefabs/Trash/Trash_BananaPeel.prefab",
                "Assets/Prefabs/Trash/Trash_AppleCore.prefab",
                "Assets/Prefabs/Trash/Trash_LeafPile.prefab",
                "Assets/Prefabs/Trash/Trash_BananaPeel.prefab"
            };

            Vector3[] positions = {
                new Vector3(-4f, 0.1f, -5f),
                new Vector3(5f, 0.1f, 3f),
                new Vector3(-7f, 0.1f, 8f),
                new Vector3(9f, 0.1f, -6f),

                new Vector3(-12f, 0.1f, -3f),
                new Vector3(14f, 0.1f, 5f),
                new Vector3(-3f, 0.1f, 12f),
                new Vector3(6f, 0.1f, -12f),

                new Vector3(-8f, 0.1f, -14f),
                new Vector3(11f, 0.1f, 15f),
                new Vector3(4f, 0.1f, 9f),
                new Vector3(-15f, 0.1f, 4f),

                new Vector3(-10f, 0.1f, 18f),
                new Vector3(16f, 0.1f, -4f),
                new Vector3(7f, 0.1f, 18f),
                new Vector3(-5f, 0.1f, -18f)
            };

            for (int i = 0; i < trashPrefabs.Length; i++)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(trashPrefabs[i]);
                GameObject item = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                item.transform.SetParent(trashRoot.transform);
                item.transform.position = positions[i];
                item.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            }
        }

        private static void BuildGameUI()
        {
            GameObject canvasObj = new GameObject("GameCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();

            // Event System
            if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            // HUD Top Bar
            GameObject hud = new GameObject("HUD");
            hud.transform.SetParent(canvasObj.transform, false);
            RectTransform hudRect = hud.AddComponent<RectTransform>();
            hudRect.anchorMin = new Vector2(0, 1);
            hudRect.anchorMax = new Vector2(1, 1);
            hudRect.pivot = new Vector2(0.5f, 1);
            hudRect.sizeDelta = new Vector2(0, 120);

            // Score Text
            GameObject scoreObj = new GameObject("ScoreText");
            scoreObj.transform.SetParent(hud.transform, false);
            Text scoreText = scoreObj.AddComponent<Text>();
            scoreText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            scoreText.fontSize = 32;
            scoreText.fontStyle = FontStyle.Bold;
            scoreText.color = Color.yellow;
            scoreText.alignment = TextAnchor.MiddleLeft;
            RectTransform sRect = scoreText.GetComponent<RectTransform>();
            sRect.anchorMin = new Vector2(0, 0.5f);
            sRect.anchorMax = new Vector2(0, 0.5f);
            sRect.anchoredPosition = new Vector2(40, 0);
            sRect.sizeDelta = new Vector2(300, 60);

            // Timer Text
            GameObject timerObj = new GameObject("TimerText");
            timerObj.transform.SetParent(hud.transform, false);
            Text timerText = timerObj.AddComponent<Text>();
            timerText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            timerText.fontSize = 36;
            timerText.fontStyle = FontStyle.Bold;
            timerText.color = Color.white;
            timerText.alignment = TextAnchor.MiddleCenter;
            RectTransform tRect = timerText.GetComponent<RectTransform>();
            tRect.anchorMin = new Vector2(0.5f, 0.5f);
            tRect.anchorMax = new Vector2(0.5f, 0.5f);
            tRect.anchoredPosition = new Vector2(0, 0);
            tRect.sizeDelta = new Vector2(300, 60);

            // Trash Count Text
            GameObject trashCountObj = new GameObject("TrashCountText");
            trashCountObj.transform.SetParent(hud.transform, false);
            Text trashCountText = trashCountObj.AddComponent<Text>();
            trashCountText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            trashCountText.fontSize = 28;
            trashCountText.fontStyle = FontStyle.Bold;
            trashCountText.color = Color.cyan;
            trashCountText.alignment = TextAnchor.MiddleRight;
            RectTransform tcRect = trashCountText.GetComponent<RectTransform>();
            tcRect.anchorMin = new Vector2(1, 0.5f);
            tcRect.anchorMax = new Vector2(1, 0.5f);
            tcRect.anchoredPosition = new Vector2(-40, 0);
            tcRect.sizeDelta = new Vector2(350, 60);

            // Prompt Text (center bottom)
            GameObject promptObj = new GameObject("PromptText");
            promptObj.transform.SetParent(canvasObj.transform, false);
            Text promptText = promptObj.AddComponent<Text>();
            promptText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            promptText.fontSize = 26;
            promptText.fontStyle = FontStyle.Bold;
            promptText.color = Color.white;
            promptText.alignment = TextAnchor.MiddleCenter;
            RectTransform pRect = promptText.GetComponent<RectTransform>();
            pRect.anchorMin = new Vector2(0.5f, 0.2f);
            pRect.anchorMax = new Vector2(0.5f, 0.2f);
            pRect.anchoredPosition = Vector2.zero;
            pRect.sizeDelta = new Vector2(700, 60);

            // Victory Panel
            GameObject victoryPanel = CreateOverlayPanel(canvasObj.transform, "VictoryPanel", "PARK CLEANED! YOU WIN!", Color.green, out Text vScore);
            // Game Over Panel
            GameObject gameOverPanel = CreateOverlayPanel(canvasObj.transform, "GameOverPanel", "TIME EXPIRED! GAME OVER", Color.red, out Text goScore);
            // Pause Panel
            GameObject pausePanel = CreateOverlayPanel(canvasObj.transform, "PausePanel", "GAME PAUSED", Color.white, out Text _);

            UIManager uiMgr = canvasObj.AddComponent<UIManager>();
            uiMgr.BindUI(scoreText, timerText, trashCountText, promptText, pausePanel, victoryPanel, gameOverPanel, vScore, goScore);
        }

        private static GameObject CreateOverlayPanel(Transform parent, string name, string title, Color titleColor, out Text scoreText)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.75f);

            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(panel.transform, false);
            Text titleTxt = titleObj.AddComponent<Text>();
            titleTxt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            titleTxt.fontSize = 48;
            titleTxt.fontStyle = FontStyle.Bold;
            titleTxt.color = titleColor;
            titleTxt.text = title;
            titleTxt.alignment = TextAnchor.MiddleCenter;
            RectTransform tRect = titleTxt.GetComponent<RectTransform>();
            tRect.anchoredPosition = new Vector2(0, 60);
            tRect.sizeDelta = new Vector2(800, 100);

            GameObject scoreObj = new GameObject("Score");
            scoreObj.transform.SetParent(panel.transform, false);
            scoreText = scoreObj.AddComponent<Text>();
            scoreText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            scoreText.fontSize = 32;
            scoreText.color = Color.yellow;
            scoreText.text = "";
            scoreText.alignment = TextAnchor.MiddleCenter;
            RectTransform sRect = scoreText.GetComponent<RectTransform>();
            sRect.anchoredPosition = new Vector2(0, -10);
            sRect.sizeDelta = new Vector2(600, 60);

            panel.SetActive(false);
            return panel;
        }
    }
}
