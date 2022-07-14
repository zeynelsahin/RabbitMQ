using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Producer.UI.Pages;

public class IndexBase: ComponentBase
{
    [Inject]
    ISnackbar Snackbar { get; set; }

    protected string ConnectionString { get; set; } = "amqp://guest:guest@localhost:8080";
    protected string Name { get; set; }
    protected string QueueName { get; set; }
    protected string Type { get; set; }
    protected string RoutingKey { get; set; }
    protected string ExchangeName { get; set; }
    protected string ExchangeType { get; set; }
    protected string Messages { get; set; }
    protected int Repeat { get; set; }
    protected bool UseCounter { get; set; }
    protected List<string> Types = new() { "direct", "fanout", "headers", "topic" };
    protected string ConnectName = "Connect";
    protected List<MyAlert> Log = new List<MyAlert>();
    protected bool isConnectionOpen = false;

    protected void Connect()
    {
        isConnectionOpen = !isConnectionOpen;
        ConnectName = isConnectionOpen ? "Disconnect" : "Connect";
        var logMessage = isConnectionOpen ? "Bağlantı açıldı" : "Bağlantı kapandı";
        var severity = isConnectionOpen ? Severity.Success : Severity.Error;
        AddLog(logMessage, severity);
    }

    protected void DeclareQueue()
    {
    }

    protected void DeclareExchange()
    {
    }

    protected void Publish()
    {
        Snackbar.Add(UseCounter.ToString());
    }

    public void AddLog(string logStr, Severity severity)
    {
        logStr = $"[{DateTime.Now:dd.MM.yyy HH:MM:ss}] - {logStr}";
        Log.Add(new MyAlert() { Message = logStr, Severity = severity });
    }

    protected class MyAlert
    {
        public string Message { get; set; }
        public Severity Severity { get; set; }
    }
}