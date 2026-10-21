using AccountManager.Server.Services; 
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using AccountManager.Server.Storage;
using System.IO;
using System;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

// Регистрируем как Singleton — один экземпляр на весь сервер
var dataFile = Path.Combine(AppContext.BaseDirectory, "accounts.json");
builder.Services.AddSingleton(new AccountStorage(dataFile));

var app = builder.Build();

// ВОТ ЗДЕСЬ МЕНЯЕМ НАЗВАНИЕ СЕРВИСА
app.MapGrpcService<AccountService>(); 

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client...");

app.Run();