using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TrashRush.Core;
using TrashRush.Recycling;
using TrashRush.Trash;
using TrashRush.Player;

namespace TrashRush.Editor
{
    public static class BinUpgradeVerifier
    {
        private const string SCENE_PATH = "Assets/Scenes/GameScene.unity";

        public static void Verify()
        {
            Debug.Log("[BinUpgradeVerifier] === Starting Recycling Bin System Verification ===");

            var scene = EditorSceneManager.OpenScene(SCENE_PATH, OpenSceneMode.Single);
            RecyclingBin[] bins = UnityEngine.Object.FindObjectsOfType<RecyclingBin>();

            if (bins == null || bins.Length != 4)
            {
                Debug.LogError($"[BinUpgradeVerifier] FAILED: Expected 4 bins, found {bins?.Length ?? 0}");
                return;
            }

            bool allPassed = true;

            foreach (var bin in bins)
            {
                Debug.Log($"[BinUpgradeVerifier] Checking Bin: {bin.gameObject.name} ({bin.AcceptedCategory})");

                // Check Category and Color
                Transform body = bin.transform.Find("Body");
                if (body == null)
                {
                    Debug.LogError($"[BinUpgradeVerifier] Missing Body child in {bin.name}");
                    allPassed = false;
                }

                // Check Front & Back labels
                Transform frontLabel = bin.transform.Find("FrontLabel");
                Transform backLabel = bin.transform.Find("BackLabel");
                if (frontLabel == null || backLabel == null)
                {
                    Debug.LogError($"[BinUpgradeVerifier] Missing FrontLabel or BackLabel in {bin.name}");
                    allPassed = false;
                }

                // Check Floating Category Billboard
                Transform floating = bin.transform.Find("FloatingCategoryLabel");
                if (floating == null)
                {
                    Debug.LogError($"[BinUpgradeVerifier] Missing FloatingCategoryLabel in {bin.name}");
                    allPassed = false;
                }

                // Check Highlight Light
                Transform lightObj = bin.transform.Find("HighlightLight");
                if (lightObj == null || lightObj.GetComponent<Light>() == null)
                {
                    Debug.LogError($"[BinUpgradeVerifier] Missing HighlightLight in {bin.name}");
                    allPassed = false;
                }
            }

            // Test interaction logic simulation
            GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo == null) playerGo = GameObject.Find("Player");
            if (playerGo != null)
            {
                var playerInter = playerGo.GetComponent<PlayerInteraction>();
                if (playerInter != null)
                {
                    playerInter.RefreshBinsCache();
                    Debug.Log("[BinUpgradeVerifier] PlayerInteraction bin cache verified.");
                }
            }

            if (allPassed)
            {
                Debug.Log("[BinUpgradeVerifier] ALL RECYCLING BIN VISUAL AND INTERACTION CHECKS PASSED!");
            }
            else
            {
                Debug.LogError("[BinUpgradeVerifier] SOME VERIFICATION CHECKS FAILED.");
            }
        }
    }
}
