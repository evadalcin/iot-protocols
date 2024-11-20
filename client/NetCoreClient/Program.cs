using NetCoreClient.Protocols;
using NetCoreClient.Sensors;

List<ISensorInterface> sensors = new();
sensors.Add(new WaterTempSensor());
sensors.Add(new WaterLevelSensor());
sensors.Add(new FilterSensor());

string endpointHttp = "http://localhost:8011/water_coolers/123";
IProtocolInterface protocol = new Http(endpointHttp);

while (true)
{
    foreach (ISensorInterface sensor in sensors)
    {
        var sensorValue = sensor.ToJson();

        protocol.Send(sensorValue, sensor.GetSlug());

        Console.WriteLine("Data sent: " + sensorValue);

        Thread.Sleep(1000);
    }
}
