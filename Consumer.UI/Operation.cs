using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.UI;

public class Operation
{
    public string connectionString = "amqp://guest:guest@localhost:5672";
    string queueName;
    public IConnection? Connection;
    public IModel? _channel=null;
    string data;
    public Operation(string queue = "dene")
    {
        queueName = queue;
        Connection = GetConnection();
        var consumerEvent = new EventingBasicConsumer(channel);
       
        consumerEvent.Received += (ch, e) =>
        {
            var byteArr = e.Body.ToArray();
            var bodyStr = Encoding.UTF8.GetString(byteArr);
            data = bodyStr;
            Console.WriteLine($"Recived Data: {bodyStr}");
        };
        channel.BasicConsume(queueName, true,consumerEvent);
        Console.WriteLine($"{queueName} listening ...\n\n\n");
        Console.WriteLine(data+"asdasd");
    }

    IModel channel
    {
        get
        {
            if (_channel == null)
            {
                _channel = CreateOrGetChannel();
            }
            
            return _channel;
        }
    }

    IConnection? GetConnection()
    {
        var factory = new ConnectionFactory()
        {
            Uri = new Uri(connectionString, UriKind.RelativeOrAbsolute)
        };
        return factory.CreateConnection();
    }

    IModel CreateOrGetChannel()
    {
        return Connection.CreateModel();
    }
}