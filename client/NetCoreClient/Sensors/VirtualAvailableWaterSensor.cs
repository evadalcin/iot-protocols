using System;
using NetCoreClient.Sensors;
using NetCoreClient.ValueObjects;
using System.Text.Json;

namespace NetCoreClient.Sensors
{
    public class VirtualAvailableWaterSensor : IAvailableWaterSensorInterface, ISensorInterface
    {
        private readonly Random _random;

        public VirtualAvailableWaterSensor()
        {
            _random = new Random();
        }

        public int AvailableWaterAmount()
        {
             return new AvailableWaterAmount(_random.Next(0, 1000)).Value;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(AvailableWaterAmount());
        }
    }
}
