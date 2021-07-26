using System;
using System.Collections.Generic;
using System.Diagnostics;
using Controller;
using ipc;
using Newtonsoft.Json;
using Sensors;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Perception.ObstacleDetection
{ 
    [RequireComponent(typeof(LidarSensor))]
    public class LidarOd : ObstacleDetection
    {
        private Process _process;
        private LidarSensor _lidarSensor;
        private void Start()
        {
            var context = new ProcessStartInfo
            {
                FileName = "/home/janib/anaconda3/envs/spconv/bin/python",
                Arguments = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server/ObstacleDetectionProcess.py",
                WorkingDirectory = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server",
                UseShellExecute = true,
            };
            _process = Process.Start(context);
            _lidarSensor = gameObject.GetComponent<LidarSensor>();
            Debug.Log("Waiting for the Lidar process to start");
            while (Ipc.IsLidarOutputReady() == false) {}
            WorldState.IsObstacleDetectionAvailable = true;
        }

        private void FixedUpdate()
        {
            if (Ipc.IsLidarOutputReady())
            {
                WorldState.OdOutput = new LidarOdOutput(JsonConvert.DeserializeObject<List<Obstacle>>(Ipc.ReadLidarOutput(), new ObstacleConverter()));
                var sensorData = _lidarSensor.ReadData();
                if (sensorData == null) return;
                var s = Convert.ToBase64String(sensorData);
                Ipc.WriteLidarImg(s);
                Ipc.UnsetLidarOutputReady();
            }
        }
        
        private void OnApplicationQuit()
        {
            _process.Kill();
        }
    }
}

