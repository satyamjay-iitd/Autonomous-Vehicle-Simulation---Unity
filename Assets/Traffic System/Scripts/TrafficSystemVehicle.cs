using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficSystemVehicle : MonoBehaviour 
{
	public enum TrafficLightDetection
	{
		NONE            = 0,           // will cause vehciles to not see other vehicles, but no performance overhead
		RAY_TRACE       = 1,           // smallest performance hit
		CAPSULE_CAST    = 2,           // greater  performance hit
		RAY_AND_CAPSULE = 3            // greatest performance hit
	}

	public enum AxisType
	{
		X    = 0,
		Y    = 1,
		Z    = 2,
		NONE = 3
	}

	public  bool                        m_useNodeToPathToOnStart       = false;
	public  TrafficSystemNode           m_nodeToPathTo         = null;                   // if this is set then the vehicle will try to get to this node via the shortest possible route
	private List<TrafficSystemNode>     m_nodeToPathToRoute    = new List<TrafficSystemNode>(); // this is the nodes that it will traverse to get to the "m_nodeToPathTo" destination
	private List<TrafficSystemNode>     m_nodeToPathToAllNodesEverUsed = new List<TrafficSystemNode>();
	private bool                        m_calculatingPathToTake = false;
	private int                         m_nodeToPathToRouteIndex = 0;

	public  TrafficSystemNode           m_nextNode             = null;                   // the next node we are too visit
	public  float                       m_nextNodeThreshold    = 1.0f;                   // determines how far away we need to be from the target node we are going to before we can change nodes.

	public  float                       m_rotationSpeed        = 3.5f;                   // the speed at which we will rotaion
	public  Vector3                     m_offsetPosVal         = Vector3.zero;           // the amount to offset the position of this object

	public  bool                        m_useRoadSpeedLimits   = false;                  // enable this if you want the vehicle to use the TrafficSystem.cs m_globalSpeedLimit and individual TrafficSystemPiece.cs m_speedLimit instead of this vehicles m_velocityMax
	public  bool                        Accelerate             { get; set; }
	public  float                       m_velocityMin          = 0.0f;                   // this is our target velocity while decelerating 
	public  float                       m_velocityMax          = 5.0f;                   // this is our target velocity while accelerating
	public  bool                        m_changeMaxVelocityOnSpeedLimitInc = true;       // set this to false if you want the vehicle to ignore the road speed so it doesn't increase the vehicles max speed initially set... meaning the car will never go beyond the initial max speed even if the road says it can go faster in a certain area.
	protected float                     m_velocityMaxOriginal  = 0.0f;                   // this is our current velocity from the start
	public  float                       m_velocity             = 0.0f;                   // this is our current velocity 
	public  float                       m_accelerationRate     = 10.0f;                  // this is the velocity we add each second while accelerating
	[Range(0.0f, 1.0f)]
	public  float                       m_decelerationPercentage = 0.4f;                 // percentage of the total m_accelerationRate to use as deceleration
	[Range(0.0f, 1.0f)]
	public  float                       m_randomLaneChange     = 0.2f;                   // 0.0f = don't change lanes at all, 1.0f change lanes (if possible) each time we visit a new node
	[Range(0.0f, 1.0f)]
	public  float                       m_randomChanceToOvertake  = 0.5f;                // 0.0f = don't change lanes at all, 1.0f change lanes (if possible) each time we identify there is a vehicle in front of us and we are on a highway
	[Range(0.0f, 1.0f)]
	public  float                       m_chanceOfUsingOfframp = 0.2f;                   // 0.0f = don't use offramps ever, 1.0f always use offramps (if possible) each time we visit a new node
	[Range(0.0f, 1.0f)]
	public  float                       m_chanceOfDirChange    = 0.2f;                   // 0.0f = don't change direction, 1.0f always change direction (if possible) each time we visit a new node

	public  Rigidbody                   m_rigidbody            = null;                   // if set, this is how we will move. If not set, we will use the gameObjects position.
	public  Collider                    m_collider             = null;                   // assign the main body collider that will sit above the wheels so pathing works correctly without any collision issues with the road. 
	public  Collider                    m_colliderOnCrash      = null;                   // assign the main body collider that will consume the entire vehicle so that the wheels don't go through the world when the rigidbody gravity is switched on. 

	public  Transform[]                 m_lightsFront;
	public  Transform[]                 m_lightsRear;
	public  Transform[]                 m_lightsBlinkerRight;
	public  Transform[]                 m_lightsBlinkerLeft;
	public  float                       m_blinkerSpeed         = 0.2f;
	public  bool                        m_frontLightsEnabled   = false;

	public  Transform[]                 m_wheelsFront;
	public  Transform[]                 m_wheelsRear;
	public  AxisType                    m_wheelAxis            = AxisType.X;
	public  float                       m_wheelRotMultiplier   = 1.0f;

	public  float                       m_vehicleCheckDist     = 1.6f;                      
	public  float                       m_vehicleCheckRadius   = 0.72f;
	[Range(0.0f, 100.0f)]
	public  float                       m_vehicleCheckDistRay  = 14.0f;
	[Range(0.0f, 20.0f)]
	public  float                       m_vehicleCheckDistLeftRayOffset  = 1.0f;
	[Range(0.0f, 20.0f)]
	public  float                       m_vehicleCheckDistRightRayOffset = 1.0f;
	[Range(0.0f, 1.0f)]
	public  float                       m_vehicleCheckDistRayMimicSpeedThreshold = 0.4f;
	public  bool                        m_vehicleCheckOvertakeOnHighway           = true;   // turn this check off if you don't want the vehicle to check to see if it can get into a faster lane.
	public  float                       m_vehicleCheckDistOvertakeHighwayOnly     = 3.2f;   // set this well ahead of your vehicle so it knows that when this sphere hits a vehicle in front it should check to change to a faster lane!
	public  float                       m_vehicleCheckRadiusOvertakeHighwayOnly   = 0.72f;  // this is the radius of the highway only check
	public  bool                        m_enableVehicleCheckMid = false;
	public  float                       m_vehicleCheckDistMid  = 1.2f;
	public  float                       m_vehicleCheckRadiusMid = 1.58f;
	public  float                       m_intersectionCheckDistRay = 14.0f;
	public  float                       m_vehicleCheckRadiusAtLightsCapsule   = 1.15f;
	public  float                       m_vehicleCheckDistAtLightsCapsule     = 6.0f;
	public  Vector3                     m_vehicleCheckOffset                      = Vector3.zero; // this offsets all vehicle check values in case the pivot point for a vehicle is incorrect.

	public  bool                        StopMoving             { get; set; }
	public  TrafficSystemVehicle        VehicleHit             { get; set; }
	public  TrafficSystemVehicle        VehicleInSights        { get; set; }

	protected float                     m_waitTimer            = 0.0f;
	protected float                     m_waitTime             = 0.0f;
	public  float                       m_waitTimeMin          = 0.05f;
	public  float                       m_waitTimeMax          = 0.25f;

	public  bool                        m_strictCrashCheck     = false;
	public  bool                        CrashDetected          { get; set; }
	public  float                       m_timetoWaitAfterCrash = 4.0f;
	protected float                     m_timetoWaitAfterCrashTimer = 0.0f;
	private int                         m_crashes              = 0;

	protected float                     m_returnToOriginalVelocityTimer    = 0.0f;
	protected float                     m_returnToOriginalVelocityTimerMax = 10.0f;

	public  TrafficSystemTrafficLight   TrafficLight           { get; set; }
	protected float                     m_trafficLightCoolDown    = 0.0f;
	public  float                       m_trafficLightCoolDownMax = 3.0f;                   // once we go through a set of traffic lights this determines a cool down peroid before we stop at another set of traffic lights ... it is used for allowing bigger vehicles a cool down peroid to move forward without stopping at adjacent traffic lights of the same intersection
	private float                       m_trafficLightYellowEnterDuration      = 0.0f;
	private float                       m_trafficLightYellowEnterDurationTimer = 0.0f;

	public  bool                        m_enableWaitingDestroyTimer = true;
	public  float                       m_waitingDestroyTimerMax = 50.0f;
	protected float                     m_waitingDestroyTimer    = 0.0f;
	public  float                       m_stuckDestroyTimerMax   = 5.0f;
	protected float                     m_stuckDestroyTimer      = 0.0f;

	public  bool                        m_killOnEmptyPath      = true;

	protected TrafficSystemNode         m_nodeHighwaySearch    = null;

	public  TrafficLightDetection       m_trafficLightDetection = TrafficLightDetection.RAY_AND_CAPSULE;

	public  AudioSource                 m_audioSource          = null;
	public  AudioClip                   m_audioClipEngine      = null;
	public  AudioClip                   m_audioClipHorn        = null;
	public  bool                        m_playHornOnCrash      = true;
	private bool                        m_playHornOnCrashProcessing = false;
	public  bool                        m_flashFrontLightsOnCrash = true;
	private bool                        m_flashFrontLightsOnCrashProcessing = false;

	public  bool                        WaitingForTraffic      { get; set; }

	protected TrafficSystemVehicle      m_previousVehicleToTryOvertaking = null;
	private Vector3                     m_previousPos                    = Vector3.zero;
	private bool                        m_hasNotMovedSinceLastCheck      = false;

	protected float                     m_speedVariation                 = 0.0f;
	protected float                     m_lanePosVariation               = 0.0f;

//	private bool		                m_pathingStarted 	   = false;
//	private bool                        m_traversePath         = false;
//	public  bool		                m_orientToPath         = true;
//	public  iTween.EaseType             m_easeType             = iTween.EaseType.linear;
//	private iTween.LoopType             m_loopType             = iTween.LoopType.none;

	// public GameObject objDetect;
	// public GameObject yoloImage;
	// public GameObject skyCar;
 //    public GameObject huracanCar;
	public virtual void Awake () 
	{
		if(TrafficSystem.Instance)
			TrafficSystem.Instance.RegisterVehicle( this );

		TrafficLight        = null;
		StopMoving          = false;
		Accelerate          = true;                                                               // TODO: turn this on and off when we want the car to start and stop moving
		VehicleHit          = null;
		CrashDetected       = false;
		WaitingForTraffic   = false;

		if(!m_audioSource && gameObject.GetComponent<AudioSource>())
		{
			m_audioSource = gameObject.GetComponent<AudioSource>();
			StartCoroutine( InitPlaySoundEngine() );
		}

		if(m_frontLightsEnabled)
			FrontLights(m_frontLightsEnabled);

		if(m_colliderOnCrash)
		{
			m_colliderOnCrash.enabled = false;
			if(m_collider)
				m_collider.enabled    = true;
		}

		m_trafficLightCoolDown = m_trafficLightCoolDownMax + 1.0f;
		m_trafficLightYellowEnterDuration = Random.Range(0.1f, 0.5f);

//		m_randomVelocityOffset = Random.Range(m_randomVelocityOffsetMin, m_randomVelocityOffsetMax);
	}

	public virtual void Start () 
	{
		if(TrafficSystem.Instance)
		{
			m_speedVariation    = Random.Range(0.0f, TrafficSystem.Instance.m_globalSpeedVariation);
			m_lanePosVariation  = Random.Range(-TrafficSystem.Instance.m_globalLanePosVariation, TrafficSystem.Instance.m_globalLanePosVariation);
		}

		m_velocityMaxOriginal = m_velocityMax + m_speedVariation;

		StartCoroutine( ProcessBlinkers() );

		// Does this even affect anything? its just a wait which shouldn't apply to coroutines
		StartCoroutine( ProcessInit() );

		StartCoroutine( TrackPosition() );

		if(m_useNodeToPathToOnStart && m_nodeToPathTo)
			CalculatePathToTake( m_nodeToPathTo );
	}


	IEnumerator ProcessInit()
	{
		yield return new WaitForSeconds(1.0f);
	}

	IEnumerator InitPlaySoundEngine()
	{
		while(!m_audioSource)
			yield return null;

		yield return new WaitForSeconds(0.5f);
		PlaySoundEngine();
	}
	
	public void CalculatePathToTake( TrafficSystemNode a_nodeToPathTo )
	{
		if(!m_nextNode)
			return; // we are not connected to the road network

		if(m_calculatingPathToTake) // we are already calculating a path to find
			return;

		m_nodeToPathTo = a_nodeToPathTo;

		if(m_nodeToPathTo)
		{
			m_calculatingPathToTake = true;
			StartCoroutine( ProcessCalculatePathToTake() );
		}
	}

	IEnumerator ProcessCalculatePathToTake()
	{
		bool debug = true;
		m_nodeToPathToRoute.Clear();
		m_nodeToPathToAllNodesEverUsed.Clear();

		TrafficSystemNode nodeSearch = m_nextNode;

		m_nodeToPathToRoute.Add( nodeSearch );
		m_nodeToPathToAllNodesEverUsed.Add( nodeSearch );

		while(nodeSearch != m_nodeToPathTo && nodeSearch)
		{
			TrafficSystemNode currentNodeToSearch = nodeSearch;
			float distToDest = -1.0f;
			
			if(currentNodeToSearch.m_connectedLocalNode)
			{
				bool alreadyUsed = false;
				for(int uIndex = 0; uIndex < m_nodeToPathToAllNodesEverUsed.Count; uIndex++)
				{
					if(m_nodeToPathToAllNodesEverUsed[uIndex] == currentNodeToSearch.m_connectedLocalNode)
						alreadyUsed = true;
				}

				if(!alreadyUsed)
				{
					nodeSearch = currentNodeToSearch.m_connectedLocalNode;
					distToDest = (m_nodeToPathTo.transform.position - nodeSearch.transform.position).magnitude;

//					if(nodeSearch)
//						transform.position = nodeSearch.transform.position;
					
					if(nodeSearch != currentNodeToSearch)
					{
						m_nodeToPathToRoute.Add( nodeSearch );
						m_nodeToPathToAllNodesEverUsed.Add( nodeSearch );
					}

					continue;
				}
			}

			for(int nIndex = 0; nIndex < currentNodeToSearch.m_connectedChangeLaneNodes.Count; nIndex++)
			{
				bool alreadyUsed = false;
				for(int uIndex = 0; uIndex < m_nodeToPathToAllNodesEverUsed.Count; uIndex++)
				{
					if(m_nodeToPathToAllNodesEverUsed[uIndex] == currentNodeToSearch.m_connectedChangeLaneNodes[nIndex])
						alreadyUsed = true;
				}
				
				if(!alreadyUsed)
				{
					float dist = (m_nodeToPathTo.transform.position - currentNodeToSearch.m_connectedChangeLaneNodes[nIndex].transform.position).magnitude;

					if(distToDest < 0.0f || dist < distToDest)
					{
						nodeSearch = currentNodeToSearch.m_connectedChangeLaneNodes[nIndex];
						distToDest = dist;
					}
				}
			}

			for(int nIndex = 0; nIndex < currentNodeToSearch.m_connectedNodes.Count; nIndex++)
			{
				bool alreadyUsed = false;
				for(int uIndex = 0; uIndex < m_nodeToPathToAllNodesEverUsed.Count; uIndex++)
				{
					if(m_nodeToPathToAllNodesEverUsed[uIndex] == currentNodeToSearch.m_connectedNodes[nIndex])
						alreadyUsed = true;
				}

				if(m_nodeToPathTo == currentNodeToSearch.m_connectedNodes[nIndex])
				{
					float dist = (m_nodeToPathTo.transform.position - currentNodeToSearch.m_connectedNodes[nIndex].transform.position).magnitude;
					nodeSearch = currentNodeToSearch.m_connectedNodes[nIndex];
					distToDest = dist;
					break;
				}
				else if(!alreadyUsed)
				{
					float dist = (m_nodeToPathTo.transform.position - currentNodeToSearch.m_connectedNodes[nIndex].transform.position).magnitude;
					
					if(distToDest < 0.0f || dist < distToDest)
					{
						nodeSearch = currentNodeToSearch.m_connectedNodes[nIndex];
						distToDest = dist;
					}
				}
			}

			//yield return new WaitForSeconds(0.01f);

//			if(nodeSearch)
//				transform.position = nodeSearch.transform.position;

			if(nodeSearch != currentNodeToSearch)
			{
				m_nodeToPathToRoute.Add( nodeSearch );
				m_nodeToPathToAllNodesEverUsed.Add( nodeSearch );
			}
			else
			{
				m_nodeToPathToRoute.Clear();
				nodeSearch = null;
				TrafficSystemNode nodeSearchTmp = null;

				for(int aIndex = 0; aIndex < m_nodeToPathToAllNodesEverUsed.Count; aIndex++)
				{
					if(nodeSearch)
						break;

					nodeSearchTmp = m_nodeToPathToAllNodesEverUsed[aIndex];
					m_nodeToPathToRoute.Add(nodeSearchTmp);

					if(nodeSearchTmp.m_connectedLocalNode)
					{
						bool alreadyUsed = false;
						for(int uIndex = 0; uIndex < m_nodeToPathToAllNodesEverUsed.Count; uIndex++)
						{
							if(m_nodeToPathToAllNodesEverUsed[uIndex] == nodeSearchTmp.m_connectedLocalNode)
								alreadyUsed = true;
						}
						
						if(!alreadyUsed)
						{
							nodeSearch = nodeSearchTmp.m_connectedLocalNode;
							distToDest = (m_nodeToPathTo.transform.position - nodeSearch.transform.position).magnitude;
							m_nodeToPathToRoute.Add(nodeSearchTmp.m_connectedLocalNode);
							m_nodeToPathToAllNodesEverUsed.Add( nodeSearch );
							break;
						}
					}

					// search for any node in the road piece that is the same as the nodeToPathTo node
					for(int nIndex = 0; nIndex < nodeSearchTmp.m_connectedNodes.Count; nIndex++)
					{
						bool alreadyUsed = false;
						for(int uIndex = 0; uIndex < m_nodeToPathToAllNodesEverUsed.Count; uIndex++)
						{
							if(m_nodeToPathToAllNodesEverUsed[uIndex] == nodeSearchTmp.m_connectedNodes[nIndex])
								alreadyUsed = true;
						}
						
						if(m_nodeToPathTo == nodeSearchTmp.m_connectedNodes[nIndex])
						{
							float dist = (m_nodeToPathTo.transform.position - nodeSearchTmp.m_connectedNodes[nIndex].transform.position).magnitude;
							nodeSearch = nodeSearchTmp.m_connectedNodes[nIndex];
							distToDest = dist;
							m_nodeToPathToRoute.Add( nodeSearch );
							m_nodeToPathToAllNodesEverUsed.Add( nodeSearch );
							break;
						}
					}

					for(int nIndex = 0; nIndex < nodeSearchTmp.m_connectedNodes.Count; nIndex++)
					{
						if(nodeSearch)
							break;

						bool alreadyUsed = false;
						for(int uIndex = 0; uIndex < m_nodeToPathToAllNodesEverUsed.Count; uIndex++)
						{
							if(m_nodeToPathToAllNodesEverUsed[uIndex] == nodeSearchTmp.m_connectedNodes[nIndex])
								alreadyUsed = true;
						}
						
						if(!alreadyUsed)
						{
							float dist = (m_nodeToPathTo.transform.position - nodeSearchTmp.m_connectedNodes[nIndex].transform.position).magnitude;
							
							nodeSearch = nodeSearchTmp.m_connectedNodes[nIndex];
							distToDest = dist;
							m_nodeToPathToRoute.Add( nodeSearch );
							m_nodeToPathToAllNodesEverUsed.Add( nodeSearch );
							break;
						}
						else
						{
							if(nodeSearchTmp.m_connectedLocalNode)
							{
								alreadyUsed = false;
								for(int uIndex = 0; uIndex < m_nodeToPathToAllNodesEverUsed.Count; uIndex++)
								{
									if(m_nodeToPathToAllNodesEverUsed[uIndex] == nodeSearchTmp.m_connectedLocalNode)
										alreadyUsed = true;
								}
								
								if(!alreadyUsed)
								{
									nodeSearch = nodeSearchTmp.m_connectedLocalNode;
									distToDest = (m_nodeToPathTo.transform.position - nodeSearch.transform.position).magnitude;
									m_nodeToPathToRoute.Add(nodeSearchTmp.m_connectedLocalNode);
									m_nodeToPathToAllNodesEverUsed.Add( nodeSearch );
									break;
								}
							}

							for(int cLIndex = 0; cLIndex < nodeSearchTmp.m_connectedNodes[nIndex].m_connectedChangeLaneNodes.Count; cLIndex++)
							{
								if(nodeSearch)
									break;

								alreadyUsed = false;
								for(int uIndex = 0; uIndex < m_nodeToPathToAllNodesEverUsed.Count; uIndex++)
								{
									if(m_nodeToPathToAllNodesEverUsed[uIndex] == nodeSearchTmp.m_connectedNodes[nIndex].m_connectedChangeLaneNodes[cLIndex])
										alreadyUsed = true;
								}
								
								if(!alreadyUsed)
								{
									float dist = (m_nodeToPathTo.transform.position - nodeSearchTmp.m_connectedNodes[nIndex].m_connectedChangeLaneNodes[cLIndex].transform.position).magnitude;
									
									nodeSearch = nodeSearchTmp.m_connectedNodes[nIndex].m_connectedChangeLaneNodes[cLIndex];
									distToDest = dist;
									m_nodeToPathToRoute.Add(nodeSearchTmp.m_connectedNodes[nIndex].m_connectedChangeLaneNodes[cLIndex]);
									m_nodeToPathToAllNodesEverUsed.Add( nodeSearch );
									break;
								}
							}
						}
					}
					
					//yield return null;
				}
			}

			yield return null;
		}

		TestTraverse1();
		m_calculatingPathToTake = false;
	}

	private void TestTraverse1()
	{
		for(int nIndex = 0; nIndex < m_nodeToPathToRoute.Count; nIndex++)
		{
			if(m_nodeToPathToRoute[nIndex] && m_nodeToPathToRoute[nIndex].GetComponent<Renderer>())
				m_nodeToPathToRoute[nIndex].GetComponent<Renderer>().material.SetColor( "_Color", Color.yellow );
		}

		return;

		/*
		while(true)
		{
			for(int nIndex = 0; nIndex < m_nodeToPathToRoute.Count; nIndex++)
			{
				transform.position = m_nodeToPathToRoute[nIndex].transform.position;
				yield return new WaitForSeconds(0.1f);
			}

			yield return null;
		}
		*/
	}

	void PlaySoundEngine()
	{
		if(m_audioSource && m_audioClipEngine)
		{
			m_audioSource.clip = m_audioClipEngine;
			m_audioSource.loop = true;
			m_audioSource.Play();
		}
	}

	void PlaySoundHorn()
	{
		if(!m_playHornOnCrashProcessing)
		{
			m_playHornOnCrashProcessing = true;
			StartCoroutine( PlaySoundHornProcess() );
		}
	}

	IEnumerator PlaySoundHornProcess()
	{
		if(m_audioSource && m_audioClipHorn)
		{
			m_audioSource.clip = m_audioClipHorn;
			m_audioSource.loop = false;
			m_audioSource.Play();

			yield return null;

			while(m_audioSource.isPlaying)
				yield return null;

			PlaySoundEngine();
		}

		m_playHornOnCrashProcessing = false;
		yield return null;
	}

	public void FrontLights( bool a_enabled, bool a_recordInput = true )
	{
		if(a_recordInput)
			m_frontLightsEnabled = a_enabled;

		for(int rIndex = 0; rIndex < m_lightsFront.Length; rIndex++)
		{
			m_lightsFront[rIndex].gameObject.SetActive( a_enabled );
		}
	}
    public void Setlayer()
    {
        gameObject.layer = 8;

    }

    void FlashFrontLights()
	{
		if(!m_flashFrontLightsOnCrashProcessing)
		{
			m_flashFrontLightsOnCrashProcessing = true;
			StartCoroutine( FlashFrontLightsProcess() );
		}
	}

	IEnumerator FlashFrontLightsProcess()
	{
		if(m_lightsFront.Length <= 0)
		{
			m_flashFrontLightsOnCrashProcessing = false;
			yield break;
		}

		FrontLights( true, false );
		yield return new WaitForSeconds( 0.35f );
		FrontLights( false, false );
		yield return new WaitForSeconds( 0.35f );
		FrontLights( true, false );
		yield return new WaitForSeconds( 0.35f );
		FrontLights( false, false );
		yield return new WaitForSeconds( 0.35f );
		FrontLights( m_frontLightsEnabled, false );

		m_flashFrontLightsOnCrashProcessing = false;
	}

	IEnumerator ProcessBlinkers()
	{
		while(true)
		{
			if(IsTurningLeft())
			{
				for(int rIndex = 0; rIndex < m_lightsBlinkerLeft.Length; rIndex++)
				{
					m_lightsBlinkerLeft[rIndex].gameObject.SetActive( !m_lightsBlinkerLeft[rIndex].gameObject.activeSelf );
				}
				yield return new WaitForSeconds(m_blinkerSpeed);
			}
			else if(IsTurningRight())
			{
				for(int rIndex = 0; rIndex < m_lightsBlinkerRight.Length; rIndex++)
				{
					m_lightsBlinkerRight[rIndex].gameObject.SetActive( !m_lightsBlinkerRight[rIndex].gameObject.activeSelf );
				}
				yield return new WaitForSeconds(m_blinkerSpeed);
			}
			else if(!IsTurning())
			{
				for(int rIndex = 0; rIndex < m_lightsBlinkerLeft.Length; rIndex++)
				{
					m_lightsBlinkerLeft[rIndex].gameObject.SetActive( false );
				}
				for(int rIndex = 0; rIndex < m_lightsBlinkerRight.Length; rIndex++)
				{
					m_lightsBlinkerRight[rIndex].gameObject.SetActive( false );
				}
			}

			yield return null;
		}
	}

	public virtual void Update () 
	{
		if(m_calculatingPathToTake)
			return;

//		CanSeeVehicleViaSphere();

		if(!TrafficLight)
			m_trafficLightCoolDown += Time.deltaTime;

		StopMoving = false;

		if(WaitingForTraffic)
			StopMoving = true;

		if(CrashDetected)
		{
			m_timetoWaitAfterCrashTimer += Time.deltaTime;
			CrashOverCheck();
		}

		if(!m_useRoadSpeedLimits && m_velocityMaxOriginal != m_velocityMax)
		{
			m_returnToOriginalVelocityTimer += Time.deltaTime;
			if(m_returnToOriginalVelocityTimer >= m_returnToOriginalVelocityTimerMax)
			{
				m_velocityMax = m_velocityMaxOriginal;
				m_returnToOriginalVelocityTimer = 0.0f;
			}
		}

		if(VehicleHit) // if we are close to a car previously, wait before checking
		{
			m_waitTimer += Time.deltaTime;
			if(m_waitTimer < m_waitTime)
				return;
		}

		if(m_nextNode)
		{
			bool isOnHighway = m_nextNode.IsHighway() && m_nextNode.IsHighwayChangeLaneAccepted() &&
			                   m_vehicleCheckOvertakeOnHighway && m_nodeHighwaySearch != m_nextNode;
			
			if(isOnHighway)
			{
				RaycastHit hitInfo;
				VehicleHit  = null;
				if(Physics.SphereCast( (transform.position + m_vehicleCheckOffset), m_vehicleCheckRadiusOvertakeHighwayOnly, transform.forward, out hitInfo, m_vehicleCheckDistOvertakeHighwayOnly ))
				{
					TrafficSystemVehicle vehicle = hitInfo.transform.GetComponent<TrafficSystemVehicle>();
					if(vehicle)
					{
						if(vehicle != this && vehicle.VehicleHit != this && vehicle != m_previousVehicleToTryOvertaking)
						{
							float randChanceToOvertake = 0.0f;
							randChanceToOvertake = Random.Range(0.0f, 1.0f);

							m_previousVehicleToTryOvertaking = vehicle;

							if(randChanceToOvertake <= m_randomChanceToOvertake)
							{
								TrafficSystemNode node = m_nextNode.GetNextNodeHighway();
								if(node)
								{
									SetNextNode( node );
									m_nodeHighwaySearch = m_nextNode;
								}
							}
						}
					}
				}

				RaycastHit[] hitInfoAll = Physics.SphereCastAll( (transform.position + m_vehicleCheckOffset), m_vehicleCheckRadius, transform.forward, m_vehicleCheckDist );
				VehicleHit  = null;
				for(int hIndex = 0; hIndex < hitInfoAll.Length; hIndex++)
				{
					TrafficSystemVehicle vehicle = hitInfoAll[hIndex].transform.GetComponent<TrafficSystemVehicle>();
					if(vehicle)
					{
						if(vehicle != this && vehicle.VehicleHit != this)
						{
							StopMoving = true;
							VehicleHit = vehicle;
							m_velocityMax = VehicleHit.m_velocityMax;
							m_waitTimer = 0.0f;
							m_waitTime = Random.Range(m_waitTimeMin, m_waitTimeMax);
						}
					}
				}
			}
			else
			{
				RaycastHit[] hitInfoAll = Physics.SphereCastAll( (transform.position + m_vehicleCheckOffset), m_vehicleCheckRadius, transform.forward, m_vehicleCheckDist );
				VehicleHit  = null;
				for(int hIndex = 0; hIndex < hitInfoAll.Length; hIndex++)
				{
					TrafficSystemVehicle vehicle = hitInfoAll[hIndex].transform.GetComponent<TrafficSystemVehicle>();
					if(vehicle)
					{
						if(vehicle != this && vehicle.VehicleHit != this)
						{
							StopMoving = true;
							VehicleHit = vehicle;
							m_velocityMax = VehicleHit.m_velocityMax;
							m_waitTimer = 0.0f;
							m_waitTime = Random.Range(m_waitTimeMin, m_waitTimeMax);
						}
					}
				}
			}
			
			if(!VehicleHit && m_enableVehicleCheckMid)
			{
				RaycastHit[] hitInfo = Physics.SphereCastAll( (transform.position + m_vehicleCheckOffset), (m_vehicleCheckRadius * m_vehicleCheckRadiusMid ), transform.forward, m_vehicleCheckDist - m_vehicleCheckDistMid );
				
				VehicleHit  = null;
				for(int hIndex = 0; hIndex < hitInfo.Length; hIndex++)
				{
					TrafficSystemVehicle vehicle = hitInfo[hIndex].transform.GetComponent<TrafficSystemVehicle>();
					if(vehicle)
					{
						if(vehicle != this && vehicle.VehicleHit != this )
						{
							StopMoving = true;
							VehicleHit = vehicle;
							m_velocityMax = VehicleHit.m_velocityMax;
							m_waitTimer = 0.0f;
							m_waitTime = Random.Range(m_waitTimeMin, m_waitTimeMax);
						}
					}
				}
			}
		}

		if(!StopMoving && TrafficLight && !CrashDetected) // determine if there is there is room to move throught the intersection
		{
			if(TrafficLight.m_status == TrafficSystemTrafficLight.Status.RED)
			{
				//print ("TrafficLight = RED");
				StopMoving = true;
			}
			else if(CanSeeVehicleViaSphere() && !TrafficLight.IgnoreCanFitAcrossIntersectionCheck())
			{
				//print ("CanSeeVehicleViaSphere: = TRUE");
				StopMoving = true;
			}
			else if(!CanFitAcrossIntersection() && !TrafficLight.IgnoreCanFitAcrossIntersectionCheck())
			{
				//print ("CanFitAcrossIntersection: = FALSE");
				StopMoving = true;
			}
			else if(TrafficLight.m_status == TrafficSystemTrafficLight.Status.YELLOW)
			{
				m_trafficLightYellowEnterDurationTimer += Time.deltaTime;
				if(m_trafficLightYellowEnterDurationTimer <= m_trafficLightYellowEnterDuration)
				{
					DriveThroughLights();
				}
				else
					//print ("TrafficLight = RED OR YELLOW");
					StopMoving = true;
			}
			else if(TrafficLight.m_status == TrafficSystemTrafficLight.Status.GREEN)
			{
				DriveThroughLights();
			}

//			print ("TrafficLight: " + TrafficLight);
		}

		if(!StopMoving && m_nextNode && !CrashDetected)// && !m_pathingStarted)
		{
			VehicleInSights = CanSeeVehicleViaRay();
			float ratio = 1.0f;
			if(VehicleInSights)
			{
				if(VehicleInSights.VehicleInSights && VehicleInSights.VehicleInSights == this)
				{
					// do nothing as these two cars are facing eachother for unknown reasons so none of them should slow down with this ray detection.
				}
				else
				{
					float velocityRatio = 1.0f;

//					if(m_velocity > 0.0f)
//						velocityRatio = VehicleInSights.m_velocity / m_velocity;

					Vector3 dirOfVehicleInSight = transform.position - VehicleInSights.transform.position;
					ratio = Mathf.Clamp((dirOfVehicleInSight.magnitude / m_vehicleCheckDistRay), 0.0f, 1.0f);

					if(ratio < m_vehicleCheckDistRayMimicSpeedThreshold)
						m_velocityMax = VehicleInSights.m_velocityMax;
				}
			}

			if(Accelerate)                                                              // are we accelerating?
				m_velocity = m_velocity + ((m_accelerationRate * ratio) * Time.deltaTime);        // add to the current velocity according while accelerating
//			else
//				m_velocity = m_velocity - (m_decelerationRate * Time.deltaTime);        // subtract from the current velocity while decelerating

			float velTmp = m_velocityMax * ratio;

			if(velTmp > m_velocity)
				m_velocity = m_velocity - ((m_accelerationRate * m_decelerationPercentage) * Time.deltaTime);
			else
				m_velocity = velTmp;

			if(VehicleHit)
				m_velocity = VehicleHit.m_velocity;                                     // ensure the velocity is the same as the car in front
			else
				m_velocity = Mathf.Clamp(m_velocity, m_velocityMin, m_velocityMax);     // ensure the velocity never goes out of the min/max boundaries

			Vector3 dir = m_nextNode.transform.position;
			dir.x      += m_lanePosVariation;
			dir.z      += m_lanePosVariation;
			dir         = dir - (transform.position + m_offsetPosVal);           // find the direction to the next node

			Vector3 speed = dir.normalized * m_velocity;                                // work out how fast we should travel in the desired directoin

			for(int rIndex = 0; rIndex < m_lightsRear.Length; rIndex++)
			{
				if(m_lightsRear[rIndex].gameObject.activeSelf)
					m_lightsRear[rIndex].gameObject.SetActive(false);
			}

			Vector3 wheelAxis = Vector3.zero;

			if(m_wheelAxis == AxisType.X)
				wheelAxis = new Vector3(1.0f, 0.0f, 0.0f);
			else if(m_wheelAxis == AxisType.Y)
				wheelAxis = new Vector3(0.0f, 1.0f, 0.0f);
			else if(m_wheelAxis == AxisType.Z)
				wheelAxis = new Vector3(0.0f, 0.0f, 1.0f);

			for(int wIndex = 0; wIndex < m_wheelsFront.Length; wIndex++)
				m_wheelsFront[wIndex].transform.Rotate(wheelAxis, m_velocity * m_wheelRotMultiplier);
			
			for(int wIndex = 0; wIndex < m_wheelsRear.Length; wIndex++)
				m_wheelsRear[wIndex].transform.Rotate(wheelAxis, m_velocity * m_wheelRotMultiplier);

			float rotSpeed = (m_velocity / m_velocityMax) * m_rotationSpeed;
			if(m_rigidbody)                                                             // if we have a rigidbody, use the following code to move us
			{
				m_rigidbody.velocity = speed;                                           // set our rigidbody to this speed to move us by the determined speed 
				transform.forward    = Vector3.Lerp( transform.forward, dir.normalized, rotSpeed * Time.deltaTime );    // rotate our forward directoin over time to face the node we are moving towards
			}
			else                                                                        // no rigidbody then use the following code to move us
			{
				if(m_collider || GetComponent<Collider>())                                              // it generally is a bad idea to move something with a collider, so we should tell someone about it if this is happening. See Unity Docs for more info: http://docs.unity3d.com/ScriptReference/Collider.html
					Debug.LogWarning("Traffic System Warning -> VehicleBase has a collider. You should think about moving the object with a rigidbody instead.");
				
				transform.position += speed * Time.deltaTime;                           // move us by the determined speed
				transform.forward   = Vector3.Lerp( transform.forward, dir.normalized, rotSpeed * Time.deltaTime );     // rotate our forward directoin over time to face the node we are moving towards
			}

			// if (Input.GetKeyUp(KeyCode.C)) // to change car type
   //  	{
   //  		skyCar.gameObject.SetActive(!skyCar.gameObject.activeSelf);
   //  		huracanCar.gameObject.SetActive(!huracanCar.gameObject.activeSelf);

   //  	}
   //  	if (Input.GetKeyUp(KeyCode.H)) // to select Huracan car
   //  	{
   //  		skyCar.SetActive(false);
   //  		huracanCar.SetActive(true);
   //  	}
        
   //      if (Input.GetKeyUp(KeyCode.O))// to put obj detection ON/Off by pressing O key on keyboard
   //  	{
   //  		objDetect.gameObject.SetActive(!objDetect.gameObject.activeSelf); // to togle ststes of object detection
   //  		yoloImage.gameObject.SetActive(!yoloImage.gameObject.activeSelf); // to disactivate yolo image on screen when not in use
    		
   //  	}

			if(dir.magnitude < m_nextNodeThreshold)                                     // if we are close enough to the node we were travelling towards then check for the next one
			{
//				if(m_nextNode.HasSplineToFollow( this ))
//				{
//					if(!m_traversePath)
//					{
//						m_traversePath = true;
//						ProcessSpline();
//					}
//				}
//				else

//				if(!IsTrafficJamOnUpcomingNodes())
//				{
//					TrafficSystemNode preNextNode = m_nextNode;
//
//					List<TrafficSystemNode> blockedNodes = new List<TrafficSystemNode>();
//					for(int nIndex = 0; nIndex < m_nextNode.m_connectedNodes.Count; nIndex++)
//					{
//						TrafficSystemNode nodeToCheck = m_nextNode.m_connectedNodes[nIndex];
//						if(VehicleExistsOnNode(nodeToCheck))
//							blockedNodes.Add(nodeToCheck);
//					}
//
				SetNextNode( m_nextNode.GetNextNode( this, true/*, blockedNodes*/ ) );                            // gets the next required node to go to and start the process all over again
				//}
			}
		}
		else if(CrashDetected)
		{
			// don't stop, but don't do anything
		}
		else if(StopMoving)
		{
			Stop();
		}

		if(m_killOnEmptyPath && !m_nextNode)
		{
			if (Debug.isDebugBuild)
				Debug.LogWarning("Vehicle Destroyed - No path node");

			Kill();
		}
	}

	void DriveThroughLights()
	{
		if(TrafficLight.m_intersection)
		{
			if(TrafficLight.m_intersection.m_intersectionSystem == TrafficSystemIntersection.IntersectionSystem.SINGLE)
			{
				//						print ("GO!");
				TrafficLight = null;
				StopMoving         = false;
				m_trafficLightYellowEnterDurationTimer = 0.0f;
			}
			else if(TrafficLight.m_intersection.m_intersectionSystem == TrafficSystemIntersection.IntersectionSystem.DUAL)
			{
				if(IsTurningIntoIncomingTraffic())
				{
					//print ("IsTurningIntoIncomingTraffic: = TRUE");
					StopMoving         = true;
				}
				else
				{
					//							print ("GO!");
					TrafficLight = null;
					StopMoving         = false;
					m_trafficLightYellowEnterDurationTimer = 0.0f;
				}
			}
		}
		else
			Debug.LogWarning("Traffic System Traffic Light is missing a parent intersection! See the example traffic system intersection and the structure setup as this is what you need to mimic");

		m_trafficLightYellowEnterDurationTimer = 0.0f;
	}

	IEnumerator TrackPosition()
	{
		bool loop = true;
		while(loop)
		{
			Vector3 dist = transform.position - m_previousPos;
			//Debug.Log("dist.magnitude: " + dist.magnitude);
			if(dist.magnitude <= 1.0f)
				m_hasNotMovedSinceLastCheck = true;
			else
			{
				m_hasNotMovedSinceLastCheck = false;
				m_previousPos = transform.position;
			}

			yield return new WaitForSeconds(1.0f);
		}
	}

	public void AssignTrafficLight( TrafficSystemTrafficLight a_trafficLight )
	{
		if(m_trafficLightCoolDown > m_trafficLightCoolDownMax)
			TrafficLight = a_trafficLight;

		m_trafficLightCoolDown = 0.0f;
	}

	protected void SetNextNode( TrafficSystemNode a_node )
	{
		m_nodeHighwaySearch            = null;
		TrafficSystemNode previousNode = m_nextNode;
		m_nextNode                     = a_node;

		//print ("m_nodeToPathToRoute.Count ( " + m_nodeToPathToRoute.Count + " ) > 0 && m_nodeToPathToRoute.Count ( " + m_nodeToPathToRoute.Count + " ) < m_nodeToPathToRouteIndex ( " + m_nodeToPathToRouteIndex + " )");
		if(m_nodeToPathToRoute.Count > 0 && m_nodeToPathToRouteIndex < m_nodeToPathToRoute.Count)
		{
			m_nextNode = m_nodeToPathToRoute[m_nodeToPathToRouteIndex];
			m_nodeToPathToRouteIndex++;
		}

		if(!m_nextNode)
		{
			Debug.LogWarning("Vehicle node invalid, check all nodes on this road piece have correct links as this is probably the issue!: \"" + previousNode.Parent.name + "\" -> Previous Node was: \"" + previousNode.name + "\"\nTIP: In \"Editor Mode\" try clicking on the road piece then clicking off it as this should remove any null links.");

			if(m_killOnEmptyPath)
				Debug.Log("Vehicle set to kill on empty path so we will destroy vehicle " + gameObject.name);
		}
		else
		{
			float newSpeed = m_nextNode.GetSpeedLimit() + m_speedVariation;

			if(m_changeMaxVelocityOnSpeedLimitInc)
				m_velocityMaxOriginal = newSpeed;

			if(newSpeed <= m_velocityMaxOriginal)
				m_velocityMax = newSpeed;
		}
	}

	public void Stop()
	{
		m_velocity = 0.0f;//m_velocity - (m_decelerationRate * Time.deltaTime);        // subtract from the current velocity while decelerating
		
		if(m_rigidbody)
		{
			m_rigidbody.velocity = Vector3.one * m_velocity;
			m_rigidbody.angularVelocity = Vector3.one * m_velocity;
		}

		for(int rIndex = 0; rIndex < m_lightsRear.Length; rIndex++)
			m_lightsRear[rIndex].gameObject.SetActive(true);
	}



	public bool CanFitAcrossIntersection()
	{
		if(m_trafficLightDetection == TrafficLightDetection.NONE ||
		   m_trafficLightDetection == TrafficLightDetection.CAPSULE_CAST)
			return true;

		RaycastHit[] rays = Physics.RaycastAll((transform.position + m_vehicleCheckOffset), transform.forward, m_intersectionCheckDistRay);

		for(int hIndex = 0; hIndex < rays.Length; hIndex++)
		{
			TrafficSystemVehicle vehicle = rays[hIndex].transform.GetComponent<TrafficSystemVehicle>();
			if(vehicle && vehicle != this && VehicleIsOnSameSide(vehicle) && vehicle.m_velocity <= 0.0f)
				return false;
		}

		return true;
	}

	public bool VehicleIsOnSameSide( TrafficSystemVehicle a_vehicle )
	{
		if(a_vehicle && a_vehicle.m_nextNode && a_vehicle.m_nextNode.m_driveSide == m_nextNode.m_driveSide)
			return true;

		return false;
	}

	public TrafficSystemVehicle CanSeeVehicleViaRay( bool a_checkVehicleIsMoving = false )
	{
		if(m_trafficLightDetection == TrafficLightDetection.NONE ||
		   m_trafficLightDetection == TrafficLightDetection.CAPSULE_CAST)
			return null;

		RaycastHit hitInfo;

		TrafficSystemVehicle vehicleInSight = null;

		if(Physics.Raycast(((transform.position + m_vehicleCheckOffset) + (-transform.right * m_vehicleCheckDistLeftRayOffset)), transform.forward, out hitInfo, m_vehicleCheckDistRay))
		{
			TrafficSystemVehicle vehicle = hitInfo.transform.GetComponent<TrafficSystemVehicle>();
			if(vehicle && vehicle != this && VehicleIsOnSameSide(vehicle))
			{
				if(a_checkVehicleIsMoving)
				{
					if(vehicle.m_velocity <= 0.0f)
						vehicleInSight = vehicle;
				}
				else
					vehicleInSight = vehicle;
			}
		}

		if(!vehicleInSight)
		{
			if(Physics.Raycast(((transform.position + m_vehicleCheckOffset) + (transform.right * m_vehicleCheckDistRightRayOffset)), transform.forward, out hitInfo, m_vehicleCheckDistRay))
			{
				TrafficSystemVehicle vehicle = hitInfo.transform.GetComponent<TrafficSystemVehicle>();
				if(vehicle && vehicle != this && VehicleIsOnSameSide(vehicle))
				{
					if(a_checkVehicleIsMoving)
					{
						if(vehicle.m_velocity <= 0.0f)
							vehicleInSight = vehicle;
					}
					else
						vehicleInSight = vehicle;
				}
			}
		}

		return vehicleInSight;
	}

	public bool CanSeeVehicleViaSphere( bool a_checkVehicleIsMoving = false )
	{
		if(m_trafficLightDetection == TrafficLightDetection.NONE ||
		   m_trafficLightDetection == TrafficLightDetection.RAY_TRACE)
			return false;

		RaycastHit[] hits;
		Vector3 p1 = (transform.position + m_vehicleCheckOffset);
		Vector3 p2 = p1 + (transform.forward.normalized * m_vehicleCheckDistAtLightsCapsule);
		hits = Physics.CapsuleCastAll(p1, p2, m_vehicleCheckRadiusAtLightsCapsule, transform.forward, m_vehicleCheckDistAtLightsCapsule);

		for(int hIndex = 0; hIndex < hits.Length; hIndex++)
		{
			TrafficSystemVehicle vehicle = hits[hIndex].transform.GetComponent<TrafficSystemVehicle>();
			if(vehicle && vehicle != this && vehicle.VehicleHit != this )
			{
				if(a_checkVehicleIsMoving)
				{
					if(vehicle.m_velocity <= 0.0f)
						return true;
					else
						return false;
				}
				else
					return true;
			}
		}

		return false;
	}
	
	public TrafficSystemVehicle VehicleExistsOnNode( TrafficSystemNode a_node )
	{
		if(!a_node)
			return null;

		RaycastHit[] hitInfo;
		hitInfo = Physics.SphereCastAll( a_node.transform.position, 2.0f, transform.forward, 0.0f );
		
		for(int hIndex = 0; hIndex < hitInfo.Length; hIndex++)
		{
			TrafficSystemVehicle vehicle = hitInfo[hIndex].transform.GetComponent<TrafficSystemVehicle>();
			if(vehicle && vehicle != this && vehicle.m_velocity <= 0.0f)
				return vehicle;
		}

		return null;
	}

	public bool IsStopped()
	{
		if(m_velocity <= 0.0f)
			return true;

		return false;
	}

	public bool IsStuck()
	{
		//Debug.Log("m_hasNotMovedSinceLastCheck: " + m_hasNotMovedSinceLastCheck + " | m_velocity: " + m_velocity + " | m_stuckDestroyTimer: " + m_stuckDestroyTimer);
		if(m_velocity >= m_velocityMax && m_hasNotMovedSinceLastCheck)
			return true;
		
		return false;
	}
	
	public void Kill()
	{
		if(TrafficSystem.Instance && this)
			TrafficSystem.Instance.UnRegisterVehicle( this );

		Destroy(gameObject);
	}

	public bool IsTurningRight()
	{
		if(!m_nextNode)
			return false;

		if(m_nextNode.m_roadType == TrafficSystem.RoadType.CHANGE_LANE && m_nextNode.m_driveSide == TrafficSystem.DriveSide.RIGHT)
			return true;

		return false;
	}

	public bool IsTurningIntoIncomingTraffic()
	{
		return IsTurningLeft() && !TrafficLight.m_turnLeftAnytime;
	}

	public bool IsTurningLeft()
	{
		if(!m_nextNode)
			return false;

		if(m_nextNode.m_roadType == TrafficSystem.RoadType.CHANGE_LANE && m_nextNode.m_driveSide == TrafficSystem.DriveSide.LEFT)
			return true;
		
		return false;
	}
	
	public bool IsTurning()
	{
		if(!m_nextNode)
			return false;

		if(m_nextNode.m_roadType == TrafficSystem.RoadType.CHANGE_LANE)
			return true;
		
		return false;
	}
	
	void OnDrawGizmos()
	{
//		if(!Application.isPlaying) // in editor
//			FindNodeDistanceCheckPosY();

		Gizmos.color = Color.yellow;
		Gizmos.DrawCube((transform.position + m_offsetPosVal), Vector3.one * 0.25f);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere((transform.position + m_vehicleCheckOffset) + ( transform.forward * m_vehicleCheckDist ), m_vehicleCheckRadius);

		// if(m_vehicleCheckOvertakeOnHighway)
		// {
		// 	Gizmos.color = Color.magenta;
		// 	Gizmos.DrawWireSphere((transform.position + m_vehicleCheckOffset) + ( transform.forward * m_vehicleCheckDistOvertakeHighwayOnly ), m_vehicleCheckRadiusOvertakeHighwayOnly);
		// }

		if(m_enableVehicleCheckMid)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere((transform.position + m_vehicleCheckOffset) + ( transform.forward * (m_vehicleCheckDist - m_vehicleCheckDistMid) ), (m_vehicleCheckRadius * m_vehicleCheckRadiusMid));
		}

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine((transform.position + m_vehicleCheckOffset), (transform.position + m_vehicleCheckOffset) + ( transform.forward * m_intersectionCheckDistRay ));

		Gizmos.color = Color.red;
		Gizmos.DrawLine(((transform.position + m_vehicleCheckOffset) + (-transform.right * m_vehicleCheckDistLeftRayOffset)), ((transform.position + m_vehicleCheckOffset) + (-transform.right * m_vehicleCheckDistLeftRayOffset)) + ( transform.forward * m_vehicleCheckDistRay ));
		Gizmos.DrawLine(((transform.position + m_vehicleCheckOffset) + (transform.right * m_vehicleCheckDistRightRayOffset)), ((transform.position + m_vehicleCheckOffset) + (transform.right * m_vehicleCheckDistRightRayOffset)) + ( transform.forward * m_vehicleCheckDistRay ));
	}

	void OnTriggerStay( Collider a_obj )
	{
		if(!m_strictCrashCheck)
			return;

		if( a_obj.GetComponent<TrafficSystemVehiclePlayer>() )
		{
			Crashed();
		}
	}
	
	void OnCollisionStay( Collision a_obj )
	{
		if(!m_strictCrashCheck)
			return;
		
		if( a_obj.gameObject.GetComponent<TrafficSystemVehiclePlayer>() )
		{
			Crashed();
		}
	}

	void OnCollisionEnter( Collision a_obj )
	{
		if(m_strictCrashCheck)
			return;

		if( a_obj.gameObject.GetComponent<TrafficSystemVehiclePlayer>() )
		{
			Crashed();
			m_crashes++;

			if(m_playHornOnCrash)
				PlaySoundHorn();
			if(m_flashFrontLightsOnCrash)
				FlashFrontLights();
		}
	}

	void OnCollisionExit( Collision a_obj)
	{
		if(m_strictCrashCheck)
			return;

		if( a_obj.gameObject.GetComponent<TrafficSystemVehiclePlayer>() )
		{
			Crashed();
			m_crashes--;
			CrashOverCheck();
		}
	}

	private void Crashed()
	{
		CrashDetected = true;
		m_timetoWaitAfterCrashTimer = 0.0f;

		if(m_rigidbody)
			m_rigidbody.useGravity = true;

		if(m_colliderOnCrash)
		{
			m_colliderOnCrash.enabled = true;
			if(m_collider)
				m_collider.enabled    = false;
		}
	}

	protected void CrashOverCheck()
	{
		if(m_timetoWaitAfterCrashTimer >= m_timetoWaitAfterCrash)
		{
			if(m_strictCrashCheck)
			{
				m_timetoWaitAfterCrashTimer = 0.0f;
				CrashDetected = false;

				if(m_rigidbody)
					m_rigidbody.useGravity = false;

				if(m_colliderOnCrash)
				{
					m_colliderOnCrash.enabled = false;
					if(m_collider)
						m_collider.enabled    = true;
				}
			}
			else
			{
				if(m_crashes <= 0)
				{
					m_timetoWaitAfterCrashTimer = 0.0f;
					CrashDetected = false;
					
					if(m_rigidbody)
						m_rigidbody.useGravity = false;

					if(m_colliderOnCrash)
					{
						m_colliderOnCrash.enabled = false;
						if(m_collider)
							m_collider.enabled    = true;
					}
				}
			}
		}
	}
}
