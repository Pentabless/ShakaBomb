//==============================================================================================
/// File Name	: Utility.cs
/// Summary		: 汎用関数
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//==============================================================================================
public static class Utility
{
    /// <summary>
    /// 自身と子の特定のコンポーネントを全て取得する
    /// </summary>
    /// <typeparam name="T">コンポーネント</typeparam>
    /// <param name="objects">取得元のオブジェクト</param>
    /// <param name="includeThis">自身を含むかどうか</param>
    /// <returns>
    /// 取得できたコンポーネント
    /// </returns>
    public static List<T> GetAllComponents<T>(List<GameObject> objects, bool includeThis)
    {
        var allComponents = new List<T>();
        foreach(var obj in objects)
        {
            if (includeThis)
            {
                var componets = obj.GetComponents<T>();
                allComponents.AddRange(componets);
            }
            var childComponents = obj.GetComponentsInChildren<T>();
            allComponents.AddRange(childComponents);
        }
        return allComponents;
    }

    /// <summary>
    /// Imageのカラーを取得する
    /// </summary>
    /// <param name="images"></param>
    /// <returns>
    /// カラー
    /// </returns>
    public static List<Color> GetColors(List<Image> images)
    {
        var colors = new List<Color>();
        foreach(var image in images)
        {
            colors.Add(image.color);
        }
        return colors;
    }

    /// <summary>
    /// Textのカラーを取得する
    /// </summary>
    /// <param name="texts"></param>
    /// <returns>
    /// カラー
    /// </returns>
    public static List<Color> GetColors(List<Text> texts)
    {
        var colors = new List<Color>();
        foreach (var text in texts)
        {
            colors.Add(text.color);
        }
        return colors;
    }
}