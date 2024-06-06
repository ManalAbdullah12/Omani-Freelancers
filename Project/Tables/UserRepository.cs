using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Tables
{
    public class UserRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public UserRepository(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<RegUserTable>().Wait();
            _database.CreateTableAsync<Posts>().Wait();
            _database.CreateTableAsync<Jobs>().Wait();
        }

        public async Task DeleteAllData()
        {
            await _database.DropTableAsync<RegUserTable>();
            await _database.CreateTableAsync<RegUserTable>();
        }

        // Method to update user data
        public async Task UpdateUserData(RegUserTable user)
        {
            await _database.UpdateAsync(user);
        }

        public async Task DeleteUser(RegUserTable user)
        {
            await _database.DeleteAsync(user);
        }

        // Method to get user data by UserName
        public async Task<RegUserTable> GetUserByUserName(string userName)
        {
            return await _database.Table<RegUserTable>().FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<bool> AddPost(Posts posts)
        {
            try
            {
                var CheckExisting = await _database.Table<Posts>().Where(p=> p.FreelancerId == posts.FreelancerId && p.PostDetails.Equals(posts.PostDetails)).FirstOrDefaultAsync();
                if (CheckExisting == null)
                {
                    await _database.InsertAsync(posts);
                    return true;
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public async Task<List<Posts>> GetClientPosts(RegUserTable regUserTable)
        {
            try
            {
                return await _database.Table<Posts>().Where(p=> p.FreelancerId == regUserTable.Id).ToListAsync();
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public async Task DeletePosts(Posts posts)
        {
            try
            {
                await _database.DeleteAsync(posts);
            }
            catch (Exception ex)
            {
            }
        }
        public async Task<bool> AddJobs(Jobs jobs)
        {
            try
            {
                await _database.InsertAsync(jobs);
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public async Task DeleteJob(Jobs jobs)
        {
            try
            {
                await _database.DeleteAsync(jobs);
            }
            catch (Exception ex)
            {
            }
        }

        public async Task UpdateJob(Jobs jobs)
        {
            try
            {
                await _database.UpdateAsync(jobs);
            }
            catch (Exception ex)
            {
            }
        }

        public async Task<List<Jobs>> GetClientJobs(int id)
        {
            try
            {
                return await _database.Table<Jobs>().Where(p => p.UserId == id && ((p.IsAccepted == false && p.IsJobFinished == false && p.IsPayment == false) || (p.IsAccepted == true && p.IsJobFinished == true && p.IsPayment == true))).ToListAsync();
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<List<Jobs>> GetFreelancerJobs(int id)
        {
            try
            {
                return await _database.Table<Jobs>().Where(p => p.FreelancerId == id && ((p.IsAccepted == false && p.IsJobFinished == false) || ( p.IsAccepted == true && p.IsJobFinished == true))).ToListAsync();
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<List<Jobs>> GetAcceptedClientJobs(int id)
        {
            try
            {
                return await _database.Table<Jobs>().Where(p => p.UserId == id && p.IsAccepted == true && p.IsPayment == false && p.IsJobFinished == true).ToListAsync();
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<List<Jobs>> GetAcceptedFreelancerJobs(int id)
        {
            try
            {
                return await _database.Table<Jobs>().Where(p => p.FreelancerId == id && p.IsAccepted == true && p.IsPayment == false && p.IsJobFinished == false).ToListAsync();
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<List<Jobs>> GetClientPayment(int id)
        {
            try
            {
                return await _database.Table<Jobs>().Where(p => p.UserId == id && p.IsAccepted == true && p.IsJobFinished == true && p.IsPayment == true).ToListAsync();
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<List<Jobs>> GetFreelancerPayment(int id)
        {
            try
            {
                return await _database.Table<Jobs>().Where(p => p.FreelancerId == id && p.IsAccepted == true && p.IsJobFinished == true && p.IsPayment == true).ToListAsync();
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
