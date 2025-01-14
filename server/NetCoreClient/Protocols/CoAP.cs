using System;
using System.Text.Json;
using CoAP;
using CoAP.Server;
using CoAP.Server.Resources;

namespace NetCoreClient.Protocols;

public class CoapProtocol : IProtocolInterface
{
    private readonly CoapServer _coapServer;
    private readonly Dictionary<string, CoapSensorResource> _sensorResources;
    private readonly Resource _sensorsRoot;

    public CoapProtocol(int port = 5683)
    {
        _coapServer = new CoapServer(port);
        _sensorResources = new Dictionary<string, CoapSensorResource>();
        _sensorsRoot = new Resource("sensors");
        _coapServer.Add(_sensorsRoot);
    }

    public void Start()
    {
        _coapServer.Start();
    }

    public void Stop()
    {
        _coapServer.Stop();
    }

    public void Send(string value, string sensor)
    {
        string resourcePath = $"sensors/{sensor}";
        lock (_sensorResources)
        {
            if (!_sensorResources.ContainsKey(resourcePath))
            {
                var resource = new CoapSensorResource(sensor);
                _sensorResources.Add(resourcePath, resource);
                _sensorsRoot.Add(resource);
            }
            _sensorResources[resourcePath].UpdateValue(value);
            Console.WriteLine($"Valore aggiornato per {sensor}: {value}");
        }
    }
}

public class CoapSensorResource : Resource
{
    private int _currentValue;
    private readonly object _lockObject = new object();

    public CoapSensorResource(string name) : base(name)
    {
        Attributes.Title = $"Sensor: {name}";
        Observable = true;
    }

    public void UpdateValue(string jsonData)
    {
        lock (_lockObject)
        {
            try
            {
                var data = JsonDocument.Parse(jsonData);
                var value = data.RootElement.GetProperty("Value").GetInt32();
                _currentValue = value;
                Changed();
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("Invalid JSON format", ex);
            }
        }
    }

    protected override void DoGet(CoapExchange exchange)
    {
        lock (_lockObject)
        {
            Console.WriteLine($"Richiesta GET ricevuta per la risorsa: {this.Name}");

            var response = new Response(StatusCode.Content)
            {
                PayloadString = _currentValue.ToString()
            };
            exchange.Respond(response);
        }
    }
}