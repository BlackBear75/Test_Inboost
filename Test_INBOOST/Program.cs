using System.Data;
using System.Data.SqlClient;
using Test_INBOOST.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();