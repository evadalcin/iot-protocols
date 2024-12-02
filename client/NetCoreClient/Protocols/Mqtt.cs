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
    public class Mqtt : IProtocolInterface
    {
        private const string SENSOR_TOPIC_PREFIX = "iot2025/water_coolers/123/sensor/";
        private const string COMMAND_TOPIC_PREFIX = "iot2025/water_coolers/123/command/";
        private const string STATUS_TOPIC = "iot2025/water_coolers/123/sensor/status";

        private IMqttClient mqttClient;
        private string endpoint;
        private bool maintenance_mode;
        private bool light_on;

        public Mqtt(string endpoint)
        {
            this.endpoint = endpoint;
            InitializeMqttClient();
        }

        private void InitializeMqttClient()
        {
            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

            var willMessage = new MqttApplicationMessageBuilder()
                .WithTopic(STATUS_TOPIC)
                .WithPayload("offline")
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .WithRetainFlag(true)
                .Build();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(this.endpoint)
                .WithCredentials("yourUsername", "yourPassword")
                .WithCleanSession(false)
                .WithWillMessage(willMessage)
                .Build();

            mqttClient.ConnectedAsync += HandleConnectedAsync;
            mqttClient.DisconnectedAsync += HandleDisconnectedAsync;

            ConnectWithRetry(options);
        }

        private void ConnectWithRetry(IMqttClientOptions options)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await mqttClient.ConnectAsync(options, CancellationToken.None);
                        break; 
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Connection failed: {ex.Message}. Retrying in 5 seconds...");
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                }
            });
        }

        private async Task HandleConnectedAsync(MqttClientConnectedEventArgs e)
        {
            Console.WriteLine("Connected to MQTT Broker");

            try
            {
                await PublishStatusMessage("online");

                await SubscribeToCommands();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in connection handling: {ex.Message}");
            }
        }

        private async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs e)
        {
            Console.WriteLine($"Disconnected from MQTT Broker. Reason: {e.Reason}");

            await Task.Delay(TimeSpan.FromSeconds(5));
            InitializeMqttClient(); 
        }

        private async Task PublishStatusMessage(string status)
        {
            var statusMessage = new MqttApplicationMessageBuilder()
                .WithTopic(STATUS_TOPIC)
                .WithPayload(status)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .WithRetainFlag(true)
                .Build();

            await mqttClient.PublishAsync(statusMessage, CancellationToken.None);
        }

        private async Task SubscribeToCommands()
        {
            try
            {
                await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic(COMMAND_TOPIC_PREFIX + "#")
                    .Build());

                mqttClient.ApplicationMessageReceivedAsync += async e =>
                {
                    var message = e.ApplicationMessage.Payload;
                    var payload = Encoding.UTF8.GetString(message);
                    HandleCommand(payload);
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error subscribing to commands: {ex.Message}");
            }
        }

        private void HandleCommand(string payload)
        {
            try
            {
                var commandData = System.Text.Json.JsonSerializer.Deserialize<CommandData>(payload);

                switch (commandData?.Command)
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
                        Console.WriteLine($"Unknown command: {commandData?.Command} with status: {commandData?.Status}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing command: {ex.Message}");
            }
        }

        private void TurnOnNightLight()
        {
            light_on = true;
            Console.WriteLine("Night light turned on.");
        }

        private void TurnOffNightLight()
        {
            light_on = false;
            Console.WriteLine("Night light turned off.");
        }

        private void EnableMaintenanceMode()
        {
            maintenance_mode = true;
            Console.WriteLine("Maintenance mode enabled.");
        }

        private void DisableMaintenanceMode()
        {
            maintenance_mode = false;
            Console.WriteLine("Maintenance mode disabled.");
        }

        public async Task Send(string data, string sensor)
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
        public string Command { get; set; }

        [JsonPropertyName("status")]
        public bool Status { get; set; }
    }
}
