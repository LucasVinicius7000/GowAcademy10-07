using Azure.Messaging.ServiceBus;
using GowAcademy.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string? connectionString = builder.Configuration.GetConnectionString("ServiceBusNamespace")?.ToString();
builder.Services.AddSingleton<IMessageService<ServiceBusMessage, ServiceBusReceivedMessage>, MessageService>(provider =>
{
    return new MessageService(connectionString);
});
builder.Services.AddHostedService<AtualizaEstoqueService>();

using (var serviceProvider = builder.Services.BuildServiceProvider())
{
    using (var scope = serviceProvider.CreateScope())
    {
        var scopedServices = scope.ServiceProvider;
        var messageService = scopedServices.GetRequiredService<IMessageService<ServiceBusMessage, ServiceBusReceivedMessage>>();
        if (!messageService.SubscriptionExists("pagamento-processado", "atualiza-estoque"))
            messageService.CreateSubscriptionForTopic("pagamento-processado", "atualiza-estoque");
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
