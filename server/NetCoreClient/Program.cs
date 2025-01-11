using NetCoreClient.Protocols;
using NetCoreClient.Sensors;

List<ISensorInterface> sensors = new();
sensors.Add(new WaterTempSensor());
sensors.Add(new WaterLevelSensor());
sensors.Add(new FilterSensor());

var waterCoolerServer = new Server(sensors);
waterCoolerServer.Start();

Console.WriteLine("Premi qualsiasi tasto per terminare...");
Console.ReadKey();