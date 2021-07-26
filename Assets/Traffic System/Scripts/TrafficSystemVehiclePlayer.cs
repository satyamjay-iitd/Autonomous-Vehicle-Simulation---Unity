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

public class TrafficSystemVehiclePlayer : TrafficSystemVehicle 
{
	public delegate void HasEnteredTrafficLightTrigger( TrafficSystemTrafficLight a_trafficLight );
	public HasEnteredTrafficLightTrigger hasEnteredTrafficLightTrigger;

	/// <summary>
	/// To use the HasEnteredTrafficLightTrigger all you need to do is add this to your script or put code in the function below
	/// 
	/// in void Start -
	///     [TrafficSystemVehiclePlayer].hasEnteredTrafficLightTrigger += ProcessHasEnteredTrafficLightTrigger;
	/// 
	/// in void Destroy -
	///     [TrafficSystemVehiclePlayer].hasEnteredTrafficLightTrigger -= ProcessHasEnteredTrafficLightTrigger;
	///
	/// Then define your own function
	///    void ProcessHasEnteredTrafficLightTrigger( TrafficSystemTrafficLight a_trafficLight )
	///    {
	///   	  do something in here...
	///    }
	/// 
	/// </summary>

	public override void Start () 
	{
		hasEnteredTrafficLightTrigger += ProcessHasEnteredTrafficLightTrigger;

		// no need to do anyting, we just need to override TrafficSystemVehicle since this is the player
	}

	public override void Update () 
	{
		// no need to do anyting, we just need to override TrafficSystemVehicle since this is the player
	}

	public void ProcessHasEnteredTrafficLightTrigger( TrafficSystemTrafficLight a_trafficLight )
	{
		// Debug.Log("Hit " + a_trafficLight + " and the light was " + a_trafficLight.m_status);
		// put code here...
	}
}
