using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficSystemAwarenessTrigger : MonoBehaviour 
{
	public  List<TrafficSystemAwarenessTrigger>  m_linkedTriggers    = new List<TrafficSystemAwarenessTrigger>();
	public  List<TrafficSystemVehicle>           m_vehiclesInTrigger = new List<TrafficSystemVehicle>();

	void Start () 
	{
	
	}

	void OnTriggerEnter( Collider a_obj )
	{
		if(a_obj.GetComponent<TrafficSystemVehicle>())
		{
			TrafficSystemVehicle vehicle = a_obj.GetComponent<TrafficSystemVehicle>();

			if(m_vehiclesInTrigger.Count > 0)
			{
				if(IsAnyVehicleStopped( vehicle ))
					vehicle.StopMoving = true;
			}

			for(int lIndex = 0; lIndex < m_linkedTriggers.Count; lIndex++)
			{
				if(m_linkedTriggers[lIndex].m_vehiclesInTrigger.Count > 0)
				{
					for(int vIndex = 0; vIndex < m_linkedTriggers[lIndex].m_vehiclesInTrigger.Count; vIndex++)
					{
						if(m_linkedTriggers[lIndex].m_vehiclesInTrigger[vIndex].IsStopped() && vehicle != m_linkedTriggers[lIndex].m_vehiclesInTrigger[vIndex])
						{
							vehicle.StopMoving = true;
							break;
						}
					}
				}
			}

			m_vehiclesInTrigger.Add(vehicle);
		}
	}

	void OnTriggerExit( Collider a_obj )
	{
		if(a_obj.GetComponent<TrafficSystemVehicle>())
		{
			TrafficSystemVehicle vehicle = a_obj.GetComponent<TrafficSystemVehicle>();

			if(m_vehiclesInTrigger.Count > 0)
			{
				if(m_linkedTriggers.Count > 0)
				{
					bool foundVehicle = false;
					for(int lIndex = 0; lIndex < m_linkedTriggers.Count; lIndex++)
					{
						if(m_linkedTriggers[lIndex].m_vehiclesInTrigger.Count > 0)
						{
							m_linkedTriggers[lIndex].m_vehiclesInTrigger[0].StopMoving = false;
							foundVehicle = true;
						}
					}

					if(!foundVehicle)
						m_vehiclesInTrigger[0].StopMoving = false;
				}
				else
					m_vehiclesInTrigger[0].StopMoving = false;
			}

			m_vehiclesInTrigger.Remove(vehicle);
		}
	}

	public bool IsAnyVehicleStopped( TrafficSystemVehicle a_vehicle )
	{
		for(int vIndex = 0; vIndex < m_vehiclesInTrigger.Count; vIndex++)
		{
			if(m_vehiclesInTrigger[vIndex].IsStopped() && a_vehicle != m_vehiclesInTrigger[vIndex])
			{
				return true;
				break;
			}
		}

		return false;
	}

	public void ClearAwarenessTriggers()
	{
		m_linkedTriggers.Clear();
	}

	/*
	public void RegisterAwarenessTrigger( TrafficSystemAwarenessTrigger a_trigger )
	{
		if(a_trigger == this)
			return;

		bool foundTrigger = false;
		for(int tIndex = 0; tIndex < m_linkedTriggers.Count; tIndex++)
		{
			TrafficSystemAwarenessTrigger trigger = m_linkedTriggers[tIndex];

			if(a_trigger == trigger)
				foundTrigger = true;
		}

		if(!foundTrigger)
			m_linkedTriggers.Add(a_trigger);
	}
	*/
}
