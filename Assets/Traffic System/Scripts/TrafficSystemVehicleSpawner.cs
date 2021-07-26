using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficSystemVehicleSpawner : MonoBehaviour 
{
	public  List<TrafficSystemVehicle> m_vehiclePrefabs      = new List<TrafficSystemVehicle>();
	
	[Range(0.0f, 1.0f)]
	public  float               m_nodeVehicleSpawnChance     = 0.0f;

	public  float               m_onStartDelay               = 2.0f;
	public  int                 m_totalToSpawn               = 5;
	private int                 m_totalSpawned               = 0;
	public  float               m_spawnCheckDist             = 0.0f;
	public  float               m_spawnCheckRadius           = 5.0f;
	public  float               m_spawnDelayBetweenTries     = 5.0f;
	public  float               m_randVelocityMin            = 1.0f;
	public  float               m_randVelocityMax            = 5.0f;
	public  TrafficSystemNode   m_startNode                  = null;
	public  bool                m_respawnVehicleOnVehicleDestroy = true;
	private List<TrafficSystemVehicle> m_vehiclePool         = new List<TrafficSystemVehicle>();     

	public TrafficSystemVehicle SpawnRandomVehicle( bool a_ignoreChangeOfSpawning = false )
	{
		if(m_vehiclePrefabs.Count <= 0)
			return null;

		if(TrafficSystem.Instance && !TrafficSystem.Instance.CanSpawn())
			return null;

		float chanceOfSpawn = Random.Range(0.0f, 1.0f);
		
		if(!a_ignoreChangeOfSpawning && chanceOfSpawn > m_nodeVehicleSpawnChance)
			return null;
		
		int randIndex = Random.Range(0, m_vehiclePrefabs.Count);
		
		TrafficSystemVehicle vehicle = Instantiate( m_vehiclePrefabs[randIndex], transform.position, transform.rotation ) as TrafficSystemVehicle;
		vehicle.m_nextNode           = m_startNode;
		vehicle.m_velocityMax        = Random.Range(m_randVelocityMin, m_randVelocityMax);
		return vehicle;
	}
    void AddTagRecursively(Transform trans, string tag)
    {
        trans.gameObject.tag = tag;
        if (trans.childCount > 0)
            foreach (Transform t in trans)
                AddTagRecursively(t, tag);
    }

    void Awake()
	{
		if(GetComponent<Renderer>())
			GetComponent<Renderer>().enabled = false;
        AddTagRecursively(transform, "Terrain");
    }

	IEnumerator Start () 
	{
		if(TrafficSystem.Instance)
			TrafficSystem.Instance.RegisterVehicleSpawner( this );

		if(m_totalToSpawn <= 0)
			yield break;

		for(int sIndex = 0; sIndex < m_totalToSpawn; sIndex++)
		{
			TrafficSystemVehicle vehicle = SpawnRandomVehicle(true);
			vehicle.gameObject.SetActive(false);
			m_vehiclePool.Add(vehicle);
		}

		yield return new WaitForSeconds(m_onStartDelay);


		while(m_totalSpawned < m_totalToSpawn)
		{
			Collider[] colliderHit = Physics.OverlapSphere( transform.position, m_spawnCheckRadius );

			bool hitObj = false; 
			for(int hIndex = 0; hIndex < colliderHit.Length; hIndex++)
			{
				if(colliderHit[hIndex].transform.GetComponent<TrafficSystemVehicle>())
					hitObj = true;
			}

			if(!hitObj)
			{
				if(m_totalSpawned < m_vehiclePool.Count)
				{
					TrafficSystemVehicle vehicle =  m_vehiclePool[m_totalSpawned];
					vehicle.gameObject.SetActive(true);

//					if(TrafficSystem.Instance && vehicle)
//						TrafficSystem.Instance.RegisterVehicle( vehicle );
				}

				m_totalSpawned++;
			}

			yield return new WaitForSeconds(m_spawnDelayBetweenTries);
		}
	}

	public void RespawnVehicle()
	{
		StartCoroutine( ProcessSpawnOnDeath() );
	}

	IEnumerator ProcessSpawnOnDeath()
	{
		bool hasSpawned = false;
		while(!hasSpawned)
		{
			Collider[] colliderHit = Physics.OverlapSphere( transform.position, m_spawnCheckRadius );
			
			bool hitObj = false; 
			for(int hIndex = 0; hIndex < colliderHit.Length; hIndex++)
			{
				if(colliderHit[hIndex].transform.GetComponent<TrafficSystemVehicle>())
					hitObj = true;
			}
			
			if(!hitObj)
			{
				TrafficSystemVehicle vehicle =  SpawnRandomVehicle();
				
//				if(TrafficSystem.Instance && vehicle)
//					TrafficSystem.Instance.RegisterVehicle( vehicle );
				
				hasSpawned = true;
			}

			if(!hasSpawned)
				yield return new WaitForSeconds(m_spawnDelayBetweenTries);
		}

		yield return null;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position + ( transform.forward * m_spawnCheckDist ), m_spawnCheckRadius);
	}
}
