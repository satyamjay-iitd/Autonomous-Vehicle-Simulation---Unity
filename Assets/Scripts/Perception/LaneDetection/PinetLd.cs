using System;
using System.Diagnostics;
using Controller;
using ipc;
using Newtonsoft.Json;
using UnityEngine;
using Sensors;
using Debug = UnityEngine.Debug;

namespace Perception.LaneDetection
{
    [RequireComponent(typeof (LdCameraSensor))]
    public class PinetLd : LaneDetection
    {
        private Process _process;
        private LdCameraSensor _ldCameraSensor;

        private void Start()
        {
            var context = new ProcessStartInfo
            {
                FileName = "/home/janib/anaconda3/envs/spconv/bin/python",
                Arguments = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server/LaneDetectionProcess.py",
                WorkingDirectory = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server",
                UseShellExecute = true,
            };
            _process = Process.Start(context);
            _ldCameraSensor = gameObject.GetComponent<LdCameraSensor>();
            Debug.Log("Waiting for the Pinet process to start");
            while (Ipc.IsPinetOutputReady() == false) {}
        }
        
        private void LateUpdate()
        {
            if (Ipc.IsPinetOutputReady())
            {
                WorldState.LdOutput = JsonConvert.DeserializeObject<PinetLdOutput>(Ipc.ReadPinetOutput());
                var s = Convert.ToBase64String(_ldCameraSensor.ReadData());
                Ipc.WritePinetImg(s);
                Ipc.UnsetPinetOutputReady();
            }
        }
        private void OnApplicationQuit()
        { 
            _process.Kill();
        }
    }
    
}
