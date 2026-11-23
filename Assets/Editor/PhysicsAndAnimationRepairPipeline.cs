using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TrashRush.Art;
using TrashRush.Player;

namespace TrashRush.Editor
{
    public static class PhysicsAndAnimationRepairPipeline
    {
        [MenuItem("TrashRush/Physics & Animation Repair — Fix Obstacle Flight & Animate")]
        public static void RunRepair()
        {
            Debug.Log("[PhysicsRepair] Starting Physics and Animation Repair...");

            // 1. Generate Rigged Limb Mesh Assets
            SaveMesh(CharacterPartMeshBuilder.CreateHeadMesh(), "Assets/Art/Models/Mesh_CharHead.asset");
            SaveMesh(CharacterPartMeshBuilder.CreateTorsoMesh(), "Assets/Art/Models/Mesh_CharTorso.asset");
            SaveMesh(CharacterPartMeshBuilder.CreateLimbMesh(0.12f, 0.48f, 0.12f), "Assets/Art/Models/Mesh_CharArm.asset");
            SaveMesh(CharacterPartMeshBuilder.CreateBootMesh(), "Assets/Art/Models/Mesh_CharLeg.asset");

            AssetDatabase.Refresh();

            // 2. Build the Rigged & Animated Player Prefab
            BuildAnimatedPlayerPrefab();

            // 3. Fix Environment Colliders (benches, trees, rocks, fences)
            FixEnvironmentPrefabs();

            // 4. Update GameScene
            UpdateGameScene();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[PhysicsRepair] Physics and Animation Repair complete!");
        }

        private static void SaveMesh(Mesh m, string path)
        {
            Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing != null)
            {
                EditorUtility.CopySerialized(m, existing);
            }
            else
            {
                AssetDatabase.CreateAsset(m, path);
            }
        }

        private static void BuildAnimatedPlayerPrefab()
        {
            Debug.Log("[PhysicsRepair] Building hierarchical animated player prefab...");

            GameObject player = new GameObject("Player");
            player.tag = "Player";

            // CharacterController configuration specifically tuned to prevent obstacle climbing/flying
            CharacterController cc = player.AddComponent<CharacterController>();
            cc.center = new Vector3(0f, 0.95f, 0f);
            cc.height = 1.9f;
            cc.radius = 0.35f;
            cc.stepOffset = 0.20f;  // Lowered from 0.35m to prevent stepping onto benches/rocks
            cc.slopeLimit = 45f;
            cc.skinWidth = 0.08f;
            cc.minMoveDistance = 0.001f;

            // Model Root
            GameObject modelRoot = new GameObject("ModelRoot");
            modelRoot.transform.SetParent(player.transform, false);

            PlayerAnimator animator = modelRoot.AddComponent<PlayerAnimator>();

            // Materials
            Material bodyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Player_Body.mat");
            Material skinMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Player_Skin.mat");
            Material pantsMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/M_Player_Pants.mat");

            // Meshes
            Mesh headMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Models/Mesh_CharHead.asset");
            Mesh torsoMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Models/Mesh_CharTorso.asset");
            Mesh armMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Models/Mesh_CharArm.asset");
            Mesh legMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Models/Mesh_CharLeg.asset");

            // Bones hierarchy
            // Torso
            GameObject torso = CreateBone("Torso", modelRoot.transform, new Vector3(0f, 1.05f, 0f), torsoMesh, bodyMat);

            // Head
            GameObject head = CreateBone("Head", torso.transform, new Vector3(0f, 0.58f, 0f), headMesh, skinMat);

            // Left Arm
            GameObject armL = CreateBone("ArmL", torso.transform, new Vector3(-0.28f, 0.24f, 0f), armMesh, bodyMat);

            // Right Arm
            GameObject armR = CreateBone("ArmR", torso.transform, new Vector3(0.28f, 0.24f, 0f), armMesh, bodyMat);

            // Left Leg
            GameObject legL = CreateBone("LegL", modelRoot.transform, new Vector3(-0.14f, 0.72f, 0f), legMesh, pantsMat);

            // Right Leg
            GameObject legR = CreateBone("LegR", modelRoot.transform, new Vector3(0.14f, 0.72f, 0f), legMesh, pantsMat);

            animator.BindBones(torso.transform, head.transform, armL.transform, armR.transform, legL.transform, legR.transform);

            // Carry Point
            GameObject carryPoint = new GameObject("CarryPoint");
            carryPoint.transform.SetParent(player.transform, false);
            carryPoint.transform.localPosition = new Vector3(0.0f, 1.05f, 0.55f);

            // Components
            PlayerController pc = player.AddComponent<PlayerController>();
            SerializedObject soPC = new SerializedObject(pc);
            soPC.FindProperty("playerAnimator").objectReferenceValue = animator;
            soPC.ApplyModifiedProperties();

            PlayerInteraction pi = player.AddComponent<PlayerInteraction>();
            SerializedObject soPI = new SerializedObject(pi);
            soPI.FindProperty("carryPoint").objectReferenceValue = carryPoint.transform;
            soPI.ApplyModifiedProperties();

            PrefabUtility.SaveAsPrefabAsset(player, "Assets/Prefabs/Player/Player.prefab");
            GameObject.DestroyImmediate(player);
        }

        private static GameObject CreateBone(string name, Transform parent, Vector3 localPos, Mesh mesh, Material mat)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = localPos;

            MeshFilter mf = obj.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;

            MeshRenderer mr = obj.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mat;
            mr.enabled = true;

            return obj;
        }

        private static void FixEnvironmentPrefabs()
        {
            Debug.Log("[PhysicsRepair] Adjusting environment colliders to prevent upward launch vectors...");

            // 1. Bench: Use standard upright BoxCollider with clean vertical walls
            AdjustBoxCollider("Assets/Prefabs/Environment/Env_ParkBench.prefab", new Vector3(0f, 0.45f, 0f), new Vector3(1.6f, 0.9f, 0.55f));

            // 2. Picnic Table: Upright clean box
            AdjustBoxCollider("Assets/Prefabs/Environment/Env_PicnicTable.prefab", new Vector3(0f, 0.45f, 0f), new Vector3(2.0f, 0.9f, 1.6f));

            // 3. Rocks: Upright box collider (not low angled box that forms ramps)
            AdjustBoxCollider("Assets/Prefabs/Environment/Env_Rock.prefab", new Vector3(0f, 0.4f, 0f), new Vector3(1.4f, 0.8f, 1.2f));

            // 4. Street Lamp: Upright thin pole box
            AdjustBoxCollider("Assets/Prefabs/Environment/Env_StreetLamp.prefab", new Vector3(0f, 1.6f, 0f), new Vector3(0.4f, 3.2f, 0.4f));

            // 5. Pine Tree: Vertical CapsuleCollider grounded properly
            AdjustCapsuleCollider("Assets/Prefabs/Environment/Env_PineTree.prefab", new Vector3(0f, 1.8f, 0f), 0.45f, 3.6f);

            // 6. Round Tree: Vertical CapsuleCollider
            AdjustCapsuleCollider("Assets/Prefabs/Environment/Env_RoundTree.prefab", new Vector3(0f, 1.8f, 0f), 0.45f, 3.6f);

            // 7. Bush: Clean vertical cylinder/box
            AdjustBoxCollider("Assets/Prefabs/Environment/Env_Bush.prefab", new Vector3(0f, 0.5f, 0f), new Vector3(1.4f, 1.0f, 1.4f));

            // 8. Fountain: Clean box
            AdjustBoxCollider("Assets/Prefabs/Environment/Env_Fountain.prefab", new Vector3(0f, 0.85f, 0f), new Vector3(3.1f, 1.7f, 3.1f));
        }

        private static void AdjustBoxCollider(string prefabPath, Vector3 center, Vector3 size)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null) return;

            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            BoxCollider col = instance.GetComponent<BoxCollider>();
            if (col == null) col = instance.AddComponent<BoxCollider>();
            col.center = center;
            col.size = size;

            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            GameObject.DestroyImmediate(instance);
        }

        private static void AdjustCapsuleCollider(string prefabPath, Vector3 center, float radius, float height)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null) return;

            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            CapsuleCollider col = instance.GetComponent<CapsuleCollider>();
            if (col == null) col = instance.AddComponent<CapsuleCollider>();
            col.center = center;
            col.radius = radius;
            col.height = height;

            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            GameObject.DestroyImmediate(instance);
        }

        private static void UpdateGameScene()
        {
            Debug.Log("[PhysicsRepair] Updating GameScene with repaired player and environment...");

            var scene = EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");

            // Replace player
            GameObject oldPlayer = GameObject.Find("Player") ?? GameObject.FindWithTag("Player");
            Vector3 pos = new Vector3(0f, 0.05f, -3.0f);
            if (oldPlayer != null)
            {
                pos = oldPlayer.transform.position;
                GameObject.DestroyImmediate(oldPlayer);
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/Player.prefab");
            GameObject player = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            player.name = "Player";
            player.transform.position = pos;

            // Camera target
            Camera cam = Camera.main;
            if (cam != null)
            {
                ThirdPersonCameraController camCtrl = cam.GetComponent<ThirdPersonCameraController>();
                if (camCtrl != null)
                {
                    camCtrl.SetTarget(player.transform);
                }
            }

            // Ensure WalkingPaths and CentralPlaza colliders don't jut above ground level
            GameObject pathsRoot = GameObject.Find("WalkingPaths");
            if (pathsRoot != null)
            {
                for (int i = 0; i < pathsRoot.transform.childCount; i++)
                {
                    Transform t = pathsRoot.transform.GetChild(i);
                    BoxCollider bc = t.GetComponent<BoxCollider>();
                    if (bc != null)
                    {
                        // Ensure flat level collision with ground plane
                        Vector3 c = bc.center;
                        Vector3 s = bc.size;
                        bc.size = new Vector3(s.x, 0.01f, s.z);
                        bc.center = new Vector3(c.x, 0.005f, c.z);
                    }
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }
}
