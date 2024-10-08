using GameofLife.Data.Interfaces;
using GameOfLife.Data.Repositories;
using GameofLife.Services.Implementations;
using GameOfLife.Services.Interfaces;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Register repositories and services
builder.Services.AddScoped<IBoardRepository, BoardRepository>();
builder.Services.AddScoped<IGameOfLifeService, GameOfLifeService>();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do MongoDB
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDb"); // Docker nomeia o serviço Mongo como "mongo"
    return new MongoClient(connectionString);
});

// Configure MongoDB database
builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var databaseName = builder.Configuration.GetSection("MongoDbSettings:DatabaseName").Value;
    return client.GetDatabase(databaseName);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();



