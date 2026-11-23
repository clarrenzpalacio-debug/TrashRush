using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TrashRush.Player;

namespace TrashRush.Editor
{
    public static class Stage3VerificationPipeline
    {
        [MenuItem("TrashRush/Stage 3 — Verify Player & Camera")]
        public static void VerifyAndSetup()
        {
            Debug.Log("[Stage3Verification] Opening GameScene.unity to verify Player and Camera...");

            var scene = EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");

            // 1. Locate Player
            GameObject player = GameObject.Find("Player") ?? GameObject.FindWithTag("Player");
            if (player == null)
            {
                var playerComp = Object.FindObjectOfType<PlayerController>();
                if (playerComp != null) player = playerComp.gameObject;
            }

            if (player == null)
            {
                Debug.LogError("[Stage3Verification] Player GameObject not found in GameScene!");
                return;
            }

            player.tag = "Player";

            // Verify / configure CharacterController
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc == null) cc = player.AddComponent<CharacterController>();
            cc.center = new Vector3(0f, 0.95f, 0f);
            cc.height = 1.9f;
            cc.radius = 0.35f;
            cc.stepOffset = 0.35f;
            cc.slopeLimit = 45f;
            cc.minMoveDistance = 0.001f;

            // Ensure Player starts firmly grounded at path intersection
            player.transform.position = new Vector3(0f, 0.05f, -3.0f);

            // 2. Locate Camera
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                var camObj = GameObject.Find("Main Camera");
                if (camObj != null) mainCam = camObj.GetComponent<Camera>();
            }

            if (mainCam != null)
            {
                mainCam.tag = "MainCamera";
                mainCam.nearClipPlane = 0.1f;
                mainCam.farClipPlane = 250f;
                mainCam.fieldOfView = 60f;

                ThirdPersonCameraController camCtrl = mainCam.GetComponent<ThirdPersonCameraController>();
                if (camCtrl == null) camCtrl = mainCam.gameObject.AddComponent<ThirdPersonCameraController>();

                camCtrl.SetTarget(player.transform);
            }
            else
            {
                Debug.LogError("[Stage3Verification] Main Camera not found in GameScene!");
                return;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Stage3Verification] Stage 3 Player and Camera verification complete. Scene saved!");
        }
    }
}
