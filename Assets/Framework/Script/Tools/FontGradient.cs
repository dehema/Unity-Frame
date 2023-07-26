using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
/// <summary>
/// 渐变字体
/// </summary>
[AddComponentMenu("UI/Effects/Gradient")]
public class FontGradient : BaseMeshEffect
{
    public Color32 topColor = Color.white;
    public Color32 bottomColor = Color.black;
    public bool useGraphicAlpha = true;
    public void SetTopColor(Color color)
    {
        topColor = (Color32)color;
    }

    public void SetBottomColor(Color color)
    {
        bottomColor = (Color32)color;
        //Color32 color32 = new Color32((byte)color.r, (byte)color.g, (byte)color.b, (byte)color.a);
        //bottomColor = color;
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }

        var count = vh.currentVertCount;
        if (count == 0)
            return;

        var vertexs = new List<UIVertex>();
        for (var i = 0; i < count; i++)
        {
            var vertex = new UIVertex();
            vh.PopulateUIVertex(ref vertex, i);
            vertexs.Add(vertex);
        }

        var topY = vertexs[0].position.y;
        var bottomY = vertexs[0].position.y;


        for (var i = 1; i < count; i++)
        {
            var y = vertexs[i].position.y;
            if (y > topY)
            {
                topY = y;
            }
            else if (y < bottomY)
            {
                bottomY = y;
            }

        }

        var height = topY - bottomY;
        for (var i = 0; i < count; i++)
        {
            var vertex = vertexs[i];

            var color = Color32.Lerp(bottomColor, topColor, (vertex.position.y - bottomY) / height);

            vertex.color = color;

            vh.SetUIVertex(vertex, i);
        }
    }
    public void FadeAlpha(float end,float time)
    {
        //ModifyMesh(_vh);
        graphic.CrossFadeAlpha(end, time, false);
    }
}