using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Yoz.Editor
{
    public class OpenSceneWindow : EditorWindow
    {
        private readonly Vector2 _sceneOpenButtonMinSize = new Vector2(70, 10);
        private readonly Vector2 _sceneOpenButtonMaxSize = new Vector2(300, 50);
        private readonly Vector2 _buttonMinSize = new Vector2(100, 20);
        private readonly Vector2 _buttonMaxSize = new Vector2(1000, 100);
        private List<string> _sceneNames = new List<string>();


        [MenuItem("YozTool/SceneWindow")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow<OpenSceneWindow>();
        }

        private void OnGUI()
        {
            GUIStyle buttonStyle = new GUIStyle("button") {fontSize = 20};
            var layoutOptions = new GUILayoutOption[]
            {
                GUILayout.MinWidth(_sceneOpenButtonMinSize.x),
                GUILayout.MinHeight(_sceneOpenButtonMinSize.y),
                GUILayout.MaxWidth(_sceneOpenButtonMaxSize.x),
                GUILayout.MaxHeight(_sceneOpenButtonMaxSize.y),
            };

            foreach (var sceneName in _sceneNames)
            {
                Debug.Log(sceneName);
                if (GUILayout.Button(sceneName, buttonStyle, layoutOptions))
                {
                    if (!EditorSceneManager.SaveModifiedScenesIfUserWantsTo(
                            new Scene[] {SceneManager.GetActiveScene()})) return;
                
                    OpenScene(sceneName);
                }
            }
            
            // Refresh Scenes In Build button
            if (GUILayout.Button("Refresh scenes", buttonStyle, layoutOptions))
            {
                _sceneNames = EditorBuildSettings.scenes
                    .Where(scene => scene.enabled)
                    .Select(scene => Path.GetFileNameWithoutExtension(scene.path))
                    .ToList();
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