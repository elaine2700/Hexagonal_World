using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexTile))]
public class HexTileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        HexTile hexTile = (HexTile)target;
        if(GUILayout.Button("Create Hexagon"))
        {
            hexTile.DrawMesh();
        }
    }
}
