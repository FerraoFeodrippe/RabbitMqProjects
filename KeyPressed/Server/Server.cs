using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class Server: IDisposable
{
    private const string EXCHANGE_EVENT = "KeyPressed_Event";
    private const string EXCHANGE_EVENT_WAITER = "KeyPressed_Event_Waiter";
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    public Server()
    {
        var connectionFactory = new ConnectionFactory() { HostName = "localhost" };
        _connection = connectionFactory.CreateConnection();       
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: EXCHANGE_EVENT
            ,type: ExchangeType.Topic);

         _channel.ExchangeDeclare(
            exchange: EXCHANGE_EVENT_WAITER
            ,type: ExchangeType.Topic);

        _channel.BasicQos(
            prefetchSize: 0
            ,prefetchCount: 1
            ,global: false
        );

        _queueName = _channel.QueueDeclare().QueueName;

        _channel.QueueBind(
            queue: _queueName
            ,exchange: EXCHANGE_EVENT
            ,routingKey: "Event.KeyPressed.#"
        );

        Console.WriteLine(" [*] Server Instance...");
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
        _channel.Dispose();
        _connection.Dispose();
    }

    public void Run()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            byte[] response =  Encoding.UTF8.GetBytes("Response Default.");;

            try
            {
                response = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(response);
                var routingKey = ea.RoutingKey;
                Console.WriteLine($" [x] Received '{routingKey}':'{message}'");

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch(Exception ex)
            {
                response =  Encoding.UTF8.GetBytes(ex?.Message ?? string.Empty);
            }
            finally
            {
                _channel.BasicPublish(
                    exchange: EXCHANGE_EVENT_WAITER
                    ,routingKey: "Event.KeyPressed.Waiter"
                    ,basicProperties: null
                    ,body: response);
            }
        };

        _channel.BasicConsume(
            queue: _queueName
            ,autoAck: false
            ,consumer: consumer);

        Console.WriteLine(" [*] Server Running...");

        while(true)
        {}
    }  
}