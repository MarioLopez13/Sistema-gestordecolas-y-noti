using StackExchange.Redis;
using TransactionQueue.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddControllers();

// Configurar Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost")); // Cambia esto si usas otro host
builder.Services.AddScoped<ITransactionQueueService, TransactionQueueService>();

var app = builder.Build();

// Configurar la tubería de solicitudes HTTP.
app.UseAuthorization();

app.MapControllers();

app.Run();
