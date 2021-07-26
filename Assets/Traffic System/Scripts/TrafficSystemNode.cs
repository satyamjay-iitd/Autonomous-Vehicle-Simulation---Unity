using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[System.Serializable]
public class TrafficSystemNode : MonoBehaviour 
{
	public enum NodeID
	{
		ID_0      = 0,
		ID_1      = 1,
		ID_2      = 2,
		ID_3      = 3,
		ID_4      = 4,
		ID_5      = 5,
		ID_6      = 6,
		ID_7      = 7,
		ID_8      = 8,
		ID_9      = 9,
		ID_10     = 10,
		ID_11     = 11,
		ID_12     = 12,
		ID_13     = 13,
		ID_14     = 14,
		ID_15     = 15,
		ID_16     = 16,
		ID_17     = 17,
		ID_18     = 18,
		ID_19     = 19,
		ID_20     = 20,
		ID_21     = 21,
		ID_22     = 22,
		ID_23     = 23,
		ID_24     = 24,
		ID_25     = 25,
		ID_26     = 26,
		ID_27     = 27,
		ID_28     = 28,
		ID_29     = 29,
		ID_30     = 30,
		ID_31     = 31,
		ID_32     = 32,
		ID_33     = 33,
		ID_34     = 34,
		ID_35     = 35,
		ID_36     = 36,
		ID_37     = 37,
		ID_38     = 38,
		ID_39     = 39,
		ID_40     = 40,
		ID_41     = 41,
		ID_42     = 42,
		ID_43     = 43,
		ID_44     = 44,
		ID_45     = 45,
		ID_46     = 46,
		ID_47     = 47,
		ID_48     = 48,
		ID_49     = 49,
		ID_MAX    = 50
	}

	public enum DirectionListing
	{
		NONE                         = 0,
		LEFT                         = 1,
		RIGHT                        = 2,
		STRAIGHT                     = 3,
		LEFT_RIGHT                   = 4,
		LEFT_STRAIGHT                = 5,
		RIGHT_STRAIGHT               = 6,
		LEFT_RIGHT_STRAIGHT          = 7,
		LEFT_RIGHT_STRAIGHT_UTURN    = 8,
		LEFT_STRAIGHT_UTURN          = 9
	}

	public enum RoundaboutExit
	{
		NONE                         = 0,
		EXIT_1                       = 1,
		EXIT_2                       = 2,
		EXIT_3                       = 3,
		EXIT_4                       = 4,
	}

	public  TrafficSystem.RoadType    m_roadType                        = TrafficSystem.RoadType.LANES_2;  // set to normal if you are not sure. The names should explain the others.
	public  bool                      m_isPrimary                       = true;                            // primary nodes are the nodes that the anchor traffic system piece will look to make a connection / link to forming the global path
	public  int                       m_lane                            = 0;                               // this is handy when there is more than 1 lane for an object to follow. This will make sure the object stays in the correct lane, unless random lane changing is greater than 0.0f
	public  bool                      m_onlyConnectWithSameLane         = false;                           // if true, only nodes with the same lane number can connect to this one
	public  TrafficSystem.DriveSide   m_driveSide                       = TrafficSystem.DriveSide.LEFT;    // which side of the road this node belongs too
	public  List<TrafficSystemNode>   m_connectedNodes;                                                    // holds all the possible nodes to traverse too
	public  TrafficSystemNode         m_connectedLocalNode;                                                // holds the node to traverse between a primary and the next primary node
	public  List<TrafficSystemNode>   m_connectedChangeLaneNodes;
	public  bool                      m_isInbound                       = false;                           // only used on CHANGE_LANE nodes
	public  bool                      m_isOutbound                      = false;                           // only used on CHANGE_LANE nodes
	public  NodeID                    m_changeLaneID                    = NodeID.ID_0;
	public  DirectionListing          m_directionListing                = DirectionListing.NONE;           // this is used to indicate if a node has multiple directions that a vehicle can take
	public  RoundaboutExit            m_roundaboutExit                  = RoundaboutExit.NONE;             // this is used to indicate what roundabout exit a change lane node is attached too
	public  TrafficSystemPiece        Parent                            { get; set; }
	private GameObject                m_linkedIndicator                 = null;

	public  bool                      m_debug                           = false;
    public bool isTurn = false;

	void Awake()
	{
		// HACK - START
		// for some reason sometimes m_connectedNodes end up with null values ... have checked every inch of the code and found nothing! ... this removed them on Play so should clean any issues up.
		for(int cIndex = m_connectedNodes.Count - 1; cIndex >= 0; cIndex--)
		{
			if(m_connectedNodes[cIndex] == null)
				m_connectedNodes.RemoveAt(cIndex);
		}
		// HACK - END

		GameObject obj = TrafficSystemGameUtils.FindParentItem( gameObject, TrafficSystemGameUtils.GameObjectItem.TRAFFIC_SYSTEM_PIECE );
		if(obj && obj.GetComponent<TrafficSystemPiece>())
			Parent = obj.GetComponent<TrafficSystemPiece>();

		if(transform.childCount > 0)
			m_linkedIndicator = transform.GetChild(0).gameObject;

		if(Application.isPlaying)
		{
#if UNITY_EDITOR
			ShowRenderers(false);
#else
			ShowRenderers(false);
#endif
		}
	}

	void ShowRenderers( bool a_show )
	{
		if(GetComponent<Renderer>())
			GetComponent<Renderer>().enabled = a_show;

		for(int cIndex = 0; cIndex < transform.childCount; cIndex++)
		{
			if(transform.GetChild(cIndex) && transform.GetChild(cIndex).GetComponent<Renderer>())
				transform.GetChild(cIndex).GetComponent<Renderer>().enabled = a_show;
		}
	}

	public float GetSpeedLimit()
	{
		float speedLimit = TrafficSystem.Instance.m_globalSpeedLimit;

		if(Parent)
			speedLimit = Parent.m_speedLimit;

		return speedLimit;// + Random.Range(0.0f, TrafficSystem.Instance.m_globalSpeedVariation);
	}

	public TrafficSystemNode GetRandomChangeLangeNode()
	{
		if(m_connectedChangeLaneNodes.Count <= 0)
			return null;

		List<int> nodeIndex = new List<int>(m_connectedChangeLaneNodes.Count);
		for(int nIndex = 0; nIndex < m_connectedChangeLaneNodes.Count; nIndex++)
		{
			nodeIndex.Insert(nIndex, nIndex);
		}

		TrafficSystemNode sameLaneNode = null;
		int count = 0;
		while(count < m_connectedChangeLaneNodes.Count)
		{
			int nIndex = Random.Range(0, nodeIndex.Count);
			int lIndex = nodeIndex[nIndex];
			
			TrafficSystemNode node = m_connectedChangeLaneNodes[lIndex];
			
			if(node)
			{
				sameLaneNode = node;
				count = m_connectedChangeLaneNodes.Count;
			}

			nodeIndex.Remove(nIndex);
			count++;
		}

		return sameLaneNode;
	}

	public void AddConnectedNode( TrafficSystemNode a_node )
	{
		bool foundNode = false;
		for(int cIndex = 0; cIndex < m_connectedNodes.Count; cIndex++)
		{
			if(m_connectedNodes[cIndex] == a_node)
				foundNode = true;
		}

		if(!foundNode && a_node)
			m_connectedNodes.Add(a_node);
	}
	
	public void RemoveConnectedNode( TrafficSystemNode a_node )
	{
		m_connectedNodes.Remove(a_node);
	}

	public void AddChangeLaneNode( TrafficSystemNode a_node )
	{
		bool foundNode = false;
		for(int cIndex = 0; cIndex < m_connectedChangeLaneNodes.Count; cIndex++)
		{
			if(m_connectedChangeLaneNodes[cIndex] == a_node)
				foundNode = true;
		}
		
		if(!foundNode && a_node)
			m_connectedChangeLaneNodes.Add(a_node);
	}
	
	public void RemoveChangeLangeNode( TrafficSystemNode a_node )
	{
		m_connectedChangeLaneNodes.Remove(a_node);
	}
    public void Setlayer()
    {
        gameObject.layer = 8;

    }

    public List<TrafficSystemNode> GetChildren() {
		List<TrafficSystemNode> all_child = new List<TrafficSystemNode>();

        if (m_connectedLocalNode)
        {
            all_child.Add(m_connectedLocalNode);
            return all_child;
        }

        if (m_connectedChangeLaneNodes.Count != 0)
        {
            for (int i = 0; i < m_connectedChangeLaneNodes.Count; i++)
                all_child.Add(m_connectedChangeLaneNodes[i]);
        }

        if (m_connectedNodes.Count != 0)
        {
            for (int i = 0; i < m_connectedNodes.Count; i++)
            {
                all_child.Add(m_connectedNodes[i]);
            }
        }

        return all_child;
    }

	public TrafficSystemNode GetNextNode( TrafficSystemVehicle a_vehicle, bool a_checkLocalConnectedNode = true, List<TrafficSystemNode> a_blockedNodes = null )
	{
		if(m_connectedNodes.Count <= 0 && !m_connectedLocalNode && m_connectedChangeLaneNodes.Count <= 0)
			return null;

		if(a_checkLocalConnectedNode && m_connectedLocalNode)
			return m_connectedLocalNode;

		float randomChanceUseOfframp  = Random.Range(0.0f, 1.0f);
		float randomChanceOfDirChange = Random.Range(0.0f, 1.0f);

		if(m_connectedChangeLaneNodes.Count > 0)
		{
			if(randomChanceOfDirChange <= a_vehicle.m_chanceOfDirChange)
			{
				TrafficSystemNode node = GetRandomChangeLangeNode();

				if(node)
					return node;
			}
			else if(m_connectedNodes.Count <= 0 && !m_connectedLocalNode)
			{
				TrafficSystemNode node = GetRandomChangeLangeNode();
				
				if(node)
					return node;
			}
		}

		TrafficSystemNode offrampNode = null;
		for(int nIndex = 0; nIndex < m_connectedNodes.Count; nIndex++)
		{
			TrafficSystemNode node = m_connectedNodes[nIndex];
			if(node && node.m_roadType == TrafficSystem.RoadType.OFFRAMP)
			{
				offrampNode = node;
				break;
			}
		}
 
		if(a_vehicle.m_randomLaneChange > 0.0f && m_connectedNodes.Count > 1)
		{
			float randomChance = Random.Range(0.0f, 1.0f);

			if(randomChance <= a_vehicle.m_randomLaneChange)
			{
				List<int> nodeIndex = new List<int>(m_connectedNodes.Count);
				for(int nIndex = 0; nIndex < m_connectedNodes.Count; nIndex++)
				{
					nodeIndex.Insert(nIndex, nIndex);
				}

				TrafficSystemNode sameLaneNode = null;
				int count = 0;
				while(count < m_connectedNodes.Count)
				{
					int nIndex = Random.Range(0, nodeIndex.Count);
					int lIndex = nodeIndex[nIndex];

					TrafficSystemNode node = m_connectedNodes[lIndex];

					if(node)
					{
						bool blockNode = false;
						if(a_blockedNodes != null)
						{
							for(int cIndex = 0; cIndex < a_blockedNodes.Count; cIndex++)
							{
								if(node == a_blockedNodes[cIndex])
								{
									blockNode = true;
									break;
								}
							}
						}

						if(!blockNode)
						{
							if(node.m_lane != m_lane || (node.m_lane == m_lane && node.m_roadType == TrafficSystem.RoadType.OFFRAMP) )
								return node;
							else
								sameLaneNode = node;
						}
					}

					nodeIndex.Remove(nIndex);
					count++;
				}

				if(sameLaneNode)
					return sameLaneNode;
			}
			else
			{
				for(int nIndex = 0; nIndex < m_connectedNodes.Count; nIndex++)
				{
					TrafficSystemNode node = m_connectedNodes[nIndex];

					if(node && node.m_lane == m_lane)
					{
						bool blockNode = false;
						if(a_blockedNodes != null)
						{
							for(int cIndex = 0; cIndex < a_blockedNodes.Count; cIndex++)
							{
								if(node == a_blockedNodes[cIndex])
								{
									blockNode = true;
									break;
								}
							}
						}
						
						if(!blockNode)
						{
							if(randomChanceUseOfframp <= a_vehicle.m_chanceOfUsingOfframp)
							{
								if(offrampNode)
									return offrampNode;
							}
							else if(node.IsNormalRoad())
								return node;
						}
					}
				}
			}
		}

		TrafficSystemNode nextNodeInOurLane    = null;
		TrafficSystemNode nextNodeNotInOurLane = null;
		for(int nIndex = 0; nIndex < m_connectedNodes.Count; nIndex++)
		{
			TrafficSystemNode node = m_connectedNodes[nIndex];
			
			if(node)
			{
				bool blockNode = false;
				if(a_blockedNodes != null && m_connectedNodes.Count > 1)
				{
					for(int cIndex = 0; cIndex < a_blockedNodes.Count; cIndex++)
					{
						if(node == a_blockedNodes[cIndex])
						{
							blockNode = true;
							break;
						}
					}
				}
				
				if(!blockNode)
				{

					if(node.m_lane == m_lane)
					{
						if(randomChanceUseOfframp <= a_vehicle.m_chanceOfUsingOfframp)
						{
							if(offrampNode)
								return offrampNode;
							else if(node.IsNormalRoad())
								return node;
							else
								nextNodeInOurLane = node;
						}
						else if(node.IsNormalRoad())
							return node;
						else
							nextNodeNotInOurLane = node;
					}
					else
						nextNodeNotInOurLane = node;
				}
			}
		}

		if(nextNodeInOurLane)
			return nextNodeInOurLane;
		else
			return nextNodeNotInOurLane;
	}

	public bool IsRoundabout()
	{
		if(!Parent)
			return false;

		if(Parent.transform.parent && Parent.transform.parent.GetComponent<TrafficSystemPiece>())
		{
			if(Parent.transform.parent.GetComponent<TrafficSystemPiece>().m_roadPieceType == TrafficSystemPiece.RoadPieceType.ROUNDABOUT)
				return true;
		}

		if(Parent.m_roadPieceType == TrafficSystemPiece.RoadPieceType.ROUNDABOUT)
			return true;

		return false;
	}

	public bool IsHighwayChangeLaneAccepted()
	{
		if(m_isPrimary && IsConnected() && !m_connectedLocalNode)
			return true;

		return false;
	}

	public bool IsConnected()
	{
		if(m_connectedNodes.Count > 0)
			return true;

		return false;
	}

	public bool IsDirectionChange()
	{
		if(m_directionListing != DirectionListing.NONE &&
		   m_directionListing != DirectionListing.STRAIGHT)
			return true;

		return false;
	}

	public bool IsDirectionStraightOnly()
	{
		if(m_directionListing == DirectionListing.NONE ||
		   m_directionListing == DirectionListing.STRAIGHT)
			return true;

		return false;
	}
	
	public bool IsDirectionStraight()
	{
		if(m_directionListing == DirectionListing.LEFT_RIGHT_STRAIGHT ||
		   m_directionListing == DirectionListing.LEFT_RIGHT_STRAIGHT_UTURN ||
		   m_directionListing == DirectionListing.LEFT_STRAIGHT ||
		   m_directionListing == DirectionListing.LEFT_STRAIGHT_UTURN ||
		   m_directionListing == DirectionListing.RIGHT_STRAIGHT ||
		   m_directionListing == DirectionListing.STRAIGHT)
			return true;
		
		return false;
	}

	public bool IsDirectionLeft()
	{
		if(m_directionListing == DirectionListing.LEFT_RIGHT_STRAIGHT ||
		   m_directionListing == DirectionListing.LEFT_RIGHT_STRAIGHT_UTURN ||
		   m_directionListing == DirectionListing.LEFT_STRAIGHT ||
		   m_directionListing == DirectionListing.LEFT_STRAIGHT_UTURN ||
		   m_directionListing == DirectionListing.LEFT ||
		   m_directionListing == DirectionListing.LEFT_RIGHT)
			return true;
		
		return false;
	}

	public bool IsDirectionRight()
	{
		if(m_directionListing == DirectionListing.LEFT_RIGHT ||
		   m_directionListing == DirectionListing.LEFT_RIGHT_STRAIGHT ||
		   m_directionListing == DirectionListing.LEFT_RIGHT_STRAIGHT_UTURN ||
		   m_directionListing == DirectionListing.RIGHT ||
		   m_directionListing == DirectionListing.RIGHT_STRAIGHT)
			return true;
		
		return false;
	}

	public bool IsDirectionUTurn()
	{
		if(m_directionListing == DirectionListing.LEFT_STRAIGHT_UTURN ||
		   m_directionListing == DirectionListing.LEFT_RIGHT_STRAIGHT_UTURN)
			return true;
		
		return false;
	}

	public bool IsHighway()
	{
		if(m_roadType == TrafficSystem.RoadType.LANES_MULTI)
			return true;

		return false;
	}
	
	public TrafficSystemNode GetNextNodeHighway()
	{
		for(int cIndex = 0; cIndex < m_connectedNodes.Count; cIndex++)
		{
			TrafficSystemNode node = m_connectedNodes[cIndex]; 

			if(node && node.IsHighway() && node.m_lane > m_lane)
				return node;
		}

		return null;
	}

	public bool IsNormalRoad()
	{
		if(m_roadType == TrafficSystem.RoadType.LANES_1 ||
		   m_roadType == TrafficSystem.RoadType.LANES_2 ||
		   m_roadType == TrafficSystem.RoadType.LANES_MULTI)
		{
			return true;
		}

		return false;
	}

	void OnDrawGizmos()
	{
		#if !UNITY_EDITOR
		return;
		#endif

		if(TrafficSystem.Instance && !TrafficSystem.Instance.m_showGizmos)
			return;

		if(m_connectedNodes.Count > 0 && m_linkedIndicator)
			m_linkedIndicator.SetActive(true);
		else if(m_linkedIndicator)
			m_linkedIndicator.SetActive(false);

		float scaleFactorCube   = 0.15f;
		float scaleFactorSphere = 0.225f;
		if(!m_connectedLocalNode)
		{
			for(int nIndex = 0; nIndex < m_connectedNodes.Count; nIndex++)
			{
				TrafficSystemNode connectedNode = m_connectedNodes[nIndex];
				if(connectedNode)
				{
					Vector3 offset = new Vector3(0.0f, 0.1f, 0.0f);
					Gizmos.color = Color.white;
					Gizmos.DrawLine( transform.position + offset, connectedNode.transform.position + offset );

					Vector3 dir = transform.position - connectedNode.transform.position;
//					Gizmos.color = Color.white;
//					Gizmos.DrawCube( (transform.position - (dir.normalized * ((dir.magnitude / 2) + scaleFactorSphere))) + offset, new Vector3(scaleFactorCube * 1.4f, scaleFactorCube * 1.4f, scaleFactorCube * 1.4f) );
					Gizmos.color = Color.yellow;
					Gizmos.DrawCube( (transform.position - (dir.normalized * ((dir.magnitude / 2) + scaleFactorSphere))) + offset, new Vector3(scaleFactorCube, scaleFactorCube, scaleFactorCube) );
					Gizmos.color = Color.white;
					Gizmos.DrawSphere( (transform.position - (dir.normalized * (dir.magnitude / 2))) + offset, scaleFactorSphere );
				}
			}
		}

		if(m_connectedLocalNode)
		{
			Vector3 offset = new Vector3(0.0f, 0.1f, 0.0f);
			Gizmos.color = Color.white;
			Gizmos.DrawLine( transform.position + offset, m_connectedLocalNode.transform.position + offset );

			Vector3 dir = transform.position - m_connectedLocalNode.transform.position;
//			Gizmos.color = Color.white;
//			Gizmos.DrawCube( (transform.position - (dir.normalized * ((dir.magnitude / 2) + scaleFactorSphere))) + offset, new Vector3(scaleFactorCube * 1.4f, scaleFactorCube * 1.4f, scaleFactorCube * 1.4f) );
			Gizmos.color = Color.yellow;
			Gizmos.DrawCube( (transform.position - (dir.normalized * ((dir.magnitude / 2) + scaleFactorSphere))) + offset, new Vector3(scaleFactorCube, scaleFactorCube, scaleFactorCube) );
			Gizmos.color = Color.white;
			Gizmos.DrawSphere( (transform.position - (dir.normalized * (dir.magnitude / 2))) + offset, scaleFactorSphere );
		}

		for(int cIndex = 0; cIndex < m_connectedChangeLaneNodes.Count; cIndex++)
		{
			if(m_connectedChangeLaneNodes[cIndex])// && m_connectedChangeLaneNodes[cIndex].m_connectedNodes.Count > 0)
			{
				Vector3 offset = new Vector3(0.0f, 0.1f, 0.0f);
				Gizmos.color = Color.white;
				Gizmos.DrawLine( transform.position + offset, m_connectedChangeLaneNodes[cIndex].transform.position + offset );
				
				Vector3 dir = transform.position - m_connectedChangeLaneNodes[cIndex].transform.position;
//				Gizmos.color = Color.white;
//				Gizmos.DrawCube( (transform.position - (dir.normalized * ((dir.magnitude / 2) + scaleFactorSphere))) + offset, new Vector3(scaleFactorCube * 1.4f, scaleFactorCube * 1.4f, scaleFactorCube * 1.4f) );
				Gizmos.color = Color.yellow;
				Gizmos.DrawCube( (transform.position - (dir.normalized * ((dir.magnitude / 2) + scaleFactorSphere))) + offset, new Vector3(scaleFactorCube, scaleFactorCube, scaleFactorCube) );
				Gizmos.color = Color.white;
				Gizmos.DrawSphere( (transform.position - (dir.normalized * (dir.magnitude / 2))) + offset, scaleFactorSphere );
			}
		}
	}
}
