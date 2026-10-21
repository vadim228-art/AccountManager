using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using AccountManager.Protos;
using Google.Protobuf.WellKnownTypes;

namespace AccountManager.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5270");
            var client = new AccountService.AccountServiceClient(channel);

            Console.WriteLine("gRPC Account Manager — клиент");
            Console.WriteLine("============================");

            while (true)
            {
                PrintMenu();
                var choice = Console.ReadLine()?.Trim();

                try
                {
                    switch (choice)
                    {
                        case "1": await ListAll(client); break;
                        case "2": await GetOne(client); break;
                        case "3": await Create(client); break;
                        case "4": await Update(client); break;
                        case "5": await Delete(client); break;
                        case "0": return;
                        default: Console.WriteLine("Неизвестная команда."); break;
                    }
                }
                catch (Grpc.Core.RpcException ex)
                {
                    Console.WriteLine($"Ошибка сервера: [{ex.StatusCode}] {ex.Status.Detail}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }

                Console.WriteLine("\nНажми Enter, чтобы продолжить...");
                Console.ReadLine();
            }
        }

        static void PrintMenu()
        {
            Console.Clear();
            Console.WriteLine("1. Показать всех");
            Console.WriteLine("2. Показать одного по ID");
            Console.WriteLine("3. Добавить");
            Console.WriteLine("4. Изменить");
            Console.WriteLine("5. Удалить");
            Console.WriteLine("0. Выход");
            Console.Write("Выбор: ");
        }

        static async Task ListAll(AccountService.AccountServiceClient client)
        {
            var reply = await client.GetAllAccountsAsync(new Empty());
            if (reply.Accounts.Count == 0)
            {
                Console.WriteLine("Список пуст.");
                return;
            }
            Console.WriteLine($"{"ID",-5} | {"Username",-20} | Email");
            Console.WriteLine(new string('-', 60));
            foreach (var a in reply.Accounts)
                Console.WriteLine($"{a.AccountId,-5} | {a.Username,-20} | {a.Email}");
        }

        static async Task GetOne(AccountService.AccountServiceClient client)
        {
            Console.Write("Введите ID: ");
            if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Некорректный ID."); return; }
            var a = await client.GetAccountAsync(new AccountRequest { AccountId = id });
            Console.WriteLine($"ID: {a.AccountId}");
            Console.WriteLine($"Username: {a.Username}");
            Console.WriteLine($"Email: {a.Email}");
        }

        static async Task Create(AccountService.AccountServiceClient client)
        {
            Console.Write("Username: ");
            var username = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Email: ");
            var email = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(username)) { Console.WriteLine("Username не может быть пустым."); return; }
            var created = await client.CreateAccountAsync(new CreateAccountRequest { Username = username, Email = email });
            Console.WriteLine($"Создан аккаунт с ID={created.AccountId}.");
        }

        static async Task Update(AccountService.AccountServiceClient client)
        {
            Console.Write("ID для изменения: ");
            if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Некорректный ID."); return; }
            Console.Write("Новый Username: ");
            var username = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Новый Email: ");
            var email = Console.ReadLine()?.Trim() ?? "";
            var updated = await client.UpdateAccountAsync(new UpdateAccountRequest
            {
                AccountId = id, Username = username, Email = email
            });
            Console.WriteLine($"Обновлён аккаунт ID={updated.AccountId}: {updated.Username}, {updated.Email}");
        }

        static async Task Delete(AccountService.AccountServiceClient client)
        {
            Console.Write("ID для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Некорректный ID."); return; }
            await client.DeleteAccountAsync(new AccountRequest { AccountId = id });
            Console.WriteLine($"Аккаунт с ID={id} удалён.");
        }
    }
}