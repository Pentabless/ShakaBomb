//==============================================================================================
/// File Name	: PlaySceneEditorWindow.cs
/// Summary		: 実行時、指定のシーンから再生する
//==============================================================================================
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
//==============================================================================================
public class PlaySceneEditorWindow : EditorWindow
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    private SceneAsset startScene = null;



    //------------------------------------------------------------------------------------------
    // summary : ShowWindow
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    [MenuItem("Tools/SetStartScene")]
    private static void ShowWindow()
    {
        var window = GetWindow<PlaySceneEditorWindow>();
        window.titleContent = new GUIContent("Start Scene");
        window.Show();
    }



    //------------------------------------------------------------------------------------------
    // summary : OnEnable
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void OnEnable()
    {
        startScene = EditorSceneManager.playModeStartScene;
    }



    //------------------------------------------------------------------------------------------
    // summary : OnGUI
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void OnGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            startScene = (SceneAsset)EditorGUILayout.ObjectField("StartSecen", startScene, typeof(SceneAsset));
            if (check.changed)
            {
                EditorSceneManager.playModeStartScene = startScene;
            }
        }

        // Start Scene を削除
        using (new EditorGUI.DisabledScope(startScene == null))
        {
            if (GUILayout.Button("Reset Start Scene"))
            {
                EditorSceneManager.playModeStartScene = null;
            }
        }
    }
}
