using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Producer.UI.Pages;

public class ConsumerBase: ComponentBase
{
    [Inject]
    ISnackbar Snackbar { get; set; }
    private IConnection? Connection;
    private IModel? _channel=null;
    protected string ConnectionString { get; set; } = "amqp://guest:guest@localhost:5672";
    protected string QueueName { get; set; } = "deneme1";
    protected List<string> Messages= new List<string>();
    
    protected async Task ReadQueue()
    {
        try
        {
            Connection = GetConnection();
            var consumerEvent = new EventingBasicConsumer(channel);
            consumerEvent.Received += (ch, e) =>
            {
                var byteArr = e.Body.ToArray();
                var bodyStr = Encoding.UTF8.GetString(byteArr);
                AddLog(bodyStr);
            };
            channel.BasicConsume(QueueName, true, consumerEvent);
            Snackbar.Add(Messages.Count.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally{StateHasChanged();}
        
    }
    public void AddLog(string deger)
    {
        Messages.Add(deger);
       
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

    private IConnection? GetConnection()
    {
        var factory = new ConnectionFactory()
        {
            Uri = new Uri(ConnectionString, UriKind.RelativeOrAbsolute)
        };
        return factory.CreateConnection();
    }
    
    IModel CreateOrGetChannel()
    {
        var model=  Connection.CreateModel();
        return model;
    }
}