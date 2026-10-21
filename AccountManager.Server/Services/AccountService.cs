using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Google.Protobuf.WellKnownTypes;
using AccountManager.Server.Storage;

namespace AccountManager.Server.Services
{
    public class AccountService : AccountManager.Server.AccountService.AccountServiceBase
    {
        private readonly ILogger<AccountService> _logger;
        private readonly AccountStorage _storage;

        public AccountService(ILogger<AccountService> logger, AccountStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public override Task<AccountReply> GetAccount(AccountRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Запрошен аккаунт ID: {Id}", request.AccountId);
            var acc = _storage.GetById(request.AccountId)
                ?? throw new RpcException(new Status(StatusCode.NotFound, $"Аккаунт с ID {request.AccountId} не найден"));
            return Task.FromResult(acc);
        }

        public override Task<AccountListReply> GetAllAccounts(Empty request, ServerCallContext context)
        {
            _logger.LogInformation("Запрошен список всех аккаунтов");
            var reply = new AccountListReply();
            reply.Accounts.AddRange(_storage.GetAll());
            return Task.FromResult(reply);
        }

        public override Task<AccountReply> CreateAccount(CreateAccountRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Создание аккаунта: {Username}", request.Username);
            var acc = _storage.Create(request.Username, request.Email);
            return Task.FromResult(acc);
        }

        public override Task<AccountReply> UpdateAccount(UpdateAccountRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Обновление аккаунта ID: {Id}", request.AccountId);
            var acc = _storage.Update(request.AccountId, request.Username, request.Email)
                ?? throw new RpcException(new Status(StatusCode.NotFound, $"Аккаунт с ID {request.AccountId} не найден"));
            return Task.FromResult(acc);
        }

        public override Task<Empty> DeleteAccount(AccountRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Удаление аккаунта ID: {Id}", request.AccountId);
            if (!_storage.Delete(request.AccountId))
                throw new RpcException(new Status(StatusCode.NotFound, $"Аккаунт с ID {request.AccountId} не найден"));
            return Task.FromResult(new Empty());
        }
    }
}