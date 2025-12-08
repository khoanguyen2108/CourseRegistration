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

        // ⭐⭐⭐ THÊM 3 HÀM MỚI NÀY ⭐⭐⭐

        private void btnAddCourse_Click(object sender, EventArgs e)
        {
            string courseId = InputBox("Mã Môn:", "Thêm Môn Học");
            if (string.IsNullOrEmpty(courseId)) return;

            string courseName = InputBox("Tên Môn:", "Thêm Môn Học");
            if (string.IsNullOrEmpty(courseName)) return;

            if (!int.TryParse(InputBox("Số Tín Chỉ:", "Thêm Môn Học"), out int credits))
            {
                MessageBox.Show("Số tín chỉ phải là số!", "Lỗi");
                return;
            }

            if (!int.TryParse(InputBox("Số Chỗ Còn:", "Thêm Môn Học"), out int slots))
            {
                MessageBox.Show("Số chỗ phải là số!", "Lỗi");
                return;
            }

            string response = clientSocket.AddCourse(courseId, courseName, credits, slots);
            DisplayResponse(response, "thêm môn");
        }

        private void btnDeleteCourse_Click(object sender, EventArgs e)
        {
            string courseId = InputBox("Nhập Mã Môn Cần Xóa:", "Xóa Môn Học");
            if (string.IsNullOrEmpty(courseId)) return;

            string response = clientSocket.DeleteCourse(courseId);
            DisplayResponse(response, "xóa môn");
        }

        private string InputBox(string prompt, string title)
        {
            Form form = new Form();
            form.Text = title;
            form.Width = 350;
            form.Height = 150;
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowInTaskbar = false;

            Label label = new Label() { Left = 20, Top = 20, Text = prompt, Width = 310 };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 310 };
            Button okButton = new Button() { Text = "OK", Left = 150, Width = 80, Top = 80, DialogResult = DialogResult.OK };
            Button cancelButton = new Button() { Text = "Cancel", Left = 240, Width = 80, Top = 80, DialogResult = DialogResult.Cancel };

            form.Controls.Add(label);
            form.Controls.Add(textBox);
            form.Controls.Add(okButton);
            form.Controls.Add(cancelButton);
            form.AcceptButton = okButton;
            form.CancelButton = cancelButton;

            return form.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }

        // ⭐⭐⭐ KẾT THÚC THÊM ⭐⭐⭐
    }
}