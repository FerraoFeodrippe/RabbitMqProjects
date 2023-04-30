using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Actions;

public class Checker : IDisposable
{
    private const string EXCHANGE_EVENT_WAITER = "KeyPressed_Event_Waiter";
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    public Checker()
    {
        var connectionFactory = new ConnectionFactory() { HostName = "localhost" };
        _connection = connectionFactory.CreateConnection();       
        _channel = _connection.CreateModel();

        _channel.BasicQos(
            prefetchSize: 0
            ,prefetchCount: 1
            ,global: false
        );

        _queueName = _channel.QueueDeclare().QueueName;

        _channel.QueueBind(
            queue: _queueName
            ,exchange: EXCHANGE_EVENT_WAITER
            ,routingKey: "Event.KeyPressed.Waiter"
        );

        Console.WriteLine(" [*] Checker Instance...");
    }

    public void Run(ICheckerAction? action)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            try
            {
                byte[] response = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(response);
                var routingKey = ea.RoutingKey;
                Console.WriteLine($" [x] Received '{routingKey}':'{message}'");

                action?.Run(new string[] { message });

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch(Exception ex)
            {
                Console.WriteLine($" [x] Error: '{ex.Message}'");
            }
        };

        _channel.BasicConsume(
            queue: _queueName
            ,autoAck: false
            ,consumer: consumer);

        Console.WriteLine(" [*] Checker Running...");

        while (true)
        {        
        }
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
        _channel.Dispose();
        _connection.Dispose();
    }
}