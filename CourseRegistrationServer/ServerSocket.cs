using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace CourseRegistrationServer
{
    public class ServerSocket
    {
        private TcpListener tcpListener;
        private List<Course> courses;
        private List<Registration> registrations;
        private UserManager userManager;
        private int port = 5000;
        private bool isRunning = false;
        private object lockObject = new object();

        public ServerSocket()
        {
            courses = new List<Course>();
            registrations = new List<Registration>();
            userManager = new UserManager();
            InitializeCourses();
            Console.WriteLine("[SERVER] Courses initialized: " + courses.Count);
        }

        private void InitializeCourses()
        {
            courses.Clear();
            courses.Add(new Course("CTT101", "Lập trình C#", 3, 10));
            courses.Add(new Course("CTT102", "Lập trình Web", 3, 5));
            courses.Add(new Course("CTT103", "Cơ sở dữ liệu", 3, 4));
            courses.Add(new Course("CTT104", "Mạng máy tính", 3, 7));
            courses.Add(new Course("CTT105", "An ninh mạng", 3, 3));

            Console.WriteLine("[SERVER] Đã khởi tạo " + courses.Count + " môn học");
        }

        public void Start()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();
                isRunning = true;
                Console.WriteLine($"[SERVER] Đang lắng nghe trên cổng {port}.. .");

                while (isRunning)
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    Console.WriteLine($"[SERVER] Có client kết nối: {client.Client.RemoteEndPoint}");

                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.AutoFlush = true;

                    while (true)
                    {
                        try
                        {
                            string request = reader.ReadLine();
                            if (request == null || request.ToUpper() == "EXIT")
                            {
                                Console.WriteLine($"[SERVER] Client {client.Client.RemoteEndPoint} đã ngắt kết nối");
                                break;
                            }

                            Console.WriteLine($"[SERVER] Nhận yêu cầu: {request}");
                            string response = ProcessRequest(request);

                            Console.WriteLine($"[SERVER] Response: {response.Substring(0, Math.Min(100, response.Length))}...");

                            writer.WriteLine(response);
                            writer.Flush();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ERROR] Lỗi xử lý request: {ex.Message}");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] HandleClient: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        private string ProcessRequest(string request)
        {
            try
            {
                string[] parts = request.Split('|');
                string command = parts[0].Trim().ToUpper();

                switch (command)
                {
                    case "LOGIN":
                        if (parts.Length < 3)
                            return "ERROR|Định dạng sai";
                        string username = parts[1].Trim();
                        string password = parts[2].Trim();
                        return userManager.Login(username, password);

                    case "REGISTER_USER":
                        if (parts.Length < 4)
                            return "ERROR|Định dạng sai";
                        string regUsername = parts[1].Trim();
                        string regPassword = parts[2].Trim();
                        string regFullName = parts[3].Trim();
                        return userManager.Register(regUsername, regPassword, regFullName);

                    case "VIEW_COURSES":
                        return ViewCourses();

                    case "REGISTER":
                        if (parts.Length < 3)
                            return "ERROR|Định dạng sai";

                        string studentId = parts[1].Trim();
                        string courseId = parts[2].Trim();
                        return RegisterCourse(studentId, courseId);

                    case "VIEW_REGISTRATIONS":
                        if (parts.Length < 2)
                            return "ERROR|Định dạng sai";

                        string viewStudentId = parts[1].Trim();
                        return ViewRegistrations(viewStudentId);

                    case "VIEW_ALL_REGISTRATIONS":
                        return ViewAllRegistrations();

                    case "ADD_COURSE":
                        if (parts.Length < 5)
                            return "ERROR|Định dạng sai";

                        string addCourseId = parts[1].Trim();
                        string addCourseName = parts[2].Trim();
                        int addCredits = int.Parse(parts[3].Trim());
                        int addSlots = int.Parse(parts[4].Trim());
                        return AddCourse(addCourseId, addCourseName, addCredits, addSlots);

                    case "DELETE_COURSE":
                        if (parts.Length < 2)
                            return "ERROR|Định dạng sai";

                        string deleteCourseId = parts[1].Trim();
                        return DeleteCourse(deleteCourseId);

                    case "GET_ALL_USERS":
                        return GetAllUsers();

                    case "DELETE_USER":
                        if (parts.Length < 2)
                            return "ERROR|Định dạng sai";
                        string deleteUserId = parts[1].Trim();
                        return userManager.DeleteUser(deleteUserId);

                    case "UPDATE_USER":
                        if (parts.Length < 4)
                            return "ERROR|Định dạng sai";
                        string updateUserId = parts[1].Trim();
                        string updateFullName = parts[2].Trim();
                        string updatePassword = parts[3].Trim();
                        return userManager.UpdateUser(updateUserId, updateFullName, updatePassword);

                    // ============ THÊM CASE MỚI ============
                    case "DELETE_REGISTRATION":
                        if (parts.Length < 3)
                            return "ERROR|Định dạng sai";
                        string deleteRegStudentId = parts[1].Trim();
                        string deleteRegCourseId = parts[2].Trim();
                        return DeleteRegistration(deleteRegStudentId, deleteRegCourseId);
                    // =======================================

                    default:
                        return "ERROR|Lệnh không tồn tại: " + command;
                }
            }
            catch (Exception ex)
            {
                return $"ERROR|{ex.Message}";
            }
        }

        private string ViewCourses()
        {
            lock (lockObject)
            {
                Console.WriteLine($"[DEBUG] ViewCourses: courses count = {courses.Count}");

                if (courses.Count == 0)
                    return "ERROR|Không có môn học nào";

                string json = JsonConvert.SerializeObject(courses);
                Console.WriteLine($"[DEBUG] JSON: {json}");
                return "SUCCESS|" + json;
            }
        }

        private string RegisterCourse(string studentId, string courseId)
        {
            lock (lockObject)
            {
                Course course = courses.FirstOrDefault(c => c.CourseId == courseId);
                if (course == null)
                    return "ERROR|Môn học không tồn tại";

                if (course.AvailableSlots <= 0)
                    return "ERROR|Hết chỗ trống";

                var existingReg = registrations.FirstOrDefault(r => r.StudentId == studentId && r.CourseId == courseId);
                if (existingReg != null)
                    return "ERROR|Bạn đã đăng ký môn này rồi";

                Registration reg = new Registration(studentId, courseId);
                reg.CourseName = course.CourseName;
                reg.Status = "Success";
                registrations.Add(reg);
                course.AvailableSlots--;

                Console.WriteLine($"[SERVER] {studentId} đã đăng ký {courseId} thành công");
                return "SUCCESS|Đăng ký thành công! ";
            }
        }

        private string ViewRegistrations(string studentId)
        {
            lock (lockObject)
            {
                var studentRegs = registrations.Where(r => r.StudentId == studentId).ToList();

                if (studentRegs.Count == 0)
                    return "SUCCESS|[]";

                string json = JsonConvert.SerializeObject(studentRegs);
                return "SUCCESS|" + json;
            }
        }

        private string ViewAllRegistrations()
        {
            lock (lockObject)
            {
                if (registrations.Count == 0)
                    return "SUCCESS|[]";

                List<dynamic> result = new List<dynamic>();
                int counter = 1;
                foreach (var reg in registrations)
                {
                    Course course = courses.FirstOrDefault(c => c.CourseId == reg.CourseId);
                    dynamic item = new System.Dynamic.ExpandoObject();
                    item.RegistrationId = counter++;
                    item.UserId = reg.StudentId;
                    item.CourseId = reg.CourseId;
                    item.CourseName = course != null ? course.CourseName : "N/A";
                    item.Credits = course != null ? course.Credits : 0;
                    item.Semester = "2024.1";
                    result.Add(item);
                }

                string json = JsonConvert.SerializeObject(result);
                return "SUCCESS|" + json;
            }
        }

        private string AddCourse(string courseId, string courseName, int credits, int availableSlots)
        {
            lock (lockObject)
            {
                Console.WriteLine($"[SERVER] ADD_COURSE: {courseId}, {courseName}, {credits}, {availableSlots}");

                // Kiểm tra mã môn đã tồn tại chưa
                if (courses.Any(c => c.CourseId == courseId))
                {
                    Console.WriteLine($"[ERROR] Mã môn {courseId} đã tồn tại");
                    return "ERROR|Mã môn này đã tồn tại";
                }

                // Thêm môn mới
                Course newCourse = new Course(courseId, courseName, credits, availableSlots);
                courses.Add(newCourse);

                Console.WriteLine($"[SERVER] ✅ Đã thêm môn: {courseId}");
                return "SUCCESS|Thêm môn thành công! ";
            }
        }

        private string DeleteCourse(string courseId)
        {
            lock (lockObject)
            {
                Console.WriteLine($"[SERVER] DELETE_COURSE: {courseId}");

                // Tìm môn học
                Course courseToDelete = courses.FirstOrDefault(c => c.CourseId == courseId);

                if (courseToDelete == null)
                {
                    Console.WriteLine($"[ERROR] Không tìm thấy môn {courseId}");
                    return "ERROR|Không tìm thấy môn học";
                }

                // Xóa môn
                courses.Remove(courseToDelete);

                Console.WriteLine($"[SERVER] ✅ Đã xóa môn: {courseId}");
                return "SUCCESS|Xóa môn thành công!";
            }
        }

        // ============ THÊM PHƯƠNG THỨC MỚI ============
        private string DeleteRegistration(string studentId, string courseId)
        {
            lock (lockObject)
            {
                Console.WriteLine($"[SERVER] DELETE_REGISTRATION: Student={studentId}, Course={courseId}");

                // Tìm đăng ký cần xóa
                Registration regToDelete = registrations.FirstOrDefault(r =>
                    r.StudentId == studentId && r.CourseId == courseId);

                if (regToDelete == null)
                {
                    Console.WriteLine($"[ERROR] Không tìm thấy đăng ký của sinh viên {studentId} cho môn {courseId}");
                    return "ERROR|Không tìm thấy đăng ký";
                }

                // Xóa đăng ký
                registrations.Remove(regToDelete);

                // Tăng số chỗ trống của môn học
                Course course = courses.FirstOrDefault(c => c.CourseId == courseId);
                if (course != null)
                {
                    course.AvailableSlots++;
                    Console.WriteLine($"[SERVER] Tăng chỗ trống môn {courseId} lên {course.AvailableSlots}");
                }

                Console.WriteLine($"[SERVER] ✅ Đã xóa đăng ký của sinh viên {studentId} khỏi môn {courseId}");
                return "SUCCESS|Đã xóa sinh viên khỏi môn học";
            }
        }
        // =============================================

        private string GetAllUsers()
        {
            lock (lockObject)
            {
                List<User> allUsers = userManager.GetAllUsers();
                if (allUsers.Count == 0)
                    return "SUCCESS|[]";

                string json = JsonConvert.SerializeObject(allUsers);
                return "SUCCESS|" + json;
            }
        }

        public void Stop()
        {
            isRunning = false;
            tcpListener?.Stop();
        }
    }
}