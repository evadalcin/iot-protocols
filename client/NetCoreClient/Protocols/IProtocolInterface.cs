public interface IProtocolInterface
{
    Task Send(string data, string sensor); // Metodo asincrono per inviare un messaggio MQTT
}
