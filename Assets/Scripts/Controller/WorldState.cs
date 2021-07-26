using System.Numerics;
using Perception.LaneDetection;
using Perception.ObstacleDetection;
using Perception.SignalDetection;

namespace Controller
{ 

    public enum LaneChangeEnum
    {
        ToLeft = -1,
        NoChange = 0,
        ToRight = 1
    }
    public static class WorldState
    {
        public static PinetLdOutput PrevLdOutput        { get; set; }
        public static LidarOdOutput PrevOdOutput        { get; set; }
        public static PinetLdOutput LdOutput            { get; set; }
        public static LidarOdOutput OdOutput            { get; set; }
        public static YoloSdOutput SdOutput             { get; set; }
        public static Vector2[] CarPos                  { get; set; }
        public static float SteeringAngle               { get; set; }
        public static bool FootBrake                    { get; set; } = false;
        public static int CurrentLane                   { get; set; }
        public static LaneChangeEnum IsChangingLane     { get; set; }
        public static bool IsObstacleDetectionAvailable { get; set; }
        public static bool IsSignalDetectionAvailable   { get; set; }        

        static WorldState()
        {
            PrevLdOutput = null;
            PrevOdOutput = null;
            LdOutput = null;
            OdOutput = null;
            CarPos = new[]
            {
                new Vector2(255.718687f, 356.866262f), new Vector2(255.718687f, 356.866262f),
                new Vector2(255.718687f, 356.866262f), new Vector2(255.718687f, 356.866262f)
            };
            CurrentLane = 4;
            SteeringAngle = 0;
            IsChangingLane = LaneChangeEnum.NoChange;
        }
    }
}