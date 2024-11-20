using NetCoreClient.ValueObjects;
using System;
using System.Text.Json;

namespace NetCoreClient.Sensors
{
    class WaterTempSensor : IWaterTempSensor, ISensorInterface
    {
        private readonly Random _random;

        public WaterTempSensor()
        {
            _random = new Random();
        }

        public int GetTemperature()
        {
            return new WaterTemperature(_random.Next(20)).Value;
        }

        public string ToJson()
        {
            var sensorData = new
            {
                Name = GetSlug(),
                Value = GetTemperature()
            };

            return JsonSerializer.Serialize(sensorData);
        }

        public string GetSlug()
        {
            return "temperature";
        }
    }
}
