using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ipc
{
    public static class Ipc
    {
        
        [DllImport("sem", EntryPoint = "ReadInt", CharSet = CharSet.Ansi)]
        private static extern int ReadInt(int mmap, int size);
        
        [DllImport("sem", EntryPoint = "WriteInt", CharSet = CharSet.Ansi)]
        private static extern int WriteInt(int val, int mmap);
        
        [DllImport("sem", EntryPoint = "semaphore_open", CharSet = CharSet.Ansi)]
        private static extern int semaphore_open(string semname, int oflag, int val);
        
        [DllImport("sem", EntryPoint = "getO_Creat", CharSet = CharSet.Ansi)]
        private static extern int getO_Creat();
        
        [DllImport("sem", EntryPoint = "wait", CharSet = CharSet.Ansi)]
        private static extern void wait(int ind);
        
        [DllImport("sem", EntryPoint = "post", CharSet = CharSet.Ansi)]
        private static extern void post(int ind);
        
        [DllImport("sem", EntryPoint = "shared_mem", CharSet = CharSet.Ansi)]
        private static extern int shared_mem(string name, int size);

        [DllImport("sem", EntryPoint = "writeMMF", CharSet = CharSet.Ansi)]
        private static extern void writeMMF(string msg, int mmap);
        
        [DllImport("sem", EntryPoint = "readMMF")]
        private static extern IntPtr readMMF(int mmap);

        [DllImport("sem", EntryPoint = "getO_CREAT_ORDWR", CharSet = CharSet.Ansi)]
        private static extern int getO_CREAT_ORDWR();
        
        [DllImport("sem", EntryPoint = "getVal", CharSet = CharSet.Ansi)]
        private static extern void getVal(int mmap);

        [DllImport("sem", EntryPoint = "reset", CharSet = CharSet.Ansi)]
        private static extern void reset();
        
        private static bool _isReset;

        private static readonly int PinetImgLock;
        private static readonly int PinetOutputLock;
        private static readonly int PinetOutputReadyLock;
        
        private static readonly int PinetImgMmf;
        private static readonly int PinetOutputMmf;
        private static readonly int PinetOutputReadyMmf;
        
        private static readonly int LidarImgLock;
        private static readonly int LidarOutputLock;
        private static readonly int LidarOutputReadyLock;
        
        private static readonly int LidarImgMmf;
        private static readonly int LidarOutputMmf;
        private static readonly int LidarOutputReadyMmf;
        
        private static readonly int YoloImgLock;
        private static readonly int YoloOutputLock;
        private static readonly int YoloOutputReadyLock;
        
        private static readonly int YoloImgMmf;
        private static readonly int YoloOutputMmf;
        private static readonly int YoloOutputReadyMmf;
        
        static Ipc()
        {
            PinetImgMmf         = shared_mem("pinet_image_mmf", 1000000);
            PinetOutputMmf      = shared_mem("pinet_output_mmf", 32768);
            PinetOutputReadyMmf = shared_mem("pinet_ready_mmf", 4);
            
            PinetImgLock         = semaphore_open("pinet_image_lock", getO_Creat(), 1);
            PinetOutputLock      = semaphore_open("pinet_output_lock", getO_Creat(), 1);
            PinetOutputReadyLock = semaphore_open("pinet_ready_lock", getO_Creat(), 1);
            
            LidarImgMmf         = shared_mem("lidar_image_mmf", 1000000);
            LidarOutputMmf      = shared_mem("lidar_output_mmf", 65536);
            LidarOutputReadyMmf = shared_mem("lidar_ready_mmf", 4);
            
            LidarImgLock         = semaphore_open("lidar_image_lock", getO_Creat(), 1);
            LidarOutputLock      = semaphore_open("lidar_output_lock", getO_Creat(), 1);
            LidarOutputReadyLock = semaphore_open("lidar_ready_lock", getO_Creat(), 1);
            
            YoloImgMmf         = shared_mem("yolo_image_mmf", 1000000);
            YoloOutputMmf      = shared_mem("yolo_output_mmf", 256);
            YoloOutputReadyMmf = shared_mem("yolo_ready_mmf", 4);
            
            YoloImgLock         = semaphore_open("yolo_image_lock", getO_Creat(), 1);
            YoloOutputLock      = semaphore_open("yolo_output_lock", getO_Creat(), 1);
            YoloOutputReadyLock = semaphore_open("yolo_ready_lock", getO_Creat(), 1);
        }

        public static void Reset()
        {
            if (!_isReset)
            {
                reset();
                _isReset = true;
                Debug.Log("All locks and semaphores have been reset.");
            }
        }

        public static bool IsPinetOutputReady()
        {
            return ReadInt(PinetOutputReadyMmf, 4) == 1;
        }
        
        public static string ReadPinetOutput()
        {
            wait(PinetOutputLock);
            var output = readMMF(PinetOutputMmf);
            post(PinetOutputLock);
            var strResult = Marshal.PtrToStringAnsi(output);
            return strResult;
        }
        public static void WritePinetImg(string img)
        {
            wait(PinetImgLock);
            writeMMF(img, PinetImgMmf);
            post(PinetImgLock);
        }

        public static void UnsetPinetOutputReady()
        {
            wait(PinetOutputReadyLock);
            WriteInt(0, PinetOutputReadyMmf);
            post(PinetOutputReadyLock);
        }
        public static bool IsLidarOutputReady()
        {
            return ReadInt(LidarOutputReadyMmf, 4) == 1;
        }
        public static string ReadLidarOutput()
        {
            wait(LidarOutputLock);
            var output = readMMF(LidarOutputMmf);
            post(LidarOutputLock);
            var strResult = Marshal.PtrToStringAnsi(output);
            return strResult;
        }
        public static void WriteLidarImg(string img)
        {
            wait(LidarImgLock);
            writeMMF(img, LidarImgMmf);
            post(LidarImgLock);
        }
        public static void UnsetLidarOutputReady()
        {
            wait(LidarOutputReadyLock);
            WriteInt(0, LidarOutputReadyMmf);
            post(LidarOutputReadyLock);
        }
        
        public static bool IsYoloOutputReady()
        {
            return ReadInt(YoloOutputReadyMmf, 4) == 1;
        }
        public static string ReadYoloOutput()
        {
            wait(YoloOutputLock);
            var output = readMMF(YoloOutputMmf);
            post(YoloOutputLock);
            var strResult = Marshal.PtrToStringAnsi(output);
            return strResult;
        }
        public static void WriteYoloImg(string img)
        {
            wait(YoloImgLock);
            writeMMF(img, YoloImgMmf);
            post(YoloImgLock);
        }
        public static void UnsetYoloOutputReady()
        {
            wait(YoloOutputReadyLock);
            WriteInt(0, YoloOutputReadyMmf);
            post(YoloOutputReadyLock);
        }
    }
}
    
