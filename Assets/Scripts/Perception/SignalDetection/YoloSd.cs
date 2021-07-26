using System;
using System.Diagnostics;
using Controller;
using ipc;
using Newtonsoft.Json;
using UnityEngine;
using Sensors;
using Debug = UnityEngine.Debug;

namespace Perception.SignalDetection
{
    [RequireComponent(typeof (SdCameraSensor))]
    public class YoloSd : SignalDetection
    {
        private Process _process;
        private SdCameraSensor _sdCameraSensor;
        private void Start()
        {
            var context = new ProcessStartInfo
            {
                FileName = "/home/janib/anaconda3/envs/spconv/bin/python",
                Arguments = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server/YoloTrafficDetectionProcess.py",
                WorkingDirectory = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server",
                UseShellExecute = true,
            };
            _process = Process.Start(context);
            _sdCameraSensor = gameObject.GetComponent<SdCameraSensor>();
            Debug.Log("Waiting for the Yolo process to start");
            while (Ipc.IsYoloOutputReady() == false) {}

            WorldState.IsSignalDetectionAvailable = true;
        }
        private void LateUpdate()
        {
            if (Ipc.IsYoloOutputReady())
            {
                WorldState.SdOutput = new YoloSdOutput(
                    JsonConvert.DeserializeObject<TrafficLightColor>(
                        Ipc.ReadYoloOutput(),
                    new YoloSdOutputConverter()));
                var s = Convert.ToBase64String(_sdCameraSensor.ReadData());
                Ipc.WriteYoloImg(s);
                Ipc.UnsetYoloOutputReady();
            }
        }
        private void OnApplicationQuit()
        { 
            _process.Kill();
        }
    }
    
}