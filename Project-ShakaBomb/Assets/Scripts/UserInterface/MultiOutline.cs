//==============================================================================================
/// File Name	: MultiOutline.cs
/// Summary		: 
//==============================================================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//==============================================================================================
[RequireComponent(typeof(Text))]
//==============================================================================================
public class MultiOutline : BaseMeshEffect
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField, Header("頂点数")]
    [Range(0, 100)]
    private int amount = 0;
    [SerializeField, Header("アウトラインの色")]
    private Color color = Color.black;
    [SerializeField]
    private float offset = 0.0f;

    private readonly List<UIVertex> outlineVertexList = new List<UIVertex>();
    private readonly List<UIVertex> vertexList = new List<UIVertex>();



    //------------------------------------------------------------------------------------------
    // ModifyMesh
    //------------------------------------------------------------------------------------------
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        vertexList.Clear();
        outlineVertexList.Clear();
        vh.GetUIVertexStream(vertexList);

        var splitAngle = 360.0f / amount;
        UIVertex v;

        var count = vertexList.Count;
        for (var i = 0; i < amount; i++)
        {
            var angle = splitAngle * i;
            for (var j = 0; j < count; j++)
            {
                v = vertexList[j];
                var pos = v.position;
                pos.x += Mathf.Cos(angle * Mathf.Deg2Rad) * offset;
                pos.y += Mathf.Sin(angle * Mathf.Deg2Rad) * offset;
                v.position = pos;
                v.color = color;
                outlineVertexList.Add(v);
            }
        }

        outlineVertexList.AddRange(vertexList);

        vh.Clear();
        vh.AddUIVertexTriangleStream(outlineVertexList);
    }
}
