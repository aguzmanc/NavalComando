using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof (Ship))]
public class ShipEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("Shoot RIGHT cannons")) {
            ((Ship)target).RightCannons.Shoot();
        }

        if(GUILayout.Button("Shoot LEFT cannons")) {
            ((Ship)target).LeftCannons.Shoot();
        }

        if(GUILayout.Button("SINK")) {
            ((Ship)target).Sink();
        }

        if(GUILayout.Button("COLLISION")) {
            ((Ship)target).DummyCollision();
        }
    }
}
