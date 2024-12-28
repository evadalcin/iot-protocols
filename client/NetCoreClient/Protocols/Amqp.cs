using RabbitMQ.Client;
using System;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreClient.Protocols
{
    public class Amqp : IProtocolInterface
    {
        private readonly string _hostname;
        private const string EXCHANGE_NAME = "sensor_data_exchange";
        private const string QUEUE_NAME = "sensor_data_queue";
        private IConnection _connection;
        private IChannel _channel;

        public Amqp(string hostname)
        {
            _hostname = hostname;
        }

        public async Task InitializeAmqpClient()
        {
            var factory = new ConnectionFactory() { Uri = new Uri(_hostname) };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
                exchange: EXCHANGE_NAME,
                type: ExchangeType.Topic,
                durable: false,
                autoDelete: false);

            await _channel.QueueDeclareAsync(
                queue: QUEUE_NAME,
                durable: false,
                exclusive: false,
                autoDelete: false);

            await _channel.QueueBindAsync(
                queue: QUEUE_NAME,
                exchange: EXCHANGE_NAME,
                routingKey: "water_coolers.123.sensor.*");
        }

        public async Task Send(string data, string sensor)
        {
            string routingKey = $"water_coolers.123.sensor.{sensor}";
            var body = Encoding.UTF8.GetBytes(data);

            await _channel.BasicPublishAsync(
                exchange: EXCHANGE_NAME,
                routingKey: routingKey,
                body: body);

            Console.WriteLine($" [x] Sent {sensor}: {data}");
        }

        public async Task Close()
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }
    }
}