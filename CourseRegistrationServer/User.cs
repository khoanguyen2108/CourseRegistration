using System;

namespace CourseRegistrationServer
{
    public class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Student" ho?c "Admin"
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }

        public User()
        {
            CreatedAt = DateTime.Now;
        }

        public User(string userId, string username, string password, string role, string fullName)
        {
            UserId = userId;
            Username = username;
            Password = password;
            Role = role;
            FullName = fullName;
            CreatedAt = DateTime.Now;
        }
    }
}
