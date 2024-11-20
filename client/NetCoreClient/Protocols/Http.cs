using NetCoreClient.Protocols;
using NetCoreClient.Protocols;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class Http : IProtocolInterface
{
    private readonly string Endpoint;
    private static readonly HttpClient client = new HttpClient();

    public Http(string endpoint)
    {
        this.Endpoint = endpoint;
    }

    public async void Send(string data, string sensor)
    {
        try
        {
            var content = new StringContent(data, Encoding.UTF8, "application/json");

            var result = await client.PostAsync(Endpoint, content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Si è verificato un errore durante l'invio dei dati: {ex.Message}");
        }
    }
}
