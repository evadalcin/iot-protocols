using System;
using NetCoreClient.Sensors;
using NetCoreClient.ValueObjects;
using System.Text.Json;

namespace NetCoreClient.Sensors
{
    public class VirtualFilterConditionSensor : IFilterConditionSensorInterface, ISensorInterface
    {
        private readonly Random _random;

        public VirtualFilterConditionSensor()
        {
            _random = new Random();
        }

        public bool IsFilterOperational()
        {
            // Simuliamo lo stato del filtro con un valore casuale (true per operativo, false per non operativo)
            return new FilterCondition(_random.Next(0, 2) == 1).Value;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(IsFilterOperational());
        }
    }
}
