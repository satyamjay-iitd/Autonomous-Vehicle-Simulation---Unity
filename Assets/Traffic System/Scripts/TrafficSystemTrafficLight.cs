using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficSystemTrafficLight : MonoBehaviour 
{
	public enum Status
	{
		GREEN  = 0,
		YELLOW = 1,
		RED    = 2
	}

	public  Status           m_status         = Status.GREEN;
	public  float            m_greenDuration  = 5.0f;
	public  Transform        m_lightRed       = null;
	public  Transform        m_lightYellow    = null;
	public  Transform        m_lightGreen     = null;
	public  Transform        m_lightRedArrow    = null;
	public  Transform        m_lightYellowArrow = null;
	public  Transform        m_lightGreenArrow  = null;
	public  TrafficSystemIntersection m_intersection = null;
	private float            m_timeSinceGreen = 0.0f;
	public  bool             m_turnLeftAnytime = false;                          // if this is enabled, whenever a vehicle gets to this traffic light it will turn left straight away. It doesn't wait for a green arrow. The default should be false but there are a few special cases like the ends of T-Intersections that need it true! 
//	public  bool               m_enableTurnChecks                 = false;
//	public  float              m_timeToWaitBetweenCheckes         = 1.0f;        // in seconds, the time we check to see if we can move again.
//	public  float              m_checkRadius                      = 5.0f;        // the size of the spherecast for checking vehicle detection.
//	public  Transform          m_checkPos;                                       // the position of the spherecast for checking vehicle detection.
//	private bool               m_checkStarted                     = false;

//	public  List<TrafficSystemVehicle> m_vehiclesStoppedAtLight = new List<TrafficSystemVehicle>();

	void Awake()
	{
		if(!m_intersection)
		{
			GameObject obj = TrafficSystemGameUtils.FindParentItem( gameObject, TrafficSystemGameUtils.GameObjectItem.TRAFFIC_SYSTEM_INTERSECTION );
			if(obj && obj.GetComponent<TrafficSystemIntersection>())
				m_intersection = obj.GetComponent<TrafficSystemIntersection>();
		}

		if(m_lightRedArrow)
			m_lightRedArrow.gameObject   .SetActive(true);
		if(m_lightYellowArrow)
			m_lightYellowArrow.gameObject.SetActive(false);
		if(m_lightGreenArrow)
			m_lightGreenArrow.gameObject .SetActive(false);
	}

	void Update()
	{
		if(m_status == Status.GREEN)
		{
			m_timeSinceGreen += Time.deltaTime;
		}
		else
		{
			m_timeSinceGreen = 0.0f;
		}
	}

	public void SetStatus( Status a_status, bool a_useLightArrows = false )
	{
		m_status = a_status;

		if(m_lightRed && m_lightYellow && m_lightGreen)
		{
			switch(m_status)
			{
			case Status.GREEN:
			{
				m_lightRed.gameObject   .SetActive(false);
				m_lightYellow.gameObject.SetActive(false);
				m_lightGreen.gameObject .SetActive(true);

				if(a_useLightArrows)
				{
					if(m_lightRedArrow)
						m_lightRedArrow.gameObject   .SetActive(false);
					if(m_lightYellowArrow)
						m_lightYellowArrow.gameObject.SetActive(false);
					if(m_lightGreenArrow)
						m_lightGreenArrow.gameObject .SetActive(true);
				}

//				m_lightRed.material   .SetColor( "_Color", Color.black );
//				m_lightYellow.material.SetColor( "_Color", Color.black );
//				m_lightGreen.material .SetColor( "_Color", Color.green );

//				List<TrafficSystemVehicle> vehiclesStoppedAtLight = new List<TrafficSystemVehicle>();
//				for(int vIndex = 0; vIndex < m_vehiclesStoppedAtLight.Count; vIndex++)
//				{
//					TrafficSystemVehicle vehicle = m_vehiclesStoppedAtLight[vIndex];
//					if(vehicle.CanFitAcrossIntersection())
//					{
//						vehicle.StopMoving   = false;
//						vehicle.TrafficLight = null;
//						vehiclesStoppedAtLight.Add(vehicle);
//					}
//				}
//
//				for(int vIndex = 0; vIndex < vehiclesStoppedAtLight.Count; vIndex++)
//				{
//					TrafficSystemVehicle vehicle = vehiclesStoppedAtLight[vIndex];
//
//					for(int vIndex2 = 0; vIndex2 < m_vehiclesStoppedAtLight.Count; vIndex2++)
//					{
//						TrafficSystemVehicle vehicle2 = m_vehiclesStoppedAtLight[vIndex2];
//						if(vehicle == vehicle2)
//						{
//							m_vehiclesStoppedAtLight.RemoveAt(vIndex2);
//							break;
//						}
//					}
//				}
//
//				vehiclesStoppedAtLight.Clear();
			}
				break;
			case Status.YELLOW:
			{
				m_lightRed.gameObject   .SetActive(false);
				m_lightYellow.gameObject.SetActive(true);
				m_lightGreen.gameObject .SetActive(false);

				if(a_useLightArrows)
				{
					if(m_lightRedArrow)
						m_lightRedArrow.gameObject   .SetActive(false);
					if(m_lightYellowArrow)
						m_lightYellowArrow.gameObject.SetActive(true);
					if(m_lightGreenArrow)
						m_lightGreenArrow.gameObject .SetActive(false);
				}

//				m_lightRed.material   .SetColor( "_Color", Color.black );
//				m_lightYellow.material.SetColor( "_Color", Color.yellow );
//				m_lightGreen.material .SetColor( "_Color", Color.black );
			}
				break;
			case Status.RED:
			{
				m_lightRed.gameObject   .SetActive(true);
				m_lightYellow.gameObject.SetActive(false);
				m_lightGreen.gameObject .SetActive(false);

				if(a_useLightArrows)
				{
					if(m_lightRedArrow)
						m_lightRedArrow.gameObject   .SetActive(true);
					if(m_lightYellowArrow)
						m_lightYellowArrow.gameObject.SetActive(false);
					if(m_lightGreenArrow)
						m_lightGreenArrow.gameObject .SetActive(false);
				}

//				m_lightRed.material   .SetColor( "_Color", Color.red );
//				m_lightYellow.material.SetColor( "_Color", Color.black );
//				m_lightGreen.material .SetColor( "_Color", Color.black );
			}
				break;
			}
		}
	}

	void OnTriggerEnter( Collider a_obj )
	{
		TrafficSystemVehicle vehicle = null;

		if(a_obj.transform.GetComponent<TrafficSystemVehicle>())
			vehicle = a_obj.transform.GetComponent<TrafficSystemVehicle>();

		if(vehicle)
		{
//			if(m_status == Status.RED)
//			{
//				vehicle.TrafficLight         = this;
//			}
//			else
//			{
//				if(vehicle.CanSeeVehicle( true ))
			vehicle.AssignTrafficLight(this);

			if(vehicle.IsTurningIntoIncomingTraffic() && !m_turnLeftAnytime)
				m_intersection.AddToPriorityLightQueue( this );

//			if(!m_checkStarted && m_turnLeftAnytime)
//			{
//				m_checkStarted = true;
//				StartCoroutine( ProcessCheck( vehicle ) );
//			}
//			}
		}

		if(a_obj.transform.GetComponent<TrafficSystemVehiclePlayer>())
		{
			TrafficSystemVehiclePlayer playerVehicle = null;
			playerVehicle = a_obj.transform.GetComponent<TrafficSystemVehiclePlayer>();

			if(playerVehicle)
			{
//				playerVehicle.AssignTrafficLight(this);
				
				if(playerVehicle.IsTurningIntoIncomingTraffic() && !m_turnLeftAnytime)
					m_intersection.AddToPriorityLightQueue( this );

				playerVehicle.ProcessHasEnteredTrafficLightTrigger( this );
			}

//			if(!m_checkStarted && m_turnLeftAnytime)
//			{
//				m_checkStarted = true;
//				StartCoroutine( ProcessCheck( vehicle ) );
//			}
		}
	}
	
	public bool IgnoreCanFitAcrossIntersectionCheck()
	{
		if(m_intersection && m_intersection.m_ignoreCanFitAcrossIntersectionCheck)
			return true;

		return false;
	}

//	IEnumerator ProcessCheck( TrafficSystemVehicle a_vehicle )
//	{
//		if(!m_checkPos)
//			yield break;
//
//		bool stillWaiting = true;
//		while(stillWaiting)
//		{
//			Collider[] hitColliders = Physics.OverlapSphere(m_checkPos.position, m_checkRadius);
//			stillWaiting = false;
//			int i = 0;
//			while ( i < hitColliders.Length ) 
//			{
//				if(hitColliders[i].gameObject.GetComponent<TrafficSystemVehicle>())
//				{
//					stillWaiting = true;
//					break;
//				}
//				i++;
//			}
//			
//			if(stillWaiting)
//			{
//				if(a_vehicle)
//					a_vehicle.WaitingForTraffic = true;
//				
//				yield return new WaitForSeconds(m_timeToWaitBetweenCheckes);
//			}
//			else
//			{
//				if(a_vehicle)
//					a_vehicle.WaitingForTraffic = false;
//				
//				yield return null;
//			}
//		}
//		
//		m_checkStarted = false;
//	}
//
//	void OnDrawGizmos()
//	{
//		if(m_checkPos)
//		{
//			Gizmos.color = Color.cyan;
//			Gizmos.DrawWireSphere(m_checkPos.position, m_checkRadius);
//		}
//	}
}
