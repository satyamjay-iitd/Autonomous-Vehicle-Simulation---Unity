using System;
using Controller;
using Perception.LaneDetection;
using Perception.ObstacleDetection;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

namespace MainMenu{
    public class BeginButtonOnClick : MonoBehaviour
    {
        public GameObject car;
        public TMP_Dropdown controllerDropdown;
        public TMP_Dropdown laneDetectionDropdown;
        public TMP_Dropdown obstacleDetectionDropdown;
        public TMP_Dropdown signalDetectionDropdown;
        public Toggle proximitySensorToggle; 
        
        public void OnClick()
        {
            // Select Controller
            var controllerType = GetSelectedType(controllerDropdown);
            if (controllerType == typeof(UserCarController)) {
                GameObject.Find("Car").GetComponent<UserCarController>().enabled = true;
                GameObject.Find("Car").GetComponent<AutoCarController>().enabled = false;
            }
            else if (controllerType == typeof(AutoCarController)) {
                GameObject.Find("Car").GetComponent<UserCarController>().enabled = false;
                GameObject.Find("Car").GetComponent<AutoCarController>().enabled = true;
                GameObject.Find("Car").GetComponent<AutoCarController>().startMoving = true;
            }
            else {
            }
            // Select Lane Detection
            var lDType = GetSelectedType(laneDetectionDropdown);
            if(lDType != null) GameObject.Find("Car").AddComponent(lDType);
            // Select Obstacle Detection
            var oDType = GetSelectedType(obstacleDetectionDropdown);
            if(oDType != null) GameObject.Find("Car").AddComponent(oDType);
            // Select Signal Detection
            var sDType = GetSelectedType(signalDetectionDropdown);
            if(sDType != null) GameObject.Find("Car").AddComponent(sDType);
            HideMenu();
        }

        private static Type GetSelectedType(TMP_Dropdown dropdown)
        {
            var cd = dropdown.options[dropdown.value] as CustomOptionData;
            if (cd.CustomData == "null")
                return null;
            else
            {
                return Type.GetType(cd.CustomData, true);
            }
            //return cd.CustomData == "null" ?  null : Type.GetType(cd.CustomData, true);
        }
        private static void HideMenu()
        {
            GameObject.Find("MainMenu").GetComponent<Canvas>().enabled = false;
        }
    }

}