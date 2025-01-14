using NetCoreClient.Protocols;
using NetCoreClient.Sensors;


List<ISensorInterface> sensors = new();
sensors.Add(new WaterTempSensor());
sensors.Add(new WaterLevelSensor());
sensors.Add(new FilterSensor());

var protocol = new CoapProtocol();

protocol.Start();

try
{
    while (true)
    {
        foreach (ISensorInterface sensor in sensors)
        {
            var sensorValue = sensor.ToJson();
            protocol.Send(sensorValue, sensor.GetSlug());
            // Console.WriteLine($"{sensor.GetSlug()}: {sensorValue}");
            
        }
        await Task.Delay(10000);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Errore: {ex.Message}");
}
finally
{
    protocol.Stop();
}
