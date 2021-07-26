using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[ExecuteInEditMode]
[CustomEditor(typeof(TrafficSystemIntersection))]
[CanEditMultipleObjects]
public class TrafficSystemIntersectionEditor : Editor
{
	TrafficSystemIntersection TrafficSystemIntersection;
	TrafficSystem             TrafficSystem;

	void Awake()
	{
		TrafficSystemIntersection = (TrafficSystemIntersection)target;

//		Debug.Log (TrafficSystemPiece.gameObject.name + ": " + TrafficSystemPiece.m_renderer.bounds.extents);
	}

	public override void OnInspectorGUI () 
	{
//		serializedObject.Update();
		DrawDefaultInspector();

		if(!TrafficSystemIntersection)
			return;

		if(!TrafficSystem)
		{
			GameObject obj = GameObject.Find ("Traffic System");
			if(obj && obj.GetComponent<TrafficSystem>())
				TrafficSystem = obj.GetComponent<TrafficSystem>();
		}

		GUILayout.BeginHorizontal("box");
		if(GUILayout.Button("Process Children"))
		{
			TrafficSystemIntersection.Refresh();
		}
		GUILayout.EndHorizontal();
	}
}
