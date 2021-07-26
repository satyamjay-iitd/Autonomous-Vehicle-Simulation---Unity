using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(TrafficSystem))]
public class TrafficSystemEditor : Editor	
{
	// WARNING: If you change anything in this class you will need to press "Play" to update the Traffic System prefab Inspector.

	public  string   m_dirRoadPieces                           =    "Assets/Traffic System/Prefabs/Road Pieces/";
	private string[] m_trafficSystemRoadPiecesLocation;/*         = {  "Assets/Traffic System/Prefabs/Road Pieces/L2_Straight.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L2_Curved_Left_Outter.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L2_Curved_Right_Outter.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L4_Straight_Entry.prefab",
																    "Assets/Traffic System/Prefabs/Road Pieces/L4_Straight.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L4_Straight_Exit.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L6_Straight_Entry.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L6_Straight.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L6_Straight_Exit.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L1_Straight_Left.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L1_Curved_Left.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L1_Offramp_Left.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L1_Straight_Left_Half.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L1_Straight_Right.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L1_Curved_Right.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L1_Offramp_Right.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L1_Straight_Right_Half.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L2_Straight_Half.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L2_Intersection.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L2_T_Intersection_Left_Turn_Left_Change.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L2_T_Intersection_Left_Turn_Right_Change.prefab",
																	"Assets/Traffic System/Prefabs/Road Pieces/L2_T_Intersection_Right_Turn_Left_Change.prefab", 
																	"Assets/Traffic System/Prefabs/Road Pieces/L2_T_Intersection_Right_Turn_Right_Change.prefab", 
																	"Assets/Traffic System/Prefabs/Road Pieces/L0.prefab" };*/
	/*private string[] m_trafficSystemRoadPiecesNames            = {  "Lanes 2 - Straight",
																    "Lanes 2 - Curved - Left - Outter",
																    "Lanes 2 - Curved - Right - Outter",
																    "Lanes 4 - Straight - Entry",
																    "Lanes 4 - Straight",
																	"Lanes 4 - Straight - Exit",
																	"Lanes 6 - Straight - Entry",
																	"Lanes 6 - Straight",
																	"Lanes 6 - Straight - Exit",
																	"Lanes 1 - Straight - Left",
																	"Lanes 1 - Curved - Left",
																	"Lanes 1 - Offramp - Left",
																	"Lanes 1 - Straight - Left - Half",
																	"Lanes 1 - Straight - Right",
																	"Lanes 1 - Curved - Right",
																	"Lanes 1 - Offramp - Right",
																	"Lanes 1 - Straight - Right - Half",
																	"Lanes 2 - Straight Half",																	
																	"Lanes 2 - Intersection",
																	"Lanes 2 - T Intersection - Left - Turn - Left - Change",
																	"Lanes 2 - T Intersection - Left - Turn - Right - Change",
																	"Lanes 2 - T Intersection - Right - Turn - Left - Change",
	                                                                "Lanes 2 - T Intersection - Right - Turn - Right - Change",
	                                                                "Lanes 0"}; */
	private List<Texture2D> m_trafficSystemRoadPiecesTextures  = new List<Texture2D>();

	//	string myString = "Hello World";
//	bool groupEnabled;
//	bool myBool = true;
//	float myFloat = 1.23f;

	public enum RoadAttachmentPoint
	{
		NORTH     = 0,
		EAST      = 1,
		SOUTH     = 2,
		WEST      = 3,
		MAX       = 4
	}

	public enum RoadType
	{
		LANES_0            = 0,
		LANES_1            = 1,
		LANES_2            = 2,
		LANES_4            = 3,
		LANES_6            = 4,
		ALL                = 5
	}


	private TrafficSystem TrafficSystem                     { get; set; }

	private string[] m_roadQualityElements                    = new string[] { "", "V2 Mobile", "V2 HD", "Original" };

	public  float         m_roadPieceSize                     = 8;
	public  RoadAttachmentPoint m_roadAttachmentPoints        = RoadAttachmentPoint.SOUTH;
	private RoadAttachmentPoint m_roadAttachmentPointIndex    = RoadAttachmentPoint.SOUTH;
	private float         m_rotateDegrees                     = 90.0f;

	static public  TrafficSystemPiece  AnchorTrafficSystemPiece  { get; set; }
	static public  TrafficSystemPiece  EditTrafficSystemPiece    { get; set; }
	static public  int                 m_trafficSystemPieceCount = 0;

	static public string  PrimaryNodeLeftSideMaterial         = "Assets/Traffic System/Materials/Traffic System Primary Node Left.mat";
	static public string  PrimaryNodeRightSideMaterial        = "Assets/Traffic System/Materials/Traffic System Primary Node Right.mat";
	static public string  SecondaryNodeLeftSideMaterial       = "Assets/Traffic System/Materials/Traffic System Secondary Node Left.mat";
	static public string  SecondaryNodeRightSideMaterial      = "Assets/Traffic System/Materials/Traffic System Secondary Node Right.mat";

	static public string  TooltipAnchorLocation               = "Assets/Traffic System/Prefabs/Resources/ToolTip-Anchor.prefab";
	static public string  TooltipEditLocation                 = "Assets/Traffic System/Prefabs/Resources/ToolTip-Edit.prefab";

	private bool          FirstNode                           { get; set; }

	private Transform     EditNodeFolder                      { get; set; }
	private string        m_editNodeFolderName                = "Edit Folder";
	private Transform     AnchorNodeFolder                    { get; set; }
	private string        m_anchorNodeFolderName              = "Anchor Folder";
	private Transform     ResourceNodeFolder                  { get; set; }
	private string        m_resourceNodeFolderName            = "Resource Folder";

	private bool          m_init                              = true;
	private int           m_initCount                         = 0;
	private int           m_initCountMax                      = 5;

	private RoadType      m_roadTypeSelected                  = RoadType.ALL;

//	[MenuItem ("Traffic System/Editor")]	
//	public static void  ShowEditor () 
//	{
//		EditorWindow.GetWindow(typeof(TrafficSystemEditor));
//	}

	void Awake()
	{
		TrafficSystem = (TrafficSystem)target;

		if(!TrafficSystem)
			Debug.LogError("Traffic System Error -> \"Traffic System\" script must be in the scene. It is missing. Drop in the \"Traffic System\" Prefab to fix this.");

		string sDataPath  = Application.dataPath + "/Traffic System/Prefabs/Road Pieces/";

		// get the system file paths of all the files in the asset folder
		string[] aFilePaths = Directory.GetFiles(sDataPath);
		
		// enumerate through the list of files loading the assets they represent and getting their type
		
		int count = 0;
		foreach (string sFilePath in aFilePaths) 
		{
			if(sFilePath.Length <= 6)
				continue;

			string sAssetPath = sFilePath.Substring(sFilePath.Length - 6, 6);

			if(sAssetPath == "prefab")
				count++;
		}

		m_trafficSystemRoadPiecesLocation = new string[count];

		count = 0;
		foreach (string sFilePath in aFilePaths) 
		{
			if(sFilePath.Length <= 6)
				continue;
			
			string sAssetPath = sFilePath.Substring(sFilePath.Length - 6, 6);
			
			if(sAssetPath == "prefab")
			{
				string file = sFilePath.Substring(sDataPath.Length);
				m_trafficSystemRoadPiecesLocation[count] = m_dirRoadPieces + file;
				//Debug.Log(m_trafficSystemRoadPiecesLocation[count]);
				count++;
			}
		}

		if(TrafficSystem && Selection.activeGameObject == TrafficSystem.gameObject)
		{
			TrafficSystem.ClearCLRevealObjsFrom();
			TrafficSystem.ClearCLRevealObjsTo();
		}

		if(TrafficSystem)
		{
			TrafficSystem.TextureIconAnchor = AssetDatabase.LoadAssetAtPath("Assets/Traffic System/Textures/icon_anchor.png", typeof(Texture2D)) as Texture2D;
			TrafficSystem.TextureIconEdit   = AssetDatabase.LoadAssetAtPath("Assets/Traffic System/Textures/icon_edit.png", typeof(Texture2D)) as Texture2D;
		}

//		m_trafficSystemRoadPiecesTextures.Clear();
//		for(int aIndex = 0; aIndex < m_trafficSystemRoadPiecesNames.Length; aIndex++)
//		{
//			Texture2D image = AssetDatabase.LoadAssetAtPath("Assets/Traffic System/Textures/" + m_trafficSystemRoadPiecesNames[aIndex] + ".png", typeof(Texture2D)) as Texture2D;
//			m_trafficSystemRoadPiecesTextures.Add( image );
//		}

		ProcessFolderChecks( true );
		//CheckForAnchorNode();
	}

	//	void OnEnable()
//	{
//		TrafficSystem = GameObject.FindObjectOfType(typeof(TrafficSystem)) as TrafficSystem;
//
//		if(!TrafficSystem)
//			Debug.LogError("Traffic System Error -> \"Traffic System\" script must be in the scene. It is missing. Drop in the \"Traffic System\" Prefab to fix this.");
//	}


	public void ChangeChildRoadQuality( TrafficSystemPiece a_obj )
	{
		if(!a_obj)
			return;

		a_obj.GetComponent<TrafficSystemPiece>().ProcessRoadQuality( TrafficSystem.Instance.GetRoadQualityVal() );

		for(int cIndex = 0; cIndex < a_obj.transform.childCount; cIndex++)
		{
			if(a_obj.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>())
				ChangeChildRoadQuality(a_obj.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>());
		}
	}

	public void ChangeRoadQuality() 
	{
		switch (TrafficSystem.Instance.RoadQualityElementsIndex) 
		{
		case 0:
			TrafficSystem.Instance.SetRoadQualityVal( TrafficSystem.RoadQuality.VERSION_2_MOBILE );
			for(int aIndex = 0; aIndex < m_trafficSystemRoadPiecesLocation.Length; aIndex++)
			{
				string file = m_trafficSystemRoadPiecesLocation[aIndex].Substring(m_dirRoadPieces.Length, m_trafficSystemRoadPiecesLocation[aIndex].Length - m_dirRoadPieces.Length - 7);
				TrafficSystemPiece roadPiece = AssetDatabase.LoadAssetAtPath(m_trafficSystemRoadPiecesLocation[aIndex], typeof(TrafficSystemPiece)) as TrafficSystemPiece;
				
				if(roadPiece)
				{
					roadPiece.ProcessRoadQuality( TrafficSystem.Instance.GetRoadQualityVal() );
					ChangeChildRoadQuality( roadPiece );
				}
			}
			break;
		case 1:
			TrafficSystem.Instance.SetRoadQualityVal( TrafficSystem.RoadQuality.VERSION_2_MOBILE );
			for(int aIndex = 0; aIndex < m_trafficSystemRoadPiecesLocation.Length; aIndex++)
			{
				string file = m_trafficSystemRoadPiecesLocation[aIndex].Substring(m_dirRoadPieces.Length, m_trafficSystemRoadPiecesLocation[aIndex].Length - m_dirRoadPieces.Length - 7);
				TrafficSystemPiece roadPiece = AssetDatabase.LoadAssetAtPath(m_trafficSystemRoadPiecesLocation[aIndex], typeof(TrafficSystemPiece)) as TrafficSystemPiece;
				
				if(roadPiece)
				{
					roadPiece.ProcessRoadQuality( TrafficSystem.Instance.GetRoadQualityVal() );
					ChangeChildRoadQuality( roadPiece );
				}
			}
			break;
		case 2:
			TrafficSystem.Instance.SetRoadQualityVal( TrafficSystem.RoadQuality.VERSION_2 );
			for(int aIndex = 0; aIndex < m_trafficSystemRoadPiecesLocation.Length; aIndex++)
			{
				string file = m_trafficSystemRoadPiecesLocation[aIndex].Substring(m_dirRoadPieces.Length, m_trafficSystemRoadPiecesLocation[aIndex].Length - m_dirRoadPieces.Length - 7);
				TrafficSystemPiece roadPiece = AssetDatabase.LoadAssetAtPath(m_trafficSystemRoadPiecesLocation[aIndex], typeof(TrafficSystemPiece)) as TrafficSystemPiece;
				
				if(roadPiece)
				{
					roadPiece.ProcessRoadQuality( TrafficSystem.Instance.GetRoadQualityVal() );
					ChangeChildRoadQuality( roadPiece );
				}
			}
			break;
		case 3:
			TrafficSystem.Instance.SetRoadQualityVal( TrafficSystem.RoadQuality.ORIGINAL );
			for(int aIndex = 0; aIndex < m_trafficSystemRoadPiecesLocation.Length; aIndex++)
			{
				string file = m_trafficSystemRoadPiecesLocation[aIndex].Substring(m_dirRoadPieces.Length, m_trafficSystemRoadPiecesLocation[aIndex].Length - m_dirRoadPieces.Length - 7);
				TrafficSystemPiece roadPiece = AssetDatabase.LoadAssetAtPath(m_trafficSystemRoadPiecesLocation[aIndex], typeof(TrafficSystemPiece)) as TrafficSystemPiece;
				
				if(roadPiece)
				{
					roadPiece.ProcessRoadQuality( TrafficSystem.Instance.GetRoadQualityVal() );
					ChangeChildRoadQuality( roadPiece );
				}
			}
			break;
		default:
			Debug.LogError("Unrecognized Option");
			break;
		}
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

		if(!TrafficSystem || !TrafficSystem.Instance)
			return;

		GUILayout.BeginHorizontal("box");
		
		TrafficSystem.Instance.RoadQualityElementsIndex = EditorGUILayout.Popup(TrafficSystem.Instance.RoadQualityElementsIndex, m_roadQualityElements);
		
		if(GUILayout.Button("Apply Road Quality"))
		{
			ChangeRoadQuality();
		}
		
		GUILayout.EndHorizontal();
		
		
		GUILayout.Space(10.0f);


		if(!EditTrafficSystemPiece)
		{
			GUILayout.Label ("Road Pieces", EditorStyles.boldLabel);

//			TrafficSystem.Instance.m_autoLinkOnSpawn = GUILayout.Toggle(TrafficSystem.Instance.m_autoLinkOnSpawn, " Auto Link On Spawn");
//			TrafficSystem.Instance.m_quickSpawn      = GUILayout.Toggle(TrafficSystem.Instance.m_quickSpawn, " Quick Spawn");

			if(EditTrafficSystemPiece && GUILayout.Button("Go to 'Edit' Piece"))
				Selection.activeObject = EditTrafficSystemPiece.transform;

			if(AnchorTrafficSystemPiece && GUILayout.Button("Go to 'Anchor' Piece"))
				Selection.activeObject = AnchorTrafficSystemPiece.transform;

			GUILayout.Space(10.0f);

			GUILayout.BeginHorizontal("box");

			if(TrafficSystem.Instance.TextureIconRoadTypeAll)
			{
				if(GUILayout.Button(TrafficSystem.Instance.TextureIconRoadTypeAll))
					m_roadTypeSelected = RoadType.ALL;
			}
			else
			{
				if(GUILayout.Button("All"))
					m_roadTypeSelected = RoadType.ALL;
			}
			if(TrafficSystem.Instance.TextureIconRoadType0Lanes)
			{
				if(GUILayout.Button(TrafficSystem.Instance.TextureIconRoadType0Lanes))
					m_roadTypeSelected = RoadType.LANES_0;
			}
			else
			{
				if(GUILayout.Button("0 Lanes"))
					m_roadTypeSelected = RoadType.LANES_0;
			}
			if(TrafficSystem.Instance.TextureIconRoadType1Lane)
			{
				if(GUILayout.Button(TrafficSystem.Instance.TextureIconRoadType1Lane))
					m_roadTypeSelected = RoadType.LANES_1;
			}
			else
			{
				if(GUILayout.Button("1 Lane"))
					m_roadTypeSelected = RoadType.LANES_1;
			}
			if(TrafficSystem.Instance.TextureIconRoadType2Lanes)
			{
				if(GUILayout.Button(TrafficSystem.Instance.TextureIconRoadType2Lanes))
					m_roadTypeSelected = RoadType.LANES_2;
			}
			else
			{
				if(GUILayout.Button("2 Lanes"))
					m_roadTypeSelected = RoadType.LANES_2;
			}
			if(TrafficSystem.Instance.TextureIconRoadType4Lanes)
			{
				if(GUILayout.Button(TrafficSystem.Instance.TextureIconRoadType4Lanes))
					m_roadTypeSelected = RoadType.LANES_4;
			}
			else
			{
				if(GUILayout.Button("4 Lanes"))
					m_roadTypeSelected = RoadType.LANES_4;
			}
			if(TrafficSystem.Instance.TextureIconRoadType6Lanes)
			{
				if(GUILayout.Button(TrafficSystem.Instance.TextureIconRoadType6Lanes))
					m_roadTypeSelected = RoadType.LANES_6;
			}
			else
			{
				if(GUILayout.Button("6 Lanes"))
					m_roadTypeSelected = RoadType.LANES_6;
			}

			GUILayout.EndHorizontal();

			GUILayout.Space(10.0f);

			int  btnColCount      = 1;
			int  btnColCloseCount = 0;
			bool hasOpened        = false;
			for(int aIndex = 0; aIndex < m_trafficSystemRoadPiecesLocation.Length; aIndex++)
			{
				string file = m_trafficSystemRoadPiecesLocation[aIndex].Substring(m_dirRoadPieces.Length, m_trafficSystemRoadPiecesLocation[aIndex].Length - m_dirRoadPieces.Length - 7);
				if(file.Contains("( Road V2 )"))
				   file = file.ToString() + " NEW";

				if(m_roadTypeSelected == RoadType.LANES_0)
				{
					if(!file.Contains("0 Lane"))
						continue;
				}
				else if(m_roadTypeSelected == RoadType.LANES_1)
				{
					if(!file.Contains("1 Lane"))
						continue;
				}
				else if(m_roadTypeSelected == RoadType.LANES_2)
				{
					if(!file.Contains("2 Lane"))
						continue;
				}
				else if(m_roadTypeSelected == RoadType.LANES_4)
				{
					if(!file.Contains("4 Lane"))
						continue;
				}
				else if(m_roadTypeSelected == RoadType.LANES_6)
				{
					if(!file.Contains("6 Lane"))
						continue;
				}

				if(aIndex % btnColCount == 0)
				{
					hasOpened = true;
					GUILayout.BeginHorizontal("box");
					btnColCloseCount = 0;
				}

				if(GUILayout.Button(file))
				{
					ProcessFolderChecks();

					TrafficSystemPiece roadPiece = AssetDatabase.LoadAssetAtPath(m_trafficSystemRoadPiecesLocation[aIndex], typeof(TrafficSystemPiece)) as TrafficSystemPiece;

					if(roadPiece)
					{
						TrafficSystemPiece roadPieceClone = PrefabUtility.InstantiatePrefab(roadPiece) as TrafficSystemPiece;
						if(roadPieceClone)
						{
							roadPieceClone.name = m_trafficSystemPieceCount + " - " + roadPieceClone.name;

//							Vector3 posOffset = roadPieceClone.m_posOffset;
//							if(AnchorTrafficSystemPiece)
//							{
//								Vector3 dir = AnchorTrafficSystemPiece.transform.position - roadPieceClone.transform.position;
//								dir         = dir.normalized;
//								posOffset.x = posOffset.x * dir.x; 
//								posOffset.y = posOffset.y * dir.y; 
//								posOffset.z = posOffset.z * dir.z; 
//							}

							roadPieceClone.transform.position = Vector3.zero;
							roadPieceClone.transform.rotation = Quaternion.identity;
							roadPieceClone.transform.localScale = TrafficSystem.m_roadScale * roadPieceClone.transform.localScale;
							m_trafficSystemPieceCount++;

							SetTrafficSystemPiece( TrafficSystem.TrafficSystemTooltip.EDIT, roadPieceClone );
							if(TrafficSystem.Instance)
								TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.EDIT, true );

							if(AnchorTrafficSystemPiece)
							{
								PositionTrafficSystemPiece(EditTrafficSystemPiece, AnchorTrafficSystemPiece, false);
								FirstNode = false;
							}
							else
							{
								SetTrafficSystemPiece( TrafficSystem.TrafficSystemTooltip.ANCHOR, EditTrafficSystemPiece );
								FirstNode = true;
							}

							if(TrafficSystem.Instance.m_autoLinkOnSpawn)
							{
								CreateAllLinks( true );
							}

							// position
							if(TrafficSystem)
							{
								roadPieceClone.transform.parent = TrafficSystem.transform;
								UpdateEditTrafficSystemPiecePos();

								// set the quality of the road
								if(TrafficSystem.Instance && TrafficSystem.Instance.m_spawnWithRoadQuality)
									roadPieceClone.ProcessRoadQuality( TrafficSystem.Instance.GetRoadQualityVal() );
							}

							if(TrafficSystem.Instance.m_quickSpawn)
							{
								SetTrafficSystemPiece( TrafficSystem.TrafficSystemTooltip.ANCHOR, EditTrafficSystemPiece );
								SetTrafficSystemPiece( TrafficSystem.TrafficSystemTooltip.EDIT, null );
								if(TrafficSystem.Instance)
									TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.EDIT, false );
							}
						}
					}
				}
				btnColCloseCount++;

				if(btnColCloseCount == btnColCount)
				{
					hasOpened = false;
					GUILayout.EndHorizontal();
					btnColCloseCount = 0;
				}
			}

			if(hasOpened)
				GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.Label ("Road Piece Options", EditorStyles.boldLabel);

			EditorGUILayout.LabelField("Road Attachment Point: " + m_roadAttachmentPointIndex);

			GUILayout.Space(10.0f);
			EditorGUILayout.LabelField("Edit      : " + EditTrafficSystemPiece);
			EditorGUILayout.LabelField("Anchor : " + AnchorTrafficSystemPiece);
			GUILayout.Space(10.0f);

			if(AnchorTrafficSystemPiece && GUILayout.Button("View 'Anchor' Piece " + AnchorTrafficSystemPiece.name))
				Selection.activeObject = AnchorTrafficSystemPiece.transform;

			if(EditTrafficSystemPiece && GUILayout.Button("View 'Edit' Piece " + EditTrafficSystemPiece.name))
				Selection.activeObject = EditTrafficSystemPiece.transform;

			GUILayout.Space(10.0f);
			if(!FirstNode)
			{
				GUILayout.BeginHorizontal("box");
				TrafficSystem.Instance.m_swapAnchorDimensions = GUILayout.Toggle(TrafficSystem.Instance.m_swapAnchorDimensions, " Flip 'Anchor' Renderer Size     .");
				TrafficSystem.Instance.m_swapEditDimensions   = GUILayout.Toggle(TrafficSystem.Instance.m_swapEditDimensions,   " Flip 'Edit' Renderer Size");
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal("box");
//				TrafficSystem.Instance.m_swapOffsetDir        = GUILayout.Toggle(TrafficSystem.Instance.m_swapOffsetDir,        " Flip Offset Dir                        .");
				TrafficSystem.Instance.m_swapOffsetSize       = GUILayout.Toggle(TrafficSystem.Instance.m_swapOffsetSize,       " Flip Offset Size (x -> z, z -> x)      .");
				TrafficSystem.Instance.m_negateOffsetSize     = GUILayout.Toggle(TrafficSystem.Instance.m_negateOffsetSize,     " Negate Offset Size (-x, -y, -z)");
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginVertical("box");
				if(GUILayout.Button("Next Attachment Point"))
					PositionTrafficSystemPiece(EditTrafficSystemPiece, AnchorTrafficSystemPiece);
				if(GUILayout.Button("Next Attachment Point Using 'Edit' Offset"))
					PositionTrafficSystemPiece(EditTrafficSystemPiece, AnchorTrafficSystemPiece, true, true);
				if(GUILayout.Button("Next Attachment Point Using 'Anchor' Offset"))
					PositionTrafficSystemPiece(EditTrafficSystemPiece, AnchorTrafficSystemPiece, true, false, true);
				GUILayout.EndHorizontal();
			}

			GUILayout.BeginHorizontal("box");

			if(GUILayout.Button("Rot Y (" + m_rotateDegrees + ")"))
			{
				EditTrafficSystemPiece.transform.Rotate(Vector3.up * m_rotateDegrees);
			}

			if(GUILayout.Button("Rot Z (" + (m_rotateDegrees * 2) + ")"))
			{
				EditTrafficSystemPiece.transform.Rotate(Vector3.forward * (m_rotateDegrees * 2));
			}

			if(GUILayout.Button("Flip (X)"))
			{
			 	Vector3 scale = EditTrafficSystemPiece.transform.localScale;
				scale.x = -scale.x;
				EditTrafficSystemPiece.transform.localScale = scale;
			}

			if(GUILayout.Button("Flip (Z)"))
			{
				Vector3 scale = EditTrafficSystemPiece.transform.localScale;
				scale.z = -scale.z;
				EditTrafficSystemPiece.transform.localScale = scale;
			}

//			if(GUILayout.Button("Flip Z"))
//			{
//				Vector3 scale = EditTrafficSystemPiece.transform.localScale;
//				scale.z       = -scale.z;
//				EditTrafficSystemPiece.transform.localScale = scale;
//			}

			GUILayout.EndHorizontal();

			if(EditTrafficSystemPiece != AnchorTrafficSystemPiece)
			{
				GUILayout.Space(15.0f);

				GUILayout.BeginHorizontal("box");

				if(GUILayout.Button("Reveal All ('A' to 'E')"))
				{
					CreateAllLinks(true, true);
				}

				if(GUILayout.Button("Reveal All ('E' to 'A')"))
				{
					CreateAllLinks(false, true);
				}

				if(GUILayout.Button("Reveal All ('A' to 'E') Same Lane"))
				{
					CreateAllLinks(true, true, true);
				}
				
				if(GUILayout.Button("Reveal All ('E' to 'A') Same Lane"))
				{
					CreateAllLinks(false, true, true);
				}

				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal("box");
				
				if(GUILayout.Button("Link All ('A' to 'E')"))
				{
					CreateAllLinks(true);
				}
				
				if(GUILayout.Button("Link All ('E' to 'A')"))
				{
					CreateAllLinks(false);
				}

				if(GUILayout.Button("Link All ('A' to 'E') Same Lane"))
				{
					CreateAllLinks(true, false, true);
				}

				if(GUILayout.Button("Link All ('E' to 'A') Same Lane"))
				{
					CreateAllLinks(false, false, true);
				}

				GUILayout.EndHorizontal();

				GUILayout.Space(15.0f);

				GUILayout.BeginHorizontal("box");
				
				if(GUILayout.Button("Reveal Opposite ('A' to 'E')"))
				{
					CreateOppositeLinks( true, true );
				}
				if(GUILayout.Button("Reveal Opposite ('E' to 'A')"))
				{
					CreateOppositeLinks( false, true );
				}
				
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal("box");
				
				if(GUILayout.Button("Link Opposite ('A' to 'E')"))
				{
					CreateOppositeLinks( true );
				}
				if(GUILayout.Button("Link Opposite ('E' to 'A')"))
				{
					CreateOppositeLinks( false );
				}
				
				GUILayout.EndHorizontal();

				GUILayout.Space(15.0f);

				GUILayout.BeginHorizontal("box");

				if(GUILayout.Button("Reveal (Blue - 'A' to 'E')"))
				{
					CreateLeftLinks(true, true);
				}
				if(GUILayout.Button("Reveal (Blue - 'E' to 'A')"))
				{
					CreateLeftLinks(false, true);
				}
				if(GUILayout.Button("Reveal (Pink - 'A' to 'E')"))
				{
					CreateRightLink(true, true);
				}
				if(GUILayout.Button("Reveal (Pink - 'E' to 'A')"))
				{
					CreateRightLink(false, true);
				}
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal("box");
				
				if(GUILayout.Button("Link (Blue - 'A' to 'E')"))
				{
					CreateLeftLinks(true);
				}
				if(GUILayout.Button("Link (Blue - 'E' to 'A')"))
				{
					CreateLeftLinks(false);
				}
				if(GUILayout.Button("Link (Pink - 'A' to 'E')"))
				{
					CreateRightLink(true);
				}
				if(GUILayout.Button("Link (Pink - 'E' to 'A')"))
				{
					CreateRightLink(false);
				}

				GUILayout.EndHorizontal();

				GUILayout.Space(15.0f);


				for(int bIndex = 0; bIndex < EditTrafficSystemPiece.m_leftLaneChangeNodes.Count; bIndex++)
				{
					TrafficSystemNode node = EditTrafficSystemPiece.m_leftLaneChangeNodes[bIndex];

					if(node && node.m_isInbound)
					{
						GUILayout.BeginHorizontal("box");
						GUILayout.Label("Link CL Inbound: ");
						if(GUILayout.Button("Reveal ->"))
						{
							CreateAllChangeLaneLinks( true, TrafficSystem.DriveSide.LEFT, (int)node.m_changeLaneID, false, true );
						}
						if(GUILayout.Button("Blue Primary -> Blue " + node.m_changeLaneID ))
						{
							CreateAllChangeLaneLinks( true, TrafficSystem.DriveSide.LEFT, (int)node.m_changeLaneID, false );
						}
						if(GUILayout.Button("Reveal ->"))
						{
							CreateAllChangeLaneLinks( true, TrafficSystem.DriveSide.LEFT, (int)node.m_changeLaneID, true, true );
						}
						if(GUILayout.Button("Pink Primary -> Blue " + node.m_changeLaneID ))
						{
							CreateAllChangeLaneLinks( true, TrafficSystem.DriveSide.LEFT, (int)node.m_changeLaneID, true );
						}
						GUILayout.EndHorizontal();
					}
				}

				for(int bIndex = 0; bIndex < EditTrafficSystemPiece.m_rightLaneChangeNodes.Count; bIndex++)
				{
					TrafficSystemNode node = EditTrafficSystemPiece.m_rightLaneChangeNodes[bIndex];
					
					if(node && node.m_isInbound)
					{
						GUILayout.BeginHorizontal("box");
						GUILayout.Label("Link CL Inbound: ");
						if(GUILayout.Button("Reveal ->"))
						{
							CreateAllChangeLaneLinks( true, TrafficSystem.DriveSide.RIGHT, (int)node.m_changeLaneID, false, true );
						}
						if(GUILayout.Button("Pink Primary -> Pink " + node.m_changeLaneID))
						{
							CreateAllChangeLaneLinks( true, TrafficSystem.DriveSide.RIGHT, (int)node.m_changeLaneID, false );
						}
						if(GUILayout.Button("Reveal ->"))
						{
							CreateAllChangeLaneLinks( true, TrafficSystem.DriveSide.RIGHT, (int)node.m_changeLaneID, true, true );
						}
						if(GUILayout.Button("Blue Primary -> Pink " + node.m_changeLaneID))
						{
							CreateAllChangeLaneLinks( true, TrafficSystem.DriveSide.RIGHT, (int)node.m_changeLaneID, true );
						}
						GUILayout.EndHorizontal();
					}
				}

				for(int bIndex = 0; bIndex < AnchorTrafficSystemPiece.m_leftLaneChangeNodes.Count; bIndex++)
				{
					TrafficSystemNode node = AnchorTrafficSystemPiece.m_leftLaneChangeNodes[bIndex];
					
					if(node && node.m_isOutbound)
					{
						GUILayout.BeginHorizontal("box");
						GUILayout.Label("Link CL Outbound: ");
						if(GUILayout.Button("Reveal ->"))
						{
							CreateAllChangeLaneLinks( false, TrafficSystem.DriveSide.LEFT, (int)node.m_changeLaneID, true, true );
						}
						if(GUILayout.Button("Blue " + node.m_changeLaneID + " -> Blue Primary"))
						{
							CreateAllChangeLaneLinks( false, TrafficSystem.DriveSide.LEFT, (int)node.m_changeLaneID, true );
						}
						if(GUILayout.Button("Reveal ->"))
						{
							CreateAllChangeLaneLinks( false, TrafficSystem.DriveSide.LEFT, (int)node.m_changeLaneID, false, true );
						}
						if(GUILayout.Button("Blue " + node.m_changeLaneID + " -> Pink Primary"))
						{
							CreateAllChangeLaneLinks( false, TrafficSystem.DriveSide.LEFT, (int)node.m_changeLaneID, false );
						}
						GUILayout.EndHorizontal();
					}
				}

				for(int bIndex = 0; bIndex < AnchorTrafficSystemPiece.m_rightLaneChangeNodes.Count; bIndex++)
				{
					TrafficSystemNode node = AnchorTrafficSystemPiece.m_rightLaneChangeNodes[bIndex];
					
					if(node && node.m_isOutbound)
					{
						GUILayout.BeginHorizontal("box");
						GUILayout.Label("Link CL Outbound: ");
						if(GUILayout.Button("Reveal ->"))
						{
							CreateAllChangeLaneLinks( false, TrafficSystem.DriveSide.RIGHT, (int)node.m_changeLaneID, true, true );
						}
						if(GUILayout.Button("Pink " + node.m_changeLaneID + " -> Pink Primary"))
						{
							CreateAllChangeLaneLinks( false, TrafficSystem.DriveSide.RIGHT, (int)node.m_changeLaneID, true );
						}
						if(GUILayout.Button("Reveal ->"))
						{
							CreateAllChangeLaneLinks( false, TrafficSystem.DriveSide.RIGHT, (int)node.m_changeLaneID, false, true );
						}
						if(GUILayout.Button("Pink " + node.m_changeLaneID + " -> Blue Primary"))
						{
							CreateAllChangeLaneLinks( false, TrafficSystem.DriveSide.RIGHT, (int)node.m_changeLaneID, false );
						}
						GUILayout.EndHorizontal();
					}
				}


				/*	
				for(int bIndex = 0; bIndex < (int)TrafficSystemNode.NodeID.ID_MAX; bIndex++)
				{
					GUILayout.BeginHorizontal("box");

					if(GUILayout.Button("Link CL Inbound " + bIndex + " (Blue)"))
					{
						CreateAllChangeLaneLinks( true, TrafficSystem.DriveSide.LEFT, bIndex );
					}
					if(GUILayout.Button("Link CL Outbound " + bIndex + " (Blue)"))
					{
						CreateAllChangeLaneLinks( false, TrafficSystem.DriveSide.LEFT, bIndex );
					}

					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal("box");

					if(GUILayout.Button("Link CL Inbound " + bIndex + " (Pink)"))
					{
						CreateAllChangeLaneLinks( true, TrafficSystem.DriveSide.RIGHT, bIndex );
					}
					if(GUILayout.Button("Link CL Outbound " + bIndex + " (Pink)"))
					{
						CreateAllChangeLaneLinks( false, TrafficSystem.DriveSide.RIGHT, bIndex );
					}

					GUILayout.EndHorizontal();
				}
				*/
			}

			GUILayout.BeginHorizontal("box");

			if(GUILayout.Button("UnLink All"))
			{
				for(int lNIndex = 0; lNIndex < EditTrafficSystemPiece.m_primaryLeftLaneNodes.Count; lNIndex++)
				{
					TrafficSystemNode lastNode = EditTrafficSystemPiece.m_primaryLeftLaneNodes[lNIndex];
					lastNode.m_connectedNodes.Clear();
					EditorUtility.SetDirty(lastNode);
				}

				for(int lNIndex = 0; lNIndex < EditTrafficSystemPiece.m_secondaryLeftLaneNodes.Count; lNIndex++)
				{
					TrafficSystemNode lastNode = EditTrafficSystemPiece.m_secondaryLeftLaneNodes[lNIndex];
					lastNode.m_connectedNodes.Clear();
					EditorUtility.SetDirty(lastNode);
				}

				for(int lNIndex = 0; lNIndex < EditTrafficSystemPiece.m_primaryRightLaneNodes.Count; lNIndex++)
				{
					TrafficSystemNode lastNode = EditTrafficSystemPiece.m_primaryRightLaneNodes[lNIndex];
					lastNode.m_connectedNodes.Clear();
					EditorUtility.SetDirty(lastNode);
				}

				for(int lNIndex = 0; lNIndex < EditTrafficSystemPiece.m_secondaryRightLaneNodes.Count; lNIndex++)
				{
					TrafficSystemNode lastNode = EditTrafficSystemPiece.m_secondaryRightLaneNodes[lNIndex];
					lastNode.m_connectedNodes.Clear();
					EditorUtility.SetDirty(lastNode);
				}
			}

			if(GUILayout.Button("UnLink (Blue)"))
			{
				for(int lNIndex = 0; lNIndex < EditTrafficSystemPiece.m_primaryLeftLaneNodes.Count; lNIndex++)
				{
					TrafficSystemNode lastNode = EditTrafficSystemPiece.m_primaryLeftLaneNodes[lNIndex];
					lastNode.m_connectedNodes.Clear();
					EditorUtility.SetDirty(lastNode);
				}
				
				for(int lNIndex = 0; lNIndex < EditTrafficSystemPiece.m_secondaryLeftLaneNodes.Count; lNIndex++)
				{
					TrafficSystemNode lastNode = EditTrafficSystemPiece.m_secondaryLeftLaneNodes[lNIndex];
					lastNode.m_connectedNodes.Clear();
					EditorUtility.SetDirty(lastNode);
				}
			}

			if(GUILayout.Button("UnLink (Pink)"))
			{
				for(int lNIndex = 0; lNIndex < EditTrafficSystemPiece.m_primaryRightLaneNodes.Count; lNIndex++)
				{
					TrafficSystemNode lastNode = EditTrafficSystemPiece.m_primaryRightLaneNodes[lNIndex];
					lastNode.m_connectedNodes.Clear();
					EditorUtility.SetDirty(lastNode);
				}
				
				for(int lNIndex = 0; lNIndex < EditTrafficSystemPiece.m_secondaryRightLaneNodes.Count; lNIndex++)
				{
					TrafficSystemNode lastNode = EditTrafficSystemPiece.m_secondaryRightLaneNodes[lNIndex];
					lastNode.m_connectedNodes.Clear();
					EditorUtility.SetDirty(lastNode);
				}
			}

			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("box");

			if(GUILayout.Button("Delete"))
			{
				DestroyImmediate(EditTrafficSystemPiece.gameObject);
				SetTrafficSystemPiece( TrafficSystem.TrafficSystemTooltip.EDIT, null );
			}

			GUILayout.EndHorizontal();
			GUILayout.Space(10.0f);

			if(EditTrafficSystemPiece)
			{
				if(GUILayout.Button("Save Changes to " + EditTrafficSystemPiece.name))
				{
//					if(EditTrafficSystemPiece && EditTrafficSystemPiece.m_ignoreChildren)
//					{
//						List<TrafficSystemPiece> pieces = new List<TrafficSystemPiece>();
//						for(int cIndex = 0; cIndex < EditTrafficSystemPiece.transform.childCount; cIndex++)
//						{
//							Transform child = EditTrafficSystemPiece.transform.GetChild(cIndex);
//							if(child.gameObject.GetComponent<TrafficSystemPiece>())
//								pieces.Add(child.gameObject.GetComponent<TrafficSystemPiece>());
//						}
//
//						for(int cIndex = 0; cIndex < pieces.Count; cIndex++)
//							pieces[cIndex].transform.parent = TrafficSystem.transform;
//					}

					TrafficSystem.ClearCLRevealObjsFrom();
					TrafficSystem.ClearCLRevealObjsTo();

					SetTrafficSystemPiece( TrafficSystem.TrafficSystemTooltip.ANCHOR, EditTrafficSystemPiece );
					SetTrafficSystemPiece( TrafficSystem.TrafficSystemTooltip.EDIT, null );
					if(TrafficSystem.Instance)
						TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.EDIT, false );
					//m_roadAttachmentPointIndex = RoadAttachmentPoint.SOUTH;
				}
			}
		}

		if(GUI.changed)
			EditorUtility.SetDirty(TrafficSystem);

		if(TrafficSystem)
		{
			if(EditNodeFolder)
			{
				for(int cIndex = 0; cIndex < EditNodeFolder.childCount; cIndex++)
					EditNodeFolder.GetChild(cIndex).transform.parent = TrafficSystem.transform;

				if(EditTrafficSystemPiece)
				{
					TrafficSystemPiece piece = FindParentTrafficSystemPiece( EditTrafficSystemPiece );
					piece.transform.parent   = EditNodeFolder;
				}
			}

			if(AnchorNodeFolder)
			{
				for(int cIndex = 0; cIndex < AnchorNodeFolder.childCount; cIndex++)
					AnchorNodeFolder.GetChild(cIndex).transform.parent = TrafficSystem.transform;
				
				if(AnchorTrafficSystemPiece)
				{
					TrafficSystemPiece piece = FindParentTrafficSystemPiece( AnchorTrafficSystemPiece );
					piece.transform.parent   = AnchorNodeFolder;
				}
			}
		}

//		groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
//		{
//			myBool = EditorGUILayout.Toggle ("Toggle", myBool);
//			myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
//			myString = EditorGUILayout.TextField ("Text Field", myString);
//		}
//		EditorGUILayout.EndToggleGroup ();
	}

	void OnSceneGUI()
	{
		if(m_init && m_initCount > m_initCountMax)
		{
			ProcessFolderChecks();
			m_init = false;
		}

		if(TrafficSystem && TrafficSystem.Instance)
		{
			if(!EditTrafficSystemPiece)
				TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.EDIT, false );
			else
				TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.EDIT, true );

			if(!AnchorTrafficSystemPiece)
				TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.ANCHOR, false );
			else
				TrafficSystem.Instance.ShowTooltip( TrafficSystem.TrafficSystemTooltip.ANCHOR, true );

			if(!TrafficSystem.Instance.TooltipAnchor)
			{
				if(ResourceNodeFolder && ResourceNodeFolder.childCount > 1)
				{
					for(int cIndex = 1; cIndex < ResourceNodeFolder.childCount; cIndex++)
					{
						if(ResourceNodeFolder.GetChild(cIndex) && ResourceNodeFolder.GetChild(cIndex).name == "Tooltip-Anchor")
							if(ResourceNodeFolder.GetChild(cIndex).gameObject)
								DestroyImmediate(ResourceNodeFolder.GetChild(cIndex).gameObject);
					}
				}

				GameObject tooltip = GameObject.Find ("Tooltip-Anchor");
				if (tooltip)
					TrafficSystem.Instance.TooltipAnchor = tooltip;
				else
				{
					GameObject toolTip = AssetDatabase.LoadAssetAtPath(TooltipAnchorLocation, typeof(GameObject)) as GameObject;
					TrafficSystem.Instance.TooltipAnchor = PrefabUtility.InstantiatePrefab(toolTip) as GameObject;
				}
			}
			else
			{
				if(ResourceNodeFolder)
					TrafficSystem.Instance.TooltipAnchor.transform.parent = ResourceNodeFolder;
			}
			
			if(!TrafficSystem.Instance.TooltipEdit)
			{
				if(ResourceNodeFolder && ResourceNodeFolder.childCount > 1)
				{
					for(int cIndex = 1; cIndex < ResourceNodeFolder.childCount; cIndex++)
					{
						if(ResourceNodeFolder.GetChild(cIndex) && ResourceNodeFolder.GetChild(cIndex).name == "Tooltip-Edit")
							if(ResourceNodeFolder.GetChild(cIndex).gameObject)
								DestroyImmediate(ResourceNodeFolder.GetChild(cIndex).gameObject);
					}
				}

				GameObject tooltip = GameObject.Find ("Tooltip-Edit");
				if (tooltip)
					TrafficSystem.Instance.TooltipEdit = tooltip;
				else
				{
					GameObject toolTip = AssetDatabase.LoadAssetAtPath(TooltipEditLocation, typeof(GameObject)) as GameObject;
					TrafficSystem.Instance.TooltipEdit = PrefabUtility.InstantiatePrefab(toolTip) as GameObject;
				}
			}
			else
			{
				if(ResourceNodeFolder)
					TrafficSystem.Instance.TooltipEdit.transform.parent = ResourceNodeFolder;
			}
			
			if(TrafficSystem.Instance.EditTrafficSystemPiece)
			{
				SetTrafficSystemPiece( TrafficSystem.TrafficSystemTooltip.EDIT, TrafficSystem.Instance.EditTrafficSystemPiece );
				TrafficSystem.Instance.SetTrafficSystemPiece( TrafficSystem.TrafficSystemTooltip.EDIT, null );
			}
			
			if(TrafficSystem.Instance.AnchorTrafficSystemPiece)
			{
				SetTrafficSystemPiece( TrafficSystem.TrafficSystemTooltip.ANCHOR, TrafficSystem.Instance.AnchorTrafficSystemPiece );
				TrafficSystem.Instance.SetTrafficSystemPiece( TrafficSystem.TrafficSystemTooltip.ANCHOR, null );
			}

			if(TrafficSystem.Instance.TooltipAnchor)
				TrafficSystem.Instance.TooltipAnchor.transform.Rotate(Vector3.up, 1.0f); 
			if(TrafficSystem.Instance.TooltipEdit)
				TrafficSystem.Instance.TooltipEdit.transform.Rotate(Vector3.up, 1.0f); 
		}

		m_initCount++;
	}

	TrafficSystemNode FindChildNode( TrafficSystemNode a_node )
	{
		if(a_node.m_connectedLocalNode)
			a_node = FindChildNode( a_node.m_connectedLocalNode );

		return a_node;
	}

	void CreateOppositeLinks( bool a_anchorToEdit, bool a_revealOnly = false )
	{
		if(EditTrafficSystemPiece != AnchorTrafficSystemPiece)
		{
			if(TrafficSystem.Instance.m_autoReverseAnchorToEdit)
				a_anchorToEdit = !a_anchorToEdit;

			if(a_anchorToEdit)
			{
				CreateConnections( EditTrafficSystemPiece.m_primaryLeftLaneNodes, AnchorTrafficSystemPiece.m_primaryRightLaneNodes, false, 0, a_revealOnly, true );
				CreateConnections( EditTrafficSystemPiece.m_secondaryLeftLaneNodes, AnchorTrafficSystemPiece.m_primaryRightLaneNodes, false, 0, a_revealOnly, true, false );
				CreateConnections( AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, EditTrafficSystemPiece.m_primaryRightLaneNodes, false, 0, a_revealOnly, true, false );
				CreateConnections( AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, EditTrafficSystemPiece.m_secondaryRightLaneNodes, false, 0, a_revealOnly, true, false );
			}
			else
			{
				CreateConnections( EditTrafficSystemPiece.m_primaryRightLaneNodes, AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, false, 0, a_revealOnly, true );
				CreateConnections( EditTrafficSystemPiece.m_secondaryRightLaneNodes, AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, false, 0, a_revealOnly, true, false );
				CreateConnections( AnchorTrafficSystemPiece.m_primaryRightLaneNodes, EditTrafficSystemPiece.m_primaryLeftLaneNodes, false, 0, a_revealOnly, true, false );
				CreateConnections( AnchorTrafficSystemPiece.m_primaryRightLaneNodes, EditTrafficSystemPiece.m_secondaryLeftLaneNodes, false, 0, a_revealOnly, true, false );
			}
		}
	}

	void CreateAllLinks( bool a_anchorToEdit, bool a_revealOnly = false, bool a_linkSameLaneOnly = false )
	{
		TrafficSystem.ClearCLRevealObjsFrom();
		TrafficSystem.ClearCLRevealObjsTo();

		if(EditTrafficSystemPiece != AnchorTrafficSystemPiece)
		{
			if(TrafficSystem.Instance.m_autoReverseAnchorToEdit)
				a_anchorToEdit = !a_anchorToEdit;

			if(IsOffRampPiece(AnchorTrafficSystemPiece.m_primaryLeftLaneNodes) || 
			   IsOffRampPiece(AnchorTrafficSystemPiece.m_primaryRightLaneNodes) || 
			   IsOffRampPiece(EditTrafficSystemPiece.m_primaryLeftLaneNodes) || 
			   IsOffRampPiece(EditTrafficSystemPiece.m_primaryRightLaneNodes))
			{
				if(a_anchorToEdit)
				{
					CreateOffRampConnection(AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, EditTrafficSystemPiece.m_primaryLeftLaneNodes, a_revealOnly);
					CreateOffRampConnection(EditTrafficSystemPiece.m_primaryRightLaneNodes, AnchorTrafficSystemPiece.m_primaryRightLaneNodes, a_revealOnly);
				}
				else
				{
					CreateOffRampConnection(EditTrafficSystemPiece.m_primaryLeftLaneNodes, AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, a_revealOnly);
					CreateOffRampConnection(AnchorTrafficSystemPiece.m_primaryRightLaneNodes, EditTrafficSystemPiece.m_primaryRightLaneNodes, a_revealOnly);
				}
			}
			else
			{
				if(a_anchorToEdit)
				{
					CreateConnections( AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, EditTrafficSystemPiece.m_primaryLeftLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly );
					CreateConnections( AnchorTrafficSystemPiece.m_secondaryLeftLaneNodes, EditTrafficSystemPiece.m_primaryLeftLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly );
					CreateConnections( EditTrafficSystemPiece.m_primaryRightLaneNodes, AnchorTrafficSystemPiece.m_primaryRightLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly);
					CreateConnections( EditTrafficSystemPiece.m_secondaryRightLaneNodes, AnchorTrafficSystemPiece.m_primaryRightLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly );
				}
				else
				{
					CreateConnections( EditTrafficSystemPiece.m_primaryLeftLaneNodes, AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly );
					CreateConnections( EditTrafficSystemPiece.m_secondaryLeftLaneNodes, AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly );
					CreateConnections( AnchorTrafficSystemPiece.m_primaryRightLaneNodes, EditTrafficSystemPiece.m_primaryRightLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly );
					CreateConnections( AnchorTrafficSystemPiece.m_secondaryRightLaneNodes, EditTrafficSystemPiece.m_primaryRightLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly );
				}
			}
		}
	}

	void CreateLeftLinks( bool a_anchorToEdit, bool a_revealOnly = false )
	{
		if(EditTrafficSystemPiece != AnchorTrafficSystemPiece)
		{
			if(TrafficSystem.Instance.m_autoReverseAnchorToEdit)
				a_anchorToEdit = !a_anchorToEdit;

			if(a_anchorToEdit)
			{
				CreateConnections( AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, EditTrafficSystemPiece.m_primaryLeftLaneNodes, false, 0, a_revealOnly, true );
				CreateConnections( AnchorTrafficSystemPiece.m_secondaryLeftLaneNodes, EditTrafficSystemPiece.m_primaryLeftLaneNodes, false, 0, a_revealOnly, true, false );
			}
			else
			{
				CreateConnections( EditTrafficSystemPiece.m_primaryLeftLaneNodes, AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, false, 0, a_revealOnly, true );
				CreateConnections( EditTrafficSystemPiece.m_secondaryLeftLaneNodes, AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, false, 0, a_revealOnly, true, false );
			}
		}
	}

	void CreateRightLink( bool a_anchorToEdit, bool a_revealOnly = false )
	{
		if(EditTrafficSystemPiece != AnchorTrafficSystemPiece)
		{
			if(TrafficSystem.Instance.m_autoReverseAnchorToEdit)
				a_anchorToEdit = !a_anchorToEdit;

			if(a_anchorToEdit)
			{
				CreateConnections( AnchorTrafficSystemPiece.m_primaryRightLaneNodes, EditTrafficSystemPiece.m_primaryRightLaneNodes, false, 0, a_revealOnly, true );
				CreateConnections( AnchorTrafficSystemPiece.m_secondaryRightLaneNodes, EditTrafficSystemPiece.m_primaryRightLaneNodes, false, 0, a_revealOnly, true, false );
			}
			else
			{
				CreateConnections( EditTrafficSystemPiece.m_primaryRightLaneNodes, AnchorTrafficSystemPiece.m_primaryRightLaneNodes, false, 0, a_revealOnly, true );
				CreateConnections( EditTrafficSystemPiece.m_secondaryRightLaneNodes, AnchorTrafficSystemPiece.m_primaryRightLaneNodes, false, 0, a_revealOnly, true, false );
			}
		}
	}

	void CreateAllChangeLaneLinks( bool a_inbound, TrafficSystem.DriveSide a_side, int a_id, bool a_switch, bool a_revealOnly = false )
	{
		TrafficSystem.ClearCLRevealObjsFrom();
		TrafficSystem.ClearCLRevealObjsTo();

		if(EditTrafficSystemPiece != AnchorTrafficSystemPiece)
		{
			if(a_inbound)
			{
				if(a_side == TrafficSystem.DriveSide.LEFT)
				{
					if(a_switch)
						CreateCLInBoundConnections( AnchorTrafficSystemPiece.m_primaryRightLaneNodes, EditTrafficSystemPiece.m_leftLaneChangeNodes, a_id, a_revealOnly );
					else
						CreateCLInBoundConnections( AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, EditTrafficSystemPiece.m_leftLaneChangeNodes, a_id, a_revealOnly );
				}
				else
				{
					if(a_switch)
						CreateCLInBoundConnections( AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, EditTrafficSystemPiece.m_rightLaneChangeNodes, a_id, a_revealOnly );
					else
						CreateCLInBoundConnections( AnchorTrafficSystemPiece.m_primaryRightLaneNodes, EditTrafficSystemPiece.m_rightLaneChangeNodes, a_id, a_revealOnly );
				}
			}
			else
			{
				if(a_side == TrafficSystem.DriveSide.LEFT)
				{
					if(a_switch)
						CreateCLOutBoundConnections( AnchorTrafficSystemPiece.m_leftLaneChangeNodes, EditTrafficSystemPiece.m_primaryLeftLaneNodes, a_id, a_revealOnly );
					else
						CreateCLOutBoundConnections( AnchorTrafficSystemPiece.m_leftLaneChangeNodes, EditTrafficSystemPiece.m_primaryRightLaneNodes, a_id, a_revealOnly );
				}
				else
				{
					if(a_switch)
						CreateCLOutBoundConnections( AnchorTrafficSystemPiece.m_rightLaneChangeNodes, EditTrafficSystemPiece.m_primaryRightLaneNodes, a_id, a_revealOnly );
					else
						CreateCLOutBoundConnections( AnchorTrafficSystemPiece.m_rightLaneChangeNodes, EditTrafficSystemPiece.m_primaryLeftLaneNodes, a_id, a_revealOnly );
				}
			}
		}
	}

	void CreateOffRampConnection( List<TrafficSystemNode> a_nodesLookingToConnect, List<TrafficSystemNode> a_nodesToConnectWith, bool a_revealOnly = false )
	{
		for(int lNIndex = 0; lNIndex < a_nodesLookingToConnect.Count; lNIndex++)
		{
			TrafficSystemNode lastNode = a_nodesLookingToConnect[lNIndex];

			for(int cNIndex = 0; cNIndex < a_nodesToConnectWith.Count; cNIndex++)
			{
				TrafficSystemNode currentNode = a_nodesToConnectWith[cNIndex];

				if(lastNode.m_driveSide != currentNode.m_driveSide)
					continue;

				if( lastNode.m_lane == 0 && currentNode.m_lane == 0 )
				{
					if(a_revealOnly)
					{
						TrafficSystem.AddToCLRevealObjsFrom(lastNode.transform);
						TrafficSystem.AddToCLRevealObjsTo(currentNode.transform);
					}
					else
					{
						lastNode.AddConnectedNode( currentNode );
						EditorUtility.SetDirty(lastNode);
					}
				}
			}
		}
	}

	bool IsOffRampPiece( List<TrafficSystemNode> a_nodes )
	{
		bool isOfframp = false;
		for(int lNIndex = 0; lNIndex < a_nodes.Count; lNIndex++)
		{
			TrafficSystemNode lastNode = a_nodes[lNIndex];

			if(lastNode.m_roadType == TrafficSystem.RoadType.OFFRAMP)
			{
				isOfframp = true;
				break;
			}
		}

		return isOfframp;
	}

	void CreateConnections( List<TrafficSystemNode> a_nodesLookingToConnect, List<TrafficSystemNode> a_nodesToConnectWith, bool a_isOutbound = false, int a_nodeID = 0, bool a_revealOnly = false, bool a_useConnectedNodeArray = false, bool a_clearReveal = true, bool a_linkSameLaneOnly = false )
	{
		if(a_clearReveal)
		{
			TrafficSystem.ClearCLRevealObjsFrom();
			TrafficSystem.ClearCLRevealObjsTo();
		}

		for(int lNIndex = 0; lNIndex < a_nodesLookingToConnect.Count; lNIndex++)
		{
			TrafficSystemNode lastNode = a_nodesLookingToConnect[lNIndex];

			if(a_isOutbound && (lastNode.m_isInbound || (int)lastNode.m_changeLaneID != a_nodeID))
				continue;

			for(int cNIndex = 0; cNIndex < a_nodesToConnectWith.Count; cNIndex++)
			{
				TrafficSystemNode currentNode = a_nodesToConnectWith[cNIndex];
				
				bool nodeFound = false;
				for(int cIndex = 0; cIndex < lastNode.m_connectedNodes.Count; cIndex++)
				{
					TrafficSystemNode connectedNode = lastNode.m_connectedNodes[cIndex];
					
					if(connectedNode == currentNode)
					{
						nodeFound = true;
						break;
					}
				}
				
				if(!nodeFound)
				{
					if(a_linkSameLaneOnly || (currentNode.m_onlyConnectWithSameLane || lastNode.m_onlyConnectWithSameLane))
					{
						if(lastNode.m_lane == currentNode.m_lane)
						{
							if(a_revealOnly)
							{
								TrafficSystem.AddToCLRevealObjsFrom(lastNode.transform);
								TrafficSystem.AddToCLRevealObjsTo(currentNode.transform);
							}
							else
							{
								if(a_useConnectedNodeArray)
									lastNode.AddConnectedNode(currentNode);
								else
									lastNode.AddChangeLaneNode(currentNode);

								EditorUtility.SetDirty(lastNode);
							}
						}
					}
					else
					{
						if(a_revealOnly)
						{
							TrafficSystem.AddToCLRevealObjsFrom(lastNode.transform);
							TrafficSystem.AddToCLRevealObjsTo(currentNode.transform);
						}
						else
						{
							if(a_useConnectedNodeArray)
								lastNode.AddConnectedNode(currentNode);
							else
								lastNode.AddChangeLaneNode(currentNode);

							EditorUtility.SetDirty(lastNode);
						}
					}
				}
			}
		}
	}

	void CreateCLInBoundConnections( List<TrafficSystemNode> a_nodesLookingToConnect, List<TrafficSystemNode> a_nodesToConnectWith, int a_nodeID, bool a_revealOnly = false )
	{
		for(int lNIndex = 0; lNIndex < a_nodesLookingToConnect.Count; lNIndex++)
		{
			TrafficSystemNode lastNode = a_nodesLookingToConnect[lNIndex];

			while(lastNode.m_connectedLocalNode)
				lastNode = lastNode.m_connectedLocalNode;

			for(int cNIndex = 0; cNIndex < a_nodesToConnectWith.Count; cNIndex++)
			{
				TrafficSystemNode currentNode = a_nodesToConnectWith[cNIndex];

				if((int)currentNode.m_changeLaneID != a_nodeID)
					continue;

				if(currentNode.m_isInbound)
				{
					if(true)//currentNode.m_onlyConnectWithSameLane || lastNode.m_onlyConnectWithSameLane)
					{
						if(lastNode.m_lane == currentNode.m_lane)
						{
							if(a_revealOnly)
							{
								TrafficSystem.AddToCLRevealObjsFrom(lastNode.transform);
								TrafficSystem.AddToCLRevealObjsTo(currentNode.transform);
							}
							else
							{
								lastNode.AddChangeLaneNode(currentNode);
								EditorUtility.SetDirty(lastNode);
							}
						}
					}
//					else
//					{
//						if(a_revealOnly)
//						{
//							TrafficSystem.AddToCLRevealObjsFrom(lastNode.transform);
//							TrafficSystem.AddToCLRevealObjsTo(currentNode.transform);
//						}
//						else
//						{
//							lastNode.AddChangeLaneNode(currentNode);
//							EditorUtility.SetDirty(lastNode);
//						}
//					}
				}
			}
		}
	}

	void CreateCLOutBoundConnections( List<TrafficSystemNode> a_nodesLookingToConnect, List<TrafficSystemNode> a_nodesToConnectWith, int a_nodeID, bool a_revealOnly = false )
	{
		for(int lNIndex = 0; lNIndex < a_nodesToConnectWith.Count; lNIndex++)
		{
			TrafficSystemNode lastNode = a_nodesToConnectWith[lNIndex];
			
			while(lastNode.m_connectedLocalNode)
				lastNode = lastNode.m_connectedLocalNode;
			
			for(int cNIndex = 0; cNIndex < a_nodesLookingToConnect.Count; cNIndex++)
			{
				TrafficSystemNode currentNode = a_nodesLookingToConnect[cNIndex];
				
				if((int)currentNode.m_changeLaneID != a_nodeID)
					continue;
				
				if(currentNode.m_isOutbound)
				{
					if(lastNode.m_lane == currentNode.m_lane)
					{
						if(a_revealOnly)
						{
							TrafficSystem.AddToCLRevealObjsFrom(currentNode.transform);
							TrafficSystem.AddToCLRevealObjsTo(lastNode.transform);
						}
						else
						{
							currentNode.AddChangeLaneNode(lastNode);
							EditorUtility.SetDirty(currentNode);
						}
					}
				}
			}
		}
	}
	
	public void UpdateEditTrafficSystemPiecePos()
	{
			SetTrafficSystemPiece( TrafficSystem.TrafficSystemTooltip.EDIT, EditTrafficSystemPiece ); // force reposition of edit icon
	}

	void PositionTrafficSystemPiece( TrafficSystemPiece a_currentPiece, TrafficSystemPiece a_attachToPiece, bool a_incIndex = true, bool a_useEditOffset = false, bool a_useAnchorOffset = false )
	{
		if(!a_currentPiece)
			return;

		if(!a_attachToPiece)
			return;

		if(a_incIndex)
			m_roadAttachmentPointIndex++;

		if(m_roadAttachmentPointIndex == RoadAttachmentPoint.MAX)
			m_roadAttachmentPointIndex = RoadAttachmentPoint.NORTH;

		EditTrafficSystemPiece.transform.position = a_attachToPiece.transform.position;

		if(TrafficSystem.Instance.m_quickSpawn)
			EditTrafficSystemPiece.transform.rotation = a_attachToPiece.transform.rotation;

		Vector3 pos = a_attachToPiece.transform.position;

		if(a_currentPiece.m_renderer && a_attachToPiece.m_renderer)
		{
			EditTrafficSystemPiece.transform.position = a_attachToPiece.m_renderer.transform.position;
//			EditTrafficSystemPiece.transform.rotation = a_attachToPiece.m_renderer.transform.rotation;
			pos = a_attachToPiece.m_renderer.transform.position;
		}

		float   roadPieceSize = m_roadPieceSize;

		switch(m_roadAttachmentPointIndex)
		{
		case RoadAttachmentPoint.EAST:
		{
			if(a_attachToPiece.m_renderer && a_currentPiece.m_renderer)
			{
				float anchorSize  = a_attachToPiece.GetRenderBounds().extents.x;
				if(TrafficSystem.Instance.m_swapAnchorDimensions)
					anchorSize    = a_attachToPiece.GetRenderBounds().extents.z;

				float currentSize = a_currentPiece.GetRenderBounds().extents.x;
				if(TrafficSystem.Instance.m_swapEditDimensions)
					currentSize   = a_currentPiece.GetRenderBounds().extents.z;

				roadPieceSize     = anchorSize + currentSize;
			}	
			pos.x = a_attachToPiece.m_renderer.transform.position.x + roadPieceSize;
		}
			break;
		case RoadAttachmentPoint.SOUTH:
		{
			if(a_attachToPiece.m_renderer && a_currentPiece.m_renderer)
			{
				float anchorSize  = a_attachToPiece.GetRenderBounds().extents.z;
				if(TrafficSystem.Instance.m_swapAnchorDimensions)
					anchorSize    = a_attachToPiece.GetRenderBounds().extents.x;
				
				float currentSize = a_currentPiece.GetRenderBounds().extents.z;
				if(TrafficSystem.Instance.m_swapEditDimensions)
					currentSize   = a_currentPiece.GetRenderBounds().extents.x;
				
				roadPieceSize     = anchorSize + currentSize;
			}
			pos.z = a_attachToPiece.m_renderer.transform.position.z - roadPieceSize;
		}
			break;
		case RoadAttachmentPoint.WEST:
		{
			if(a_attachToPiece.m_renderer && a_currentPiece.m_renderer)
			{
				float anchorSize  = a_attachToPiece.GetRenderBounds().extents.x;
				if(TrafficSystem.Instance.m_swapAnchorDimensions)
					anchorSize    = a_attachToPiece.GetRenderBounds().extents.z;
				
				float currentSize = a_currentPiece.GetRenderBounds().extents.x;
				if(TrafficSystem.Instance.m_swapEditDimensions)
					currentSize   = a_currentPiece.GetRenderBounds().extents.z;
				
				roadPieceSize     = anchorSize + currentSize;
			}
			pos.x = a_attachToPiece.m_renderer.transform.position.x - roadPieceSize;
		}
			break;
		case RoadAttachmentPoint.NORTH:
		{
			if(a_attachToPiece.m_renderer && a_currentPiece.m_renderer)
			{
				float anchorSize  = a_attachToPiece.GetRenderBounds().extents.z;
				if(TrafficSystem.Instance.m_swapAnchorDimensions)
					anchorSize    = a_attachToPiece.GetRenderBounds().extents.x;
				
				float currentSize = a_currentPiece.GetRenderBounds().extents.z;
				if(TrafficSystem.Instance.m_swapEditDimensions)
					currentSize   = a_currentPiece.GetRenderBounds().extents.x;
				
				roadPieceSize     = anchorSize + currentSize;
			}
			pos.z  = a_attachToPiece.m_renderer.transform.position.z + roadPieceSize;
		}
			break;
		}

		Vector3 posOffset = a_currentPiece.m_posOffset;
		if(a_useAnchorOffset)
			posOffset = a_attachToPiece.m_posOffset;

		Vector3 dir = a_attachToPiece.transform.position - pos;

		if(a_attachToPiece.m_renderer)
			dir = a_attachToPiece.m_renderer.transform.position - pos;

		dir = dir.normalized;

		if(a_useEditOffset || a_useAnchorOffset)
		{
//			if(TrafficSystem.Instance.m_swapOffsetDir)
//			{
//				if(TrafficSystem.Instance.m_swapOffsetSize)
//				{
//					posOffset.x = posOffset.z * dir.z; 
//					posOffset.y = posOffset.y * dir.y; 
//					posOffset.z = posOffset.x * dir.x; 
//				}
//				else
//				{
//					posOffset.x = posOffset.x * dir.z; 
//					posOffset.y = posOffset.y * dir.y; 
//					posOffset.z = posOffset.z * dir.x; 
//				}
//			}
//			else
//			{
			if (TrafficSystem.Instance.m_swapOffsetSize)
			{
				float x = posOffset.x;
				posOffset.x = posOffset.z;
				posOffset.z = x;
			}
//			}

			if(TrafficSystem.Instance.m_negateOffsetSize)
			{
				posOffset.x = -posOffset.x; 
				posOffset.y = -posOffset.y; 
				posOffset.z = -posOffset.z; 
			}

			a_currentPiece.transform.position = pos + posOffset;
		}
		else
			a_currentPiece.transform.position = pos;

		UpdateEditTrafficSystemPiecePos();
	}

	void ProcessFolderChecks( bool a_avoidCreation = false )
	{
		bool foundEditNodeFolder = false;
		for(int cIndex = 0; cIndex < TrafficSystem.transform.childCount; cIndex++)
		{
			if(TrafficSystem.transform.GetChild(cIndex).name == m_editNodeFolderName)
			{
				EditNodeFolder      = TrafficSystem.transform.GetChild(cIndex);
				foundEditNodeFolder = true;
				break;
			}
		}
		
		if(!foundEditNodeFolder && !a_avoidCreation)
		{
			EditNodeFolder = new GameObject().transform;
			EditNodeFolder.name = m_editNodeFolderName;
			EditNodeFolder.transform.parent = TrafficSystem.transform;
		}

		bool foundAnchorNodeFolder = false;
		for(int cIndex = 0; cIndex < TrafficSystem.transform.childCount; cIndex++)
		{
			if(TrafficSystem.transform.GetChild(cIndex).name == m_anchorNodeFolderName)
			{
				AnchorNodeFolder      = TrafficSystem.transform.GetChild(cIndex);
				foundAnchorNodeFolder = true;
				break;
			}
		}
		
		if(!foundAnchorNodeFolder && !a_avoidCreation)
		{
			AnchorNodeFolder = new GameObject().transform;
			AnchorNodeFolder.name = m_anchorNodeFolderName;
			AnchorNodeFolder.transform.parent = TrafficSystem.transform;
		}

		bool foundResourceNodeFolder = false;
		for(int cIndex = 0; cIndex < TrafficSystem.transform.childCount; cIndex++)
		{
			if(TrafficSystem.transform.GetChild(cIndex).name == m_resourceNodeFolderName)
			{
				ResourceNodeFolder      = TrafficSystem.transform.GetChild(cIndex);
				foundResourceNodeFolder = true;
				break;
			}
		}
		
		if(!foundResourceNodeFolder && !a_avoidCreation)
		{
			ResourceNodeFolder = new GameObject().transform;
			ResourceNodeFolder.name = m_resourceNodeFolderName;
			ResourceNodeFolder.transform.parent = TrafficSystem.transform;
		}

	}

	void CheckForAnchorNode()
	{
		if(!AnchorNodeFolder)
			return;

		for(int cIndex = 0; cIndex < AnchorNodeFolder.childCount; cIndex++)
		{
			if(AnchorNodeFolder.GetChild(cIndex).GetComponent<TrafficSystemPiece>())
			{
				SetTrafficSystemPiece( TrafficSystem.TrafficSystemTooltip.ANCHOR, AnchorNodeFolder.GetChild(cIndex).GetComponent<TrafficSystemPiece>() );
			}
		}
	}

	public void SetTrafficSystemPiece( TrafficSystemPiece a_piece )
	{
		EditTrafficSystemPiece   = a_piece;
		AnchorTrafficSystemPiece = a_piece;
	}

	void SetTrafficSystemPiece( TrafficSystem.TrafficSystemTooltip a_tooltip, TrafficSystemPiece a_obj )
	{
		switch(a_tooltip)
		{
		case TrafficSystem.TrafficSystemTooltip.ANCHOR:
		{
			AnchorTrafficSystemPiece = a_obj;
			
			if(TrafficSystem.Instance && AnchorTrafficSystemPiece)
			{
				TrafficSystem.Instance.PositionTooltip(a_tooltip, AnchorTrafficSystemPiece);
			}
		}
			break;
		case TrafficSystem.TrafficSystemTooltip.EDIT:
		{
			EditTrafficSystemPiece = a_obj;

			if(TrafficSystem.Instance && EditTrafficSystemPiece)
			{
				TrafficSystem.Instance.PositionTooltip(a_tooltip, EditTrafficSystemPiece);
			}
		}
			break;
		}

	}

	TrafficSystemPiece FindParentTrafficSystemPiece( TrafficSystemPiece a_obj )
	{
		if(a_obj.transform.parent && a_obj.transform.parent.GetComponent<TrafficSystemPiece>())
			a_obj = FindParentTrafficSystemPiece( a_obj.transform.parent.GetComponent<TrafficSystemPiece>() );

		return a_obj;
	}
}
