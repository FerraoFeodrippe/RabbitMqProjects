using RabbitMQ.Client;
using System.Text;

public class Client : IDisposable
{
    private const string EXCHANGE_EVENT = "KeyPressed_Event";
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public Client()
    {
        var connectionFactory = new ConnectionFactory() { HostName = "localhost" };
        _connection = connectionFactory.CreateConnection();       
        _channel = _connection.CreateModel();

        Console.WriteLine(" [*] Client Instance...");
    }

    public void Run()
    {
        string keyPressed;

        Console.WriteLine(" [*] Client Running...");

        while (true)
        {
            try
            {
                keyPressed = Console.ReadKey(true).Key.ToString();

                var body = Encoding.UTF8.GetBytes(keyPressed);
                _channel.BasicPublish(exchange: EXCHANGE_EVENT,
                                    routingKey: "Event.KeyPressed",
                                    basicProperties: null,
                                    body: body);

            }
            catch(Exception ex)
            {
                Console.WriteLine($" [x] Error: '{ex.Message}'");
            }
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