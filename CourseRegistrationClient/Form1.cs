using System;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CourseRegistrationClient
{
    public partial class Form1 : Form
    {
        private ClientSocket clientSocket;

        public Form1()
        {
            InitializeComponent();
            clientSocket = new ClientSocket();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!clientSocket.Connect())
            {
                MessageBox.Show("Không thể kết nối đến server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnViewCourses_Click(object sender, EventArgs e)
        {
            string response = clientSocket.ViewCourses();
            DisplayResponse(response, "danh sách môn học");
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentId.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên!", "Thông báo");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCourseId.Text))
            {
                MessageBox.Show("Vui lòng nhập mã môn học!", "Thông báo");
                return;
            }

            string response = clientSocket.RegisterCourse(txtStudentId.Text, txtCourseId.Text);
            DisplayResponse(response, "đăng ký");
        }

        private void btnViewRegistrations_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentId.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên!", "Thông báo");
                return;
            }

            string response = clientSocket.ViewRegistrations(txtStudentId.Text);
            DisplayResponse(response, "lịch sử đăng ký");
        }

        private void DisplayResponse(string response, string action)
        {
            if (response.StartsWith("SUCCESS"))
            {
                string data = response.Substring("SUCCESS|".Length);

                // ⭐ Parse JSON để hiển thị đẹp
                try
                {
                    // Nếu là JSON array
                    if (data.StartsWith("["))
                    {
                        var courses = JsonConvert.DeserializeObject<dynamic>(data);
                        txtResult.Text = "";
                        foreach (var course in courses)
                        {
                            txtResult.AppendText($"[{course["CourseId"]}] {course["CourseName"]} - {course["Credits"]} tín chỉ - Còn: {course["AvailableSlots"]} chỗ\n");
                        }
                    }
                    else
                    {
                        txtResult.Text = data;
                    }
                }
                catch
                {
                    txtResult.Text = data;
                }
            }
            else if (response.StartsWith("ERROR"))
            {
                string error = response.Substring("ERROR|".Length);
                MessageBox.Show(error, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Phản hồi không hợp lệ", "Lỗi");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            clientSocket.Disconnect();
        }
    }
}