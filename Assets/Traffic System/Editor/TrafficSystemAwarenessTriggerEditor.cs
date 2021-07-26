using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[ExecuteInEditMode]
[CustomEditor(typeof(TrafficSystemAwarenessTrigger))]
[CanEditMultipleObjects]
public class TrafficSystemAwarenessTriggerEditor : Editor
{
	TrafficSystemAwarenessTrigger TrafficSystemAwarenessTrigger;
	TrafficSystem                 TrafficSystem;

	void Awake()
	{
		TrafficSystemAwarenessTrigger = (TrafficSystemAwarenessTrigger)target;
	}

	public override void OnInspectorGUI () 
	{
//		serializedObject.Update();
		DrawDefaultInspector();

		if(!TrafficSystemAwarenessTrigger)
			return;

		if(!TrafficSystem)
		{
			GameObject obj = GameObject.Find ("Traffic System");
			if(obj && obj.GetComponent<TrafficSystem>())
				TrafficSystem = obj.GetComponent<TrafficSystem>();
		}

//		RefreshEditor( false, true );

//		GUILayout.Space(10.0f);
//		GUILayout.BeginHorizontal("box");
//		if(GUILayout.Button("Process Children"))
//		{
//			RefreshEditor();
//		}
//		GUILayout.EndHorizontal();
	}
}
