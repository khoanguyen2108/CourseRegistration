using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace CourseRegistrationServer
{
    public class UserManager
    {
        private List<User> users;
        private string dataFilePath = "users.json";
        private object lockObject = new object();

        public UserManager()
        {
            users = new List<User>();
            LoadUsers();
            InitializeDefaultUsers();
        }

        private void LoadUsers()
        {
            try
            {
                if (File.Exists(dataFilePath))
                {
                    string json = File.ReadAllText(dataFilePath);
                    users = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
                    Console.WriteLine($"[SERVER] Loaded {users.Count} users from file");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] LoadUsers: {ex.Message}");
                users = new List<User>();
            }
        }

        private void SaveUsers()
        {
            try
            {
                string json = JsonConvert.SerializeObject(users, Formatting.Indented);
                File.WriteAllText(dataFilePath, json);
                Console.WriteLine("[SERVER] Users saved successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] SaveUsers: {ex.Message}");
            }
        }

        private void InitializeDefaultUsers()
        {
            lock (lockObject)
            {
                // N?u ch?a có user nào, t?o admin m?c ??nh
                if (users.Count == 0)
                {
                    users.Add(new User("ADMIN001", "admin", "admin123", "Admin", "Qu?n tr? viên"));
                    users.Add(new User("SV001", "student", "student123", "Student", "Sinh viên 001"));
                    SaveUsers();
                    Console.WriteLine("[SERVER] Default users initialized");
                }
            }
        }

        public string Login(string username, string password)
        {
            lock (lockObject)
            {
                User user = users.FirstOrDefault(u => u.Username == username);
                
                if (user == null)
                {
                    Console.WriteLine($"[ERROR] User not found: {username}");
                    return "ERROR|Tài kho?n không t?n t?i";
                }

                if (user.Password != password)
                {
                    Console.WriteLine($"[ERROR] Wrong password for user: {username}");
                    return "ERROR|M?t kh?u sai";
                }

                Console.WriteLine($"[SERVER] User logged in: {username} ({user.Role})");
                return $"SUCCESS|{user.UserId}|{user.Role}|{user.FullName}";
            }
        }

        public string Register(string username, string password, string fullName)
        {
            lock (lockObject)
            {
                // Ki?m tra username ?ã t?n t?i
                if (users.Any(u => u.Username == username))
                {
                    Console.WriteLine($"[ERROR] Username already exists: {username}");
                    return "ERROR|Tên ??ng nh?p ?ã t?n t?i";
                }

                // T?o user ID t? ??ng
                string userId = "SV" + DateTime.Now.Ticks.ToString().Substring(DateTime.Now.Ticks.ToString().Length - 6);

                User newUser = new User(userId, username, password, "Student", fullName);
                users.Add(newUser);
                SaveUsers();

                Console.WriteLine($"[SERVER] New user registered: {username} (ID: {userId})");
                return $"SUCCESS|{userId}|Student|{fullName}";
            }
        }

        public List<User> GetAllUsers()
        {
            lock (lockObject)
            {
                return new List<User>(users);
            }
        }

        public User GetUserById(string userId)
        {
            lock (lockObject)
            {
                return users.FirstOrDefault(u => u.UserId == userId);
            }
        }

        public string DeleteUser(string userId)
        {
            lock (lockObject)
            {
                User user = users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                    return "ERROR|Không tìm th?y ng??i dùng";

                // Không cho phép xóa admin
                if (user.Role == "Admin")
                    return "ERROR|Không th? xóa tài kho?n Admin";

                users.Remove(user);
                SaveUsers();

                Console.WriteLine($"[SERVER] User deleted: {userId}");
                return "SUCCESS|Xóa ng??i dùng thành công";
            }
        }

        public string UpdateUser(string userId, string fullName, string password)
        {
            lock (lockObject)
            {
                User user = users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                    return "ERROR|Không tìm th?y ng??i dùng";

                if (!string.IsNullOrEmpty(fullName))
                    user.FullName = fullName;

                if (!string.IsNullOrEmpty(password))
                    user.Password = password;

                SaveUsers();

                Console.WriteLine($"[SERVER] User updated: {userId}");
                return "SUCCESS|C?p nh?t thành công";
            }
        }
    }
}
