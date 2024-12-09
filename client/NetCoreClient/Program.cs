using NetCoreClient.Protocols;
using NetCoreClient.Sensors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        List<ISensorInterface> sensors = new();
        sensors.Add(new WaterTempSensor());
        sensors.Add(new WaterLevelSensor());
        sensors.Add(new FilterSensor());

        IProtocolInterface protocol = new Amqp(Environment.GetEnvironmentVariable("AMQP_URL"));

        await ((Amqp)protocol).InitializeAmqpClient();

        while (true)
        {
            foreach (ISensorInterface sensor in sensors)
            {
                var sensorValue = sensor.ToJson();

                await protocol.Send(sensorValue, sensor.GetSlug());

                Console.WriteLine("Data sent: " + sensorValue);

                await Task.Delay(1000);
            }
        }
    }
}
