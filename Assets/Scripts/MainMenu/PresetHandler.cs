using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class PresetHandler : MonoBehaviour
    {
        public TMP_Dropdown controllerDropdown;
        public TMP_Dropdown laneDetectionDropdown;
        public TMP_Dropdown objectDetectionDropdown;
        public Toggle proximitySensorToggle;
        public void OnValueChange(int val)
        {
            var autonomyLevel = val - 1;
            switch (autonomyLevel)
            {
                case 0:
                    // Select User controller and turn everything off.
                    SetValueByName(controllerDropdown, "UserCarController");
                    proximitySensorToggle.isOn = false;
                    SetValueByName(laneDetectionDropdown, "Off");
                    SetValueByName(objectDetectionDropdown, "Off");
                    break;
                case 1:
                    // Select User controller but turn on collision sensor.
                    SetValueByName(controllerDropdown, "UserCarController");
                    proximitySensorToggle.isOn = true;
                    SetValueByName(laneDetectionDropdown, "Off");
                    SetValueByName(objectDetectionDropdown, "Off");
                    break;
                case 2:
                    // Select Auto Controller, turn off lane detection and obstacle recognition.
                    SetValueByName(controllerDropdown, "AutoCarController");
                    proximitySensorToggle.isOn = true;
                    SetValueByName(laneDetectionDropdown, "Off");
                    SetValueByName(objectDetectionDropdown, "Off");
                    break;
                case 3:
                    // Select Auto Controller, turn on lane detection and obstacle recognition.
                    SetValueByName(controllerDropdown, "AutoCarController");
                    proximitySensorToggle.isOn = true;
                    SetValueByName(laneDetectionDropdown, "PinetLD");
                    SetValueByName(objectDetectionDropdown, "LidarOD");
                    break;
            }
        }

        private static void SetValueByName(TMP_Dropdown dropdown, string name)
        {
            var i = 0;
            foreach (var option in dropdown.options)
            {
                if (option.text.ToLower().Contains(name.ToLower()))
                    dropdown.value = i;
                i++;
            }
        }
    }
}

