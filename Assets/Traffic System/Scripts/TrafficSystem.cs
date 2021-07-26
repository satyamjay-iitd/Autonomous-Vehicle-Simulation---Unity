using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[ExecuteInEditMode]
public class TrafficSystem : MonoBehaviour 
{
	public  static TrafficSystem Instance                    { get; set; }

	// BEGIN - Values used in Editor
	public enum TrafficSystemTooltip
	{
		ANCHOR    = 0,
		EDIT      = 1
	}

	public enum RoadQuality
	{
		ORIGINAL         = 0,
		VERSION_2        = 1,
		VERSION_2_MOBILE = 2,
		MAX              = 3
	}

	public  bool                m_showGizmos                 = true;
	public  Texture2D           TextureIconAnchor            = null;
	public  Texture2D           TextureIconEdit              = null;
	public  Texture2D           TextureIconAnchorRevealSmall = null;
	public  Texture2D           TextureIconEditRevealSmall   = null;
	public  Texture2D           TextureIconAnchorSmall       = null;
	public  Texture2D           TextureIconEditSmall         = null;
	public  Texture2D           TextureIconRoadTypeAll       = null;
	public  Texture2D           TextureIconRoadType0Lanes    = null;
	public  Texture2D           TextureIconRoadType1Lane     = null;
	public  Texture2D           TextureIconRoadType2Lanes    = null;
	public  Texture2D           TextureIconRoadType4Lanes    = null;
	public  Texture2D           TextureIconRoadType6Lanes    = null;

	private RoadQuality         m_roadQualityVal             = RoadQuality.VERSION_2_MOBILE;
	public  RoadQuality         GetRoadQualityVal()          { return m_roadQualityVal; }
	public  void                SetRoadQualityVal( RoadQuality a_quality )          { m_roadQualityVal = a_quality; }
	public  int                 RoadQualityElementsIndex     { get; set; }
	public  float               m_roadScale                  = 1.0f;

	public  TrafficSystemPiece  AnchorTrafficSystemPiece     { get; set; }
	public  TrafficSystemPiece  EditTrafficSystemPiece       { get; set; }
	public  GameObject          TooltipAnchor                { get; set; }
	public  GameObject          TooltipEdit                  { get; set; }
	public  bool                m_autoLinkOnSpawn            = true;
	public  bool                m_quickSpawn                 = false;
	public  bool                m_autoReverseAnchorToEdit    = false;
	private List<Transform>     CLRevealObjectsFrom          = new List<Transform>();
	private List<Transform>     CLRevealObjectsTo            = new List<Transform>();

	public  int                 m_vehicleSpawnCountMax       = -1;                                              // if -1 then unlimited vehicles can spawn. If higher than only this amount will ever spawn using the Traffic System spawn options
	public  bool                m_randomVehicleSpawnPerNode  = false; 
	[Range(0, 1)]
	public  float               m_randomVehicleSpawnChancePerNode = 0.0f;
	public  float               m_randVehicleVelocityMin     = 1.0f;
	public  float               m_randVehicleVelocityMax     = 5.0f;

	public  int                 m_globalSpeedLimit           = 5;                                               // used for when individual Road Piece have a speed limit of -1 (which is the default), they will use this global limit.
	[Range(0.0f, 5.0f)]
	public  float               m_globalSpeedVariation       = 1.5f;                                            // used to generate a slight variation of speed each node a vehicle gets to

	[Range(0.0f, 0.4f)]
	public  float               m_globalLanePosVariation     = 0.0f;                                            // used to generate a slight variation of lane position for all vehicles
	
	public  List<TrafficSystemVehicle>        m_vehiclePrefabs      = new List<TrafficSystemVehicle>();
	private List<TrafficSystemVehicleSpawner> m_vehicleSpawners     = new List<TrafficSystemVehicleSpawner>();
	public List<TrafficSystemVehicle>        m_spawnedVehicles     = new List<TrafficSystemVehicle>();
	public  List<TrafficSystemVehicle>        GetSpawnedVehicles()  { return m_spawnedVehicles; }

	[HideInInspector]
	public bool          m_swapAnchorDimensions              = false;
	[HideInInspector]
	public bool          m_swapEditDimensions                = false;
	[HideInInspector]
	public bool          m_swapOffsetDir                     = false;
	[HideInInspector]
	public bool          m_swapOffsetSize                    = false;
	[HideInInspector]
	public bool          m_negateOffsetSize                  = false;

	public  TrafficSystemFollowCamera        m_followCameraScript = null;

	// END

	public enum DriveSide
	{
		RIGHT = 0,
		LEFT  = 1
	}

	public enum RoadType
	{
		LANES_0            = 0,
		LANES_1            = 1,
		LANES_2            = 2,
		LANES_MULTI        = 3,
		OFFRAMP            = 4,
		CHANGE_LANE        = 5
	}

	public  bool                m_spawnWithRoadQuality       = true;

	void Awake()
	{
		if(Instance != null)
		{
			Destroy(this);
			return;
		}

		Instance = this;
        AddTagRecursively(transform, "Terrain");


    }
    void AddTagRecursively(Transform trans, string tag)
    {
        trans.gameObject.tag = tag;
        if (trans.childCount > 0)
            foreach (Transform t in trans)
                AddTagRecursively(t, tag);
    }

    void Start () 
	{
		if(Instance != this)
			return;

		if(m_randomVehicleSpawnPerNode && Application.isPlaying)
		{
			TrafficSystemPiece[] roadPieces = GameObject.FindObjectsOfType<TrafficSystemPiece>();

			for(int rIndex = 0; rIndex < roadPieces.Length; rIndex++)
			{
				float rand = Random.Range(0.0f, 1.0f);
//				Debug.Log("rand (" + rand + ") <= m_randomVehicleSpawnChancePerNode (" + m_randomVehicleSpawnChancePerNode + ")");
				if(rand <= m_randomVehicleSpawnChancePerNode && CanSpawn())
				{
					int randVehicleIndex = Random.Range(0, m_vehiclePrefabs.Count);
					roadPieces[rIndex].SpawnRandomVehicle( m_vehiclePrefabs[randVehicleIndex] );
				}
			}
		}

		if(!m_followCameraScript)
			if(transform.GetComponent<TrafficSystemFollowCamera>())
				m_followCameraScript = transform.GetComponent<TrafficSystemFollowCamera>();
	}

	public void SetTrafficSystemPiece( TrafficSystemTooltip a_tooltip, TrafficSystemPiece a_obj )
	{
		switch(a_tooltip)
		{
		case TrafficSystemTooltip.ANCHOR:
		{
			AnchorTrafficSystemPiece = a_obj;
			if(AnchorTrafficSystemPiece)
			{
				PositionTooltip(TrafficSystem.TrafficSystemTooltip.ANCHOR, AnchorTrafficSystemPiece);
			}
		}
			break;
		case TrafficSystemTooltip.EDIT:
		{
			EditTrafficSystemPiece = a_obj;
			if(EditTrafficSystemPiece)
			{
				PositionTooltip(TrafficSystem.TrafficSystemTooltip.EDIT, EditTrafficSystemPiece);
			}
		}
			break;
		}
	}

	public void ShowTooltip( TrafficSystemTooltip a_tooltip, bool a_show )
	{
		switch(a_tooltip)
		{
		case TrafficSystemTooltip.ANCHOR:
		{
			if(TooltipAnchor)
			{
				TooltipAnchor.SetActive( a_show );
			}
		}
			break;
		case TrafficSystemTooltip.EDIT:
		{
			if(TooltipEdit)
			{
				TooltipEdit.SetActive( a_show );
			}
		}
			break;
		}
	}

	public void PositionTooltip( TrafficSystemTooltip a_tooltip, TrafficSystemPiece a_obj )
	{
		switch(a_tooltip)
		{
		case TrafficSystemTooltip.ANCHOR:
		{
			if(TooltipAnchor)
			{
				TooltipAnchor.transform.position = new Vector3(a_obj.transform.position.x, a_obj.transform.position.y + a_obj.m_renderer.bounds.extents.y + 2.0f, a_obj.transform.position.z);
			}
		}
			break;
		case TrafficSystemTooltip.EDIT:
		{
			if(TooltipEdit)
			{
				TooltipEdit.transform.position = new Vector3(a_obj.transform.position.x, a_obj.transform.position.y + a_obj.m_renderer.bounds.extents.y + 2.4f, a_obj.transform.position.z);
			}
		}
			break;
		}
	}

	public void RegisterVehicle( TrafficSystemVehicle a_vehicle )
	{
		if(TrafficSystemUI.Instance)
			TrafficSystemUI.Instance.AssignVehicleToFollow( a_vehicle );

		m_spawnedVehicles.Add( a_vehicle );
	}

	public void UnRegisterVehicle( TrafficSystemVehicle a_vehicle )
	{
		m_spawnedVehicles.Remove( a_vehicle );
		RespawnVehicle();
	}

	public void RegisterVehicleSpawner( TrafficSystemVehicleSpawner a_spawner )
	{
		m_vehicleSpawners.Add( a_spawner );
	}

	public void RespawnVehicle()
	{
		if(m_vehicleSpawners.Count <= 0)
			return;

		TrafficSystemVehicleSpawner spawners = m_vehicleSpawners[Random.Range(0, m_vehicleSpawners.Count)];
		spawners.RespawnVehicle();
	}

	public void SetAllVehicleFrontLights( bool a_enabled )
	{
		for(int vIndex = 0; vIndex < m_spawnedVehicles.Count; vIndex++)
			m_spawnedVehicles[vIndex].FrontLights( a_enabled );
	}

	public void AddToCLRevealObjsFrom( Transform a_obj )
	{
		bool foundObj = false;
//		for(int rIndex = 0; rIndex < CLRevealObjectsFrom.Count; rIndex++)
//		{
//			if(CLRevealObjectsFrom[rIndex] == a_obj)
//			{
//				foundObj = true;
//				break;
//			}
//		}

		if(!foundObj)
			CLRevealObjectsFrom.Add(a_obj);
	}

	public void ClearCLRevealObjsFrom()
	{
		CLRevealObjectsFrom.Clear();
	}

	public void AddToCLRevealObjsTo( Transform a_obj )
	{
		bool foundObj = false;
//		for(int rIndex = 0; rIndex < CLRevealObjectsTo.Count; rIndex++)
//		{
//			if(CLRevealObjectsTo[rIndex] == a_obj)
//			{
//				foundObj = true;
//				break;
//			}
//		}
		
		if(!foundObj)
			CLRevealObjectsTo.Add(a_obj);
	}
	
	public void ClearCLRevealObjsTo()
	{
		CLRevealObjectsTo.Clear();
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.L))
			SetAllVehicleFrontLights( true );
		else if(Input.GetKeyDown(KeyCode.K))
			SetAllVehicleFrontLights( false );
	}

	void OnDrawGizmos()
	{
		#if !UNITY_EDITOR
		return;
		#endif

		if(CLRevealObjectsFrom.Count > 0 && CLRevealObjectsFrom.Count == CLRevealObjectsTo.Count)
		{
			float scaleFactorCube   = 0.25f;
			float scaleFactorSphere = 0.35f;

			for(int rIndex = 0; rIndex < CLRevealObjectsFrom.Count; rIndex++)
			{
				if(CLRevealObjectsFrom[rIndex] == null)
					continue;
				
				if(CLRevealObjectsTo[rIndex] == null)
					continue;

				Vector3 offset = new Vector3(0.0f, 0.225f, 0.0f);
				Gizmos.color = Color.red;
				Gizmos.DrawLine( CLRevealObjectsFrom[rIndex].position + offset, CLRevealObjectsTo[rIndex].position + offset );
				
				Vector3 dir = CLRevealObjectsFrom[rIndex].position - CLRevealObjectsTo[rIndex].position;
				Gizmos.color = Color.yellow;
				Gizmos.DrawCube( (CLRevealObjectsFrom[rIndex].position - (dir.normalized * ((dir.magnitude / 2) + scaleFactorSphere))) + offset, new Vector3(scaleFactorCube, scaleFactorCube, scaleFactorCube) );
				Gizmos.color = Color.red;
				Gizmos.DrawSphere( (CLRevealObjectsFrom[rIndex].position - (dir.normalized * (dir.magnitude / 2))) + offset, scaleFactorSphere );

				Gizmos.color = Color.red;
				Gizmos.DrawSphere( CLRevealObjectsFrom[rIndex].position + offset, scaleFactorSphere );
				Gizmos.DrawSphere( CLRevealObjectsTo[rIndex].position + offset, scaleFactorSphere );
			}
		}
	}

	public bool VehicleHasFocus( TrafficSystemVehicle a_vehicle ) 
	{
		if(m_followCameraScript)
		{
			if(m_followCameraScript.m_vehicleToFollow == a_vehicle)
				return true;
		}

		return false;
	}

	public bool CanSpawn()
	{
		if(m_vehicleSpawnCountMax <= -1)
			return true;

		if(m_spawnedVehicles.Count < m_vehicleSpawnCountMax)
			return true;

		return false;
	}

//	public TrafficSystemVehicle GetVehiclePrefabToSpawn()
//	{
//		if(m_vehicleToSpawn.Count <= 0)
//			return null;
//
//		return m_vehicleToSpawn[Random.Range(0, m_vehicleToSpawn.Count)];
//	}
//
//	public bool ShouldSpawnVehicle()
//	{
//		if(Random.Range(0.0f, 1.0f) <= m_chanceToSpawnOnNode)
//			return true;
//
//		return false;
//	}
}
