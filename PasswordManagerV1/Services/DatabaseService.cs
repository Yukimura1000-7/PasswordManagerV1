using System.Collections.Generic;
using System.Threading.Tasks;
using System;
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
        public Task<List<UserAccount>> GetAccountsAsync(int userId) =>
            _database.Table<UserAccount>().Where(a => a.UserId == userId).ToListAsync();

        public Task<int> SaveAccountAsync(UserAccount account) =>
            _database.InsertAsync(account);

        public Task<int> DeleteAccountAsync(UserAccount account) =>
            _database.DeleteAsync(account);

        // Методы для UserProfile
        public Task<List<UserProfile>> GetUserProfilesAsync() =>
            _database.Table<UserProfile>().ToListAsync();

        public Task<int> SaveUserProfileAsync(UserProfile profile) =>
            _database.InsertAsync(profile);

        public Task<UserProfile?> GetUserProfileAsync(string userName) =>
            _database.Table<UserProfile>().FirstOrDefaultAsync(p => p.UserName == userName);
    }
}