using System;
using UnityEngine;

namespace Sensors
{
    public class LdCameraSensor : MonoBehaviour, ISensor<byte[]>
    {
        private Camera _camera;
        public string cameraName;

        private void Start()
        {
            _camera = GameObject.Find("LaneDetectionCamera").GetComponent<Camera>();
        }
        
        public byte[] ReadData()
        {
            var camTargetTexture = _camera.targetTexture;
            RenderTexture targetTexture = camTargetTexture;
            RenderTexture.active = camTargetTexture;
            Texture2D texture = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0, false);
            texture.Apply();
            byte[] image = texture.EncodeToPNG();

            return image;
        }
    }
}

