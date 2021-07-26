	/// <summary>
/// 
/// Traffic system vehicle player.
/// 
/// All you have to do is drop the "TrafficSystemVehiclePlayer.cs" script onto any part of your player vehicle that has a collider on it,
/// or to be more correct, any part of your vehicle that you "want" to collider with the Traffic System vehicles.
/// 
/// The colliders for the player can be collision or triggers, the script will detect both. The Traffic System vehicles will then detect
/// that a player controlled vehicle has collidered with them and they will brake and use the correct physics for collision.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class TrafficSystemVehiclePlayerAuto : TrafficSystemVehicle 
{
	public delegate void HasEnteredTrafficLightTrigger( TrafficSystemTrafficLight a_trafficLight );
	public HasEnteredTrafficLightTrigger hasEnteredTrafficLightTrigger;

	public bool          WaitForUserInput       { get; set; }
	public bool          ChangeLaneActivated    { get; set; }

	public override void Start () 
	{
		WaitForUserInput    = false;
		ChangeLaneActivated = false;

		base.Start();
		hasEnteredTrafficLightTrigger += ProcessHasEnteredTrafficLightTrigger;
	}

	public override void Update () 
	{
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
		
		if(IsStuck())
		{
			m_stuckDestroyTimer += Time.deltaTime;
			if(m_stuckDestroyTimer >= m_stuckDestroyTimerMax)
			{
				if (Debug.isDebugBuild)
					Debug.LogWarning("Vehicle Destroyed - m_stuckDestroyTimerMax - Vehicle had unusual behaviour");
				
				Kill();
			}
		}
		else
		{
			m_stuckDestroyTimer   = 0.0f;
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

			if(!VehicleHit && m_enableVehicleCheckMid)
			{
				RaycastHit[] hitInfo = Physics.SphereCastAll( (transform.position + m_vehicleCheckOffset), (m_vehicleCheckRadius * m_vehicleCheckRadiusMid ), transform.forward, m_vehicleCheckDist - m_vehicleCheckDistMid );
				
				VehicleHit  = null;
				for(int hIndex = 0; hIndex < hitInfo.Length; hIndex++)
				{
					TrafficSystemVehicle vehicle = hitInfo[hIndex].transform.GetComponent<TrafficSystemVehicle>();
					if(vehicle)
					{
						if(vehicle.VehicleHit != this )
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
		
		if(!StopMoving && !WaitForUserInput && TrafficLight && !CrashDetected) // determine if there is there is room to move throught the intersection
		{
			if(TrafficLight.m_status == TrafficSystemTrafficLight.Status.RED ||
			   TrafficLight.m_status == TrafficSystemTrafficLight.Status.YELLOW)
			{
				//				print ("TrafficLight = RED OR YELLOW");
				StopMoving = true;
			}
			else if(CanSeeVehicleViaSphere())
			{
				//				print ("CanSeeVehicleViaSphere: = TRUE");
				StopMoving = true;
			}
			else if(!CanFitAcrossIntersection())
			{
				//				print ("CanFitAcrossIntersection: = FALSE");
				StopMoving = true;
			}
			else if(TrafficLight.m_status == TrafficSystemTrafficLight.Status.GREEN)
			{
				if(TrafficLight.m_intersection)
				{
					if(TrafficLight.m_intersection.m_intersectionSystem == TrafficSystemIntersection.IntersectionSystem.SINGLE)
					{
						//						print ("GO!");
						TrafficLight = null;
						StopMoving         = false;
					}
					else if(TrafficLight.m_intersection.m_intersectionSystem == TrafficSystemIntersection.IntersectionSystem.DUAL)
					{
						if(IsTurningIntoIncomingTraffic())
						{
							//							print ("IsTurningIntoIncomingTraffic: = TRUE");
							StopMoving         = true;
						}
						else
						{
							//							print ("GO!");
							TrafficLight = null;
							StopMoving         = false;
						}
					}
				}
				else
					Debug.LogWarning("Traffic System Traffic Light is missing a parent intersection! See the example traffic system intersection and the structure setup as this is what you need to mimic");
			}
			
			//			print ("TrafficLight: " + TrafficLight);
		}
		
		if(!StopMoving && !WaitForUserInput && m_nextNode && !CrashDetected)// && !m_pathingStarted)
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
			
			if(dir.magnitude < m_nextNodeThreshold)                                     // if we are close enough to the node we were travelling towards then check for the next one
			{
				if(m_nextNode.IsDirectionChange())
					WaitForUserInput = true;
				else
					SetNextNode( m_nextNode.GetNextNode( this, true/*, blockedNodes*/ ) );  // gets the next required node to go to and start the process all over again

				ChangeLaneActivated = false;
			}
		}
		else if(CrashDetected)
		{
			// don't stop, but don't do anything
		}
		else if(StopMoving || WaitForUserInput)
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

	public void ProcessHasEnteredTrafficLightTrigger( TrafficSystemTrafficLight a_trafficLight )
	{
		// Debug.Log("Hit " + a_trafficLight + " and the light was " + a_trafficLight.m_status);
		// put code here...
	}

	void OnGUI()
	{
		if(!TrafficSystem.Instance)
			return;

		if(!TrafficSystem.Instance.VehicleHasFocus( this ))
			return;

		if(!m_nextNode)
			return;

		Vector2 buttonSize      = new Vector2(100.0f, 30.0f);
		float   buttonSpacerX   = 20.0f;
		float   buttonSpacerY   = 40.0f;
		float   initScreenPosY  = 10.0f;

		if(m_nextNode.IsDirectionChange())
		{
			int buttonCount = 3;

			if(m_nextNode.m_directionListing == TrafficSystemNode.DirectionListing.LEFT_RIGHT_STRAIGHT_UTURN)
				buttonCount = 4;

			float initScreenPosX = (Screen.width / 2) - ((buttonSize.x + buttonSpacerX) * (buttonCount / 2.0f));

			int btnIndex = 0;
			if(m_nextNode.IsDirectionLeft())
			{
				if(GUI.Button(new Rect(initScreenPosX + ((buttonSize.x + buttonSpacerX) * btnIndex), initScreenPosY, buttonSize.x, buttonSize.y), "LEFT"))
				{
					TrafficSystemNode node = null;
					if(m_nextNode.IsRoundabout())
					{
						for(int cIndex = 0; cIndex < m_nextNode.m_connectedChangeLaneNodes.Count; cIndex++)
						{
							if(!node && m_nextNode.m_connectedChangeLaneNodes[cIndex].m_roundaboutExit == TrafficSystemNode.RoundaboutExit.EXIT_3)
							{
								node = m_nextNode.m_connectedChangeLaneNodes[cIndex];
								break;
							}
						}
					}
					else
					{
						for(int cIndex = 0; cIndex < m_nextNode.m_connectedChangeLaneNodes.Count; cIndex++)
						{
							if(!node && m_nextNode.m_connectedChangeLaneNodes[cIndex].m_driveSide == TrafficSystem.DriveSide.LEFT)
							{
								node = m_nextNode.m_connectedChangeLaneNodes[cIndex];
								break;
							}
						}
					}

					if(node)
						SetNextNode( node );

					WaitForUserInput = false;
				}
			}
			btnIndex++;

			if(m_nextNode.IsDirectionStraight())
			{
				if(GUI.Button(new Rect(initScreenPosX + ((buttonSize.x + buttonSpacerX) * btnIndex), initScreenPosY, buttonSize.x, buttonSize.y), "STRAIGHT"))
				{
					TrafficSystemNode node = null;
					if(m_nextNode.IsRoundabout())
					{
						for(int cIndex = 0; cIndex < m_nextNode.m_connectedChangeLaneNodes.Count; cIndex++)
						{
							if(!node && m_nextNode.m_connectedChangeLaneNodes[cIndex].m_roundaboutExit == TrafficSystemNode.RoundaboutExit.EXIT_2)
							{
								node = m_nextNode.m_connectedChangeLaneNodes[cIndex];
								break;
							}
						}
					}
					else
					{
						for(int cIndex = 0; cIndex < m_nextNode.m_connectedNodes.Count; cIndex++)
						{
							if(!node && m_nextNode.m_driveSide == m_nextNode.m_connectedNodes[cIndex].m_driveSide)
							{
								node = m_nextNode.m_connectedNodes[cIndex];
								break;
							}
						}
					}

					if(node)
						SetNextNode( node );

					WaitForUserInput = false;
				}
			}
			btnIndex++;

			if(m_nextNode.IsDirectionRight())
			{
				if(GUI.Button(new Rect(initScreenPosX + ((buttonSize.x + buttonSpacerX) * btnIndex), initScreenPosY, buttonSize.x, buttonSize.y), "RIGHT"))
				{
					TrafficSystemNode node = null;
					if(m_nextNode.IsRoundabout())
					{
						for(int cIndex = 0; cIndex < m_nextNode.m_connectedChangeLaneNodes.Count; cIndex++)
						{
							if(!node && m_nextNode.m_connectedChangeLaneNodes[cIndex].m_roundaboutExit == TrafficSystemNode.RoundaboutExit.EXIT_1)
							{
								node = m_nextNode.m_connectedChangeLaneNodes[cIndex];
								break;
							}
						}
					}
					else
					{
						for(int cIndex = 0; cIndex < m_nextNode.m_connectedChangeLaneNodes.Count; cIndex++)
						{
							if(!node && m_nextNode.m_connectedChangeLaneNodes[cIndex].m_driveSide == TrafficSystem.DriveSide.RIGHT)
							{
								node = m_nextNode.m_connectedChangeLaneNodes[cIndex];
								break;
							}
						}
					}

					if(node)
						SetNextNode( node );

					WaitForUserInput = false;
				}
			}
			btnIndex++;

			if(m_nextNode.IsDirectionUTurn())
			{
				if(GUI.Button(new Rect(initScreenPosX + ((buttonSize.x + buttonSpacerX) * btnIndex), initScreenPosY, buttonSize.x, buttonSize.y), "U-TURN"))
				{
					TrafficSystemNode node = null;
					if(m_nextNode.IsRoundabout())
					{
						for(int cIndex = 0; cIndex < m_nextNode.m_connectedChangeLaneNodes.Count; cIndex++)
						{
							if(!node && m_nextNode.m_connectedChangeLaneNodes[cIndex].m_roundaboutExit == TrafficSystemNode.RoundaboutExit.EXIT_4)
							{
								node = m_nextNode.m_connectedChangeLaneNodes[cIndex];
								break;
							}
						}
					}

					if(node)
						SetNextNode( node );

					WaitForUserInput = false;
				}
			}
			btnIndex++;
		}
		else if(!ChangeLaneActivated && m_nextNode.m_roadType == TrafficSystem.RoadType.LANES_MULTI && m_nextNode.m_connectedNodes.Count > 1)
		{
			int     buttonCount     = 2;
			float   initScreenPosX  = (Screen.width / 2) - ((buttonSize.x + buttonSpacerX) * (buttonCount / 2.0f));

			bool isOnHighwayAndCanChangeLanes = false;
			if(m_nextNode.IsHighway() && m_nextNode.IsHighwayChangeLaneAccepted() && m_vehicleCheckOvertakeOnHighway)
				isOnHighwayAndCanChangeLanes = true;

			if(!isOnHighwayAndCanChangeLanes)
				return;

			TrafficSystemNode rightNode = null;
			TrafficSystemNode leftNode  = null;
			for(int cIndex = 0; cIndex < m_nextNode.m_connectedNodes.Count; cIndex++)
			{
				if(!rightNode && (m_nextNode.m_lane - 1) == m_nextNode.m_connectedNodes[cIndex].m_lane)
					rightNode = m_nextNode.m_connectedNodes[cIndex];
				if(!leftNode && (m_nextNode.m_lane + 1) == m_nextNode.m_connectedNodes[cIndex].m_lane)
					leftNode = m_nextNode.m_connectedNodes[cIndex];
			}

			int btnIndex = 0;
			if(leftNode)
			{
				if(GUI.Button(new Rect(initScreenPosX + ((buttonSize.x + buttonSpacerX) * btnIndex), initScreenPosY, buttonSize.x, buttonSize.y), "LEFT"))
					SetNextNode( leftNode);
			}
			btnIndex++;

			if(rightNode)
			{
				if(GUI.Button(new Rect(initScreenPosX + ((buttonSize.x + buttonSpacerX) * btnIndex), initScreenPosY, buttonSize.x, buttonSize.y), "RIGHT"))
					SetNextNode( rightNode);
			}
			btnIndex++;
		}
	}
}
