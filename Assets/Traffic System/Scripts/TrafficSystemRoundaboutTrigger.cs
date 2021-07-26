using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficSystemRoundaboutTrigger : MonoBehaviour 
{
	public  float              m_timeToWaitBetweenCheckes         = 1.0f;        // in seconds, the time we check to see if we can move again.
	public  float              m_checkRadius                      = 5.0f;        // the size of the spherecast for checking vehicle detection.
	public  Transform          m_checkPos;                                       // the position of the spherecast for checking vehicle detection.
	private bool               m_checkStarted                     = false;

	IEnumerator ProcessCheck( TrafficSystemVehicle a_vehicle )
	{
		bool stillWaiting = true;
		while(stillWaiting)
		{
			Collider[] hitColliders = Physics.OverlapSphere(m_checkPos.position, m_checkRadius);
			stillWaiting = false;
			int i = 0;
			while ( i < hitColliders.Length ) 
			{
				if(hitColliders[i].gameObject.GetComponent<TrafficSystemVehicle>())
				{
					stillWaiting = true;
					break;
				}
				i++;
			}

			/*
			RaycastHit[] hitInfo = Physics.SphereCastAll( m_checkPos.position, 10.0f, m_checkPos.transform.up );

			stillWaiting = false;
			for(int hIndex = 0; hIndex < hitInfo.Length; hIndex++)
			{
				TrafficSystemVehicle vehicle = hitInfo[hIndex].transform.GetComponent<TrafficSystemVehicle>();
				if(vehicle)
					stillWaiting = true;
			}
			*/

			if(stillWaiting)
			{
				if(a_vehicle)
					a_vehicle.WaitingForTraffic = true;

				yield return new WaitForSeconds(m_timeToWaitBetweenCheckes);
			}
			else
			{
				if(a_vehicle)
					a_vehicle.WaitingForTraffic = false;

				yield return null;
			}
		}

		m_checkStarted = false;
	}
	
	void OnTriggerEnter( Collider a_obj )
	{
		TrafficSystemVehicle vehicle = null;

		if(a_obj.transform.GetComponent<TrafficSystemVehicle>())
			vehicle = a_obj.transform.GetComponent<TrafficSystemVehicle>();

		if(vehicle)
		{
			if(!m_checkStarted)
			{
				m_checkStarted = true;
				StartCoroutine( ProcessCheck( vehicle ) );
			}
		}
	}

	void OnDrawGizmos()
	{
		if(m_checkPos)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(m_checkPos.position, m_checkRadius);
		}
	}
}
