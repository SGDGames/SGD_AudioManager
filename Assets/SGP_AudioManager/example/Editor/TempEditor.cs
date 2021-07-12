using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Temp))]
public class TempEditor : Editor
{
    private Temp t;

    private void OnEnable()
    {
        t = target as Temp;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Load")) t.Load();
        if (GUILayout.Button("Play")) t.Play();
    }
}