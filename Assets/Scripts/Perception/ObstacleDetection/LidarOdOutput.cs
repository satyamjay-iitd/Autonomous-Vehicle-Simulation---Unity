using System;
using System.Collections.Generic;
using System.Numerics;
using Controller;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Perception.ObstacleDetection
{
    public class LidarOdOutput: IOdOutput
    {
        public LidarOdOutput(List<Obstacle> obstacles)
        {
            Obstacles = obstacles;
        }
        public List<Obstacle> Obstacles { get; set; }
    }

    public class Obstacle
    {
        public Vector2                                   Center { get; set; }
        public Tuple<Vector2, Vector2, Vector2, Vector2> Bbox   { get; set; }
        public float                                     YMax   { get; set; }
        private int                                      Lane   { get; set; }
        public int AssignLane(List<float> leftLanes, List<float> rightLanes, IReadOnlyList<float> b1, IReadOnlyList<float> b2)
        {
            var fLx = Brain.CalculateY(b1, Center.Y);
            var fRx = Brain.CalculateY(b2, Center.Y);

            if (leftLanes.Count > 0
                && fLx + leftLanes[0] < Center.X
                && rightLanes.Count > 0
                && fRx + rightLanes[0] < Center.X)
            {
                Lane = 0;
                return 0;
            }
            if (leftLanes.Count > 1 && fLx + leftLanes[1] < Center.X && fLx + leftLanes[0] > Center.X)
            {
                Lane = -1;
                return -1;
            }
            for (var i = 2; i < leftLanes.Count; i++)   // check for object in other left lane
            {
                if(fLx + leftLanes[i] < Center.X && Center.X < fLx + leftLanes[i - 1]){
                    Lane = -i;
                    return -i;
                }
            }
            if (rightLanes.Count > 1 && fRx + rightLanes[1] > Center.X && fRx + rightLanes[0] > Center.X)
            {
                Lane = 1;
                return 1;
            }
            for (var i = 2; i < rightLanes.Count; i++)   // check for object in other right lane
            {
                if(fRx + rightLanes[i] > Center.X && Center.X > fRx + rightLanes[i - 1]){
                    Lane = i;
                    return -i;
                }
            }
            // Represents "Cannot assign lane"
            return -1000;
        }
    }

    public class ObstacleConverter : JsonConverter<Obstacle>
    {
        public override bool CanRead  => true;
        public override bool CanWrite => false;
        
        public override Obstacle ReadJson(JsonReader reader, Type objectType, Obstacle existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var obstacle = new Obstacle();
            var jsonObject = JObject.Load(reader);
            var centerCord = jsonObject.GetValue("Center") as JArray;
            obstacle.Center = new Vector2((float)centerCord[0], (float)centerCord[1]);
            var bbox = jsonObject.GetValue("Bbox") as JArray;
            var bbox0 = bbox[0] as JArray;
            var bbox1 = bbox[1] as JArray;
            var bbox2 = bbox[2] as JArray;
            var bbox3 = bbox[3] as JArray;
            obstacle.Bbox = new Tuple<Vector2, Vector2, Vector2, Vector2>(
                new Vector2((float)bbox0[0], (float)bbox0[1]),
                new Vector2((float)bbox1[0], (float)bbox1[1]),
                new Vector2((float)bbox2[0], (float)bbox2[1]),
                new Vector2((float)bbox3[0], (float)bbox3[1]));
            var yMax = (float)jsonObject.GetValue("YMax");
            obstacle.YMax = yMax;
            return obstacle;
        }
        
        public override void WriteJson(JsonWriter writer, Obstacle value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}