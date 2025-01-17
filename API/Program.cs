using System.Data;
using System.Text.Json.Serialization;
using Dapper;
using Microsoft.Data.Sqlite;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => 
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IDbConnection>(sp => 
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("SQLiteConnection");
    var connection = new SqliteConnection(connectionString);

    connection.Open();
    connection.ExecuteAsync("PRAGMA foreign_keys = ON;");

    return connection;
});

builder.Services.AddScoped<ICreditsRepository, CreditsRepository>();
builder.Services.AddScoped<ICreditsService, CreditsService>();

var app = builder.Build();

using var scope = app.Services.CreateScope();

var dbConnection = scope.ServiceProvider.GetRequiredService<IDbConnection>();
await InitialCreate.CreateAsync(dbConnection);
await Seed.SeedDataAsync(dbConnection);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.MapControllers();

app.Run();
