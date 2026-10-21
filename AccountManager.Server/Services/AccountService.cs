using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace AccountManager.Server.Services
{
    // Класс наследуется от базового класса, сгенерированного из .proto файла
    public class AccountService : AccountManager.Server.AccountService.AccountServiceBase
    {
        private readonly ILogger<AccountService> _logger;

        public AccountService(ILogger<AccountService> logger)
        {
            _logger = logger;
        }

        // Переопределяем метод из .proto файла
        public override Task<AccountReply> GetAccount(AccountRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Запрошен аккаунт с ID: {Id}", request.AccountId);

            // Тут мы имитируем базу данных и просто возвращаем тестовые данные
            // В реальности тут был бы запрос к БД (Entity Framework, Dapper и т.д.)
            var reply = new AccountReply
            {
                AccountId = request.AccountId,
                Username = "Ivan_Ivanov",
                Email = $"user{request.AccountId}@example.com"
            };

            return Task.FromResult(reply);
        }
    }
}