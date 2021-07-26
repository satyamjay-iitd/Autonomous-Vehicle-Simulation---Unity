using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Perception.SignalDetection
{
    public enum TrafficLightColor
    {
        Red,
        Green
    }
    public class YoloSdOutput: ISdOutput
    {
        public YoloSdOutput(TrafficLightColor color)
        {
            TrafficLightColor = color;
        }
        public TrafficLightColor TrafficLightColor     { get; set; }
    }

    public class YoloSdOutputConverter : JsonConverter<TrafficLightColor>
    {
        public override bool CanRead  => true;
        public override bool CanWrite => false;
        
        public override TrafficLightColor ReadJson(JsonReader reader, Type objectType, TrafficLightColor existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var colorChar = jsonObject.GetValue("signal");
            if ((char) colorChar == 'g') return TrafficLightColor.Green;
            return TrafficLightColor.Red;
        }
        
        public override void WriteJson(JsonWriter writer, TrafficLightColor value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}