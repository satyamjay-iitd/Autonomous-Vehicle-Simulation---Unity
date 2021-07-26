using System.IO;
using UnityEngine;

namespace Sensors
{
    // public class LidarSensor : MonoBehaviour
    // {
    //     public int numChannels = 20;
    //     public int numRays = 36;
    //
    //     private Rigidbody _rigidCar;
    //     private Vector3[,] _dirs;
    //     private Quaternion _horQuat;
    //     private volatile int _pointsIdx;
    //     private const float Res = 0.5f;
    //     private const float Floor = -1.2f;
    //     
    //     private const int OffsetX = 40, OffsetY = 40;
    //     private Texture2D _voxelGrid;
    //     
    //     // Start is called before the first frame update
    //     void Start()
    //     {
    //         _pointsIdx = 0;
    //         _voxelGrid=new Texture2D(OffsetX*2, OffsetY*2, TextureFormat.RGB24, false);
    //         Quaternion vertAngleUp = Quaternion.AngleAxis(1f, transform.right);
    //         Quaternion vertAngleDown = Quaternion.AngleAxis(-1f, transform.right);
    //         _dirs = new Vector3[numRays, numChannels+3];
    //         _horQuat = Quaternion.AngleAxis(1, transform.up);
    //         _dirs[0, 0] = vertAngleDown * transform.forward;
    //         for (int i = 1; i < 3; i++)
    //         {
    //             _dirs[0, i] = vertAngleDown * _dirs[0, i - 1];
    //         }
    //
    //         _dirs[0, 3] = vertAngleUp * transform.forward;
    //         for (int i = 4; i < numChannels+3; i++)
    //         {
    //             _dirs[0, i] = vertAngleUp * _dirs[0, i - 1];
    //         }
    //
    //         float tempAngle = 360.0f / numRays;
    //         for (int i = 1; i < numRays; i++)
    //         {
    //             Quaternion temp = Quaternion.AngleAxis(tempAngle, transform.up);
    //             for (int j = 0; j < numChannels+3; j++)
    //             {
    //                 _dirs[i, j] = temp * _dirs[i - 1, j];
    //             }
    //         }
    //
    //         _rigidCar = (Rigidbody) GetComponentInParent(typeof(Rigidbody));
    //     }
    //
    //     // Update is called once per frame
    //     private void FixedUpdate()
    //     {
    //         var origin = transform.position + transform.up;
    //         for (int j = 0; j < numRays; j++)
    //         {
    //             for (int i = 0; i < numChannels+3; i++)
    //             {
    //                 var isHit = Physics.Raycast(origin, _dirs[j, i], out var outhit, 20f);
    //                 if (isHit && !outhit.collider.CompareTag("Terrain"))
    //                 {
    //                     var tmp = transform.InverseTransformPoint(outhit.point);
    //                     if(tmp[1]>Floor)
    //                       _voxelGrid.SetPixel((int)(tmp[0]/Res)+OffsetY, (int)(tmp[2]/Res)+OffsetX, Color.white);
    //                 }
    //                 _dirs[j, i] = _horQuat * _dirs[j, i];
    //             }
    //         }
    //         _pointsIdx++;
    //         if(_pointsIdx >= 5){ 
    //             _voxelGrid.Apply();
    //             byte[] image = _voxelGrid.EncodeToPNG();
    //             string strPts = Convert.ToBase64String(image);
    //             _voxelGrid = new Texture2D(OffsetX*2, OffsetY*2, TextureFormat.RGB24, false);
    //             int speed = (int) (_rigidCar.velocity.magnitude * 100);
    //             WritePcAndSpeed(strPts, speed);
    //             _pointsIdx=0;
    //         }
    //     }
    //
    //     private void OnApplicationQuit()
    //     {
    //         Ipc.Reset();
    //     }
    // }
    
    public class LidarSensor : MonoBehaviour, ISensor<byte []>
    {
        public int numChannels = 20;
        public int numRays = 36;
        public int density = 5;
        private Transform _lidarPos;
        
        private Vector3[,] _dirs;
        private Quaternion _horQuat;
        private volatile int _pointsIdx;
        private const float Res = 0.5f;
        private const float Floor = -1.2f;
        
        private const int OffsetX = 40, OffsetY = 40;
        private Texture2D _voxelGrid;
        
        // Start is called before the first frame update
        private void Start()
        {
            _lidarPos = GameObject.Find("LIDAR").transform;
            _pointsIdx = 0;
            _voxelGrid=new Texture2D(OffsetX*2, OffsetY*2, TextureFormat.RGB24, false);
            var vertAngleUp = Quaternion.AngleAxis(1f, _lidarPos.right);
            var vertAngleDown = Quaternion.AngleAxis(-1f, _lidarPos.right);
            _dirs = new Vector3[numRays, numChannels+3];
            _horQuat = Quaternion.AngleAxis(1, _lidarPos.up);
            _dirs[0, 0] = vertAngleDown * _lidarPos.forward;
            for (int i = 1; i < 3; i++)
            {
                _dirs[0, i] = vertAngleDown * _dirs[0, i - 1];
            }
        
            _dirs[0, 3] = vertAngleUp * _lidarPos.forward;
            for (int i = 4; i < numChannels+3; i++)
            {
                _dirs[0, i] = vertAngleUp * _dirs[0, i - 1];
            }
        
            float tempAngle = 360.0f / numRays;
            for (int i = 1; i < numRays; i++)
            {
                Quaternion temp = Quaternion.AngleAxis(tempAngle, _lidarPos.up);
                for (int j = 0; j < numChannels+3; j++)
                {
                    _dirs[i, j] = temp * _dirs[i - 1, j];
                }
            }
        }
        
        public byte[] ReadData()
        {
            var origin = _lidarPos.position + _lidarPos.up;
            for (var j = 0; j < numRays; j++)
            {
                for (var i = 0; i < numChannels+3; i++)
                {
                    var isHit = Physics.Raycast(origin, _dirs[j, i], out var outhit, 20f);
                    if (isHit && !outhit.collider.CompareTag("Terrain"))
                    {
                        var tmp = _lidarPos.InverseTransformPoint(outhit.point);
                        if(tmp[1]>Floor)
                            _voxelGrid.SetPixel((int)(tmp[0]/Res)+OffsetY, (int)(tmp[2]/Res)+OffsetX, Color.white);
                    }
                    _dirs[j, i] = _horQuat * _dirs[j, i];
                }
            }
            _pointsIdx++;
            if(_pointsIdx >= density){ 
                _voxelGrid.Apply();
                var image = _voxelGrid.EncodeToPNG();
                _voxelGrid = new Texture2D(OffsetX*2, OffsetY*2, TextureFormat.RGB24, false);
                _pointsIdx = 0;
                return image;
            }
            else
            {
                return null;
            }
        }
    }
}