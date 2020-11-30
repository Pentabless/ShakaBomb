//==============================================================================================
/// File Name	: AppealGraphic.cs
/// Summary		: 
//==============================================================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//==============================================================================================
[RequireComponent(typeof(Image))]
//==============================================================================================
public class AppealGraphic : BaseMeshEffect
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    private bool playOnAwake = true;
    [SerializeField]
    private float power;
    [SerializeField]
    private float time;
    [SerializeField]
    private bool isRepeat;
    [SerializeField]
    private float repeatInterval;

    private float current;
    private float t;
    private readonly List<UIVertex> vertexList = new List<UIVertex>();

    private static readonly Vector3[] normals =
    {
        new Vector3(-1, -1, 0).normalized,
        new Vector3(-1, 1, 0).normalized,
        new Vector3(1, 1, 0).normalized,
        new Vector3(1, 1, 0).normalized,
        new Vector3(1, -1, 0).normalized,
        new Vector3(-1, -1, 0).normalized,
    };


    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        current = playOnAwake ? 0 : time;
    }



    //------------------------------------------------------------------------------------------
    // Play
    //------------------------------------------------------------------------------------------
    private void Play()
    {
        current = 0;
    }



    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        if (current < 0)
        {
            current += Time.deltaTime;
            return;
        }
        if (current > time)
        {
            if (isRepeat)
                current = -repeatInterval;

            return;
        }

        current += Time.deltaTime;
        t = Mathf.Clamp(current / time, 0, 1);
        graphic.SetVerticesDirty();
    }



    //------------------------------------------------------------------------------------------
    // ModifyMesh
    //------------------------------------------------------------------------------------------
    public override void ModifyMesh(VertexHelper helper)
    {
        if (vertexList.Count != 6)
        {
            vertexList.Clear();
            helper.GetUIVertexStream(vertexList);

            var count = vertexList.Count;
            for (var i = 0; i < count; ++i)
            {
                var vertex = vertexList[i];
                var vertexDuplicate = vertexList[i];

                vertexList[i] = vertex;
                vertexList.Add(vertexDuplicate);
            }
        }

        for (int i = 0; i < 6; i++)
        {
            var vertex = vertexList[i + 6];
            vertex.position += normals[i] * power * t;
            var c = vertex.color;
            c.a = (byte)(255 * (1f - t));
            vertex.color = c;
            vertexList[i + 6] = vertex;
        }

        helper.Clear();
        helper.AddUIVertexTriangleStream(vertexList);
    }
}
