#define WEB_DEMO_ENABLED

using UnityEngine;
using System.Collections;

public class TrafficSystemUI : MonoBehaviour 
{
	public static TrafficSystemUI Instance { get; set; }
	public  Camera   m_camera                     = null;
	private int      m_spawnedVehicleIndex        = 0;

	public  Material[] m_materials; 

	#if WEB_DEMO_ENABLED
	public  TBOrbit  m_TBOrbit                    = null;
	#endif

	public  Shader   m_mobileVertexLit;
	public  Shader   m_mobileDiffuseLit;
	private bool     m_diffuseOn                  = true;

	void Awake()
	{
		if(Instance != null)
		{
			Destroy(this);
			return;
		}

		Instance = this;

		#if WEB_DEMO_ENABLED
		if(!m_TBOrbit)
			m_TBOrbit = GetComponent<TBOrbit>();
		#endif

		if(!m_camera)
			m_camera = GetComponent<Camera>();

		ChangeMaterials( m_diffuseOn );
	}

	void Update()
	{
		if(Instance != this)
			return;

		if(Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();

		#if WEB_DEMO_ENABLED
		if(Input.GetMouseButtonDown(0))
		{
			if(m_camera && m_TBOrbit)
			{
				Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] rays = Physics.RaycastAll(ray);
				
				for(int hIndex = 0; hIndex < rays.Length; hIndex++)
				{
					TrafficSystemVehicle vehicle = rays[hIndex].transform.GetComponent<TrafficSystemVehicle>();
					if(vehicle)
					{
						m_TBOrbit.target = vehicle.transform;
						break;
					}
				}
			}
		}
		#endif
	}

	void OnGUI()
	{
		if(Instance != this)
			return;

#if UNITY_ANDROID || UNITY_IPHONE
		string str = "Turn ON Shadows";
		if(m_diffuseOn)
			str = "Turn OFF Shadows";

		if( GUI.Button( new Rect( Screen.width - 140.0f, 10.0f, 130.0f, 30.0f ), str ) )
		{
			m_diffuseOn = !m_diffuseOn;
			ChangeMaterials(m_diffuseOn);
		}
		#if WEB_DEMO_ENABLED
		GUI.Label( new Rect( Screen.width / 2 - 100.0f, Screen.height - 30.0f, 200.0f, 20.0f ), "Tap on a vehicle to follow it" );

		#endif
#else
		#if WEB_DEMO_ENABLED
		GUI.Label( new Rect( Screen.width / 2 - 100.0f, Screen.height - 30.0f, 200.0f, 20.0f ), "Click on a vehicle to follow it" );
		#endif
#endif

//		return; // redundant with the click to focus option now in play
//
//		if(!TrafficSystem.Instance)
//			return;
//
//		if(TrafficSystem.Instance.m_spawnedVehicles.Count > 0)
//		{
//			if( GUI.Button( new Rect( 10.0f, 10.0f, 120.0f, 20.0f ), "Previous Vehicle" ) )
//			{
//				if(m_spawnedVehicleIndex < 0)
//					m_spawnedVehicleIndex = TrafficSystem.Instance.m_spawnedVehicles.Count - 1;
//
//				//m_TBOrbit.target = TrafficSystem.Instance.m_spawnedVehicles[m_spawnedVehicleIndex].transform;
//				m_spawnedVehicleIndex--;
//			}
//
//			if( GUI.Button( new Rect( 140.0f, 10.0f, 120.0f, 20.0f ), "Next Vehicle" ) )
//			{
//				if(m_spawnedVehicleIndex >= TrafficSystem.Instance.m_spawnedVehicles.Count)
//					m_spawnedVehicleIndex = 0;
//				
//				//m_TBOrbit.target = TrafficSystem.Instance.m_spawnedVehicles[m_spawnedVehicleIndex].transform;
//				m_spawnedVehicleIndex++;
//			}
//		}
	}

	void ChangeMaterials( bool a_diffuseOn )
	{
		for(int mIndex = 0; mIndex < m_materials.Length; mIndex++)
		{
			Material mat = m_materials[mIndex];

			if(a_diffuseOn)
				mat.shader = m_mobileDiffuseLit;
			else
				mat.shader = m_mobileVertexLit;
		}
	}

	public void AssignVehicleToFollow( TrafficSystemVehicle a_vehicle )
	{
		#if WEB_DEMO_ENABLED
		if(m_camera && m_TBOrbit && !m_TBOrbit.target && a_vehicle.name.Contains("Full Test Car"))
			m_TBOrbit.target = a_vehicle.transform;
		#endif
	}
}
