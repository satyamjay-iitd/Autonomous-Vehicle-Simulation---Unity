using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class TrafficSystemEditorWindow : EditorWindow	
{
	[MenuItem ("Traffic System/Spawn Traffic System Prefab")]	
	public static void  SpawnTrafficSystemPrefab () 
	{
		if(TrafficSystem.Instance)
		{
			// Debug.LogError("Traffic System already exists");
			return;
		}

		TrafficSystem[] trafficSystems = GameObject.FindObjectsOfType<TrafficSystem>();

		if(trafficSystems.Length > 0)
		{
			Debug.LogError("Traffic System already exists");
			return;
		}

		string trafficSystemPrefabPath  = "Assets/Traffic System/Prefabs/Traffic System.prefab";

		TrafficSystem trafficSystem = AssetDatabase.LoadAssetAtPath(trafficSystemPrefabPath, typeof(TrafficSystem)) as TrafficSystem;

		if(trafficSystem)
			PrefabUtility.InstantiatePrefab(trafficSystem);
		else
			Debug.LogError("Traffic System Prefab can not be found at location: " + trafficSystemPrefabPath);
	}

//	[MenuItem ("Traffic System/Globals")]	
//	public static void  ShowEditor () 
//	{
//		EditorWindow.GetWindow(typeof(TrafficSystemEditorWindow));
//	}
}
