using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace CourseRegistrationClient
{
    public class ClientSocket
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;
        private string serverAddress = "127.0.0.1";
        private int serverPort = 5000;

        public bool Connect()
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(serverAddress, serverPort);
                stream = tcpClient.GetStream();
                reader = new StreamReader(stream, Encoding.UTF8);
                writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                System.Windows.Forms.MessageBox.Show("[CLIENT] Đã kết nối đến server", "Thông báo");
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"[ERROR] Không thể kết nối: {ex.Message}", "Lỗi");
                return false;
            }
        }

        public string SendRequest(string request)
        {
            try
            {
                writer.WriteLine(request);

                // ⭐ Đơn giản: Chỉ đọc 1 dòng (JSON đã ở trên 1 dòng)
                string response = reader.ReadLine();

                if (response == null)
                    return "ERROR|Không nhận được phản hồi";

                return response;
            }
            catch (Exception ex)
            {
                return $"ERROR|{ex.Message}";
            }
        }

        public string ViewCourses()
        {
            return SendRequest("VIEW_COURSES");
        }

        public string RegisterCourse(string studentId, string courseId)
        {
            return SendRequest($"REGISTER|{studentId}|{courseId}");
        }

        public string ViewRegistrations(string studentId)
        {
            return SendRequest($"VIEW_REGISTRATIONS|{studentId}");
        }

        public void Disconnect()
        {
            try
            {
                writer?.WriteLine("EXIT");
                stream?.Close();
                tcpClient?.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"[ERROR] {ex.Message}", "Lỗi");
            }
        }
    }
}