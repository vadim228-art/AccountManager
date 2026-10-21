using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using AccountManager.Protos; 

namespace AccountManager.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // 1. Подключаемся к серверу. 
  
            using var channel = GrpcChannel.ForAddress("http://localhost:5270"); 
            
            // 2. Создаем клиент для нашего gRPC сервиса
            var client = new AccountService.AccountServiceClient(channel);

            try
            {
                // 3. Формируем запрос 
                var request = new AccountRequest { AccountId = 1 };
                
                Console.WriteLine($"Отправка запроса на получение данных для аккаунта ID: {request.AccountId}...");

                // 4. Вызываем метод GetAccount на сервере
                var reply = await client.GetAccountAsync(request);

                // 5. Выводим результат
                Console.WriteLine("\n--- Данные аккаунта получены ---");
                Console.WriteLine($"ID: {reply.AccountId}");
                Console.WriteLine($"Username: {reply.Username}");
                Console.WriteLine($"Email: {reply.Email}");
                Console.WriteLine("--------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка: {ex.Message}");
                Console.WriteLine("Проверь, запущен ли сервер и правильный ли порт указан.");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}