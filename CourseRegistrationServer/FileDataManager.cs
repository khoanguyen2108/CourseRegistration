using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace CourseRegistrationServer
{
    public class FileDataManager
    {
        private string coursesFile = "courses.json";
        private string registrationsFile = "registrations.json";

        public FileDataManager()
        {
            InitializeFiles();
        }

        private void InitializeFiles()
        {
            try
            {
                // Tạo file courses.json nếu chưa có
                if (!File.Exists(coursesFile))
                {
                    var courses = new List<Course>();
                    SaveCourses(courses);
                    Console.WriteLine("[FILE] Created courses.json");
                }

                // Tạo file registrations.json nếu chưa có
                if (!File.Exists(registrationsFile))
                {
                    var registrations = new List<Registration>();
                    SaveRegistrations(registrations);
                    Console.WriteLine("[FILE] Created registrations.json");
                }

                Console.WriteLine("[FILE] ✅ Files initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] InitializeFiles failed: {ex.Message}");
            }
        }

        public List<Course> GetAllCourses()
        {
            try
            {
                if (!File.Exists(coursesFile))
                    return new List<Course>();

                string json = File.ReadAllText(coursesFile);
                return JsonConvert.DeserializeObject<List<Course>>(json) ?? new List<Course>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetAllCourses failed: {ex.Message}");
                return new List<Course>();
            }
        }

        public bool AddCourse(Course course)
        {
            try
            {
                var courses = GetAllCourses();
                courses.Add(course);
                SaveCourses(courses);
                Console.WriteLine($"[FILE] ✅ AddCourse ({course.CourseId}) success");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] AddCourse failed: {ex.Message}");
                return false;
            }
        }

        public bool DeleteCourse(string courseId)
        {
            try
            {
                var courses = GetAllCourses();
                var courseToRemove = courses.Find(c => c.CourseId == courseId);

                if (courseToRemove == null)
                    return false;

                courses.Remove(courseToRemove);
                SaveCourses(courses);
                Console.WriteLine($"[FILE] ✅ DeleteCourse ({courseId}) success");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] DeleteCourse failed: {ex.Message}");
                return false;
            }
        }

        public bool UpdateAvailableSlots(string courseId, int newSlots)
        {
            try
            {
                var courses = GetAllCourses();
                var course = courses.Find(c => c.CourseId == courseId);

                if (course == null)
                    return false;

                course.AvailableSlots = newSlots;
                SaveCourses(courses);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] UpdateAvailableSlots failed: {ex.Message}");
                return false;
            }
        }

        public List<Registration> GetStudentRegistrations(string studentId)
        {
            try
            {
                var registrations = GetAllRegistrations();
                return registrations.FindAll(r => r.StudentId == studentId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetStudentRegistrations failed: {ex.Message}");
                return new List<Registration>();
            }
        }

        public bool AddRegistration(Registration registration)
        {
            try
            {
                var registrations = GetAllRegistrations();
                registrations.Add(registration);
                SaveRegistrations(registrations);
                Console.WriteLine($"[FILE] ✅ AddRegistration success");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] AddRegistration failed: {ex.Message}");
                return false;
            }
        }

        public bool IsRegistered(string studentId, string courseId)
        {
            try
            {
                var registrations = GetAllRegistrations();
                return registrations.Exists(r => r.StudentId == studentId && r.CourseId == courseId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] IsRegistered failed: {ex.Message}");
                return false;
            }
        }

        private List<Registration> GetAllRegistrations()
        {
            try
            {
                if (!File.Exists(registrationsFile))
                    return new List<Registration>();

                string json = File.ReadAllText(registrationsFile);
                return JsonConvert.DeserializeObject<List<Registration>>(json) ?? new List<Registration>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetAllRegistrations failed: {ex.Message}");
                return new List<Registration>();
            }
        }

        private void SaveCourses(List<Course> courses)
        {
            try
            {
                string json = JsonConvert.SerializeObject(courses, Formatting.Indented);
                File.WriteAllText(coursesFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] SaveCourses failed: {ex.Message}");
            }
        }

        private void SaveRegistrations(List<Registration> registrations)
        {
            try
            {
                string json = JsonConvert.SerializeObject(registrations, Formatting.Indented);
                File.WriteAllText(registrationsFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] SaveRegistrations failed: {ex.Message}");
            }
        }
    }
}