using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using PasswordManagerV1.Models;

namespace PasswordManagerV1.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;


        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<UserProfile>().Wait();
            _database.CreateTableAsync<UserAccount>().Wait();
        }

        // Методы для UserAccount
        public async Task<List<UserAccount>> GetAccountsAsync(int userId)
        {
            try
            {
                var accounts = await _database.Table<UserAccount>().Where(a => a.UserId == userId).ToListAsync();
                Console.WriteLine($"Found {accounts.Count} accounts for user {userId}");
                return accounts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting accounts: {ex.Message}");
                throw;
            }
        }
        public async Task<int> SaveAccountAsync(UserAccount account)
        {
            try
            {
                var rowsAffected = await _database.InsertAsync(account);
                Console.WriteLine($"Inserted account with ID {account.Id} for user {account.UserId}");
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving account: {ex.Message}");
                throw;
            }
        }

        public Task<int> DeleteAccountAsync(UserAccount account) => _database.DeleteAsync(account);

        // Методы для UserProfile
        public Task<List<UserProfile>> GetUserProfilesAsync() => _database.Table<UserProfile>().ToListAsync();

        public Task<int> SaveUserProfileAsync(UserProfile profile) => _database.InsertAsync(profile);

        public Task<UserProfile?> GetUserProfileAsync(string userName) =>
            _database.Table<UserProfile>().FirstOrDefaultAsync(p => p.UserName == userName);
    }
}