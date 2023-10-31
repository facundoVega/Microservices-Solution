using PlatformService.Dtos;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration config)
        {
            _config = config;
            var factory = new ConnectionFactory()
            {
                HostName = _config["RabbitMQHost"],
                Port = int.Parse(_config["RabbitMQPort"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;

                Console.WriteLine("--> Connected to message bus");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: { ex.Message }");
            }
        }
        public void PublishNewPlatform(PlatformPublishedDTO platformPublishedDTO)
        {
            var message = JsonSerializer.Serialize(platformPublishedDTO);

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ Connection closed, not sending");

            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "trigger", 
                routingKey: "",
                basicProperties: null,
                body: body);
            Console.WriteLine($"We have sent {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("MessageBus Disposed");
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
        private void RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }
    }
}
