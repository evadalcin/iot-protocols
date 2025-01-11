using System;
using NetCoreClient.Sensors;
using NetCoreClient.ValueObjects;
using System.Text.Json;

namespace NetCoreClient.Sensors
{
    public class WaterLevelSensor : IWaterLevelSensor, ISensorInterface
    {
        private readonly Random _random;

        public WaterLevelSensor()
        {
            _random = new Random();
        }

        public int GetAvailableWater()
        {
            return new AvailableWaterAmount(_random.Next(0, 1000)).Value;
        }

        public string ToJson()
        {
            var sensorData = new
            {
                Name = GetSlug(),
                Value = GetAvailableWater()
            };

            return JsonSerializer.Serialize(sensorData);
        }

        public string GetSlug()
        {
            return "waterLevel";
        }
    }
}
