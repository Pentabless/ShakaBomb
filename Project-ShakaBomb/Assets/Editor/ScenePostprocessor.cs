//==============================================================================================
/// File Name    : ScenePostprocessor.cs
/// Summary      : シーンファイルが作成、削除された時に自動でScenes In Buildに追加、削除する
//==============================================================================================
using System.Linq;
using UnityEditor;
//==============================================================================================
public sealed class ScenePostprocessor : AssetPostprocessor
{
    //------------------------------------------------------------------------------------------
    // summary : OnPostprocessAllAssets
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private static void OnPostprocessAllAssets(
    string[] importedAssets,
    string[] deletedAssets,
    string[] movedAssets,
    string[] movedFromAssetPaths
        )
    {
        var sceneList = EditorBuildSettings.scenes.ToList();

        sceneList.AddRange(importedAssets
            .Where(c => c.EndsWith(".unity"))
            .Where(c => !sceneList.Any(s => s.path == c))
            .Select(c => new EditorBuildSettingsScene(c, true)));
        sceneList.RemoveAll(c => deletedAssets.Contains(c.path));

        EditorBuildSettings.scenes = sceneList.ToArray();
    }
}