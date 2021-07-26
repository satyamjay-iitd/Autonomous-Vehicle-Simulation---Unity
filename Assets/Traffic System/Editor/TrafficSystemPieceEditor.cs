using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[ExecuteInEditMode]
[CustomEditor(typeof(TrafficSystemPiece))]
[CanEditMultipleObjects]
public class TrafficSystemPieceEditor : Editor
{
	TrafficSystemPiece TrafficSystemPiece;
	TrafficSystem      TrafficSystem;

	private float      m_rotateDegrees             = 90.0f;

	void Awake()
	{
		TrafficSystemPiece = (TrafficSystemPiece)target;

		TrafficSystemPiece.CleanUp( TrafficSystemPiece );

//		Debug.Log (TrafficSystemPiece.gameObject.name + ": " + TrafficSystemPiece.m_renderer.bounds.extents);
	}

	public override void OnInspectorGUI () 
	{
//		serializedObject.Update();
		DrawDefaultInspector();

		if(!TrafficSystemPiece)
			return;

		if(!TrafficSystem)
		{
			GameObject obj = GameObject.Find ("Traffic System");
			if(obj && obj.GetComponent<TrafficSystem>())
				TrafficSystem = obj.GetComponent<TrafficSystem>();
		}

		RefreshEditor( false, true );

		if(TrafficSystemPiece.m_primaryLeftLaneNodes.Count > 0 || TrafficSystemPiece.m_primaryRightLaneNodes.Count > 0 || TrafficSystemPiece.m_leftLaneChangeNodes.Count > 0 || TrafficSystemPiece.m_rightLaneChangeNodes.Count > 0)
		{
			GUILayout.Space(10.0f);
			GUILayout.Label("Primary Nodes:"); 

			int connections = 0;
			for(int lNIndex = 0; lNIndex < TrafficSystemPiece.m_primaryLeftLaneNodes.Count; lNIndex++)
			{
				TrafficSystemNode lastNode = TrafficSystemPiece.m_primaryLeftLaneNodes[lNIndex];
				if(lastNode)
				{
					for(int cNIndex = 0; cNIndex < lastNode.m_connectedNodes.Count; cNIndex++)
					{
						TrafficSystemNode currentNode = lastNode.m_connectedNodes[cNIndex];
						if(currentNode)
						{
							string driverSide = "Pink";
							if(currentNode.m_driveSide == TrafficSystem.DriveSide.LEFT)
								driverSide = "Blue";
							
							GUILayout.BeginVertical("box");
							GUILayout.Label((lNIndex + 1) + " " + driverSide + " : " + currentNode.Parent); 
							GUILayout.Space(1.0f);
							GUILayout.BeginHorizontal("box");
							if(GUILayout.Button("Reveal ->"))
							{
								TrafficSystem.ClearCLRevealObjsFrom();
								TrafficSystem.ClearCLRevealObjsTo();
								TrafficSystem.AddToCLRevealObjsFrom(lastNode.transform);
								TrafficSystem.AddToCLRevealObjsTo(currentNode.transform);
							}
							if(GUILayout.Button("Remove"))
							{
								lastNode.m_connectedNodes.Remove(currentNode);
								EditorUtility.SetDirty(lastNode);
								TrafficSystem.ClearCLRevealObjsFrom();
								TrafficSystem.ClearCLRevealObjsTo();
							}
							if(GUILayout.Button("Switch Link"))
							{
								bool nodeFound = false;
								for(int cIndex = 0; cIndex < currentNode.m_connectedNodes.Count; cIndex++)
								{
									TrafficSystemNode nodeToSwitch = currentNode.m_connectedNodes[cIndex];
									if(nodeToSwitch == lastNode)
									{
										nodeFound = true;
										break;
									}
								}

								if(!nodeFound)
								{
									if(lastNode)
										currentNode.m_connectedNodes.Add(lastNode);
									if(currentNode)
										lastNode.m_connectedNodes.Remove(currentNode);

									EditorUtility.SetDirty(currentNode);
									EditorUtility.SetDirty(lastNode);
									TrafficSystem.ClearCLRevealObjsFrom();
									TrafficSystem.ClearCLRevealObjsTo();
								}
							}
							GUILayout.EndHorizontal();
							connections++;
							GUILayout.EndVertical();
						}
					}

					for(int cIndex = 0; cIndex < lastNode.m_connectedChangeLaneNodes.Count; cIndex++)
					{
						if(lastNode.m_connectedChangeLaneNodes[cIndex] && lastNode.m_connectedChangeLaneNodes[cIndex].Parent != TrafficSystemPiece )
						{
							string driverSide = "Pink";
							if(lastNode.m_connectedChangeLaneNodes[cIndex].m_driveSide == TrafficSystem.DriveSide.LEFT)
								driverSide = "Blue";

							GUILayout.BeginVertical("box");
							GUILayout.Label( "ID: " + lastNode.m_connectedChangeLaneNodes[cIndex].m_changeLaneID + " " + driverSide + " CL Inbound  : " + lastNode.m_connectedChangeLaneNodes[cIndex].Parent); 
							GUILayout.Space(1.0f);
							GUILayout.BeginHorizontal("box");
							if(GUILayout.Button("Reveal ->"))
							{
								TrafficSystem.ClearCLRevealObjsFrom();
								TrafficSystem.ClearCLRevealObjsTo();
								TrafficSystem.AddToCLRevealObjsFrom(lastNode.transform);
								TrafficSystem.AddToCLRevealObjsTo(lastNode.m_connectedChangeLaneNodes[cIndex].transform);
							}
							if(GUILayout.Button("Remove"))
							{
								lastNode.RemoveChangeLangeNode(lastNode.m_connectedChangeLaneNodes[cIndex]);
								EditorUtility.SetDirty(lastNode);
								TrafficSystem.ClearCLRevealObjsFrom();
								TrafficSystem.ClearCLRevealObjsTo();
							}
							GUILayout.EndHorizontal();
							GUILayout.EndVertical();
						}
					}
				}
			}

			for(int lNIndex = 0; lNIndex < TrafficSystemPiece.m_primaryRightLaneNodes.Count; lNIndex++)
			{
				TrafficSystemNode lastNode = TrafficSystemPiece.m_primaryRightLaneNodes[lNIndex];
				if(lastNode)
				{
					for(int cNIndex = 0; cNIndex < lastNode.m_connectedNodes.Count; cNIndex++)
					{
						TrafficSystemNode currentNode = lastNode.m_connectedNodes[cNIndex];
						if(currentNode)
						{
							string driverSide = "Pink";
							if(currentNode.m_driveSide == TrafficSystem.DriveSide.LEFT)
								driverSide = "Blue";

							GUILayout.BeginVertical("box");
							GUILayout.Label((lNIndex + 1) + " " + driverSide + " : " + currentNode.Parent); 
							GUILayout.Space(1.0f);
							GUILayout.BeginHorizontal("box");
							if(GUILayout.Button("Reveal ->"))
							{
								TrafficSystem.ClearCLRevealObjsFrom();
								TrafficSystem.ClearCLRevealObjsTo();
								TrafficSystem.AddToCLRevealObjsFrom(lastNode.transform);
								TrafficSystem.AddToCLRevealObjsTo(currentNode.transform);
							}
							if(GUILayout.Button("Remove"))
							{
								lastNode.m_connectedNodes.Remove(currentNode);
								EditorUtility.SetDirty(lastNode);
								TrafficSystem.ClearCLRevealObjsFrom();
								TrafficSystem.ClearCLRevealObjsTo();
							}
							if(GUILayout.Button("Switch Link"))
							{
								bool nodeFound = false;
								for(int cIndex = 0; cIndex < currentNode.m_connectedNodes.Count; cIndex++)
								{
									TrafficSystemNode nodeToSwitch = currentNode.m_connectedNodes[cIndex];
									if(nodeToSwitch == lastNode)
									{
										nodeFound = true;
										break;
									}
								}
								
								if(!nodeFound)
								{
									if(lastNode)
										currentNode.m_connectedNodes.Add(lastNode);
									if(currentNode)
										lastNode.m_connectedNodes.Remove(currentNode);

									EditorUtility.SetDirty(currentNode);
									EditorUtility.SetDirty(lastNode);
									TrafficSystem.ClearCLRevealObjsFrom();
									TrafficSystem.ClearCLRevealObjsTo();
								}
							}
							GUILayout.EndHorizontal();
							connections++;
							GUILayout.EndVertical();
						}
					}

					for(int cIndex = 0; cIndex < lastNode.m_connectedChangeLaneNodes.Count; cIndex++)
					{
						if(lastNode.m_connectedChangeLaneNodes[cIndex] && lastNode.m_connectedChangeLaneNodes[cIndex].Parent != TrafficSystemPiece)
						{
							string driverSide = "Pink";
							if(lastNode.m_connectedChangeLaneNodes[cIndex].m_driveSide == TrafficSystem.DriveSide.LEFT)
								driverSide = "Blue";

							GUILayout.BeginVertical("box");
							GUILayout.Label( "ID: " + lastNode.m_connectedChangeLaneNodes[cIndex].m_changeLaneID + " " + driverSide + " CL Inbound  : " + lastNode.m_connectedChangeLaneNodes[cIndex].Parent); 
							GUILayout.Space(1.0f);
							GUILayout.BeginHorizontal("box");
							if(GUILayout.Button("Reveal ->"))
							{
								TrafficSystem.ClearCLRevealObjsFrom();
								TrafficSystem.ClearCLRevealObjsTo();
								TrafficSystem.AddToCLRevealObjsFrom(lastNode.transform);
								TrafficSystem.AddToCLRevealObjsTo(lastNode.m_connectedChangeLaneNodes[cIndex].transform);
							}
							if(GUILayout.Button("Remove"))
							{
								lastNode.RemoveChangeLangeNode(lastNode.m_connectedChangeLaneNodes[cIndex]);
								EditorUtility.SetDirty(lastNode);
								TrafficSystem.ClearCLRevealObjsFrom();
								TrafficSystem.ClearCLRevealObjsTo();
							}
							GUILayout.EndHorizontal();
							GUILayout.EndVertical();
						}
					}
				}
			}

			if(connections <= 0)
				GUILayout.Label("No Connections ...");

			GUILayout.Space(10.0f);
			GUILayout.Label("Change Lane Nodes:"); 

			connections = 0;
			for(int lNIndex = 0; lNIndex < TrafficSystemPiece.m_leftLaneChangeNodes.Count; lNIndex++)
			{
				TrafficSystemNode lastNode = TrafficSystemPiece.m_leftLaneChangeNodes[lNIndex];
				if(lastNode)
				{
					for(int cNIndex = 0; cNIndex < lastNode.m_connectedChangeLaneNodes.Count; cNIndex++)
					{
						TrafficSystemNode currentNode = lastNode.m_connectedChangeLaneNodes[cNIndex];
						if(currentNode)
						{
							string driverSide = "Pink";
							if(currentNode.m_driveSide == TrafficSystem.DriveSide.LEFT)
								driverSide = "Blue";
							
							GUILayout.BeginVertical("box");
							GUILayout.Label((lNIndex + 1) + " " + driverSide + " CL Outbound   : " + currentNode.Parent); 
							GUILayout.Space(1.0f);
							GUILayout.BeginHorizontal("box");
							if(GUILayout.Button("Reveal ->"))
							{
								TrafficSystem.ClearCLRevealObjsFrom();
								TrafficSystem.ClearCLRevealObjsTo();
								TrafficSystem.AddToCLRevealObjsFrom(lastNode.transform);
								TrafficSystem.AddToCLRevealObjsTo(currentNode.transform);
							}
							if(GUILayout.Button("Remove"))
							{
								lastNode.m_connectedChangeLaneNodes.Remove(currentNode);
								EditorUtility.SetDirty(lastNode);
								TrafficSystem.ClearCLRevealObjsFrom();
								TrafficSystem.ClearCLRevealObjsTo();
							}
							GUILayout.EndHorizontal();
							connections++;
						}
					}

					/*
					for(int cIndex = 0; cIndex < lastNode.m_connectedChangeLaneNodes.Count; cIndex++)
					{
						if(lastNode.m_connectedChangeLaneNodes[cIndex])
						{
							GUILayout.BeginVertical("box");
							GUILayout.Label( "ID: " + lastNode.m_connectedChangeLaneNodes[cIndex].m_changeLaneID + " Blue CL Local Outbound  : " + lastNode.m_connectedChangeLaneNodes[cIndex].Parent); 
							GUILayout.Space(1.0f);
							GUILayout.BeginHorizontal("box");
							if(GUILayout.Button("Reveal ->"))
							{
								TrafficSystem.ClearCLRevealObjsFrom();
								TrafficSystem.ClearCLRevealObjsTo();
								TrafficSystem.AddToCLRevealObjsFrom(lastNode.transform);
								TrafficSystem.AddToCLRevealObjsTo(lastNode.m_connectedChangeLaneNodes[cIndex].transform);
							}
							if(GUILayout.Button("Remove"))
							{
								lastNode.RemoveChangeLangeNode(lastNode.m_connectedChangeLaneNodes[cIndex]);
								EditorUtility.SetDirty(lastNode);
							}
							GUILayout.EndHorizontal();
							GUILayout.EndVertical();
							connections++;
						}
					}
					*/
				}
			}

			for(int lNIndex = 0; lNIndex < TrafficSystemPiece.m_rightLaneChangeNodes.Count; lNIndex++)
			{
				TrafficSystemNode lastNode = TrafficSystemPiece.m_rightLaneChangeNodes[lNIndex];
				if(lastNode)
				{
					for(int cNIndex = 0; cNIndex < lastNode.m_connectedChangeLaneNodes.Count; cNIndex++)
					{
						TrafficSystemNode currentNode = lastNode.m_connectedChangeLaneNodes[cNIndex];
						if(currentNode)
						{
							string driverSide = "Pink";
							if(currentNode.m_driveSide == TrafficSystem.DriveSide.LEFT)
								driverSide = "Blue";
							
							GUILayout.BeginVertical("box");
							GUILayout.Label((lNIndex + 1) + " " + driverSide + " CL Outbound  : " + currentNode.Parent); 
							GUILayout.Space(1.0f);
							GUILayout.BeginHorizontal("box");
							if(GUILayout.Button("Reveal ->"))
							{
								TrafficSystem.ClearCLRevealObjsFrom();
								TrafficSystem.ClearCLRevealObjsTo();
								TrafficSystem.AddToCLRevealObjsFrom(lastNode.transform);
								TrafficSystem.AddToCLRevealObjsTo(currentNode.transform);
							}
							if(GUILayout.Button("Remove"))
							{
								lastNode.m_connectedChangeLaneNodes.Remove(currentNode);
								EditorUtility.SetDirty(lastNode);
								TrafficSystem.ClearCLRevealObjsFrom();
								TrafficSystem.ClearCLRevealObjsTo();
							}
							GUILayout.EndHorizontal();
							GUILayout.EndVertical();
							connections++;
						}
					}

					/*
					for(int cIndex = 0; cIndex < lastNode.m_connectedChangeLaneNodes.Count; cIndex++)
					{
						if(lastNode.m_connectedChangeLaneNodes[cIndex])
						{
							GUILayout.BeginVertical("box");
							GUILayout.Label( "ID: " + lastNode.m_connectedChangeLaneNodes[cIndex].m_changeLaneID + " Pink CL Local Outbound  : " + lastNode.m_connectedChangeLaneNodes[cIndex].Parent); 
							GUILayout.Space(1.0f);
							GUILayout.BeginHorizontal("box");
							if(GUILayout.Button("Reveal ->"))
							{
								TrafficSystem.ClearCLRevealObjsFrom();
								TrafficSystem.ClearCLRevealObjsTo();
								TrafficSystem.AddToCLRevealObjsFrom(lastNode.transform);
								TrafficSystem.AddToCLRevealObjsTo(lastNode.m_connectedChangeLaneNodes[cIndex].transform);
							}
							if(GUILayout.Button("Remove"))
							{
								lastNode.RemoveChangeLangeNode(lastNode.m_connectedChangeLaneNodes[cIndex]);
								EditorUtility.SetDirty(lastNode);
							}
							GUILayout.EndHorizontal();
							GUILayout.EndVertical();
							connections++;
						}
					}
					*/
				}
			}

			if(connections <= 0)
				GUILayout.Label("No Connections ...");

//			GUILayout.Space(10.0f);
//
//			GUILayout.Label("Left Lanes:"); 
//			
//			for(int lIndex = 0; lIndex < TrafficSystemPiece.m_leftLanes.Count; lIndex++)
//			{
//				TrafficSystemLane lane = TrafficSystemPiece.m_leftLanes[lIndex];
//				for(int pIndex = 0; pIndex < lane.m_path.Count; pIndex++)
//				{
//					Vector3 pos = lane.m_path[pIndex];
//					GUILayout.Label((lIndex + 1) + " Lane " + lIndex + ", Pos " + pIndex + ": " + pos); 
//				}
//			}
//			
//			GUILayout.Space(10.0f);
//
//			GUILayout.Label("Right Lanes:"); 
//			
//			for(int lIndex = 0; lIndex < TrafficSystemPiece.m_rightLanes.Count; lIndex++)
//			{
//				TrafficSystemLane lane = TrafficSystemPiece.m_rightLanes[lIndex];
//				for(int pIndex = 0; pIndex < lane.m_path.Count; pIndex++)
//				{
//					Vector3 pos = lane.m_path[pIndex];
//					GUILayout.Label((lIndex + 1) + " Lane " + lIndex + ", Pos " + pIndex + ": " + pos); 
//				}
//			}
			
		}

		GUILayout.Space(20.0f);

		GUILayout.BeginHorizontal("box");

		if(TrafficSystem && TrafficSystem.TextureIconEdit)
		{
			if(GUILayout.Button(TrafficSystem.TextureIconEdit))
			{
				if (TrafficSystem && TrafficSystem.Instance)
				{
					TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.EDIT, TrafficSystemPiece );
					TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.EDIT, true );	
					Selection.activeObject = TrafficSystem.Instance;
				}
			}
		}
		else
		{
			if(GUILayout.Button("Select as Edit"))
			{
				if (TrafficSystem && TrafficSystem.Instance)
				{
					TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.EDIT, TrafficSystemPiece );
					TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.EDIT, true );	
					Selection.activeObject = TrafficSystem.Instance;
				}
			}
		}

		if(TrafficSystem && TrafficSystem.TextureIconAnchor)
		{
			if(GUILayout.Button(TrafficSystem.TextureIconAnchor))
			{
				if (TrafficSystem && TrafficSystem.Instance)
				{
					TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.ANCHOR, TrafficSystemPiece );
					TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.ANCHOR, true );
					Selection.activeObject = TrafficSystem.Instance;
				}
			}
		}
		else
		{
			if(GUILayout.Button("Select as Anchor"))
			{
				if (TrafficSystem && TrafficSystem.Instance)
				{
					TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.ANCHOR, TrafficSystemPiece );
					TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.ANCHOR, true );	
					Selection.activeObject = TrafficSystem.Instance;
				}
			}
		}
		GUILayout.EndHorizontal();

		if(TrafficSystemPiece.transform.childCount > 0 && TrafficSystem)
		{
			GUILayout.BeginHorizontal("box");
			for(int cIndex = 0; cIndex < TrafficSystemPiece.transform.childCount; cIndex++)
			{
				if(TrafficSystemPiece.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>() && !TrafficSystemPiece.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>().m_ignoreTrafficSystemEditor)
				{
					if(TrafficSystem && TrafficSystem.TextureIconEditRevealSmall)
					{
						if(GUILayout.Button(TrafficSystem.TextureIconEditRevealSmall))
						{
							if (TrafficSystem.Instance)
							{
								TrafficSystem.Instance.PositionTooltip(TrafficSystem.TrafficSystemTooltip.EDIT, TrafficSystemPiece.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>() );
								TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.EDIT, true );			
							}
						}
					}
					else
					{
						if(GUILayout.Button("Reveal E" + cIndex))
						{
							if (TrafficSystem && TrafficSystem.Instance)
							{
								//TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.EDIT, TrafficSystemPiece );
								TrafficSystem.Instance.PositionTooltip(TrafficSystem.TrafficSystemTooltip.EDIT, TrafficSystemPiece.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>() );
								TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.EDIT, true );			
							}
						}
					}

					if(TrafficSystem && TrafficSystem.TextureIconAnchorRevealSmall)
					{
						if(GUILayout.Button(TrafficSystem.TextureIconAnchorRevealSmall))
						{
							if (TrafficSystem.Instance)
							{
								TrafficSystem.Instance.PositionTooltip(TrafficSystem.TrafficSystemTooltip.ANCHOR, TrafficSystemPiece.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>() );
								TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.ANCHOR, true );			
							}
						}
					}
					else
					{
						if(GUILayout.Button("Reveal A" + cIndex))
						{
							if (TrafficSystem && TrafficSystem.Instance)
							{
								//TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.ANCHOR, TrafficSystemPiece );
								TrafficSystem.Instance.PositionTooltip(TrafficSystem.TrafficSystemTooltip.ANCHOR, TrafficSystemPiece.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>() );
								TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.ANCHOR, true );			
							}
						}
					}
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("box");
			for(int cIndex = 0; cIndex < TrafficSystemPiece.transform.childCount; cIndex++)
			{
				if(TrafficSystemPiece.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>() && !TrafficSystemPiece.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>().m_ignoreTrafficSystemEditor)
				{
					if(TrafficSystem && TrafficSystem.TextureIconEditSmall)
					{
						if(GUILayout.Button(TrafficSystem.TextureIconEditSmall))
						{
							if (TrafficSystem.Instance)
							{
								TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.EDIT, TrafficSystemPiece.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>() );
								TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.EDIT, true );			
								Selection.activeObject = TrafficSystem.Instance;
							}
						}
					}
					else
					{
						if(GUILayout.Button("Select E" + cIndex))
						{
							if (TrafficSystem && TrafficSystem.Instance)
							{
								TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.EDIT, TrafficSystemPiece.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>() );
								TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.EDIT, true );			
								Selection.activeObject = TrafficSystem.Instance;
							}
						}
					}

					if(TrafficSystem && TrafficSystem.TextureIconAnchorSmall)
					{
						if(GUILayout.Button(TrafficSystem.TextureIconAnchorSmall))
						{
							if (TrafficSystem.Instance)
							{
								TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.ANCHOR, TrafficSystemPiece.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>() );
								TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.ANCHOR, true );
								Selection.activeObject = TrafficSystem.Instance;
							}
						}
					}
					else
					{
						if(GUILayout.Button("Select A" + cIndex))
						{
							if (TrafficSystem && TrafficSystem.Instance)
							{
								TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.ANCHOR, TrafficSystemPiece.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>() );
								TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.ANCHOR, true );
								Selection.activeObject = TrafficSystem.Instance;
							}
						}
					}
				}
			}
			GUILayout.EndHorizontal();
		}

		//		Selection.activeTransform = GameObject.Find ("GameObject").transform;

		GUILayout.BeginHorizontal("box");
		if(GUILayout.Button("Go to Traffic System"))
		{
			if (TrafficSystem && TrafficSystem.Instance)
				Selection.activeObject = TrafficSystem.Instance;
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal("box");
		if(GUILayout.Button("Rot Y (" + m_rotateDegrees + ")"))
		{
			TrafficSystemPiece.transform.Rotate(Vector3.up * m_rotateDegrees);
		}
		
		if(GUILayout.Button("Rot Z (" + (m_rotateDegrees * 2) + ")"))
		{
			TrafficSystemPiece.transform.Rotate(Vector3.forward * (m_rotateDegrees * 2));
		}
		
		if(GUILayout.Button("Flip (X)"))
		{
			Vector3 scale = TrafficSystemPiece.transform.localScale;
			scale.x = -scale.x;
			TrafficSystemPiece.transform.localScale = scale;
		}
		
		if(GUILayout.Button("Flip (Z)"))
		{
			Vector3 scale = TrafficSystemPiece.transform.localScale;
			scale.z = -scale.z;
			TrafficSystemPiece.transform.localScale = scale;
		}

		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal("box");

		if(GUILayout.Button("UnLink All"))
		{
			for(int lNIndex = 0; lNIndex < TrafficSystemPiece.m_primaryLeftLaneNodes.Count; lNIndex++)
			{
				TrafficSystemNode lastNode = TrafficSystemPiece.m_primaryLeftLaneNodes[lNIndex];
				lastNode.m_connectedNodes.Clear();
				EditorUtility.SetDirty(lastNode);
			}
			
			for(int lNIndex = 0; lNIndex < TrafficSystemPiece.m_secondaryLeftLaneNodes.Count; lNIndex++)
			{
				TrafficSystemNode lastNode = TrafficSystemPiece.m_secondaryLeftLaneNodes[lNIndex];
				lastNode.m_connectedNodes.Clear();
				EditorUtility.SetDirty(lastNode);
			}
			
			for(int lNIndex = 0; lNIndex < TrafficSystemPiece.m_primaryRightLaneNodes.Count; lNIndex++)
			{
				TrafficSystemNode lastNode = TrafficSystemPiece.m_primaryRightLaneNodes[lNIndex];
				lastNode.m_connectedNodes.Clear();
				EditorUtility.SetDirty(lastNode);
			}

			for(int lNIndex = 0; lNIndex < TrafficSystemPiece.m_secondaryRightLaneNodes.Count; lNIndex++)
			{
				TrafficSystemNode lastNode = TrafficSystemPiece.m_secondaryRightLaneNodes[lNIndex];
				lastNode.m_connectedNodes.Clear();
				EditorUtility.SetDirty(lastNode);
			}
			
			RefreshEditor( false, true );
		}

		if(GUILayout.Button("UnLink Left (Blue)"))
		{
			for(int lNIndex = 0; lNIndex < TrafficSystemPiece.m_primaryLeftLaneNodes.Count; lNIndex++)
			{
				TrafficSystemNode lastNode = TrafficSystemPiece.m_primaryLeftLaneNodes[lNIndex];
				lastNode.m_connectedNodes.Clear();
				EditorUtility.SetDirty(lastNode);
			}
			
			for(int lNIndex = 0; lNIndex < TrafficSystemPiece.m_secondaryLeftLaneNodes.Count; lNIndex++)
			{
				TrafficSystemNode lastNode = TrafficSystemPiece.m_secondaryLeftLaneNodes[lNIndex];
				lastNode.m_connectedNodes.Clear();
				EditorUtility.SetDirty(lastNode);
			}
			
			RefreshEditor( false, true );
		}
		
		if(GUILayout.Button("UnLink Right (Pink)"))
		{
			for(int lNIndex = 0; lNIndex < TrafficSystemPiece.m_primaryRightLaneNodes.Count; lNIndex++)
			{
				TrafficSystemNode lastNode = TrafficSystemPiece.m_primaryRightLaneNodes[lNIndex];
				lastNode.m_connectedNodes.Clear();
				EditorUtility.SetDirty(lastNode);
			}
			
			for(int lNIndex = 0; lNIndex < TrafficSystemPiece.m_secondaryRightLaneNodes.Count; lNIndex++)
			{
				TrafficSystemNode lastNode = TrafficSystemPiece.m_secondaryRightLaneNodes[lNIndex];
				lastNode.m_connectedNodes.Clear();
				EditorUtility.SetDirty(lastNode);
			}
			
			RefreshEditor( false, true );
		}
		
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal("box");

		if(GUILayout.Button("Link Local Primary (B to P)"))
		{
			CreateConnections( TrafficSystemPiece.m_primaryRightLaneNodes, TrafficSystemPiece.m_primaryLeftLaneNodes );
//			CreateConnections( TrafficSystemPiece.m_secondaryRightLaneNodes, TrafficSystemPiece.m_primaryLeftLaneNodes );
		}
		if(GUILayout.Button("Link Local Primary (P to B)"))
		{
			CreateConnections( TrafficSystemPiece.m_primaryLeftLaneNodes, TrafficSystemPiece.m_primaryRightLaneNodes );
//			CreateConnections( TrafficSystemPiece.m_secondaryLeftLaneNodes, TrafficSystemPiece.m_primaryRightLaneNodes );
		}
		GUILayout.EndHorizontal();

		for(int cIndex = 0; cIndex < TrafficSystemPiece.m_primaryLeftLaneNodes.Count; cIndex++)
		{
			TrafficSystemNode node = TrafficSystemPiece.m_primaryLeftLaneNodes[cIndex];
			
			if(node)
			{
				GUILayout.BeginHorizontal("box");
				GUILayout.Label("PR Blue - Lane " + node.m_lane + ": ");
				if(GUILayout.Button("Switch Driver Side"))
				{
					if(node.m_driveSide == TrafficSystem.DriveSide.LEFT)
					{
						ChangeNodeMaterial( node, TrafficSystem.DriveSide.RIGHT );
					}
					else
					{
						ChangeNodeMaterial( node, TrafficSystem.DriveSide.LEFT );
					}
					
					EditorUtility.SetDirty(node);
					RefreshEditor();
				}
				GUILayout.EndHorizontal();
			}
		}

		for(int cIndex = 0; cIndex < TrafficSystemPiece.m_primaryRightLaneNodes.Count; cIndex++)
		{
			TrafficSystemNode node = TrafficSystemPiece.m_primaryRightLaneNodes[cIndex];
			
			if(node)
			{
				GUILayout.BeginHorizontal("box");
				GUILayout.Label("PR Pink - Lane " + node.m_lane + ": ");
				if(GUILayout.Button("Switch Driver Side"))
				{
					if(node.m_driveSide == TrafficSystem.DriveSide.LEFT)
					{
						ChangeNodeMaterial( node, TrafficSystem.DriveSide.RIGHT );
					}
					else
					{
						ChangeNodeMaterial( node, TrafficSystem.DriveSide.LEFT );
					}
					
					EditorUtility.SetDirty(node);
					RefreshEditor();
				}
				GUILayout.EndHorizontal();
			}
		}
		
		for(int cIndex = 0; cIndex < TrafficSystemPiece.m_leftLaneChangeNodes.Count; cIndex++)
		{
			TrafficSystemNode node = TrafficSystemPiece.m_leftLaneChangeNodes[cIndex];
			
			if(node && node.m_connectedLocalNode)
			{
				GUILayout.BeginHorizontal("box");
				GUILayout.Label("CL Blue " + node.m_changeLaneID + ": ");
				if(GUILayout.Button("Switch Dir"))
				{
					node.m_connectedLocalNode.m_connectedLocalNode = node;
					node.m_connectedLocalNode.m_isInbound = true;
					node.m_isInbound = false;
					EditorUtility.SetDirty(node.m_connectedLocalNode);
					node.m_connectedLocalNode = null;
					//RefreshEditor();
					EditorUtility.SetDirty(node);
				}
				if(GUILayout.Button("Switch Driver Side (" + node.m_driveSide + ")"))
				{
					if(node.m_driveSide == TrafficSystem.DriveSide.LEFT)
					{
						ChangeNodeMaterial( node, TrafficSystem.DriveSide.RIGHT );
						ChangeNodeMaterial( node.m_connectedLocalNode, TrafficSystem.DriveSide.RIGHT );
					}
					else
					{
						ChangeNodeMaterial( node, TrafficSystem.DriveSide.LEFT );
						ChangeNodeMaterial( node.m_connectedLocalNode, TrafficSystem.DriveSide.LEFT );
					}
					
					EditorUtility.SetDirty(node.m_connectedLocalNode);
					EditorUtility.SetDirty(node);
					RefreshEditor();
				}
				GUILayout.EndHorizontal();
			}
		}
		
		for(int cIndex = 0; cIndex < TrafficSystemPiece.m_rightLaneChangeNodes.Count; cIndex++)
		{
			TrafficSystemNode node = TrafficSystemPiece.m_rightLaneChangeNodes[cIndex];
			
			if(node && node.m_connectedLocalNode)
			{
				GUILayout.BeginHorizontal("box");
				GUILayout.Label("CL Pink " + node.m_changeLaneID + ": ");
				if(GUILayout.Button("Switch Dir"))
				{
					node.m_connectedLocalNode.m_connectedLocalNode = node;;
					node.m_connectedLocalNode.m_isInbound = true;
					node.m_isInbound = false;
					EditorUtility.SetDirty(node.m_connectedLocalNode);
					node.m_connectedLocalNode = null;
					//RefreshEditor();
					EditorUtility.SetDirty(node);
				}
				if(GUILayout.Button("Switch Driver Side (" + node.m_driveSide + ")"))
				{
					if(node.m_driveSide == TrafficSystem.DriveSide.LEFT)
					{
						ChangeNodeMaterial( node, TrafficSystem.DriveSide.RIGHT );
						ChangeNodeMaterial( node.m_connectedLocalNode, TrafficSystem.DriveSide.RIGHT );
					}
					else
					{
						ChangeNodeMaterial( node, TrafficSystem.DriveSide.LEFT );
						ChangeNodeMaterial( node.m_connectedLocalNode, TrafficSystem.DriveSide.LEFT );
					}

					EditorUtility.SetDirty(node.m_connectedLocalNode);
					EditorUtility.SetDirty(node);
					RefreshEditor();
				}
				GUILayout.EndHorizontal();
			}
		}


//		GUILayout.BeginHorizontal("box");
//		if(GUILayout.Button("Connect Lane Change (Blue)"))
//		{
//
//		}
//		if(GUILayout.Button("Connect Lane Change (Pink)"))
//		{
//		}
//		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal("box");
		if(GUILayout.Button("Process Children"))
		{
			RefreshEditor();
		}
		else
		{
			//			GUILayout.Label("Traffic Node Count: " + TrafficSystemPiece.m_nodes.Count);
			//
			//			for(int nIndex = 0; nIndex < TrafficSystemPiece.m_nodes.Count; nIndex++)
			//			{
			//				if(TrafficSystemPiece.m_nodes[nIndex])
			//					GUILayout.Label(TrafficSystemPiece.m_nodes[nIndex].name);
			//				else
			//				{
			//					TrafficSystemPiece.FindAllTrafficNodes();
			//					break;
			//				}
			//			}
		}
		GUILayout.EndHorizontal();

		/*
		if((TrafficSystemPiece.m_roadModelsOriginal != null || TrafficSystemPiece.m_roadModelsVersion2 != null || TrafficSystemPiece.m_roadModelsVersion2Mobile != null ))
		{
			GUILayout.BeginHorizontal("box");
			if(TrafficSystemPiece.m_roadModelsOriginal != null)
			{
				if(GUILayout.Button("Original"))
					TrafficSystemPiece.ProcessRoadQuality(TrafficSystem.RoadQuality.ORIGINAL);
			}
			if(TrafficSystemPiece.m_roadModelsVersion2 != null)
			{
				if(GUILayout.Button("V2 HD"))
					TrafficSystemPiece.ProcessRoadQuality(TrafficSystem.RoadQuality.VERSION_2);
			}
			if(TrafficSystemPiece.m_roadModelsVersion2Mobile != null)
			{
				if(GUILayout.Button("V2 Mobile"))
						TrafficSystemPiece.ProcessRoadQuality(TrafficSystem.RoadQuality.VERSION_2_MOBILE);
			}
			if(TrafficSystemPiece.m_footpaths != null && TrafficSystemPiece.m_footpaths.Length > 0)
				if(GUILayout.Button("Footpaths On/Off"))
					TrafficSystemPiece.ProcessFootpaths();
			GUILayout.EndHorizontal();
		}
		*/

		GUILayout.BeginHorizontal("box");
		if(GUILayout.Button("Delete"))
			DestroyImmediate (TrafficSystemPiece.gameObject);
		GUILayout.EndHorizontal();
	}

	public void RefreshEditor( bool a_children = true, bool a_refreshMaterials = true)
	{
		if(a_children)
			TrafficSystemPiece.Refresh();

		if(!a_refreshMaterials)
			return;

		for(int nIndex = 0; nIndex < TrafficSystemPiece.m_primaryLeftLaneNodes.Count; nIndex++)
		{
			Material material = AssetDatabase.LoadAssetAtPath(TrafficSystemEditor.PrimaryNodeLeftSideMaterial, typeof(Material)) as Material;
			if(TrafficSystemPiece.m_primaryLeftLaneNodes[nIndex])
				TrafficSystemPiece.m_primaryLeftLaneNodes[nIndex].GetComponent<Renderer>().material = material;
		}
		
		for(int nIndex = 0; nIndex < TrafficSystemPiece.m_primaryRightLaneNodes.Count; nIndex++)
		{
			Material material = AssetDatabase.LoadAssetAtPath(TrafficSystemEditor.PrimaryNodeRightSideMaterial, typeof(Material)) as Material;
			if(TrafficSystemPiece.m_primaryRightLaneNodes[nIndex])
				TrafficSystemPiece.m_primaryRightLaneNodes[nIndex].GetComponent<Renderer>().material = material;
		}

		for(int nIndex = 0; nIndex < TrafficSystemPiece.m_secondaryLeftLaneNodes.Count; nIndex++)
		{
			Material material = AssetDatabase.LoadAssetAtPath(TrafficSystemEditor.SecondaryNodeLeftSideMaterial, typeof(Material)) as Material;
			if(TrafficSystemPiece.m_secondaryLeftLaneNodes[nIndex])
				TrafficSystemPiece.m_secondaryLeftLaneNodes[nIndex].GetComponent<Renderer>().material = material;
		}
		
		for(int nIndex = 0; nIndex < TrafficSystemPiece.m_secondaryRightLaneNodes.Count; nIndex++)
		{
			Material material = AssetDatabase.LoadAssetAtPath(TrafficSystemEditor.SecondaryNodeRightSideMaterial, typeof(Material)) as Material;
			if(TrafficSystemPiece.m_secondaryRightLaneNodes[nIndex])
				TrafficSystemPiece.m_secondaryRightLaneNodes[nIndex].GetComponent<Renderer>().material = material;
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
			if(TrafficSystem.Instance.TooltipEdit)
				TrafficSystem.Instance.TooltipEdit.transform.Rotate(Vector3.up, 1.0f); 
		}

		for(int cIndex = 0; cIndex < TrafficSystemPiece.m_leftLaneChangeNodes.Count; cIndex++)
		{
			TrafficSystemNode node = TrafficSystemPiece.m_leftLaneChangeNodes[cIndex];

			if(node)
			{
				string str = "Inbound";
				if(!node.m_isInbound)
					str    = "Outbound";
				Handles.Label(node.transform.position + (Vector3.up * 0.5f), "CL " + node.m_changeLaneID.ToString() + " ( Blue / " + node.m_driveSide + " - " + str + " )");
			}
		}

		for(int cIndex = 0; cIndex < TrafficSystemPiece.m_rightLaneChangeNodes.Count; cIndex++)
		{
			TrafficSystemNode node = TrafficSystemPiece.m_rightLaneChangeNodes[cIndex];
			
			if(node)
			{
				string str = "Inbound";
				if(!node.m_isInbound)
					str    = "Outbound";
				Handles.Label(node.transform.position + (Vector3.up * 0.5f), "CL " + node.m_changeLaneID.ToString() + " ( Pink / " + node.m_driveSide + " - " + str + " )");
			}
		}

		Handles.Label(TrafficSystemPiece.transform.position + (Vector3.up * 3.0f), TrafficSystemPiece.name);
	}

	public void CreateConnections( List<TrafficSystemNode> a_nodesLookingToConnect, List<TrafficSystemNode> a_nodesToConnectWith )
	{
		for(int pLIndex = 0; pLIndex < a_nodesLookingToConnect.Count; pLIndex++)
		{
			TrafficSystemNode primaryLocalNode = a_nodesLookingToConnect[pLIndex];

			for(int lNIndex = 0; lNIndex < a_nodesToConnectWith.Count; lNIndex++)
			{
				TrafficSystemNode lastNode = a_nodesToConnectWith[lNIndex];
				bool nodeFound = false;
				for(int cNIndex = 0; cNIndex < lastNode.m_connectedNodes.Count; cNIndex++)
				{
					TrafficSystemNode connectedNode = lastNode.m_connectedNodes[cNIndex];
					
					if(connectedNode == primaryLocalNode)
					{
						nodeFound = true;
						break;
					}
					
					if(!nodeFound)
					{
						if(primaryLocalNode.m_onlyConnectWithSameLane || connectedNode.m_onlyConnectWithSameLane)
						{
							if(lastNode.m_lane == primaryLocalNode.m_lane)
							{
								if(primaryLocalNode)
									lastNode.m_connectedNodes.Add(primaryLocalNode);
								EditorUtility.SetDirty(lastNode);
								break;
							}
						}
						else
						{
							if(primaryLocalNode)
								lastNode.m_connectedNodes.Add(primaryLocalNode);
							EditorUtility.SetDirty(lastNode);
							break;
						}
					}
				}
			}
		}
	}

	void ChangeNodeMaterial( TrafficSystemNode a_node, TrafficSystem.DriveSide a_side )
	{
		a_node.m_driveSide = a_side; 

		if(a_node.m_driveSide == TrafficSystem.DriveSide.LEFT)
		{
			if(a_node.m_isPrimary)
			{
				Material material = AssetDatabase.LoadAssetAtPath(TrafficSystemEditor.PrimaryNodeLeftSideMaterial, typeof(Material)) as Material;
				a_node.GetComponent<Renderer>().material = material;
			}
			else
			{
				Material material = AssetDatabase.LoadAssetAtPath(TrafficSystemEditor.SecondaryNodeLeftSideMaterial, typeof(Material)) as Material;
				a_node.GetComponent<Renderer>().material = material;
			}
		}
		else
		{
			if(a_node.m_isPrimary)
			{
				Material material = AssetDatabase.LoadAssetAtPath(TrafficSystemEditor.PrimaryNodeRightSideMaterial, typeof(Material)) as Material;
				a_node.GetComponent<Renderer>().material = material;
			}
			else
			{
				Material material = AssetDatabase.LoadAssetAtPath(TrafficSystemEditor.SecondaryNodeRightSideMaterial, typeof(Material)) as Material;
				a_node.GetComponent<Renderer>().material = material;
			}			
		}
	}
}
