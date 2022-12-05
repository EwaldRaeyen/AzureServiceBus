// See https://aka.ms/new-console-template for more information
using AzureServiceBus.Message;
using AzureServiceBus.Services;
using AzureServiceBus.Services.Interfaces;
using AzureServiceBus.Settings;
using AzureServiceBus.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Threading;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(c =>
    {
        c.AddJsonFile("appsettings.json");
    })
    .ConfigureServices((builder, services) =>
    {
        services.Configure<AzureServiceBusSettings>(builder.Configuration.GetSection(nameof(AzureServiceBusSettings)));
        
        //builder.Configuration.GetSection(nameof(AzureServiceBusSettings)).Get<AzureServiceBusSettings>();
        
        services.ConfigureServiceBus(builder.Configuration);

        services.AddScoped<IMessageSender, AzureServiceBusSender>();
        services.AddScoped<IMessageReceiver, AzureServiceBusReceiver>();
    })
    .Build();

Console.WriteLine("Test sending messages");

try
{
    var options = host.Services.GetService<IOptions<AzureServiceBusSettings>>();
    //await SendMessage();
    //await Task.Delay(2000);
    //await ReceiveMessage();

    await SendEvent();
    await ReceiveEvent(); // use subscription and listen to events
}
catch (Exception ex)
{
    Console.WriteLine(ex.GetType().ToString());
    Console.WriteLine(ex.Message.ToString());
}



async Task SendMessage()
{
    var messageSender = host.Services.GetService<IMessageSender>();
    if (messageSender == null) throw new Exception($"{nameof(messageSender)} is null");

    await messageSender.SendAsync("test");
}

async Task SendEvent()
{
    var options = host.Services.GetService<IOptions<AzureServiceBusSettings>>();
    if (options == null) throw new Exception("options is null");

    var messageSender = host.Services.GetService<IMessageSender>();
    if (messageSender == null) throw new Exception($"{nameof(messageSender)} is null");

    await messageSender.SendEventAsync(new Event(Guid.NewGuid(), "-1", new { Text = "test" }), options.Value.TodoItemTopic, default);
}

async Task ReceiveMessage()
{
    var messageReceiver = host.Services.GetService<IMessageReceiver>();
    if (messageReceiver == null) throw new Exception($"{nameof(messageReceiver)} is null");

    var message = await messageReceiver.ConsumeAsync();

    Console.WriteLine($"Message: {message}");
}

async Task ReceiveEvent()
{
    var options = host.Services.GetService<IOptions<AzureServiceBusSettings>>();
    if (options == null) throw new Exception("options is null");

    var messageReceiver = host.Services.GetService<IMessageReceiver>();
    if (messageReceiver == null) throw new Exception($"{nameof(messageReceiver)} is null");

    var @event = await messageReceiver.ConsumeEventAsync(options.Value.TodoItemTopic, default);

    Console.WriteLine($"Message: {@event}");
}