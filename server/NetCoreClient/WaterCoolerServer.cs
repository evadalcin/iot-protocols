using CoAP;
using CoAP.Server;
using CoAP.Server.Resources;
using NetCoreClient.Sensors;
using System.Text.Json;

class Server
{
    private readonly CoapServer _server;
    private readonly List<ISensorInterface> _sensors;
    private const int COAP_PORT = 5683;

    public Server(List<ISensorInterface> sensors)
    {
        _sensors = sensors;
        _server = new CoapServer(COAP_PORT);
        InitializeServer();
    }

    private void InitializeServer()
    {
        var waterCoolerResource = new Resource("water_cooler", _sensors);
        _server.Add(waterCoolerResource);
    }

    public void Start()
    {
        _server.Start();
        Console.WriteLine($"Water Cooler CoAP Server avviato sulla porta {COAP_PORT}");
        // Aggiorna continuamente i dati dei sensori
        Task.Run(async () =>
        {
            while (true)
            {
                foreach (var sensor in _sensors)
                {
                    var sensorData = sensor.ToJson();
                    Console.WriteLine($"Aggiornati dati sensore {sensor.GetSlug()}: {sensorData}");
                }
                await Task.Delay(1000);
            }
        });
    }
}

class Resource : CoAP.Server.Resources.Resource
{
    private readonly List<ISensorInterface> _sensors;

    public Resource(string name, List<ISensorInterface> sensors)
        : base(name)
    {
        _sensors = sensors;
        Observable = true;  // Permette ai client di osservare questa risorsa

        // Supporta sia GET che POST
        Attributes.Title = "Water Cooler Sensors";
        Attributes.AddResourceType("sensors");
        Attributes.AddContentType(MediaType.ApplicationJson);
    }

    protected override void DoGet(CoapExchange exchange)  // Cambiato da HandleGet a DoGet
    {
        var response = new List<object>();
        foreach (var sensor in _sensors)
        {
            // Deserializza il JSON del sensore per includerlo nella risposta
            var sensorData = JsonSerializer.Deserialize<object>(sensor.ToJson());
            response.Add(sensorData);
        }
        var jsonResponse = JsonSerializer.Serialize(response);
        exchange.Respond(StatusCode.Content, jsonResponse, MediaType.ApplicationJson);
    }

    protected override void DoPost(CoapExchange exchange)  // Cambiato da HandlePost a DoPost
    {
        try
        {
            var payload = exchange.Request.PayloadString;
            var request = JsonSerializer.Deserialize<SensorRequest>(payload);
            var sensor = _sensors.FirstOrDefault(s => s.GetSlug() == request.SensorSlug);
            if (sensor != null)
            {
                exchange.Respond(StatusCode.Content, sensor.ToJson(), MediaType.ApplicationJson);
            }
            else
            {
                exchange.Respond(StatusCode.NotFound);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore nella gestione della richiesta POST: {ex.Message}");
            exchange.Respond(StatusCode.BadRequest);
        }
    }
}

public class SensorRequest
{
    public string SensorSlug { get; set; }
}