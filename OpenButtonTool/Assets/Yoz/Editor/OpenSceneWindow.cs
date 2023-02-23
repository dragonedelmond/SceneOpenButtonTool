using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Yoz.Editor
{
    public class OpenSceneWindow : EditorWindow
    {
        private readonly Vector2 _buttonMinSize = new Vector2(100, 20);
        private readonly Vector2 _buttonMaxSize = new Vector2(1000, 100);

        [MenuItem("YozTool/SceneWindow")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow<OpenSceneWindow>();
        }

        private void OnGUI()
        {
            GUIStyle buttonStyle = new GUIStyle("button") {fontSize = 30};
            var layoutOptions = new GUILayoutOption[]
            {
                GUILayout.MinWidth(_buttonMinSize.x),
                GUILayout.MinHeight(_buttonMinSize.y),
                GUILayout.MaxWidth(_buttonMaxSize.x),
                GUILayout.MaxHeight(_buttonMaxSize.y),
            };
            
            // Title Button
            if (GUILayout.Button("Title", buttonStyle, layoutOptions))
            {
                if (!EditorSceneManager.SaveModifiedScenesIfUserWantsTo(
                        new Scene[] {SceneManager.GetActiveScene()})) return;
                
                OpenScene("Title");
            }
        }

        private void OpenScene(string sceneName)
        {
            var sceneAssets = AssetDatabase.FindAssets("t:SceneAsset")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset)))
                .Where(obj => obj != null)
                .Select(obj => (SceneAsset) obj)
                .Where(asset => asset.name == sceneName);
            var scenePath = AssetDatabase.GetAssetPath(sceneAssets.First());
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}

