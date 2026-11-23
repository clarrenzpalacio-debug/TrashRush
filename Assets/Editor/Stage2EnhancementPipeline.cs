using System.IO;
using System.Collections.Generic;
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
    /// <summary>
    /// Stage 2 Enhancement Pipeline — adds polish, fixes, flowers, lights,
    /// particle effects, multi-material trees, boundary fences, build settings,
    /// and additional environmental detail on top of the Stage 1 foundation.
    /// Does NOT delete existing assets.
    /// </summary>
    public static class Stage2EnhancementPipeline
    {
        [MenuItem("TrashRush/Stage 2 — Enhance Environment")]
        public static void RunStage2()
        {
            Debug.Log("[Stage2] ========== STAGE 2: ENVIRONMENT ENHANCEMENT PIPELINE ==========");

            // Step 0: Make sure all base assets exist first (idempotent)
            GameAssetPipeline.GenerateAllAndBuild();

            // Step 1: Create new materials needed for Stage 2
            CreateAdditionalMaterials();

            // Step 2: Create new mesh assets (flowers, fountain, picnic table, trash bin, gazebo pillar)
            CreateAdditionalMeshAssets();

            // Step 3: Create new prefabs with multi-material support
            CreateEnhancedPrefabs();

            // Step 4: Rebuild the scene with full Stage 2 environment
            BuildEnhancedScene();

            // Step 5: Register scene in EditorBuildSettings
            RegisterSceneInBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Stage2] ========== STAGE 2 COMPLETE ==========");
        }

        // ================================================================
        // STEP 1: Additional Materials
        // ================================================================
        private static void CreateAdditionalMaterials()
        {
            Debug.Log("[Stage2] Creating additional materials...");

            CreateMat("Assets/Art/Materials/M_Flower_Red.mat", new Color(0.91f, 0.12f, 0.39f));
            CreateMat("Assets/Art/Materials/M_Flower_Yellow.mat", new Color(1.0f, 0.92f, 0.23f));
            CreateMat("Assets/Art/Materials/M_Flower_White.mat", new Color(0.98f, 0.98f, 0.98f));
            CreateMat("Assets/Art/Materials/M_Flower_Purple.mat", new Color(0.67f, 0.28f, 0.74f));
            CreateMat("Assets/Art/Materials/M_Flower_Stem.mat", new Color(0.33f, 0.55f, 0.18f));
            CreateMat("Assets/Art/Materials/M_Concrete.mat", new Color(0.76f, 0.76f, 0.74f));
            CreateMat("Assets/Art/Materials/M_DarkWood.mat", new Color(0.36f, 0.25f, 0.20f));
            CreateMat("Assets/Art/Materials/M_Player_Hair.mat", new Color(0.36f, 0.25f, 0.22f));
            CreateMat("Assets/Art/Materials/M_Player_Pants.mat", new Color(0.22f, 0.22f, 0.28f));
            CreateMat("Assets/Art/Materials/M_Player_Boots.mat", new Color(0.30f, 0.22f, 0.16f));
            CreateMat("Assets/Art/Materials/M_Player_Backpack.mat", new Color(0.60f, 0.44f, 0.18f));
            CreateMat("Assets/Art/Materials/M_Bin_Lid.mat", new Color(0.30f, 0.30f, 0.30f), 0.6f, 0.3f);
            CreateMat("Assets/Art/Materials/M_Fountain_Stone.mat", new Color(0.70f, 0.68f, 0.65f));
            CreateMat("Assets/Art/Materials/M_GrassEdge.mat", new Color(0.24f, 0.58f, 0.24f));
            CreateMat("Assets/Art/Materials/M_Dirt.mat", new Color(0.52f, 0.40f, 0.28f));

            // Particle materials
            CreateParticleMaterial("Assets/Art/Materials/M_Particle_Green.mat", new Color(0.2f, 0.9f, 0.3f, 0.8f));
            CreateParticleMaterial("Assets/Art/Materials/M_Particle_Red.mat", new Color(0.9f, 0.2f, 0.2f, 0.8f));

            AssetDatabase.Refresh();
        }

        private static Material CreateMat(string path, Color c, float metallic = 0f, float smooth = 0.25f)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Standard"));
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.color = c;
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Glossiness", smooth);
            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static Material CreateParticleMaterial(string path, Color c)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                Shader shader = Shader.Find("Particles/Standard Unlit");
                if (shader == null) shader = Shader.Find("Standard");
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.color = c;
            mat.SetFloat("_Mode", 2); // Fade
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
            EditorUtility.SetDirty(mat);
            return mat;
        }

        // ================================================================
        // STEP 2: Additional Mesh Assets
        // ================================================================
        private static void CreateAdditionalMeshAssets()
        {
            Debug.Log("[Stage2] Creating additional mesh assets...");

            // Flower cluster
            SaveMesh(CreateFlowerClusterMesh(), "Assets/Art/Models/Mesh_FlowerCluster.asset");
            // Fountain
            SaveMesh(CreateFountainMesh(), "Assets/Art/Models/Mesh_Fountain.asset");
            // Picnic table
            SaveMesh(CreatePicnicTableMesh(), "Assets/Art/Models/Mesh_PicnicTable.asset");
            // Small trash can (decorative - non-recyclable bin)
            SaveMesh(CreateSmallTrashCanMesh(), "Assets/Art/Models/Mesh_SmallTrashCan.asset");
            // Stepping stones
            SaveMesh(CreateSteppingStoneMesh(), "Assets/Art/Models/Mesh_SteppingStone.asset");
            // Gazebo (simple 4-pillar shelter)
            SaveMesh(CreateGazeboMesh(), "Assets/Art/Models/Mesh_Gazebo.asset");

            AssetDatabase.Refresh();
        }

        private static Mesh CreateFlowerClusterMesh()
        {
            // 5 small flowers: each is a stem cylinder + petal disc
            var parts = new List<(Mesh, Matrix4x4)>();
            System.Random rng = new System.Random(123);

            for (int i = 0; i < 5; i++)
            {
                float offsetX = (float)(rng.NextDouble() * 0.3 - 0.15);
                float offsetZ = (float)(rng.NextDouble() * 0.3 - 0.15);
                float stemHeight = 0.2f + (float)(rng.NextDouble() * 0.15);

                Mesh stem = LowPolyMeshBuilder.CreateCylinder(0.015f, 0.012f, stemHeight, 4);
                Mesh petal = LowPolyMeshBuilder.CreateCylinder(0.06f, 0.04f, 0.03f, 6);
                Mesh center = LowPolyMeshBuilder.CreateIcosphere(0.02f, 0, 0f);

                parts.Add((stem, Matrix4x4.TRS(new Vector3(offsetX, stemHeight * 0.5f, offsetZ), Quaternion.identity, Vector3.one)));
                parts.Add((petal, Matrix4x4.TRS(new Vector3(offsetX, stemHeight + 0.02f, offsetZ), Quaternion.identity, Vector3.one)));
                parts.Add((center, Matrix4x4.TRS(new Vector3(offsetX, stemHeight + 0.04f, offsetZ), Quaternion.identity, Vector3.one)));
            }

            Mesh combined = LowPolyMeshBuilder.CombineMeshes(parts.ToArray());
            combined.name = "FlowerCluster";
            return combined;
        }

        private static Mesh CreateFountainMesh()
        {
            Mesh basin = LowPolyMeshBuilder.CreateCylinder(1.5f, 1.5f, 0.4f, 8);
            Mesh basinRim = LowPolyMeshBuilder.CreateCylinder(1.6f, 1.55f, 0.15f, 8);
            Mesh pillar = LowPolyMeshBuilder.CreateCylinder(0.25f, 0.20f, 1.2f, 6);
            Mesh topBowl = LowPolyMeshBuilder.CreateCylinder(0.6f, 0.65f, 0.3f, 8);

            var parts = new (Mesh, Matrix4x4)[] {
                (basin, Matrix4x4.TRS(new Vector3(0, 0.2f, 0), Quaternion.identity, Vector3.one)),
                (basinRim, Matrix4x4.TRS(new Vector3(0, 0.45f, 0), Quaternion.identity, Vector3.one)),
                (pillar, Matrix4x4.TRS(new Vector3(0, 1.0f, 0), Quaternion.identity, Vector3.one)),
                (topBowl, Matrix4x4.TRS(new Vector3(0, 1.75f, 0), Quaternion.identity, Vector3.one))
            };

            Mesh combined = LowPolyMeshBuilder.CombineMeshes(parts);
            combined.name = "Fountain";
            return combined;
        }

        private static Mesh CreatePicnicTableMesh()
        {
            Mesh tableTop = LowPolyMeshBuilder.CreateBox(new Vector3(2.0f, 0.08f, 0.9f));
            Mesh seat1 = LowPolyMeshBuilder.CreateBox(new Vector3(2.0f, 0.06f, 0.35f));
            Mesh seat2 = LowPolyMeshBuilder.CreateBox(new Vector3(2.0f, 0.06f, 0.35f));
            Mesh leg1 = LowPolyMeshBuilder.CreateBox(new Vector3(0.08f, 0.7f, 0.9f));
            Mesh leg2 = LowPolyMeshBuilder.CreateBox(new Vector3(0.08f, 0.7f, 0.9f));

            var parts = new (Mesh, Matrix4x4)[] {
                (tableTop, Matrix4x4.TRS(new Vector3(0, 0.75f, 0), Quaternion.identity, Vector3.one)),
                (seat1, Matrix4x4.TRS(new Vector3(0, 0.44f, 0.62f), Quaternion.identity, Vector3.one)),
                (seat2, Matrix4x4.TRS(new Vector3(0, 0.44f, -0.62f), Quaternion.identity, Vector3.one)),
                (leg1, Matrix4x4.TRS(new Vector3(-0.65f, 0.35f, 0), Quaternion.identity, Vector3.one)),
                (leg2, Matrix4x4.TRS(new Vector3(0.65f, 0.35f, 0), Quaternion.identity, Vector3.one))
            };

            Mesh combined = LowPolyMeshBuilder.CombineMeshes(parts);
            combined.name = "PicnicTable";
            return combined;
        }

        private static Mesh CreateSmallTrashCanMesh()
        {
            Mesh body = LowPolyMeshBuilder.CreateCylinder(0.22f, 0.26f, 0.6f, 8);
            Mesh rim = LowPolyMeshBuilder.CreateCylinder(0.27f, 0.28f, 0.06f, 8);
            Mesh basePlate = LowPolyMeshBuilder.CreateCylinder(0.24f, 0.22f, 0.05f, 8);

            var parts = new (Mesh, Matrix4x4)[] {
                (body, Matrix4x4.TRS(new Vector3(0, 0.3f, 0), Quaternion.identity, Vector3.one)),
                (rim, Matrix4x4.TRS(new Vector3(0, 0.63f, 0), Quaternion.identity, Vector3.one)),
                (basePlate, Matrix4x4.TRS(new Vector3(0, 0.025f, 0), Quaternion.identity, Vector3.one))
            };

            Mesh combined = LowPolyMeshBuilder.CombineMeshes(parts);
            combined.name = "SmallTrashCan";
            return combined;
        }

        private static Mesh CreateSteppingStoneMesh()
        {
            Mesh stone = LowPolyMeshBuilder.CreateIcosphere(0.35f, 1, 0.06f);
            Matrix4x4 m = Matrix4x4.TRS(new Vector3(0, 0.05f, 0), Quaternion.identity, new Vector3(1.0f, 0.25f, 1.0f));
            Mesh combined = LowPolyMeshBuilder.CombineMeshes((stone, m));
            combined.name = "SteppingStone";
            return combined;
        }

        private static Mesh CreateGazeboMesh()
        {
            var parts = new List<(Mesh, Matrix4x4)>();

            // 4 pillars
            for (int i = 0; i < 4; i++)
            {
                float angle = i * 90f * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * 2.0f;
                float z = Mathf.Sin(angle) * 2.0f;
                Mesh pillar = LowPolyMeshBuilder.CreateCylinder(0.12f, 0.10f, 2.8f, 6);
                parts.Add((pillar, Matrix4x4.TRS(new Vector3(x, 1.4f, z), Quaternion.identity, Vector3.one)));
            }

            // Floor
            Mesh floor = LowPolyMeshBuilder.CreateCylinder(2.5f, 2.5f, 0.12f, 8);
            parts.Add((floor, Matrix4x4.TRS(new Vector3(0, 0.06f, 0), Quaternion.identity, Vector3.one)));

            // Roof (cone)
            Mesh roof = LowPolyMeshBuilder.CreateCylinder(3.0f, 0.1f, 1.0f, 8);
            parts.Add((roof, Matrix4x4.TRS(new Vector3(0, 3.3f, 0), Quaternion.identity, Vector3.one)));

            Mesh combined = LowPolyMeshBuilder.CombineMeshes(parts.ToArray());
            combined.name = "Gazebo";
            return combined;
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

        // ================================================================
        // STEP 3: Enhanced Prefabs (multi-material trees, decorated bins, etc.)
        // ================================================================
        private static void CreateEnhancedPrefabs()
        {
            Debug.Log("[Stage2] Creating enhanced prefabs...");

            // Pine tree with trunk material + foliage material (separate children)
            CreateMultiPartTreePrefab(
                "Assets/Prefabs/Environment/Env_PineTree.prefab",
                "PineTree",
                LowPolyMeshBuilder.CreateCylinder(0.22f, 0.16f, 1.8f, 6),
                new Mesh[] {
                    LowPolyMeshBuilder.CreateCylinder(1.6f, 0.2f, 1.4f, 6),
                    LowPolyMeshBuilder.CreateCylinder(1.3f, 0.15f, 1.3f, 6),
                    LowPolyMeshBuilder.CreateCylinder(0.9f, 0.05f, 1.2f, 6)
                },
                new Vector3[] {
                    new Vector3(0, 1.8f, 0),
                    new Vector3(0, 2.6f, 0),
                    new Vector3(0, 3.4f, 0)
                },
                "M_TreeTrunk",
                "M_TreeFoliage_Dark"
            );

            // Round tree with trunk material + foliage material
            CreateMultiPartTreePrefab(
                "Assets/Prefabs/Environment/Env_RoundTree.prefab",
                "RoundTree",
                LowPolyMeshBuilder.CreateCylinder(0.25f, 0.20f, 2.2f, 6),
                new Mesh[] { LowPolyMeshBuilder.CreateIcosphere(1.4f, 1, 0.12f) },
                new Vector3[] { new Vector3(0, 2.8f, 0) },
                "M_TreeTrunk",
                "M_TreeFoliage_Light"
            );

            // Flower cluster
            CreateSimplePrefab("Assets/Prefabs/Environment/Env_FlowerCluster.prefab", "Mesh_FlowerCluster", "M_Flower_Red");

            // Fountain
            CreateSimplePrefab("Assets/Prefabs/Environment/Env_Fountain.prefab", "Mesh_Fountain", "M_Fountain_Stone");

            // Picnic table
            CreateSimplePrefab("Assets/Prefabs/Environment/Env_PicnicTable.prefab", "Mesh_PicnicTable", "M_Wood");

            // Small trash can
            CreateSimplePrefab("Assets/Prefabs/Environment/Env_SmallTrashCan.prefab", "Mesh_SmallTrashCan", "M_Metal_Dark");

            // Stepping stone
            CreateSimplePrefab("Assets/Prefabs/Environment/Env_SteppingStone.prefab", "Mesh_SteppingStone", "M_Stone");

            // Gazebo
            CreateSimplePrefab("Assets/Prefabs/Environment/Env_Gazebo.prefab", "Mesh_Gazebo", "M_Wood");

            // Enhanced recycling bins with particle effects
            CreateEnhancedBinPrefab("Assets/Prefabs/RecyclingBins/Bin_Plastic.prefab", TrashCategory.Plastic, "PLASTIC", "M_Bin_Plastic");
            CreateEnhancedBinPrefab("Assets/Prefabs/RecyclingBins/Bin_Paper.prefab", TrashCategory.Paper, "PAPER", "M_Bin_Paper");
            CreateEnhancedBinPrefab("Assets/Prefabs/RecyclingBins/Bin_Metal.prefab", TrashCategory.Metal, "METAL", "M_Bin_Metal");
            CreateEnhancedBinPrefab("Assets/Prefabs/RecyclingBins/Bin_Organic.prefab", TrashCategory.Organic, "ORGANIC", "M_Bin_Organic");

            // Enhanced player with multi-material
            CreateEnhancedPlayerPrefab("Assets/Prefabs/Player/Player.prefab");

            // Enhanced lamp with Point Light child
            CreateEnhancedLampPrefab("Assets/Prefabs/Environment/Env_StreetLamp.prefab");

            AssetDatabase.Refresh();
        }

        private static void CreateMultiPartTreePrefab(string path, string name, Mesh trunkMesh, Mesh[] foliageMeshes, Vector3[] foliagePositions, string trunkMatName, string foliageMatName)
        {
            GameObject root = new GameObject(name);

            // Trunk child
            GameObject trunk = new GameObject("Trunk");
            trunk.transform.SetParent(root.transform, false);
            trunk.transform.localPosition = new Vector3(0, 0.9f, 0);
            MeshFilter trunkMF = trunk.AddComponent<MeshFilter>();
            trunkMF.sharedMesh = trunkMesh;
            MeshRenderer trunkMR = trunk.AddComponent<MeshRenderer>();
            trunkMR.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Art/Materials/{trunkMatName}.mat");

            // Foliage children
            for (int i = 0; i < foliageMeshes.Length; i++)
            {
                GameObject foliage = new GameObject($"Foliage_{i}");
                foliage.transform.SetParent(root.transform, false);
                foliage.transform.localPosition = foliagePositions[i];
                if (i == 1) foliage.transform.localRotation = Quaternion.Euler(0, 30, 0);
                MeshFilter fMF = foliage.AddComponent<MeshFilter>();
                fMF.sharedMesh = foliageMeshes[i];
                MeshRenderer fMR = foliage.AddComponent<MeshRenderer>();
                fMR.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Art/Materials/{foliageMatName}.mat");
            }

            // Collider on root
            CapsuleCollider col = root.AddComponent<CapsuleCollider>();
            col.center = new Vector3(0, 2f, 0);
            col.radius = 0.6f;
            col.height = 4f;

            // Mark static for performance
            root.isStatic = true;

            PrefabUtility.SaveAsPrefabAsset(root, path);
            GameObject.DestroyImmediate(root);
        }

        private static void CreateSimplePrefab(string path, string meshName, string matName)
        {
            GameObject go = new GameObject(Path.GetFileNameWithoutExtension(path));
            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>($"Assets/Art/Models/{meshName}.asset");
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Art/Materials/{matName}.mat");

            // Add a collider for objects the player might walk into
            if (meshName.Contains("Fountain") || meshName.Contains("Gazebo") || meshName.Contains("PicnicTable"))
            {
                BoxCollider bc = go.AddComponent<BoxCollider>();
            }

            go.isStatic = true;
            PrefabUtility.SaveAsPrefabAsset(go, path);
            GameObject.DestroyImmediate(go);
        }

        private static void CreateEnhancedBinPrefab(string path, TrashCategory category, string labelText, string binMatName)
        {
            GameObject binRoot = new GameObject($"Bin_{labelText}");

            // Body with category-colored material
            GameObject bodyObj = new GameObject("BinBody");
            bodyObj.transform.SetParent(binRoot.transform, false);
            MeshFilter mfBody = bodyObj.AddComponent<MeshFilter>();
            mfBody.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Models/Mesh_BinBody.asset");
            MeshRenderer mrBody = bodyObj.AddComponent<MeshRenderer>();
            mrBody.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Art/Materials/{binMatName}.mat");

            // Lid with neutral dark material (child — for lid animation pivot)
            GameObject lidPivot = new GameObject("LidPivot");
            lidPivot.transform.SetParent(binRoot.transform, false);
            lidPivot.transform.localPosition = new Vector3(0, 1.15f, -0.45f); // hinge at back edge

            GameObject lidObj = new GameObject("BinLid");
            lidObj.transform.SetParent(lidPivot.transform, false);
            lidObj.transform.localPosition = new Vector3(0, 0, 0.45f); // offset from hinge
            MeshFilter mfLid = lidObj.AddComponent<MeshFilter>();
            mfLid.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Models/Mesh_BinLid.asset");
            MeshRenderer mrLid = lidObj.AddComponent<MeshRenderer>();
            mrLid.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Bin_Lid.mat");

            // Category label — Quad facing forward
            GameObject labelQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            labelQuad.name = "CategoryLabel";
            labelQuad.transform.SetParent(binRoot.transform, false);
            labelQuad.transform.localPosition = new Vector3(0, 0.75f, 0.47f);
            labelQuad.transform.localScale = new Vector3(0.52f, 0.26f, 1f);
            GameObject.DestroyImmediate(labelQuad.GetComponent<Collider>());
            // Apply bin texture if available
            string texPath = $"Assets/Art/Textures/Tex_Bin_{labelText.Substring(0, 1) + labelText.Substring(1).ToLower()}.png";
            Texture2D labelTex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            if (labelTex != null)
            {
                Material labelMat = new Material(Shader.Find("Standard"));
                labelMat.mainTexture = labelTex;
                string labelMatPath = $"Assets/Art/Materials/M_Label_{labelText}.mat";
                Material existingLabelMat = AssetDatabase.LoadAssetAtPath<Material>(labelMatPath);
                if (existingLabelMat == null)
                {
                    AssetDatabase.CreateAsset(labelMat, labelMatPath);
                }
                else
                {
                    labelMat = existingLabelMat;
                    labelMat.mainTexture = labelTex;
                    EditorUtility.SetDirty(labelMat);
                }
                labelQuad.GetComponent<MeshRenderer>().sharedMaterial = labelMat;
            }

            // Label on back too
            GameObject labelBack = GameObject.CreatePrimitive(PrimitiveType.Quad);
            labelBack.name = "CategoryLabelBack";
            labelBack.transform.SetParent(binRoot.transform, false);
            labelBack.transform.localPosition = new Vector3(0, 0.75f, -0.47f);
            labelBack.transform.localScale = new Vector3(0.52f, 0.26f, 1f);
            labelBack.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            GameObject.DestroyImmediate(labelBack.GetComponent<Collider>());
            if (labelTex != null)
            {
                labelBack.GetComponent<MeshRenderer>().sharedMaterial = labelQuad.GetComponent<MeshRenderer>().sharedMaterial;
            }

            // Interaction trigger collider
            BoxCollider col = binRoot.AddComponent<BoxCollider>();
            col.center = new Vector3(0, 0.6f, 0);
            col.size = new Vector3(1.2f, 1.5f, 1.2f);
            col.isTrigger = true;

            // Particle Systems for feedback
            GameObject correctFX = CreateParticleSystem("CorrectFX", new Color(0.2f, 0.9f, 0.3f, 0.8f), "M_Particle_Green");
            correctFX.transform.SetParent(binRoot.transform, false);
            correctFX.transform.localPosition = new Vector3(0, 1.3f, 0);

            GameObject incorrectFX = CreateParticleSystem("IncorrectFX", new Color(0.9f, 0.2f, 0.2f, 0.8f), "M_Particle_Red");
            incorrectFX.transform.SetParent(binRoot.transform, false);
            incorrectFX.transform.localPosition = new Vector3(0, 1.3f, 0);

            // RecyclingBin component
            RecyclingBin binComp = binRoot.AddComponent<RecyclingBin>();
            binComp.SetCategory(category, labelText);

            SerializedObject so = new SerializedObject(binComp);
            so.FindProperty("lidTransform").objectReferenceValue = lidPivot.transform;
            so.FindProperty("correctEffect").objectReferenceValue = correctFX.GetComponent<ParticleSystem>();
            so.FindProperty("incorrectEffect").objectReferenceValue = incorrectFX.GetComponent<ParticleSystem>();
            so.ApplyModifiedProperties();

            PrefabUtility.SaveAsPrefabAsset(binRoot, path);
            GameObject.DestroyImmediate(binRoot);
        }

        private static GameObject CreateParticleSystem(string name, Color color, string matName)
        {
            GameObject psObj = new GameObject(name);
            ParticleSystem ps = psObj.AddComponent<ParticleSystem>();

            var main = ps.main;
            main.duration = 0.8f;
            main.loop = false;
            main.startLifetime = 0.6f;
            main.startSpeed = 3f;
            main.startSize = 0.15f;
            main.startColor = color;
            main.maxParticles = 30;
            main.playOnAwake = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 20)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 35f;
            shape.radius = 0.3f;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 1, 1, 0));

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = grad;

            ParticleSystemRenderer psr = psObj.GetComponent<ParticleSystemRenderer>();
            Material pMat = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Art/Materials/{matName}.mat");
            if (pMat != null) psr.sharedMaterial = pMat;

            return psObj;
        }

        private static void CreateEnhancedPlayerPrefab(string path)
        {
            GameObject player = new GameObject("Player");
            CharacterController cc = player.AddComponent<CharacterController>();
            cc.center = new Vector3(0, 0.95f, 0);
            cc.height = 1.9f;
            cc.radius = 0.35f;

            // Visual Root (for procedural animation)
            GameObject visualRoot = new GameObject("VisualRoot");
            visualRoot.transform.SetParent(player.transform, false);

            Material bodyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Player_Body.mat");
            Material skinMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Player_Skin.mat");
            Material pantsMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Player_Pants.mat");
            Material bootsMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Player_Boots.mat");
            Material hairMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Player_Hair.mat");
            Material bpMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Player_Backpack.mat");

            // Head (skin)
            AddMeshChild(visualRoot.transform, "Head", LowPolyMeshBuilder.CreateIcosphere(0.24f, 1, 0.01f), skinMat, new Vector3(0, 1.62f, 0));
            // Cap (body color)
            AddMeshChild(visualRoot.transform, "Cap", LowPolyMeshBuilder.CreateBox(new Vector3(0.28f, 0.04f, 0.18f)), bodyMat, new Vector3(0, 1.76f, 0.12f), Quaternion.Euler(-10, 0, 0));
            // Torso (body/jacket)
            AddMeshChild(visualRoot.transform, "Torso", LowPolyMeshBuilder.CreateBox(new Vector3(0.44f, 0.62f, 0.28f)), bodyMat, new Vector3(0, 1.05f, 0));
            // Backpack
            AddMeshChild(visualRoot.transform, "Backpack", LowPolyMeshBuilder.CreateBox(new Vector3(0.34f, 0.45f, 0.16f)), bpMat, new Vector3(0, 1.05f, -0.19f));
            // Left Leg (pants)
            AddMeshChild(visualRoot.transform, "LegLeft", LowPolyMeshBuilder.CreateBox(new Vector3(0.15f, 0.60f, 0.16f)), pantsMat, new Vector3(-0.13f, 0.44f, 0));
            // Right Leg (pants)
            AddMeshChild(visualRoot.transform, "LegRight", LowPolyMeshBuilder.CreateBox(new Vector3(0.15f, 0.60f, 0.16f)), pantsMat, new Vector3(0.13f, 0.44f, 0));
            // Left Boot
            AddMeshChild(visualRoot.transform, "BootLeft", LowPolyMeshBuilder.CreateBox(new Vector3(0.17f, 0.14f, 0.24f)), bootsMat, new Vector3(-0.13f, 0.07f, 0.04f));
            // Right Boot
            AddMeshChild(visualRoot.transform, "BootRight", LowPolyMeshBuilder.CreateBox(new Vector3(0.17f, 0.14f, 0.24f)), bootsMat, new Vector3(0.13f, 0.07f, 0.04f));
            // Left Arm (body)
            AddMeshChild(visualRoot.transform, "ArmLeft", LowPolyMeshBuilder.CreateBox(new Vector3(0.12f, 0.52f, 0.12f)), bodyMat, new Vector3(-0.29f, 1.02f, 0), Quaternion.Euler(0, 0, 5));
            // Right Arm (body)
            AddMeshChild(visualRoot.transform, "ArmRight", LowPolyMeshBuilder.CreateBox(new Vector3(0.12f, 0.52f, 0.12f)), bodyMat, new Vector3(0.29f, 1.02f, 0), Quaternion.Euler(0, 0, -5));

            // Carry Point
            GameObject carryPoint = new GameObject("CarryPoint");
            carryPoint.transform.SetParent(player.transform, false);
            carryPoint.transform.localPosition = new Vector3(0.35f, 0.9f, 0.45f);

            // Components
            PlayerController pc = player.AddComponent<PlayerController>();
            SerializedObject soPC = new SerializedObject(pc);
            soPC.FindProperty("visualRoot").objectReferenceValue = visualRoot.transform;
            soPC.ApplyModifiedProperties();

            PlayerInteraction pi = player.AddComponent<PlayerInteraction>();
            SerializedObject soPI = new SerializedObject(pi);
            soPI.FindProperty("carryPoint").objectReferenceValue = carryPoint.transform;
            soPI.ApplyModifiedProperties();

            PrefabUtility.SaveAsPrefabAsset(player, path);
            GameObject.DestroyImmediate(player);
        }

        private static GameObject AddMeshChild(Transform parent, string name, Mesh mesh, Material mat, Vector3 localPos, Quaternion? localRot = null)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(parent, false);
            child.transform.localPosition = localPos;
            if (localRot.HasValue) child.transform.localRotation = localRot.Value;

            MeshFilter mf = child.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            MeshRenderer mr = child.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mat;

            return child;
        }

        private static void CreateEnhancedLampPrefab(string path)
        {
            GameObject lamp = new GameObject("Env_StreetLamp");

            // Mesh
            MeshFilter mf = lamp.AddComponent<MeshFilter>();
            mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Models/Mesh_StreetLamp.asset");
            MeshRenderer mr = lamp.AddComponent<MeshRenderer>();
            mr.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Lamp_Post.mat");

            // Glass glow child
            GameObject glow = new GameObject("LampGlow");
            glow.transform.SetParent(lamp.transform, false);
            glow.transform.localPosition = new Vector3(0, 3.25f, 0);
            MeshFilter gmf = glow.AddComponent<MeshFilter>();
            gmf.sharedMesh = LowPolyMeshBuilder.CreateCylinder(0.18f, 0.26f, 0.35f, 6);
            MeshRenderer gmr = glow.AddComponent<MeshRenderer>();
            gmr.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Lamp_Light.mat");

            // Point Light child
            GameObject lightObj = new GameObject("LampLight");
            lightObj.transform.SetParent(lamp.transform, false);
            lightObj.transform.localPosition = new Vector3(0, 3.4f, 0);
            Light pointLight = lightObj.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.color = new Color(1.0f, 0.93f, 0.70f);
            pointLight.intensity = 1.8f;
            pointLight.range = 12f;
            pointLight.shadows = LightShadows.Soft;

            lamp.AddComponent<BoxCollider>();
            lamp.isStatic = true;

            PrefabUtility.SaveAsPrefabAsset(lamp, path);
            GameObject.DestroyImmediate(lamp);
        }

        // ================================================================
        // STEP 4: Build Enhanced Scene
        // ================================================================
        private static void BuildEnhancedScene()
        {
            Debug.Log("[Stage2] Building enhanced game scene...");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // ---- LIGHTING ----
            SetupLighting();

            // ---- TERRAIN / GROUND ----
            BuildGround();

            // ---- WALKING PATHS ----
            BuildPaths();

            // ---- WATER FEATURES ----
            BuildWaterFeatures();

            // ---- RECYCLING STATION ----
            BuildRecyclingStation();

            // ---- ENVIRONMENT PROPS ----
            BuildEnvironmentProps();

            // ---- TRASH ITEMS ----
            ScatterTrash();

            // ---- PLAYER & CAMERA ----
            SetupPlayerAndCamera();

            // ---- MANAGERS ----
            SetupManagers();

            // ---- UI ----
            BuildUI();

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/GameScene.unity");
            Debug.Log("[Stage2] Scene saved to Assets/Scenes/GameScene.unity");
        }

        private static void SetupLighting()
        {
            // Directional sun
            GameObject sun = new GameObject("Sun_DirectionalLight");
            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1.0f, 0.96f, 0.88f);
            light.intensity = 1.3f;
            light.shadows = LightShadows.Soft;
            light.shadowStrength = 0.6f;
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Fill light (secondary directional from opposite side)
            GameObject fill = new GameObject("Fill_DirectionalLight");
            Light fillLight = fill.AddComponent<Light>();
            fillLight.type = LightType.Directional;
            fillLight.color = new Color(0.6f, 0.7f, 0.9f);
            fillLight.intensity = 0.3f;
            fillLight.shadows = LightShadows.None;
            fill.transform.rotation = Quaternion.Euler(30f, 150f, 0f);

            // Render Settings
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.55f, 0.65f, 0.80f);
            RenderSettings.ambientEquatorColor = new Color(0.40f, 0.50f, 0.45f);
            RenderSettings.ambientGroundColor = new Color(0.20f, 0.22f, 0.15f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.75f, 0.85f, 0.92f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.006f;
        }

        private static void BuildGround()
        {
            // Main grass plane
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "ParkGround";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(7f, 1f, 7f); // 70m x 70m
            ground.GetComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Grass.mat");
            ground.isStatic = true;

            // Dirt border ring
            GameObject dirtRing = GameObject.CreatePrimitive(PrimitiveType.Plane);
            dirtRing.name = "DirtBorder";
            dirtRing.transform.position = new Vector3(0, -0.01f, 0);
            dirtRing.transform.localScale = new Vector3(7.5f, 1f, 7.5f);
            dirtRing.GetComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Dirt.mat");
            dirtRing.isStatic = true;
        }

        private static void BuildPaths()
        {
            GameObject pathsRoot = new GameObject("WalkingPaths");
            pathsRoot.isStatic = true;
            Material pathMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Path.mat");
            Material concreteMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Concrete.mat");

            // North-South main path
            CreatePathSegment(pathsRoot.transform, "Path_NS", pathMat, new Vector3(0, 0.02f, 0), new Vector3(3.5f, 0.04f, 65f));
            // East-West main path
            CreatePathSegment(pathsRoot.transform, "Path_EW", pathMat, new Vector3(0, 0.02f, 0), new Vector3(65f, 0.04f, 3.5f));
            // Central plaza (concrete circle)
            GameObject plaza = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            plaza.name = "CentralPlaza";
            plaza.transform.SetParent(pathsRoot.transform);
            plaza.transform.position = new Vector3(0, 0.025f, 0);
            plaza.transform.localScale = new Vector3(12f, 0.04f, 12f);
            plaza.GetComponent<MeshRenderer>().sharedMaterial = concreteMat;
            plaza.isStatic = true;

            // Diagonal paths from center
            CreatePathSegment(pathsRoot.transform, "Path_NE", pathMat, new Vector3(12, 0.015f, 12), new Vector3(2.5f, 0.03f, 20f), 45f);
            CreatePathSegment(pathsRoot.transform, "Path_NW", pathMat, new Vector3(-12, 0.015f, 12), new Vector3(2.5f, 0.03f, 20f), -45f);
            CreatePathSegment(pathsRoot.transform, "Path_SE", pathMat, new Vector3(12, 0.015f, -12), new Vector3(2.5f, 0.03f, 20f), -45f);
            CreatePathSegment(pathsRoot.transform, "Path_SW", pathMat, new Vector3(-12, 0.015f, -12), new Vector3(2.5f, 0.03f, 20f), 45f);

            // Stepping stones near pond
            var stonePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_SteppingStone.prefab");
            if (stonePrefab != null)
            {
                Vector3[] stonePositions = {
                    new Vector3(-16, 0.03f, 10), new Vector3(-15, 0.03f, 11.5f),
                    new Vector3(-13.5f, 0.03f, 12.5f), new Vector3(-12, 0.03f, 11)
                };
                foreach (var p in stonePositions)
                {
                    var s = PrefabUtility.InstantiatePrefab(stonePrefab) as GameObject;
                    s.transform.SetParent(pathsRoot.transform);
                    s.transform.position = p;
                }
            }
        }

        private static void CreatePathSegment(Transform parent, string name, Material mat, Vector3 position, Vector3 scale, float yRotation = 0f)
        {
            GameObject path = GameObject.CreatePrimitive(PrimitiveType.Cube);
            path.name = name;
            path.transform.SetParent(parent);
            path.transform.position = position;
            path.transform.localScale = scale;
            path.transform.rotation = Quaternion.Euler(0, yRotation, 0);
            path.GetComponent<MeshRenderer>().sharedMaterial = mat;
            path.isStatic = true;
        }

        private static void BuildWaterFeatures()
        {
            // Pond
            GameObject pond = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pond.name = "ParkPond";
            pond.transform.position = new Vector3(-16f, 0.04f, 12f);
            pond.transform.localScale = new Vector3(10f, 0.05f, 7f);
            pond.GetComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Water.mat");
            pond.isStatic = true;

            // Fountain in SE quadrant
            var fountainPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_Fountain.prefab");
            if (fountainPrefab != null)
            {
                var fountain = PrefabUtility.InstantiatePrefab(fountainPrefab) as GameObject;
                fountain.transform.position = new Vector3(14f, 0, 14f);
            }
        }

        private static void BuildRecyclingStation()
        {
            GameObject stationRoot = new GameObject("RecyclingStation");
            stationRoot.transform.position = new Vector3(0, 0, 8f);

            // Concrete pad
            GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pad.name = "StationPad";
            pad.transform.SetParent(stationRoot.transform, false);
            pad.transform.localPosition = new Vector3(0, 0.035f, 0);
            pad.transform.localScale = new Vector3(10f, 0.07f, 4f);
            pad.GetComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Concrete.mat");
            pad.isStatic = true;

            // 4 Bins with spacing
            string[] binPaths = {
                "Assets/Prefabs/RecyclingBins/Bin_Plastic.prefab",
                "Assets/Prefabs/RecyclingBins/Bin_Paper.prefab",
                "Assets/Prefabs/RecyclingBins/Bin_Metal.prefab",
                "Assets/Prefabs/RecyclingBins/Bin_Organic.prefab"
            };
            float[] xOffsets = { -3.2f, -1.1f, 1.1f, 3.2f };
            for (int i = 0; i < 4; i++)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(binPaths[i]);
                var bin = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                bin.transform.SetParent(stationRoot.transform, false);
                bin.transform.localPosition = new Vector3(xOffsets[i], 0.08f, 0);
            }

            // Station sign
            var signPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_ParkSign.prefab");
            if (signPrefab != null)
            {
                var sign = PrefabUtility.InstantiatePrefab(signPrefab) as GameObject;
                sign.transform.SetParent(stationRoot.transform, false);
                sign.transform.localPosition = new Vector3(0, 0.08f, 1.6f);
                sign.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            }
        }

        private static void BuildEnvironmentProps()
        {
            GameObject propsRoot = new GameObject("EnvironmentProps");

            // Load prefabs
            var pine = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_PineTree.prefab");
            var roundTree = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_RoundTree.prefab");
            var bush = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_Bush.prefab");
            var bench = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_ParkBench.prefab");
            var lamp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_StreetLamp.prefab");
            var fence = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_Fence.prefab");
            var rock = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_Rock.prefab");
            var flower = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_FlowerCluster.prefab");
            var gazebo = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_Gazebo.prefab");
            var picnic = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_PicnicTable.prefab");
            var trashCan = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_SmallTrashCan.prefab");
            var sign = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Env_ParkSign.prefab");

            // ---- TREES (mixed pine + round, more variety) ----
            Vector3[] pinePositions = {
                new Vector3(-22, 0, 18), new Vector3(-25, 0, -14), new Vector3(20, 0, 20),
                new Vector3(24, 0, -16), new Vector3(-10, 0, 24), new Vector3(16, 0, -22),
                new Vector3(-28, 0, 5), new Vector3(28, 0, -5), new Vector3(-18, 0, -22),
                new Vector3(5, 0, 26)
            };
            PlaceMany(propsRoot.transform, pine, pinePositions, true);

            Vector3[] roundPositions = {
                new Vector3(-14, 0, -20), new Vector3(12, 0, 18), new Vector3(26, 0, 8),
                new Vector3(-26, 0, 2), new Vector3(8, 0, -26), new Vector3(-6, 0, -26),
                new Vector3(22, 0, 14)
            };
            PlaceMany(propsRoot.transform, roundTree, roundPositions, true);

            // ---- BUSHES ----
            Vector3[] bushPositions = {
                new Vector3(-7, 0, 5), new Vector3(7, 0, 5), new Vector3(-9, 0, -7),
                new Vector3(9, 0, -8), new Vector3(-17, 0, 14), new Vector3(18, 0, -12),
                new Vector3(-3, 0, 15), new Vector3(3, 0, -14),
                new Vector3(-20, 0, -8), new Vector3(20, 0, 8)
            };
            PlaceMany(propsRoot.transform, bush, bushPositions, true);

            // ---- BENCHES (along paths) ----
            PlaceSingle(propsRoot.transform, bench, new Vector3(-3.5f, 0, 2.5f), 90f);
            PlaceSingle(propsRoot.transform, bench, new Vector3(3.5f, 0, -2.5f), -90f);
            PlaceSingle(propsRoot.transform, bench, new Vector3(-2.5f, 0, -9f), 0f);
            PlaceSingle(propsRoot.transform, bench, new Vector3(2.5f, 0, 14f), 180f);
            PlaceSingle(propsRoot.transform, bench, new Vector3(10f, 0, 2f), -90f);
            PlaceSingle(propsRoot.transform, bench, new Vector3(-10f, 0, -2f), 90f);

            // ---- LAMPS (around paths and station) ----
            Vector3[] lampPositions = {
                new Vector3(-2.5f, 0, 5f), new Vector3(2.5f, 0, -5f),
                new Vector3(5f, 0, 2.5f), new Vector3(-5f, 0, -2.5f),
                new Vector3(-2.5f, 0, 12f), new Vector3(2.5f, 0, 12f),
                new Vector3(10f, 0, 10f), new Vector3(-10f, 0, -10f)
            };
            PlaceMany(propsRoot.transform, lamp, lampPositions);

            // ---- ROCKS ----
            Vector3[] rockPositions = {
                new Vector3(-13, 0, 9), new Vector3(-18, 0, 16), new Vector3(16, 0, 14),
                new Vector3(21, 0, -20), new Vector3(-22, 0, -16), new Vector3(10, 0, -20)
            };
            PlaceMany(propsRoot.transform, rock, rockPositions, true);

            // ---- FLOWERS (scattered around path edges and pond) ----
            if (flower != null)
            {
                Vector3[] flowerPositions = {
                    new Vector3(-5, 0, 4), new Vector3(5, 0, -4),
                    new Vector3(-8, 0, 3), new Vector3(8, 0, -3),
                    new Vector3(-18, 0, 8), new Vector3(-14, 0, 15),
                    new Vector3(15, 0, 10), new Vector3(-6, 0, -6),
                    new Vector3(12, 0, -8), new Vector3(-3, 0, 20)
                };

                Material[] flowerMats = {
                    AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Flower_Red.mat"),
                    AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Flower_Yellow.mat"),
                    AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Flower_White.mat"),
                    AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Flower_Purple.mat")
                };

                for (int i = 0; i < flowerPositions.Length; i++)
                {
                    var f = PrefabUtility.InstantiatePrefab(flower) as GameObject;
                    f.transform.SetParent(propsRoot.transform);
                    f.transform.position = flowerPositions[i];
                    f.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    // Assign varied color
                    var mr = f.GetComponent<MeshRenderer>();
                    if (mr != null) mr.sharedMaterial = flowerMats[i % flowerMats.Length];
                }
            }

            // ---- GAZEBO (NW quadrant) ----
            if (gazebo != null) PlaceSingle(propsRoot.transform, gazebo, new Vector3(-16, 0, -14), 15f);

            // ---- PICNIC TABLES ----
            if (picnic != null)
            {
                PlaceSingle(propsRoot.transform, picnic, new Vector3(-16, 0, -10), 30f);
                PlaceSingle(propsRoot.transform, picnic, new Vector3(18, 0, -6), -20f);
            }

            // ---- SMALL TRASH CANS (decorative) ----
            if (trashCan != null)
            {
                PlaceSingle(propsRoot.transform, trashCan, new Vector3(-2, 0, 5), 0f);
                PlaceSingle(propsRoot.transform, trashCan, new Vector3(2, 0, -5), 0f);
                PlaceSingle(propsRoot.transform, trashCan, new Vector3(5, 0, 2), 0f);
            }

            // ---- BOUNDARY FENCES (all 4 edges) ----
            if (fence != null)
            {
                float edge = 32f;
                for (float x = -edge; x <= edge; x += 3.6f)
                {
                    PlaceSingle(propsRoot.transform, fence, new Vector3(x, 0, edge), 0f);
                    PlaceSingle(propsRoot.transform, fence, new Vector3(x, 0, -edge), 0f);
                }
                for (float z = -edge; z <= edge; z += 3.6f)
                {
                    PlaceSingle(propsRoot.transform, fence, new Vector3(edge, 0, z), 90f);
                    PlaceSingle(propsRoot.transform, fence, new Vector3(-edge, 0, z), 90f);
                }
            }

            // ---- PARK ENTRANCE SIGNS ----
            if (sign != null)
            {
                PlaceSingle(propsRoot.transform, sign, new Vector3(0, 0, 31), 0f);
                PlaceSingle(propsRoot.transform, sign, new Vector3(0, 0, -31), 180f);
            }
        }

        private static void ScatterTrash()
        {
            GameObject trashRoot = new GameObject("ScatteredTrash");

            string[] trashPrefabs = {
                "Assets/Prefabs/Trash/Trash_PlasticBottle.prefab",
                "Assets/Prefabs/Trash/Trash_PlasticCup.prefab",
                "Assets/Prefabs/Trash/Trash_PlasticContainer.prefab",
                "Assets/Prefabs/Trash/Trash_PlasticBottle.prefab",

                "Assets/Prefabs/Trash/Trash_Newspaper.prefab",
                "Assets/Prefabs/Trash/Trash_CardboardBox.prefab",
                "Assets/Prefabs/Trash/Trash_PaperBall.prefab",
                "Assets/Prefabs/Trash/Trash_Newspaper.prefab",

                "Assets/Prefabs/Trash/Trash_SodaCan.prefab",
                "Assets/Prefabs/Trash/Trash_FoodCan.prefab",
                "Assets/Prefabs/Trash/Trash_MetalLid.prefab",
                "Assets/Prefabs/Trash/Trash_SodaCan.prefab",

                "Assets/Prefabs/Trash/Trash_BananaPeel.prefab",
                "Assets/Prefabs/Trash/Trash_AppleCore.prefab",
                "Assets/Prefabs/Trash/Trash_LeafPile.prefab",
                "Assets/Prefabs/Trash/Trash_BananaPeel.prefab"
            };

            Vector3[] positions = {
                new Vector3(-5f, 0.1f, -6f), new Vector3(6f, 0.1f, 4f),
                new Vector3(-8f, 0.1f, 10f), new Vector3(10f, 0.1f, -7f),

                new Vector3(-14f, 0.1f, -4f), new Vector3(16f, 0.1f, 6f),
                new Vector3(-4f, 0.1f, 14f), new Vector3(7f, 0.1f, -14f),

                new Vector3(-9f, 0.1f, -16f), new Vector3(13f, 0.1f, 17f),
                new Vector3(5f, 0.1f, 11f), new Vector3(-17f, 0.1f, 5f),

                new Vector3(-12f, 0.1f, 20f), new Vector3(18f, 0.1f, -5f),
                new Vector3(8f, 0.1f, 20f), new Vector3(-6f, 0.1f, -20f)
            };

            for (int i = 0; i < trashPrefabs.Length; i++)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(trashPrefabs[i]);
                var item = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                item.transform.SetParent(trashRoot.transform);
                item.transform.position = positions[i];
                item.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            }
        }

        private static void SetupPlayerAndCamera()
        {
            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/Player.prefab");
            var player = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
            player.transform.position = new Vector3(0, 0.01f, -3f);

            GameObject camObj = new GameObject("Main Camera");
            Camera cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 200f;
            cam.fieldOfView = 60f;
            camObj.AddComponent<AudioListener>();
            ThirdPersonCameraController camCtrl = camObj.AddComponent<ThirdPersonCameraController>();
            camCtrl.SetTarget(player.transform);
        }

        private static void SetupManagers()
        {
            // Audio Manager
            GameObject audioObj = new GameObject("AudioManager");
            AudioManager audioMgr = audioObj.AddComponent<AudioManager>();
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

            // Game Manager
            GameObject gmObj = new GameObject("GameManager");
            gmObj.AddComponent<GameManager>();
        }

        private static void BuildUI()
        {
            // Reuse the existing UI builder from the base pipeline
            // (it creates the full HUD, pause, victory, game over screens)
            // We call it via reflection to avoid code duplication
            var method = typeof(GameAssetPipeline).GetMethod("BuildGameUI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (method != null) method.Invoke(null, null);
            else Debug.LogWarning("[Stage2] Could not find BuildGameUI — UI skipped. Run base pipeline first.");
        }

        // ================================================================
        // STEP 5: Register Scene in Build Settings
        // ================================================================
        private static void RegisterSceneInBuildSettings()
        {
            Debug.Log("[Stage2] Registering scenes in Build Settings...");

            var scenes = new List<EditorBuildSettingsScene>();
            scenes.Add(new EditorBuildSettingsScene("Assets/Scenes/GameScene.unity", true));
            if (File.Exists("Assets/Scenes/SampleScene.unity"))
            {
                scenes.Add(new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", false));
            }
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        // ================================================================
        // Helpers
        // ================================================================
        private static void PlaceMany(Transform parent, GameObject prefab, Vector3[] positions, bool randomRotation = false)
        {
            if (prefab == null) return;
            foreach (var pos in positions)
            {
                var obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                obj.transform.SetParent(parent);
                obj.transform.position = pos;
                if (randomRotation) obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            }
        }

        private static void PlaceSingle(Transform parent, GameObject prefab, Vector3 position, float yRotation)
        {
            if (prefab == null) return;
            var obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            obj.transform.SetParent(parent);
            obj.transform.position = position;
            obj.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }
}
