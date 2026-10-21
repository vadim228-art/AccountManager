using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AccountManager.Server; // namespace для AccountReply

namespace AccountManager.Server.Storage
{
    public class AccountStorage
    {
        private readonly string _filePath;
        private readonly object _lock = new();
        private List<AccountReply> _accounts = new();
        private int _nextId = 1;

        // Простой DTO для сериализации (в AccountReply есть поля от protobuf, 
        // которые System.Text.Json может не очень любить, поэтому сделаем свой класс)
        private class AccountDto
        {
            public int AccountId { get; set; }
            public string Username { get; set; } = "";
            public string Email { get; set; } = "";
        }

        public AccountStorage(string filePath)
        {
            _filePath = filePath;
            Load();
        }

        private void Load()
        {
            lock (_lock)
            {
                if (!File.Exists(_filePath))
                {
                    _accounts = new List<AccountReply>();
                    _nextId = 1;
                    return;
                }

                var json = File.ReadAllText(_filePath);
                var dtos = JsonSerializer.Deserialize<List<AccountDto>>(json) ?? new List<AccountDto>();
                _accounts = dtos.Select(d => new AccountReply
                {
                    AccountId = d.AccountId,
                    Username = d.Username,
                    Email = d.Email
                }).ToList();

                _nextId = _accounts.Count == 0 ? 1 : _accounts.Max(a => a.AccountId) + 1;
            }
        }

        private void Save()
        {
            var dtos = _accounts.Select(a => new AccountDto
            {
                AccountId = a.AccountId,
                Username = a.Username,
                Email = a.Email
            }).ToList();

            var json = JsonSerializer.Serialize(dtos, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public List<AccountReply> GetAll()
        {
            lock (_lock) return _accounts.ToList(); // копия, чтобы снаружи не мутировали
        }

        public AccountReply? GetById(int id)
        {
            lock (_lock) return _accounts.FirstOrDefault(a => a.AccountId == id);
        }

        public AccountReply Create(string username, string email)
        {
            lock (_lock)
            {
                var account = new AccountReply
                {
                    AccountId = _nextId++,
                    Username = username,
                    Email = email
                };
                _accounts.Add(account);
                Save();
                return account;
            }
        }

        public AccountReply? Update(int id, string username, string email)
        {
            lock (_lock)
            {
                var acc = _accounts.FirstOrDefault(a => a.AccountId == id);
                if (acc == null) return null;
                acc.Username = username;
                acc.Email = email;
                Save();
                return acc;
            }
        }

        public bool Delete(int id)
        {
            lock (_lock)
            {
                var acc = _accounts.FirstOrDefault(a => a.AccountId == id);
                if (acc == null) return false;
                _accounts.Remove(acc);
                Save();
                return true;
            }
        }
    }
}