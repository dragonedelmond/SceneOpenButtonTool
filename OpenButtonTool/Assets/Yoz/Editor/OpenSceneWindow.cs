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
        private readonly Vector2 _sceneOpenButtonMinSize = new Vector2(100, 10);
        private readonly Vector2 _sceneOpenButtonMaxSize = new Vector2(400, 50);
        private readonly Vector2 _setButtonMinSize = new Vector2(100, 20);
        private readonly Vector2 _setButtonMaxSize = new Vector2(1000, 70);
        private List<string> _sceneNames = new List<string>();
        private bool _flag = false;

        [MenuItem("YozTool/SceneWindow")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow<OpenSceneWindow>();
        }

        private void OnGUI()
        {
            // Set Open Button Layout
            GUIStyle openButtonStyle = new GUIStyle("button") {fontSize = 20};
            var layoutOptions = new GUILayoutOption[]
            {
                GUILayout.MinWidth(_sceneOpenButtonMinSize.x),
                GUILayout.MinHeight(_sceneOpenButtonMinSize.y),
                GUILayout.MaxWidth(_sceneOpenButtonMaxSize.x),
                GUILayout.MaxHeight(_sceneOpenButtonMaxSize.y),
            };
            
            // Set Reload Button Layout
            GUIStyle setButtonStyle = new GUIStyle("button") {fontSize = 30};
            setButtonStyle.normal.textColor = Color.cyan;
            var setLayoutOptions = new GUILayoutOption[]
            {
                GUILayout.MinWidth(_setButtonMinSize.x),
                GUILayout.MinHeight(_setButtonMinSize.y),
                GUILayout.MaxWidth(_setButtonMaxSize.x),
                GUILayout.MaxHeight(_setButtonMaxSize.y),
            };

            // Set Open Button Scenes
            foreach (var sceneName in _sceneNames)
            {
                // Open Button
                if (GUILayout.Button(sceneName, openButtonStyle, layoutOptions) || _flag == false)
                {
                    _flag = true;
# if UNITY_EDITOR
                    if (!EditorSceneManager.SaveModifiedScenesIfUserWantsTo(
                            new Scene[] {SceneManager.GetActiveScene()})) return;
#endif
                    OpenScene(sceneName);
                }
            }

            // Reload Button
            if (GUILayout.Button("Reload Scenes", setButtonStyle, setLayoutOptions))
            {
                ReloadScenes();
            }
        }

        /// <summary>
        /// Reload the scenes in Build Settings
        /// </summary>
        private void ReloadScenes()
        {
            _sceneNames = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => Path.GetFileNameWithoutExtension(scene.path))
                .ToList();
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