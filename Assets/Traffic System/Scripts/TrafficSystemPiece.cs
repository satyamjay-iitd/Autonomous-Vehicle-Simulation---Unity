// only enable this if you have purchased the "City Generator" package from the Unity Asset Store and installed it into your project.
// #define CITY_GENERATOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TrafficSystemPiece : MonoBehaviour
{
	public enum RoadPieceType
	{
		NORMAL             = 0,
		T_INTERSECTION     = 1,
		CROSS_INTERSECTION = 2,
		ROUNDABOUT         = 3
	}

	public  bool                               m_ignoreTrafficSystemEditor    = false;
	public  bool                               m_ignoreChildren               = false;
	public  RoadPieceType                      m_roadPieceType                = RoadPieceType.NORMAL; 
	[Range(-1, 20)]
	public  int                                m_speedLimit                   = -1;
	public  List<TrafficSystemNode>            m_primaryLeftLaneNodes         = new List<TrafficSystemNode>();
	public  List<TrafficSystemNode>            m_secondaryLeftLaneNodes       = new List<TrafficSystemNode>();       
	public  List<TrafficSystemNode>            m_leftLaneChangeNodes          = new List<TrafficSystemNode>();       
	public  List<TrafficSystemNode>            m_primaryRightLaneNodes        = new List<TrafficSystemNode>();
	public  List<TrafficSystemNode>            m_secondaryRightLaneNodes      = new List<TrafficSystemNode>();       
	public  List<TrafficSystemNode>            m_rightLaneChangeNodes         = new List<TrafficSystemNode>();       
	
	//	[HideInInspector]public List<TrafficSystemLane>            m_leftLanes                    = new List<TrafficSystemLane>();
	//	[HideInInspector]public List<TrafficSystemLane>            m_rightLanes                   = new List<TrafficSystemLane>();
	
	public  Renderer                           m_renderer                     = null;
	//	public  List<TrafficSystemAttachmentPoint> m_attachmentPoints     = new List<TrafficSystemAttachmentPoint>();
	//	private int                                m_attachmentPointIndex = 0;

	public Vector3                             m_posOffset                    = Vector3.zero;

	public  GameObject[]                       m_roadModelsOriginal            = new GameObject[0];
	public  GameObject[]                       m_roadModelsVersion2            = new GameObject[0];
	public  GameObject[]                       m_roadModelsVersion2Mobile      = new GameObject[0];
	public  GameObject[]                       m_footpaths                     = new GameObject[0];



	#if CITY_GENERATOR
	public Vector3 m_roadClearZoneOffset = new Vector3(0.0f, 0.0f, 0.0f);
	#endif
	
	void Awake()
	{
		Refresh();

		if(Application.isPlaying && m_speedLimit == -1)
			m_speedLimit = TrafficSystem.Instance.m_globalSpeedLimit;
	}
	
	public void Refresh()
	{
		if(m_ignoreChildren)
			return;

		if(!m_renderer)
		{
			GameObject obj = TrafficSystemGameUtils.FindItem( gameObject, TrafficSystemGameUtils.GameObjectItem.RENDERER );
			if(obj && obj.GetComponent<Renderer>())
				m_renderer = obj.GetComponent<Renderer>();
		}
		
		//		FindAllAttachmentPoints();
	}
	
	void FindAllNodes()
	{
		m_primaryLeftLaneNodes   .Clear();
		m_secondaryLeftLaneNodes .Clear();
		m_primaryRightLaneNodes  .Clear();
		m_secondaryRightLaneNodes.Clear();
		m_leftLaneChangeNodes    .Clear();
		m_rightLaneChangeNodes   .Clear();
		//		m_leftLanes              .Clear();
		//		m_rightLanes             .Clear();
		
		FindAllNodesRecursive(this, gameObject);
		
		
	}
	
	void FindAllNodesRecursive(TrafficSystemPiece a_trafficSystemPiece, GameObject a_obj)
	{
		if(a_obj)
		{
			if(a_obj.GetComponent<TrafficSystemNode>())
			{
				TrafficSystemNode node = a_obj.GetComponent<TrafficSystemNode>();
				if(node)
				{
					if(node.m_driveSide == TrafficSystem.DriveSide.LEFT)
					{
						if(node.m_roadType == TrafficSystem.RoadType.CHANGE_LANE)
							a_trafficSystemPiece.m_leftLaneChangeNodes.Add(node);
						else if(node.m_isPrimary)
							a_trafficSystemPiece.m_primaryLeftLaneNodes.Add(node);
						else
						{
							a_trafficSystemPiece.m_secondaryLeftLaneNodes.Add(node);
						}
					}
					else
					{
						if(node.m_roadType == TrafficSystem.RoadType.CHANGE_LANE)
							a_trafficSystemPiece.m_rightLaneChangeNodes.Add(node);
						else if(node.m_isPrimary)
							a_trafficSystemPiece.m_primaryRightLaneNodes.Add(node);
						else
						{
							a_trafficSystemPiece.m_secondaryRightLaneNodes.Add(node);
						}
					}
				}
			}
			
			for(int cIndex = 0; cIndex < a_obj.transform.childCount; cIndex++)
			{
				Transform child = a_obj.transform.GetChild(cIndex);
				if(child.gameObject)
					FindAllNodesRecursive(a_trafficSystemPiece, child.gameObject);
			}
		}
	}

	public void CleanUp( TrafficSystemPiece a_piece )
	{
		if(a_piece)
		{
			a_piece.PerformCleanUp();

			for(int cIndex = 0; cIndex < a_piece.transform.childCount; cIndex++)
			{
				if(a_piece.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>())
				{
					CleanUp( a_piece.transform.GetChild(cIndex).GetComponent<TrafficSystemPiece>() );
				}
			}
		}
	}

	public void PerformCleanUp()
	{
		List<TrafficSystemNode> cleanList = new List<TrafficSystemNode>();
		
		for(int lIndex = 0; lIndex < m_primaryLeftLaneNodes.Count; lIndex++)
		{
			for(int cIndex = 0; cIndex < m_primaryLeftLaneNodes[lIndex].m_connectedNodes.Count; cIndex++)
			{
				TrafficSystemNode node = m_primaryLeftLaneNodes[lIndex].m_connectedNodes[cIndex];
				
				if(node)
				{
					cleanList.Add(node);
				}
			}
			
			m_primaryLeftLaneNodes[lIndex].m_connectedNodes.Clear();
			
			for(int cIndex = 0; cIndex < cleanList.Count; cIndex++)
			{
				TrafficSystemNode node = cleanList[cIndex];
				
				if(node)
				{
					bool found = false;
					for(int nIndex = 0; nIndex < m_primaryLeftLaneNodes[lIndex].m_connectedNodes.Count; nIndex++)
					{
						TrafficSystemNode checkNode = m_primaryLeftLaneNodes[lIndex].m_connectedNodes[nIndex];
						
						if(node == checkNode)
						{
							found = true;
							break;
						}
					}
					
					if(!found && node)
						m_primaryLeftLaneNodes[lIndex].m_connectedNodes.Add(node);
				}
			}
			
			cleanList.Clear();
		}
		
		cleanList.Clear();
		
		for(int lIndex = 0; lIndex < m_primaryLeftLaneNodes.Count; lIndex++)
		{
			for(int cIndex = 0; cIndex < m_primaryLeftLaneNodes[lIndex].m_connectedChangeLaneNodes.Count; cIndex++)
			{
				TrafficSystemNode node = m_primaryLeftLaneNodes[lIndex].m_connectedChangeLaneNodes[cIndex];
				
				if(node)
				{
					cleanList.Add(node);
				}
			}
			
			m_primaryLeftLaneNodes[lIndex].m_connectedChangeLaneNodes.Clear();
			
			for(int cIndex = 0; cIndex < cleanList.Count; cIndex++)
			{
				TrafficSystemNode node = cleanList[cIndex];
				
				if(node)
				{
					bool found = false;
					for(int nIndex = 0; nIndex < m_primaryLeftLaneNodes[lIndex].m_connectedChangeLaneNodes.Count; nIndex++)
					{
						TrafficSystemNode checkNode = m_primaryLeftLaneNodes[lIndex].m_connectedChangeLaneNodes[nIndex];
						
						if(node == checkNode)
						{
							found = true;
							break;
						}
					}
					
					if(!found && node)
						m_primaryLeftLaneNodes[lIndex].m_connectedChangeLaneNodes.Add(node);
				}
			}
			
			cleanList.Clear();
		}
		
		cleanList.Clear();
		
		for(int lIndex = 0; lIndex < m_primaryRightLaneNodes.Count; lIndex++)
		{
			for(int cIndex = 0; cIndex < m_primaryRightLaneNodes[lIndex].m_connectedNodes.Count; cIndex++)
			{
				TrafficSystemNode node = m_primaryRightLaneNodes[lIndex].m_connectedNodes[cIndex];
				
				if(node)
				{
					cleanList.Add(node);
				}
			}
			
			m_primaryRightLaneNodes[lIndex].m_connectedNodes.Clear();
			
			for(int cIndex = 0; cIndex < cleanList.Count; cIndex++)
			{
				TrafficSystemNode node = cleanList[cIndex];
				
				if(node)
				{
					bool found = false;
					for(int nIndex = 0; nIndex < m_primaryRightLaneNodes[lIndex].m_connectedNodes.Count; nIndex++)
					{
						TrafficSystemNode checkNode = m_primaryRightLaneNodes[lIndex].m_connectedNodes[nIndex];
						
						if(node == checkNode)
						{
							found = true;
							break;
						}
					}
					
					if(!found && node)
						m_primaryRightLaneNodes[lIndex].m_connectedNodes.Add(node);
				}
			}
			
			cleanList.Clear();
		}
		
		cleanList.Clear();
		
		for(int lIndex = 0; lIndex < m_primaryRightLaneNodes.Count; lIndex++)
		{
			for(int cIndex = 0; cIndex < m_primaryRightLaneNodes[lIndex].m_connectedChangeLaneNodes.Count; cIndex++)
			{
				TrafficSystemNode node = m_primaryRightLaneNodes[lIndex].m_connectedChangeLaneNodes[cIndex];
				
				if(node)
				{
					cleanList.Add(node);
				}
			}
			
			m_primaryRightLaneNodes[lIndex].m_connectedChangeLaneNodes.Clear();
			
			for(int cIndex = 0; cIndex < cleanList.Count; cIndex++)
			{
				TrafficSystemNode node = cleanList[cIndex];
				
				if(node)
				{
					bool found = false;
					for(int nIndex = 0; nIndex < m_primaryRightLaneNodes[lIndex].m_connectedChangeLaneNodes.Count; nIndex++)
					{
						TrafficSystemNode checkNode = m_primaryRightLaneNodes[lIndex].m_connectedChangeLaneNodes[nIndex];
						
						if(node == checkNode)
						{
							found = true;
							break;
						}
					}
					
					if(!found && node)
						m_primaryRightLaneNodes[lIndex].m_connectedChangeLaneNodes.Add(node);
				}
			}
			
			cleanList.Clear();
		}
		
		cleanList.Clear();
		
		for(int lIndex = 0; lIndex < m_secondaryLeftLaneNodes.Count; lIndex++)
		{
			for(int cIndex = 0; cIndex < m_secondaryLeftLaneNodes[lIndex].m_connectedNodes.Count; cIndex++)
			{
				TrafficSystemNode node = m_secondaryLeftLaneNodes[lIndex].m_connectedNodes[cIndex];
				
				if(node)
				{
					cleanList.Add(node);
				}
			}
			
			m_secondaryLeftLaneNodes[lIndex].m_connectedNodes.Clear();
			
			for(int cIndex = 0; cIndex < cleanList.Count; cIndex++)
			{
				TrafficSystemNode node = cleanList[cIndex];
				
				if(node)
				{
					bool found = false;
					for(int nIndex = 0; nIndex < m_secondaryLeftLaneNodes[lIndex].m_connectedNodes.Count; nIndex++)
					{
						TrafficSystemNode checkNode = m_secondaryLeftLaneNodes[lIndex].m_connectedNodes[nIndex];
						
						if(node == checkNode)
						{
							found = true;
							break;
						}
					}
					
					if(!found && node)
						m_secondaryLeftLaneNodes[lIndex].m_connectedNodes.Add(node);
				}
			}
			
			cleanList.Clear();
		}
		
		cleanList.Clear();
		
		for(int lIndex = 0; lIndex < m_secondaryLeftLaneNodes.Count; lIndex++)
		{
			for(int cIndex = 0; cIndex < m_secondaryLeftLaneNodes[lIndex].m_connectedChangeLaneNodes.Count; cIndex++)
			{
				TrafficSystemNode node = m_secondaryLeftLaneNodes[lIndex].m_connectedChangeLaneNodes[cIndex];
				
				if(node)
				{
					cleanList.Add(node);
				}
			}
			
			m_secondaryLeftLaneNodes[lIndex].m_connectedChangeLaneNodes.Clear();
			
			for(int cIndex = 0; cIndex < cleanList.Count; cIndex++)
			{
				TrafficSystemNode node = cleanList[cIndex];
				
				if(node)
				{
					bool found = false;
					for(int nIndex = 0; nIndex < m_secondaryLeftLaneNodes[lIndex].m_connectedChangeLaneNodes.Count; nIndex++)
					{
						TrafficSystemNode checkNode = m_secondaryLeftLaneNodes[lIndex].m_connectedChangeLaneNodes[nIndex];
						
						if(node == checkNode)
						{
							found = true;
							break;
						}
					}
					
					if(!found && node)
						m_secondaryLeftLaneNodes[lIndex].m_connectedChangeLaneNodes.Add(node);
				}
			}
			
			cleanList.Clear();
		}
		
		cleanList.Clear();
		
		for(int lIndex = 0; lIndex < m_secondaryRightLaneNodes.Count; lIndex++)
		{
			for(int cIndex = 0; cIndex < m_secondaryRightLaneNodes[lIndex].m_connectedNodes.Count; cIndex++)
			{
				TrafficSystemNode node = m_secondaryRightLaneNodes[lIndex].m_connectedNodes[cIndex];
				
				if(node)
				{
					cleanList.Add(node);
				}
			}
			
			m_secondaryRightLaneNodes[lIndex].m_connectedNodes.Clear();
			
			for(int cIndex = 0; cIndex < cleanList.Count; cIndex++)
			{
				TrafficSystemNode node = cleanList[cIndex];
				
				if(node)
				{
					bool found = false;
					for(int nIndex = 0; nIndex < m_secondaryRightLaneNodes[lIndex].m_connectedNodes.Count; nIndex++)
					{
						TrafficSystemNode checkNode = m_secondaryRightLaneNodes[lIndex].m_connectedNodes[nIndex];
						
						if(node == checkNode)
						{
							found = true;
							break;
						}
					}
					
					if(!found && node)
						m_secondaryRightLaneNodes[lIndex].m_connectedNodes.Add(node);
				}
			}
			
			cleanList.Clear();
		}
		
		cleanList.Clear();
		
		for(int lIndex = 0; lIndex < m_secondaryRightLaneNodes.Count; lIndex++)
		{
			for(int cIndex = 0; cIndex < m_secondaryRightLaneNodes[lIndex].m_connectedChangeLaneNodes.Count; cIndex++)
			{
				TrafficSystemNode node = m_secondaryRightLaneNodes[lIndex].m_connectedChangeLaneNodes[cIndex];
				
				if(node)
				{
					cleanList.Add(node);
				}
			}
			
			m_secondaryRightLaneNodes[lIndex].m_connectedChangeLaneNodes.Clear();
			
			for(int cIndex = 0; cIndex < cleanList.Count; cIndex++)
			{
				TrafficSystemNode node = cleanList[cIndex];
				
				if(node)
				{
					bool found = false;
					for(int nIndex = 0; nIndex < m_secondaryRightLaneNodes[lIndex].m_connectedChangeLaneNodes.Count; nIndex++)
					{
						TrafficSystemNode checkNode = m_secondaryRightLaneNodes[lIndex].m_connectedChangeLaneNodes[nIndex];
						
						if(node == checkNode)
						{
							found = true;
							break;
						}
					}
					
					if(!found && node)
						m_secondaryRightLaneNodes[lIndex].m_connectedChangeLaneNodes.Add(node);
				}
			}
			
			cleanList.Clear();
		}
		
		cleanList.Clear();
		
		for(int lIndex = 0; lIndex < m_leftLaneChangeNodes.Count; lIndex++)
		{
			for(int cIndex = 0; cIndex < m_leftLaneChangeNodes[lIndex].m_connectedNodes.Count; cIndex++)
			{
				TrafficSystemNode node = m_leftLaneChangeNodes[lIndex].m_connectedNodes[cIndex];
				
				if(node)
				{
					cleanList.Add(node);
				}
			}
			
			m_leftLaneChangeNodes[lIndex].m_connectedNodes.Clear();
			
			for(int cIndex = 0; cIndex < cleanList.Count; cIndex++)
			{
				TrafficSystemNode node = cleanList[cIndex];
				
				if(node)
				{
					bool found = false;
					for(int nIndex = 0; nIndex < m_leftLaneChangeNodes[lIndex].m_connectedNodes.Count; nIndex++)
					{
						TrafficSystemNode checkNode = m_leftLaneChangeNodes[lIndex].m_connectedNodes[nIndex];
						
						if(node == checkNode)
						{
							found = true;
							break;
						}
					}
					
					if(!found && node)
						m_leftLaneChangeNodes[lIndex].m_connectedNodes.Add(node);
				}
			}
			
			cleanList.Clear();
		}
		
		cleanList.Clear();
		
		for(int lIndex = 0; lIndex < m_leftLaneChangeNodes.Count; lIndex++)
		{
			for(int cIndex = 0; cIndex < m_leftLaneChangeNodes[lIndex].m_connectedChangeLaneNodes.Count; cIndex++)
			{
				TrafficSystemNode node = m_leftLaneChangeNodes[lIndex].m_connectedChangeLaneNodes[cIndex];
				
				if(node)
				{
					cleanList.Add(node);
				}
			}
			
			m_leftLaneChangeNodes[lIndex].m_connectedChangeLaneNodes.Clear();
			
			for(int cIndex = 0; cIndex < cleanList.Count; cIndex++)
			{
				TrafficSystemNode node = cleanList[cIndex];
				
				if(node)
				{
					bool found = false;
					for(int nIndex = 0; nIndex < m_leftLaneChangeNodes[lIndex].m_connectedChangeLaneNodes.Count; nIndex++)
					{
						TrafficSystemNode checkNode = m_leftLaneChangeNodes[lIndex].m_connectedChangeLaneNodes[nIndex];
						
						if(node == checkNode)
						{
							found = true;
							break;
						}
					}
					
					if(!found && node)
						m_leftLaneChangeNodes[lIndex].m_connectedChangeLaneNodes.Add(node);
				}
			}
			
			cleanList.Clear();
		}
		
		cleanList.Clear();
		
		for(int lIndex = 0; lIndex < m_rightLaneChangeNodes.Count; lIndex++)
		{
			for(int cIndex = 0; cIndex < m_rightLaneChangeNodes[lIndex].m_connectedNodes.Count; cIndex++)
			{
				TrafficSystemNode node = m_rightLaneChangeNodes[lIndex].m_connectedNodes[cIndex];
				
				if(node)
				{
					cleanList.Add(node);
				}
			}
			
			m_rightLaneChangeNodes[lIndex].m_connectedNodes.Clear();
			
			for(int cIndex = 0; cIndex < cleanList.Count; cIndex++)
			{
				TrafficSystemNode node = cleanList[cIndex];
				
				if(node)
				{
					bool found = false;
					for(int nIndex = 0; nIndex < m_rightLaneChangeNodes[lIndex].m_connectedNodes.Count; nIndex++)
					{
						TrafficSystemNode checkNode = m_rightLaneChangeNodes[lIndex].m_connectedNodes[nIndex];
						
						if(node == checkNode)
						{
							found = true;
							break;
						}
					}
					
					if(!found && node)
						m_rightLaneChangeNodes[lIndex].m_connectedNodes.Add(node);
				}
			}
			
			cleanList.Clear();
		}
		
		cleanList.Clear();
		
		for(int lIndex = 0; lIndex < m_rightLaneChangeNodes.Count; lIndex++)
		{
			for(int cIndex = 0; cIndex < m_rightLaneChangeNodes[lIndex].m_connectedChangeLaneNodes.Count; cIndex++)
			{
				TrafficSystemNode node = m_rightLaneChangeNodes[lIndex].m_connectedChangeLaneNodes[cIndex];
				
				if(node)
				{
					cleanList.Add(node);
				}
			}
			
			m_rightLaneChangeNodes[lIndex].m_connectedChangeLaneNodes.Clear();
			
			for(int cIndex = 0; cIndex < cleanList.Count; cIndex++)
			{
				TrafficSystemNode node = cleanList[cIndex];
				
				if(node)
				{
					bool found = false;
					for(int nIndex = 0; nIndex < m_rightLaneChangeNodes[lIndex].m_connectedChangeLaneNodes.Count; nIndex++)
					{
						TrafficSystemNode checkNode = m_rightLaneChangeNodes[lIndex].m_connectedChangeLaneNodes[nIndex];
						
						if(node == checkNode)
						{
							found = true;
							break;
						}
					}
					
					if(!found && node)
						m_rightLaneChangeNodes[lIndex].m_connectedChangeLaneNodes.Add(node);
				}
			}
			
			cleanList.Clear();
		}

		cleanList.Clear();
	}
	
	public Bounds GetRenderBounds()
	{
		if(!m_renderer)
			return new Bounds( transform.position, new Vector3(1.0f, 1.0f, 1.0f) );
		Bounds bounds = m_renderer.bounds;

		#if CITY_GENERATOR
		Vector3 size  = bounds.size;
		size.x       += m_roadClearZoneOffset.x;
		size.y       += m_roadClearZoneOffset.y;
		size.z       += m_roadClearZoneOffset.z;
		bounds.size   = size;
		#endif

		return bounds;
	}

	public void SpawnRandomVehicle( TrafficSystemVehicle a_vehiclePrefab )
	{
		if(!TrafficSystem.Instance)
			return;

		TrafficSystemNode node = null;

		int randomLane = Random.Range(0, 4);

		switch(randomLane)
		{
		case 0:
		{
			for(int nIndex = 0; nIndex < m_primaryLeftLaneNodes.Count; nIndex++)
			{
				if(m_primaryLeftLaneNodes[nIndex])
				{
					node = m_primaryLeftLaneNodes[nIndex];
					break;
				}
			}
			
			if(!node)
			{
				for(int nIndex = 0; nIndex < m_primaryRightLaneNodes.Count; nIndex++)
				{
					if(m_primaryRightLaneNodes[nIndex])
					{
						node = m_primaryRightLaneNodes[nIndex];
						break;
					}
				}
			}
			
			if(!node)
			{
				for(int nIndex = 0; nIndex < m_secondaryLeftLaneNodes.Count; nIndex++)
				{
					if(m_secondaryLeftLaneNodes[nIndex])
					{
						node = m_secondaryLeftLaneNodes[nIndex];
						break;
					}
				}
			}
			
			if(!node)
			{
				for(int nIndex = 0; nIndex < m_secondaryRightLaneNodes.Count; nIndex++)
				{
					if(m_secondaryRightLaneNodes[nIndex])
					{
						node = m_secondaryRightLaneNodes[nIndex];
						break;
					}
				}
			}
		}
			break;
		case 1:
		{
			for(int nIndex = 0; nIndex < m_primaryRightLaneNodes.Count; nIndex++)
			{
				if(m_primaryRightLaneNodes[nIndex])
				{
					node = m_primaryRightLaneNodes[nIndex];
					break;
				}
			}
			
			if(!node)
			{
				for(int nIndex = 0; nIndex < m_primaryLeftLaneNodes.Count; nIndex++)
				{
					if(m_primaryLeftLaneNodes[nIndex])
					{
						node = m_primaryLeftLaneNodes[nIndex];
						break;
					}
				}
			}
			
			if(!node)
			{
				for(int nIndex = 0; nIndex < m_secondaryLeftLaneNodes.Count; nIndex++)
				{
					if(m_secondaryLeftLaneNodes[nIndex])
					{
						node = m_secondaryLeftLaneNodes[nIndex];
						break;
					}
				}
			}
			
			if(!node)
			{
				for(int nIndex = 0; nIndex < m_secondaryRightLaneNodes.Count; nIndex++)
				{
					if(m_secondaryRightLaneNodes[nIndex])
					{
						node = m_secondaryRightLaneNodes[nIndex];
						break;
					}
				}
			}
		}
			break;
		case 2:
		{
			for(int nIndex = 0; nIndex < m_primaryLeftLaneNodes.Count; nIndex++)
			{
				if(m_primaryLeftLaneNodes[nIndex])
				{
					node = m_primaryLeftLaneNodes[nIndex];
					break;
				}
			}
			
			if(!node)
			{
				for(int nIndex = 0; nIndex < m_secondaryLeftLaneNodes.Count; nIndex++)
				{
					if(m_primaryRightLaneNodes[nIndex])
					{
						node = m_primaryRightLaneNodes[nIndex];
						break;
					}
				}
			}
			
			if(!node)
			{
				for(int nIndex = 0; nIndex < m_secondaryRightLaneNodes.Count; nIndex++)
				{
					if(m_secondaryRightLaneNodes[nIndex])
					{
						node = m_secondaryRightLaneNodes[nIndex];
						break;
					}
				}
			}
			
			if(!node)
			{
				for(int nIndex = 0; nIndex < m_secondaryLeftLaneNodes.Count; nIndex++)
				{
					if(m_secondaryLeftLaneNodes[nIndex])
					{
						node = m_secondaryLeftLaneNodes[nIndex];
						break;
					}
				}
			}
		}
			break;
		case 3:
		{
			for(int nIndex = 0; nIndex < m_primaryRightLaneNodes.Count; nIndex++)
			{
				if(m_primaryRightLaneNodes[nIndex])
				{
					node = m_primaryRightLaneNodes[nIndex];
					break;
				}
			}
			
			if(!node)
			{
				for(int nIndex = 0; nIndex < m_primaryLeftLaneNodes.Count; nIndex++)
				{
					if(m_primaryLeftLaneNodes[nIndex])
					{
						node = m_primaryLeftLaneNodes[nIndex];
						break;
					}
				}
			}
			
			if(!node)
			{
				for(int nIndex = 0; nIndex < m_secondaryRightLaneNodes.Count; nIndex++)
				{
					if(m_secondaryRightLaneNodes[nIndex])
					{
						node = m_secondaryRightLaneNodes[nIndex];
						break;
					}
				}
			}
			
			if(!node)
			{
				for(int nIndex = 0; nIndex < m_secondaryLeftLaneNodes.Count; nIndex++)
				{
					if(m_secondaryLeftLaneNodes[nIndex])
					{
						node = m_secondaryLeftLaneNodes[nIndex];
						break;
					}
				}
			}
		}
			break;
		}

		if(node)
		{
			Vector3 pos = node.transform.position;
			pos -= a_vehiclePrefab.m_offsetPosVal;

			TrafficSystemVehicle vehicle = Instantiate( a_vehiclePrefab, pos, node.transform.rotation ) as TrafficSystemVehicle;
			vehicle.m_nextNode           = node;
			vehicle.m_velocityMax        = Random.Range(TrafficSystem.Instance.m_randVehicleVelocityMin, TrafficSystem.Instance.m_randVehicleVelocityMax);

			TrafficSystemNode nextNode = node.GetNextNode( vehicle, false );
			if(nextNode)
				vehicle.transform.forward = nextNode.transform.position - vehicle.transform.position;
			
		}
	}

	void OnDrawGizmosSelected() 
	{
		#if !UNITY_EDITOR
		return;
		#endif

		if(!m_renderer)
			return;

		#if CITY_GENERATOR
		if(CityGenerator.Instance)
		{
			Gizmos.color = new Color(0, 0, 1, 0.5F);
			Vector3 pos  = transform.position;
			
			Bounds bounds = GetRenderBounds();
			Vector3 size  = bounds.size;
			size.x       += CityGenerator.Instance.m_roadClearZoneOffset.x;
			size.y       += CityGenerator.Instance.m_roadClearZoneOffset.y;
			size.z       += CityGenerator.Instance.m_roadClearZoneOffset.z;
			
			Gizmos.DrawCube( pos, size );
		}
		#endif

		if(TrafficSystem.Instance)
		{
			float speedLimit = m_speedLimit;
			if(speedLimit == -1)
				speedLimit = TrafficSystem.Instance.m_globalSpeedLimit;

			float ratio    = Mathf.Clamp(speedLimit / 20.0f, 0f, 1f);
			Gizmos.color   = new Color(ratio, 1f - ratio, 0f, 0.2F);
			
			Vector3 pos    = transform.position;
			
			Bounds bounds  = GetRenderBounds();
			Vector3 size   = bounds.size;
			size.x         = size.x * 0.5f;
			size.y        += 1.0f;
			size.z         = size.z * 0.5f;
			Gizmos.DrawCube( pos, size );
		}
	}

	public void ProcessRoadQuality( TrafficSystem.RoadQuality a_roadQuality )
	{
		switch(a_roadQuality)
		{
		case TrafficSystem.RoadQuality.ORIGINAL:
		{
			if(m_roadModelsOriginal.Length > 0)
			{
				ShowOriginalRoadModels(true);
				ShowVersion2RoadModels(false);
				ShowVersion2MobileRoadModels(false);
			}
			else
			{
				ShowVersion2RoadModels(false);
				ShowVersion2MobileRoadModels(true);
			}
		}
			break;
		case TrafficSystem.RoadQuality.VERSION_2:
		{
			if(m_roadModelsVersion2.Length > 0)
			{
				ShowOriginalRoadModels(false);
				ShowVersion2RoadModels(true);
				ShowVersion2MobileRoadModels(false);
			}
			else
			{
				ShowVersion2RoadModels(false);
				ShowVersion2MobileRoadModels(true);
			}
		}
			break;
		case TrafficSystem.RoadQuality.VERSION_2_MOBILE:
		{
			if(m_roadModelsVersion2Mobile.Length > 0)
			{
				ShowOriginalRoadModels(false);
				ShowVersion2RoadModels(false);
				ShowVersion2MobileRoadModels(true);
			}
			else
			{
				ShowVersion2RoadModels(true);
				ShowVersion2MobileRoadModels(false);
			}
		}
			break;
		}
	}

	public void ShowOriginalRoadModels( bool a_show )
	{
		bool isCustomRenderer = IsRendererCustom();
		for(int pIndex = 0; pIndex < m_roadModelsOriginal.Length; pIndex++)
		{
			m_roadModelsOriginal[pIndex].SetActive(a_show);

			if(a_show && m_roadModelsOriginal[pIndex].GetComponent<Renderer>() && !isCustomRenderer)
				m_renderer = m_roadModelsOriginal[pIndex].GetComponent<Renderer>();
		}
	}

	public void ShowVersion2RoadModels( bool a_show )
	{
		bool isCustomRenderer = IsRendererCustom();
		for(int pIndex = 0; pIndex < m_roadModelsVersion2.Length; pIndex++)
		{
			m_roadModelsVersion2[pIndex].SetActive(a_show);
			
			if(a_show && m_roadModelsVersion2[pIndex].GetComponent<Renderer>() && !isCustomRenderer)
				m_renderer = m_roadModelsVersion2[pIndex].GetComponent<Renderer>();
		}
	}
		
	public void ShowVersion2MobileRoadModels( bool a_show )
	{
		bool isCustomRenderer = IsRendererCustom();
		for(int pIndex = 0; pIndex < m_roadModelsVersion2Mobile.Length; pIndex++)
		{
			m_roadModelsVersion2Mobile[pIndex].SetActive(a_show);
			
			if(a_show && m_roadModelsVersion2Mobile[pIndex].GetComponent<Renderer>() && !isCustomRenderer)
				m_renderer = m_roadModelsVersion2Mobile[pIndex].GetComponent<Renderer>();
		}
	}

	public bool IsRendererCustom()
	{
		bool found = true;

		for(int pIndex = 0; pIndex < m_roadModelsOriginal.Length; pIndex++)
		{
			if(m_roadModelsOriginal[pIndex].GetComponent<Renderer>() && m_roadModelsOriginal[pIndex].GetComponent<Renderer>() == m_renderer)
			{
				found = false;
				break;
			}
		}

		if(!found)
		{
			for(int pIndex = 0; pIndex < m_roadModelsVersion2.Length; pIndex++)
			{
				if(m_roadModelsVersion2[pIndex].GetComponent<Renderer>() && m_roadModelsVersion2[pIndex].GetComponent<Renderer>() == m_renderer)
				{
					found = false;
					break;
				}
			}

			if(!found)
			{
				for(int pIndex = 0; pIndex < m_roadModelsVersion2Mobile.Length; pIndex++)
				{
					if(m_roadModelsVersion2Mobile[pIndex].GetComponent<Renderer>() && m_roadModelsVersion2Mobile[pIndex].GetComponent<Renderer>() == m_renderer)
					{
						found = false;
						break;
					}
				}
			}
		}

		return found;
	}
	
	public void ProcessFootpaths()
	{
		if(m_footpaths != null && m_footpaths.Length > 0)
		{
			bool show = !m_footpaths[0].activeSelf;
			
			for(int fIndex = 0; fIndex < m_footpaths.Length; fIndex++)
				m_footpaths[fIndex].SetActive(show);
		}
	}
}
