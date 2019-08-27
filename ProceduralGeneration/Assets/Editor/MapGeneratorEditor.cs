using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Generator))]
public class MapGeneratorEditor : Editor {


    public override void OnInspectorGUI()
    {
        Generator mapGen = (Generator)target;

        if (DrawDefaultInspector()){
            if(mapGen.autoUpdate)
                mapGen.DrawMap();
        }
        if (GUILayout.Button("Create")) {
            mapGen.DrawMap();
        }
        
    }
}
