using UnityEngine;
using System.Collections;

public class TrafficSystemFollowCamera : MonoBehaviour 
{
	public  bool         m_enableVehicleCamera               = true;
	public  bool         m_enableVehicleChangeMouse          = true;
	public  bool         m_enableVehicleChangeKeyboard       = true;
	public Camera        m_vehicleCamera                     = null;
	public TrafficSystemVehicle m_vehicleToFollow            = null;
	public float         m_vehicleCameraOffsetAbove          = 8.5f;
	public float         m_vehicleCameraOffsetBehind         = 6.0f;
	public Vector3       m_vehicleCameraRotation             = new Vector3(0.0f, -0.77f, 0.0f);
	[Range(0.0f, 10.0f)]
	public float         m_vehicleCameraFaceForwardDelay     = 2.0f;
	private int          m_vehicleCameraIndex                = 0;

	void Start () 
	{
		if(m_vehicleCamera && m_vehicleToFollow)
		{
			m_vehicleCamera.transform.position = m_vehicleToFollow.transform.position - (m_vehicleToFollow.transform.forward.normalized * m_vehicleCameraOffsetBehind) + new Vector3(0.0f, m_vehicleCameraOffsetAbove, 0.0f);
			m_vehicleCamera.transform.forward  = m_vehicleToFollow.transform.forward + m_vehicleCameraRotation;
		}
	}
	
	void Update () 
	{
		if(Application.isPlaying && m_enableVehicleCamera)
		{
			if(m_enableVehicleChangeKeyboard)
			{
				if(Input.GetKeyDown(KeyCode.Comma))
					AttachVehicleCameraToPreviousVehicle();
				else if(Input.GetKeyDown(KeyCode.Period))
					AttachVehicleCameraToNextVehicle();
			}
			
			if(m_enableVehicleChangeMouse && Input.GetMouseButtonDown(0) && m_vehicleCamera)
			{
				Ray ray = m_vehicleCamera.ScreenPointToRay (new Vector3(Input.mousePosition.x,Input. mousePosition.y, 0.0f));
				RaycastHit raycastHit;
				if(Physics.Raycast(ray, out raycastHit))
				{
					if(raycastHit.collider.GetComponent<TrafficSystemVehicle>())
						m_vehicleToFollow = raycastHit.collider.GetComponent<TrafficSystemVehicle>();
				}
			}
			
			if(m_vehicleCamera && m_vehicleToFollow)
			{
				m_vehicleCamera.transform.position = m_vehicleToFollow.transform.position - (m_vehicleToFollow.transform.forward.normalized * m_vehicleCameraOffsetBehind) + new Vector3(0.0f, m_vehicleCameraOffsetAbove, 0.0f);
				m_vehicleCamera.transform.forward  = Vector3.Slerp( m_vehicleCamera.transform.forward, m_vehicleToFollow.transform.forward + m_vehicleCameraRotation, m_vehicleCameraFaceForwardDelay * Time.deltaTime );
			}
		}
	}

	public void AttachVehicleCameraToPreviousVehicle()
	{
		if(!TrafficSystem.Instance)
			return;

		if(TrafficSystem.Instance.GetSpawnedVehicles().Count <= 0)
			return;

		if(!m_vehicleCamera)
			return;
		
		m_vehicleCameraIndex--;
		if(m_vehicleCameraIndex < 0)
			m_vehicleCameraIndex = TrafficSystem.Instance.GetSpawnedVehicles().Count - 1;
		
		m_vehicleToFollow = TrafficSystem.Instance.GetSpawnedVehicles()[m_vehicleCameraIndex];
	}
	
	public void AttachVehicleCameraToNextVehicle()
	{
		if(TrafficSystem.Instance.GetSpawnedVehicles().Count <= 0)
			return;
		
		if(!m_vehicleCamera)
			return;

		if(!m_vehicleCamera)
			return;
		
		m_vehicleCameraIndex++;
		if(m_vehicleCameraIndex >= TrafficSystem.Instance.GetSpawnedVehicles().Count)
			m_vehicleCameraIndex = 0;
		
		m_vehicleToFollow = TrafficSystem.Instance.GetSpawnedVehicles()[m_vehicleCameraIndex];
	}
}
