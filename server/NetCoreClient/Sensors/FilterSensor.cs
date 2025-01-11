using System;
using NetCoreClient.Sensors;
using NetCoreClient.ValueObjects;
using System.Text.Json;

namespace NetCoreClient.Sensors
{
    public class FilterSensor : IFilterSensor, ISensorInterface
    {
        private readonly Random _random;

        public FilterSensor()
        {
            _random = new Random();
        }

        public int IsOperational()
        {
            return new FilterCondition(_random.Next(0, 2) == 1).Value ? 1 : 0;
        }

        public string ToJson()
        {
            var sensorData = new
            {
                Name = GetSlug(),
                Value = IsOperational()  
            };

            return JsonSerializer.Serialize(sensorData);
        }

        public string GetSlug()
        {
            return "filterStatus";
        }
    }
}
