using NetCoreClient.ValueObjects;
using System.Text.Json;

namespace NetCoreClient.Sensors
{
    class VirtualWaterTempSensor : IWaterTempSensorInterface, ISensorInterface
    {
        private readonly Random Random;

        public VirtualWaterTempSensor()
        {
            Random = new Random();
        }

        public int WaterTemperature()
        {
            return new WaterTemperature(Random.Next(20)).Value;
        }

        //public string ToJson()
        //{
        //    return JsonSerializer.Serialize(WaterTemperature());
        //}


        public string ToJson()
        {
            // Crea un oggetto con la proprietà waterTemperature
            var sensorData = new
            {
                waterTemperature = WaterTemperature()
            };

            // Serializza l'oggetto come JSON
            return JsonSerializer.Serialize(sensorData);
        }
    }
}
