using AccountManager.Server.Services; 
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

// ВОТ ЗДЕСЬ МЕНЯЕМ НАЗВАНИЕ СЕРВИСА
app.MapGrpcService<AccountService>(); 

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client...");

app.Run();