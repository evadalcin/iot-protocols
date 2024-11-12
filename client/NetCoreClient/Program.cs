
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NetCoreClient.Sensors;
using NetCoreClient.Protocols;


// Definisci i sensori
List<ISensorInterface> sensors = new();
sensors.Add(new VirtualWaterTempSensor());
sensors.Add(new VirtualAvailableWaterSensor());
sensors.Add(new VirtualFilterConditionSensor());


var url = "http://localhost:8011/water_coolers/123"; 

using var client = new HttpClient();

while (true)
{
    var sensorData = new
    {
        waterTemperature = ((VirtualWaterTempSensor)sensors[0]).WaterTemperature(),
        availableWaterAmount = ((VirtualAvailableWaterSensor)sensors[1]).AvailableWaterAmount(),
        isFilterOperational = ((VirtualFilterConditionSensor)sensors[2]).IsFilterOperational()
    };

    var jsonData = JsonSerializer.Serialize(sensorData);

    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

    var response = await client.PostAsync(url, content);

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine("Data sent successfully: " + jsonData);
    }
    else
    {
        Console.WriteLine("Error sending data: " + response.StatusCode);
    }

    Thread.Sleep(1000); ; 
}

