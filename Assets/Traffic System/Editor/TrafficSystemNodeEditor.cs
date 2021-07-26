using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[CustomEditor(typeof(TrafficSystemNode))]
[CanEditMultipleObjects]
public class TrafficSystemNodeEditor : Editor 
{
	TrafficSystemNode               TrafficSystemNode;
	private TrafficSystem.DriveSide m_previousDriveSide       = TrafficSystem.DriveSide.LEFT;
	private bool                    m_previousIsPrimary       = true;
	TrafficSystem                   TrafficSystem;

	void Awake()
	{
		TrafficSystemNode = (TrafficSystemNode)target;
		m_previousDriveSide = TrafficSystemNode.m_driveSide;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

//		serializedObject.Update();

		if(m_previousDriveSide != TrafficSystemNode.m_driveSide ||
		   m_previousIsPrimary != TrafficSystemNode.m_isPrimary)
		{
			if(TrafficSystemNode.m_driveSide == TrafficSystem.DriveSide.LEFT)
			{
				if(TrafficSystemNode.m_isPrimary)
				{
					Material material = AssetDatabase.LoadAssetAtPath(TrafficSystemEditor.PrimaryNodeLeftSideMaterial, typeof(Material)) as Material;
					TrafficSystemNode.GetComponent<Renderer>().material = material;
				}
				else
				{
					Material material = AssetDatabase.LoadAssetAtPath(TrafficSystemEditor.SecondaryNodeLeftSideMaterial, typeof(Material)) as Material;
					TrafficSystemNode.GetComponent<Renderer>().material = material;
				}
			}
			else
			{
				if(TrafficSystemNode.m_isPrimary)
				{
					Material material = AssetDatabase.LoadAssetAtPath(TrafficSystemEditor.PrimaryNodeRightSideMaterial, typeof(Material)) as Material;
					TrafficSystemNode.GetComponent<Renderer>().material = material;
				}
				else
				{
					Material material = AssetDatabase.LoadAssetAtPath(TrafficSystemEditor.SecondaryNodeRightSideMaterial, typeof(Material)) as Material;
					TrafficSystemNode.GetComponent<Renderer>().material = material;
				}			
			}

			m_previousDriveSide = TrafficSystemNode.m_driveSide;
			m_previousIsPrimary = TrafficSystemNode.m_isPrimary;
		}
	}

	void OnSceneGUI()
	{
		if(!TrafficSystem)
		{
			GameObject obj = GameObject.Find ("Traffic System");
			if(obj && obj.GetComponent<TrafficSystem>())
				TrafficSystem = obj.GetComponent<TrafficSystem>();
		}
		
		if(TrafficSystem && TrafficSystem.Instance)
		{
			if(TrafficSystem.Instance.TooltipAnchor)
				TrafficSystem.Instance.TooltipAnchor.transform.Rotate(Vector3.up, 1.0f); 
		}

//		if(!TrafficSystemNode.m_connectedLocalNode)
//		{
//			for(int nIndex = 0; nIndex < TrafficSystemNode.m_connectedNodes.Count; nIndex++)
//			{
//				TrafficSystemNode connectedNode = TrafficSystemNode.m_connectedNodes[nIndex];
//				if(connectedNode)
//				{
//					Handles.color = Color.red;
//					Handles.ArrowCap(0,
//					                 connectedNode.transform.position + new Vector3(5,0,0),
//					                 connectedNode.transform.rotation,
//					                 10.0f);
//				}
//			}
//		}
	}
}
