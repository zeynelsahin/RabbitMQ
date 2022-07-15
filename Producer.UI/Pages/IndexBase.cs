using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Producer.UI.Pages;

public class IndexBase: ComponentBase
{
    [Inject]
    ISnackbar Snackbar { get; set; }

    protected string ConnectionString { get; set; } = "amqp://guest:guest@localhost:5672";
    protected string Name { get; set; }
    protected string QueueName { get; set; }
    protected string Type { get; set; }
    protected string Key { get; set; }
    protected string RoutingKey { get; set; }
    protected string ExchangeName { get; set; }
    protected string ExchangeType { get; set; }
    protected string Message { get; set; }
    protected int Repeat { get; set; } = 1;
    protected bool UseCounter { get; set; }
    protected List<string> Types = new() { "direct", "fanout", "headers", "topic" };
    protected string ConnectName = "Connect";
    protected List<MyAlert> Log = new List<MyAlert>();
    protected bool isConnectionOpen = false;

    protected IConnection _connection;


    private IModel _channel=null;
    protected IModel? channel
    {
        get {
            if (_channel==null)
            {
                _channel = CreateOrGetChannel(); 
            }
            return _channel;
        }
    }

    protected void Connect()
    {
        if (!isConnectionOpen || _connection==null)
        {
           _connection= GetConnection();
        }
        else
        {
            _connection.Close();
        }
        isConnectionOpen = _connection.IsOpen;
        ConnectName = isConnectionOpen ? "Disconnect" : "Connect";
        var logMessage = isConnectionOpen ? "Bağlantı açıldı" : "Bağlantı kapandı";
        var severity = isConnectionOpen ? Severity.Success : Severity.Error;
        AddLog(logMessage, severity);
    }

    private IConnection GetConnection()
    {
        ConnectionFactory factory = new ConnectionFactory()
        {
            Uri = new Uri(ConnectionString,UriKind.RelativeOrAbsolute)  
        };
        return factory.CreateConnection();
    }
    protected void DeclareQueue()
    {
        channel.QueueDeclare(QueueName,autoDelete:false,exclusive:false);
    AddLog($"Queue created with Name:{QueueName}",Severity.Normal);
    }

    private IModel CreateOrGetChannel()
    {
      var model=  _connection.CreateModel();
      AddLog($"Channel created",Severity.Normal);
      return model;
    }
    protected void DeclareExchange()
    {
        channel.ExchangeDeclare(Name, Type);
        AddLog($"Exchange created with Name: {Name}, Type: {Type}",Severity.Normal);
    }

    protected void BindQueue()
    {
        channel.QueueBind(QueueName,Name,Key);
        AddLog($"Bind  with Queue Name: {QueueName}, Exchange Name: {Name}, Routing Key: {Key}",Severity.Normal);
    }

    protected void Publish()
    {
        var message = Message;
    
        for (int i = 0; i < Repeat; i++)
        {
            if (UseCounter)
            {
                message = $"[{i + 1}] - {Message}";
            }
            WriteDaaToExhange(ExchangeName,RoutingKey,message);
        }
        AddLog($"Publish with Exchange Name: {ExchangeName}, Routing Key: {RoutingKey}, Message: {message}",Severity.Normal);
    }

    private void WriteDaaToExhange(string exchangeName, string routingKey, object data)
    {
        var dataArr = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
        channel.BasicPublish(exchangeName,routingKey,null,dataArr);
    }
    public void AddLog(string logStr, Severity severity)
    {
        logStr = $"[{DateTime.Now:dd.MM.yyy HH:MM:ss}] - {logStr}";
        Log.Add(new MyAlert() { Message = logStr, Severity = severity });
        Snackbar.Add(logStr, severity);
    }


    protected class MyAlert
    {
        public string Message { get; set; }
        public Severity Severity { get; set; }
    }
}