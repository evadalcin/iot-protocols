using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreClient.Protocols
{
    internal class Mqtt : IProtocolInterface
    {
        private const string SENSOR_TOPIC_PREFIX = "iot2025/water_coolers/123/sensor/";
        private const string COMMAND_TOPIC_PREFIX = "iot2025/water_coolers/123/command/";
        private IMqttClient mqttClient;
        private string endpoint;

        public Mqtt(string endpoint)
        {
            this.endpoint = endpoint;
            Connect().GetAwaiter().GetResult();
            SubscribeToCommands();
        }

        private async Task<MqttClientConnectResult> Connect()
        {
            var factory = new MqttFactory();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(this.endpoint)
                .WithCredentials("yourUsername", "yourPassword")
                .WithCleanSession(false)
                .Build();

            mqttClient = factory.CreateMqttClient();

            try
            {
                await mqttClient.ConnectAsync(options, CancellationToken.None);
                Console.WriteLine("MQTT client connected with authentication.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to MQTT broker: {ex.Message}");
            }

            return new MqttClientConnectResult();
        }

        private bool light_on = false;
        private bool maintenance_mode = false;

        private void SubscribeToCommands()
        {
            try
            {
                mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(COMMAND_TOPIC_PREFIX + "#").Build());

                mqttClient.ApplicationMessageReceivedAsync += async e =>
                {
                    var message = e.ApplicationMessage.Payload;
                    var payload = Encoding.UTF8.GetString(message);
                    HandleCommand(payload);
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nella sottoscrizione ai comandi: {ex.Message}");
            }
        }

        private void HandleCommand(string payload)
        {
            try
            {
                var commandData = System.Text.Json.JsonSerializer.Deserialize<CommandData>(payload);

                switch (commandData.Command)
                {
                    case "light_on":
                        if (commandData.Status)
                            TurnOnNightLight();
                        else
                            TurnOffNightLight();
                        break;
                    case "maintenance_mode":
                        if (commandData.Status)
                            EnableMaintenanceMode();
                        else
                            DisableMaintenanceMode();
                        break;
                    default:
                        Console.WriteLine($"Comando sconosciuto: {commandData.Command} stato: {commandData.Status}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nel parsing del comando: {ex.Message}");
            }
        }

        private void TurnOnNightLight()
        {
            light_on = true;
            Console.WriteLine("Luce notturna accesa.");
            Console.WriteLine($"Stato luce notturna: {light_on}");
        }

        private void TurnOffNightLight()
        {
            light_on = false;
            Console.WriteLine("Luce notturna spenta.");
            Console.WriteLine($"Stato luce notturna: {light_on}");
        }

        private void EnableMaintenanceMode()
        {
            maintenance_mode = true;
            Console.WriteLine("Modalità manutenzione attivata.");
            Console.WriteLine($"Stato manutenzione: {maintenance_mode}");
        }

        private void DisableMaintenanceMode()
        {
            maintenance_mode = false;
            Console.WriteLine("Modalità manutenzione disattivata.");
            Console.WriteLine($"Stato manutenzione: {maintenance_mode}");
        }

        public async void Send(string data, string sensor)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(SENSOR_TOPIC_PREFIX + sensor)
                .WithPayload(data)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .WithRetainFlag(true)
                .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None);
        }
    }

    public class CommandData
    {
        [JsonPropertyName("command")]
        public string? Command { get; set; }

        [JsonPropertyName("status")]
        public bool Status { get; set; }
    }
}
