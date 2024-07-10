using Azure.Messaging.ServiceBus;
using GowAcademy.Shared;
using Microsoft.Extensions.Azure;
using NotificacaoService;
using NotificacaoService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
string? connectionString = builder.Configuration.GetConnectionString("ServiceBusNamespace")?.ToString();
builder.Services.AddScoped<IMessageService<ServiceBusMessage, ServiceBusReceivedMessage>, MessageService>(provider =>
{
    return new MessageService(connectionString);
});
builder.Services.AddHostedService<NotificaPedidoCriado>();
builder.Services.AddHostedService<NotificaPagamentoProcessado>();

using (var serviceProvider = builder.Services.BuildServiceProvider())
{
    using (var scope = serviceProvider.CreateScope())
    {
        var scopedServices = scope.ServiceProvider;
        var messageService = scopedServices.GetRequiredService<IMessageService<ServiceBusMessage, ServiceBusReceivedMessage>>();
        if (!messageService.SubscriptionExists("pagamento-processado", "notifica-pagamento"))
            messageService.CreateSubscriptionForTopic("pagamento-processado", "notifica-pagamento");
    }
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
