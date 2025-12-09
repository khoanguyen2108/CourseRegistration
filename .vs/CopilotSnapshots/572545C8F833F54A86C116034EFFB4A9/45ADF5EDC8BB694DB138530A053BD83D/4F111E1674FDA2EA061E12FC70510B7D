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
        private int port = 5000;
        private bool isRunning = false;
        private object lockObject = new object();

        public ServerSocket()
        {
            courses = new List<Course>();
            registrations = new List<Registration>();
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

                    // ⭐⭐⭐ THÊM 2 CASE MỚI NÀY ⭐⭐⭐
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
                    // ⭐⭐⭐ KẾT THÚC THÊM ⭐⭐⭐

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

        // ⭐⭐⭐ THÊM 2 HÀM MỚI NÀY ⭐⭐⭐
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
        // ⭐⭐⭐ KẾT THÚC THÊM ⭐⭐⭐

        public void Stop()
        {
            isRunning = false;
            tcpListener?.Stop();
        }
    }
}