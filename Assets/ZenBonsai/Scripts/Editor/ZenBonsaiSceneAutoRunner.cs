#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ZenBonsai.Editor
{
    [InitializeOnLoad]
    public class ZenBonsaiSceneAutoRunner : IPreprocessBuildWithReport
    {
        private const string FlagKey = "ZenBonsai_BuildScene_HD_v24";

        public int callbackOrder => 0;

        static ZenBonsaiSceneAutoRunner()
        {
            EditorApplication.delayCall += RunOnce;
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            RunOnce();
        }

        private static void RunOnce()
        {
            if (!SessionState.GetBool(FlagKey, false))
            {
                SessionState.SetBool(FlagKey, true);
                BuildAndSaveActiveScene();
            }
        }

        private static void BuildAndSaveActiveScene()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            Debug.Log("<color=yellow>[AutoRunner]</color> Ejecutando montaje HD v24 en la escena activa...");
            ZenBonsaiSetupEditor.BuildZenBonsaiScene();
            var activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.IsValid())
            {
                EditorSceneManager.SaveScene(activeScene);
                Debug.Log("<color=green>[AutoRunner]</color> ¡Escena v24 guardada exitosamente!");
            }
        }
    }
}
#endif
