using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexGrid))]
public class HexGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        HexGrid hexGrid = (HexGrid)target;

        if(GUILayout.Button("Update Grid"))
        {
            hexGrid.UpdateGrid();
        }
    }
}
