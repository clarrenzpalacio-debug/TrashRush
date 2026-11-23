using System.IO;
using UnityEditor;
using UnityEngine;

namespace TrashRush.Editor
{
    [InitializeOnLoad]
    public static class AutoAssetRunner
    {
        private const string RanKey = "TrashRush_AutoAssetRunner_Ran_v1";

        static AutoAssetRunner()
        {
            EditorApplication.delayCall += OnEditorReady;
        }

        private static void OnEditorReady()
        {
            if (EditorPrefs.GetBool(RanKey, false))
            {
                return;
            }

            Debug.Log("[AutoAssetRunner] Detected project launch in active Unity Editor. Running asset pipeline...");
            EditorPrefs.SetBool(RanKey, true);
            GameAssetPipeline.GenerateAllAndBuild();
        }

        [MenuItem("TrashRush/Force Re-run Asset Generation")]
        public static void ForceRerun()
        {
            EditorPrefs.DeleteKey(RanKey);
            GameAssetPipeline.GenerateAllAndBuild();
        }
    }
}
