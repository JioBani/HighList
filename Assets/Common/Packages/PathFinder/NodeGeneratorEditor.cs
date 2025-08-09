using System.Collections.Generic;
using Common.Packages.PathFinder;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NodeGenerator))]
public class NodeGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NodeGenerator generator = (NodeGenerator)target;
        

        GUILayout.Space(10);
        if (GUILayout.Button("노드 생성"))
        {
            generator.GenerateNodes();
        }

        if (GUILayout.Button("노드 삭제"))
        {
            generator.ClearNodes();
        }
        
        if (GUILayout.Button("길찾기"))
        {
            generator.FindRoute();
        }
    }
}