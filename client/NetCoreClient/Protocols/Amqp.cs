using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreClient.Protocols
{
    public class Amqp : IProtocolInterface
    {
        private readonly string _hostname;
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

            _channel.QueueDeclareAsync(QUEUE_NAME, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public async Task Send(string data, string sensor)
        {
            var body = Encoding.UTF8.GetBytes(data);

            _channel.BasicPublishAsync("", QUEUE_NAME, body);

            Console.WriteLine($" [x] Sent {sensor}: {data}");
        }

        public async Task Close()
        {
            await _channel.CloseAsync();  
            await _connection.CloseAsync();  
        }
    }
}
