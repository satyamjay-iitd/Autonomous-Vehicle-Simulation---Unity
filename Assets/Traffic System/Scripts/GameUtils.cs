using UnityEngine;
using System.Collections;

static public class TrafficSystemGameUtils 
{
	public enum GameObjectItem
	{
		RIGIDBODY                   = 0,
		RENDERER                    = 1,
		COLLIDER                    = 2,
		NAV_MESH_AGENT              = 3,
		ANIMATOR                    = 4,
		TRAFFIC_SYSTEM_PIECE        = 5,
		TRAFFIC_SYSTEM_INTERSECTION = 6
	}

	static public GameObject FindItem(GameObject a_obj, GameObjectItem a_item)
	{
		GameObject returnObj = null;
		FindItemRecursive( a_obj, ref returnObj, a_item );
		return returnObj;
	}

	static GameObject FindItemRecursive(GameObject a_obj, ref GameObject a_returnObj, GameObjectItem a_item)
	{
		if(a_returnObj)
			return a_returnObj;

		if(a_obj)
		{
			switch(a_item)
			{
			case TrafficSystemGameUtils.GameObjectItem.RIGIDBODY:
			{
				if(a_obj.GetComponent<Rigidbody>())
					a_returnObj = a_obj;
			}
				break;
			case TrafficSystemGameUtils.GameObjectItem.RENDERER:
			{
				if(a_obj.GetComponent<Renderer>())
					a_returnObj = a_obj;
			}
				break;
			case TrafficSystemGameUtils.GameObjectItem.COLLIDER:
			{
				if(a_obj.GetComponent<Collider>())
					a_returnObj = a_obj;
			}
				break;
			case TrafficSystemGameUtils.GameObjectItem.NAV_MESH_AGENT:
			{
				if(a_obj.GetComponent<UnityEngine.AI.NavMeshAgent>())
					a_returnObj = a_obj;
			}
				break;
			case TrafficSystemGameUtils.GameObjectItem.ANIMATOR:
			{
				if(a_obj.GetComponent<Animator>())
					a_returnObj = a_obj;
			}
				break;
			case TrafficSystemGameUtils.GameObjectItem.TRAFFIC_SYSTEM_PIECE:
			{
				if(a_obj.GetComponent<TrafficSystemPiece>())
					a_returnObj = a_obj;
			}
				break;
			}
			
			if(!a_returnObj)
			{
				for(int cIndex = 0; cIndex < a_obj.transform.childCount; cIndex++)
				{
					Transform child = a_obj.transform.GetChild(cIndex);
					if(child.gameObject && !a_returnObj)
						a_returnObj = FindItemRecursive(child.gameObject, ref a_returnObj, a_item);
				}
			}
		}

		return a_returnObj;
	}

	static public GameObject FindParentItem(GameObject a_obj, GameObjectItem a_item)
	{
		GameObject returnObj = null;
		FindParentItemRecursive( a_obj, ref returnObj, a_item );
		return returnObj;
	}
	
	static GameObject FindParentItemRecursive(GameObject a_obj, ref GameObject a_returnObj, GameObjectItem a_item)
	{
		if(a_returnObj)
			return a_returnObj;
		
		if(a_obj)
		{
			switch(a_item)
			{
			case TrafficSystemGameUtils.GameObjectItem.RIGIDBODY:
			{
				if(a_obj.GetComponent<Rigidbody>())
					a_returnObj = a_obj;
			}
				break;
			case TrafficSystemGameUtils.GameObjectItem.RENDERER:
			{
				if(a_obj.GetComponent<Renderer>())
					a_returnObj = a_obj;
			}
				break;
			case TrafficSystemGameUtils.GameObjectItem.COLLIDER:
			{
				if(a_obj.GetComponent<Collider>())
					a_returnObj = a_obj;
			}
				break;
			case TrafficSystemGameUtils.GameObjectItem.NAV_MESH_AGENT:
			{
				if(a_obj.GetComponent<UnityEngine.AI.NavMeshAgent>())
					a_returnObj = a_obj;
			}
				break;
			case TrafficSystemGameUtils.GameObjectItem.ANIMATOR:
			{
				if(a_obj.GetComponent<Animator>())
					a_returnObj = a_obj;
			}
				break;
			case TrafficSystemGameUtils.GameObjectItem.TRAFFIC_SYSTEM_PIECE:
			{
				if(a_obj.GetComponent<TrafficSystemPiece>())
					a_returnObj = a_obj;
			}
				break;
			case TrafficSystemGameUtils.GameObjectItem.TRAFFIC_SYSTEM_INTERSECTION:
			{
				if(a_obj.GetComponent<TrafficSystemIntersection>())
					a_returnObj = a_obj;
			}
				break;
			}
			
			if(!a_returnObj)
			{
				if(a_obj.transform.parent)
					a_returnObj = FindParentItemRecursive(a_obj.transform.parent.gameObject, ref a_returnObj, a_item);
			}
		}
		
		return a_returnObj;
	}
}
