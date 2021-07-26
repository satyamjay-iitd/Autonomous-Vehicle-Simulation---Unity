using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatorController : MonoBehaviour
{
	public GameObject skyCar;
	public GameObject huracanCar;
	public GameObject objDetect;
	public GameObject objTrackingColourBased;
	// public GameObject rawImageDisplay;
    // Start is called beforre the first frame update
    // void Start()
    // {
        
    // 	// objTrackingColourBased.gameObject.SetActive(false);
    // 	// objDetect.gameObject.SetActive(false);
    // 	// rawImageDisplay.gameObject.SetActive(false);
    // }

    // Update is called once per frame
    void Update()
    {
    	if (Input.GetKeyUp(KeyCode.C))
    	{
    		skyCar.gameObject.SetActive(!skyCar.gameObject.activeSelf);
    		huracanCar.gameObject.SetActive(!huracanCar.gameObject.activeSelf);

    	}
    	if (Input.GetKeyUp(KeyCode.H))
    	{
    		skyCar.SetActive(false);
    		huracanCar.SetActive(true);
    	}
        
        if (Input.GetKeyUp(KeyCode.O))
    	{
    		objTrackingColourBased.gameObject.SetActive(false);
    		objDetect.gameObject.SetActive(!objDetect.gameObject.activeSelf);
    		// rawImageDisplay.gameObject.SetActive(!rawImageDisplay.gameObject.activeSelf);
    		
    	}
    	if (Input.GetKeyUp(KeyCode.T))
    	{
    		
    		objDetect.gameObject.SetActive(false);
    		objTrackingColourBased.gameObject.SetActive(!objTrackingColourBased.gameObject.activeSelf);
    		// rawImageDisplay.gameObject.SetActive(!rawImageDisplay.gameObject.activeSelf);
    		
    	}
    }
}
