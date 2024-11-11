using NetCoreClient.ValueObjects;
using System;
using System.Text.Json;

namespace NetCoreClient.Sensors;

class VirtualFlowMeterSensor : IFlowMeterInterface, ISensorInterface
{
    private readonly Random Random;

    public VirtualFlowMeterSensor()
    {
        Random = new Random();
    }

    public int FlowRate()
    {
        return new FlowRate(Random.Next(0, 100)).Value;
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(FlowRate());
    }
}
