using System.Collections.Generic;

namespace Perception.LaneDetection
{
    public class PinetLdOutput: ILdOutput
    {
        public float SteeringAngle      { get; set; }
        public int TotalPts             { get; set; }
        public int NumLeftLanes         { get; set; }
        public int NumRightLanes        { get; set; }
        public float cLyMax             { get; set; }
        public float cRyMax             { get; set; }
        public IReadOnlyList<float> B1  { get; set; }
        public IReadOnlyList<float> B2  { get; set; }
        public int Offset               { get; set; }
    }
}