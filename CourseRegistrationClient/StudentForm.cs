using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CourseRegistrationClient
{
    public partial class StudentForm : Form
    {
        private ClientSocket clientSocket;
        private string studentId;
        private string fullName;
        private SplitContainer splitContainer;
        private DataGridView dgvAvailableCourses;
        private DataGridView dgvMyRegistrations;
        private Button btnRegisterCourse;
        private Button btnCancelRegistration;
        private Button btnRefresh;
        private List<string> registeredCourseIds = new List<string>(); // Danh sách ID môn đã đăng ký

        public StudentForm(ClientSocket socket, string id, string name)
        {
            clientSocket = socket;
            studentId = id;
            fullName = name;

            this.Text = "SINH VIÊN - Hệ thống đăng ký Môn học";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

            InitializeComponent();
            this.Load += StudentForm_Load;
            this.Shown += (s, e) => LoadAllData();
        }

        private void StudentForm_Load(object sender, EventArgs e)
        {
            LoadAllData();
            Console.WriteLine("[DEBUG] StudentForm loaded, calling LoadAllData()");
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Top toolbar - GIỐNG ADMINFORM
            Panel toolbar = new Panel();
            toolbar.Dock = DockStyle.Top;
            toolbar.Height = 40;
            toolbar.BackColor = Color.FromArgb(0, 123, 255);
            this.Controls.Add(toolbar);

            Label lblTitle = new Label();
            lblTitle.Text = $"SINH VIÊN - {studentId}";
            lblTitle.Location = new Point(20, 8);
            lblTitle.Font = new Font("Microsoft Sans Serif", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            toolbar.Controls.Add(lblTitle);

            Button btnLogout = new Button();
            btnLogout.Text = "Đăng xuất";
            btnLogout.Location = new Point(1050, 3);
            btnLogout.Size = new Size(120, 35);
            btnLogout.BackColor = Color.FromArgb(220, 53, 69);
            btnLogout.ForeColor = Color.White;
            btnLogout.Font = new Font("Microsoft Sans Serif", 10);
            btnLogout.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            toolbar.Controls.Add(btnLogout);

            // Split container chia 2 phần GIỐNG ADMINFORM
            splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Orientation = Orientation.Horizontal;
            splitContainer.SplitterDistance = 400; // Phần trên cao hơn
            splitContainer.SplitterWidth = 5;
            this.Controls.Add(splitContainer);

            // ========== PHẦN TRÊN: DANH SÁCH MÔN HỌC CÓ THỂ ĐĂNG KÝ/HỦY ==========
            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Fill;
            topPanel.BackColor = Color.White;
            splitContainer.Panel1.Controls.Add(topPanel);

            // Panel nút bấm
            Panel buttonPanel = new Panel();
            buttonPanel.Height = 40;
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.BackColor = Color.FromArgb(248, 249, 250);
            buttonPanel.BorderStyle = BorderStyle.FixedSingle;
            topPanel.Controls.Add(buttonPanel);

            // Nút Đăng ký môn
            btnRegisterCourse = new Button()
            {
                Text = "ĐĂNG KÝ MÔN",
                Location = new Point(15,4),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(40, 167, 69), // Xanh lá
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnRegisterCourse.Click += BtnRegisterCourse_Click;
            buttonPanel.Controls.Add(btnRegisterCourse);

            // Nút Hủy đăng ký
            btnCancelRegistration = new Button()
            {
                Text = "HỦY ĐĂNG KÝ",
                Location = new Point(150, 4),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(220, 53, 69), // Đỏ
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnCancelRegistration.Click += BtnCancelRegistration_Click;
            buttonPanel.Controls.Add(btnCancelRegistration);

            // Nút Làm mới
            btnRefresh = new Button()
            {
                Text = "LÀM MỚI",
                Location = new Point(285, 4),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(0, 123, 255), // Xanh dương
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnRefresh.Click += (s, e) => LoadAllData();
            buttonPanel.Controls.Add(btnRefresh);

            // Label tiêu đề phần trên
            Label lblAvailableCourses = new Label()
            {
                Text = "DANH SÁCH MÔN HỌC",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };
            topPanel.Controls.Add(lblAvailableCourses);

            // DataGridView môn học
            dgvAvailableCourses = new DataGridView();
            dgvAvailableCourses.Dock = DockStyle.Fill;
            dgvAvailableCourses.AutoGenerateColumns = false;
            dgvAvailableCourses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAvailableCourses.AllowUserToAddRows = false;
            dgvAvailableCourses.ReadOnly = true; // Chỉ đọc
            dgvAvailableCourses.BackgroundColor = Color.White;
            dgvAvailableCourses.GridColor = Color.LightGray;
            dgvAvailableCourses.ColumnHeadersHeight = 100;
            dgvAvailableCourses.Font = new Font("Microsoft Sans Serif", 10);
            dgvAvailableCourses.RowTemplate.Height = 30;
            dgvAvailableCourses.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
            dgvAvailableCourses.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgvAvailableCourses.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAvailableCourses.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            dgvAvailableCourses.EnableHeadersVisualStyles = false;

            // Cột MÃ MÔN
            dgvAvailableCourses.Columns.Add("CourseId", "MÃ MÔN");
            dgvAvailableCourses.Columns[0].Width = 180;
            dgvAvailableCourses.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            // Cột TÊN MÔN HỌC
            dgvAvailableCourses.Columns.Add("CourseName", "TÊN MÔN HỌC");
            dgvAvailableCourses.Columns[1].Width = 390;
            dgvAvailableCourses.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            // Cột TÍN CHỈ
            dgvAvailableCourses.Columns.Add("Credits", "TÍN CHỈ");
            dgvAvailableCourses.Columns[2].Width = 190;
            dgvAvailableCourses.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            // Cột CHỖ TRỐNG
            dgvAvailableCourses.Columns.Add("AvailableSlots", "CHỖ TRỐNG");
            dgvAvailableCourses.Columns[3].Width = 190;
            dgvAvailableCourses.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            // Cột TRẠNG THÁI (mới: hiển thị đã đăng ký hay chưa)
            dgvAvailableCourses.Columns.Add("Status", "TRẠNG THÁI");
            dgvAvailableCourses.Columns[4].Width = 190;
            dgvAvailableCourses.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            dgvAvailableCourses.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            topPanel.Controls.Add(dgvAvailableCourses);

            // ========== PHẦN DƯỚI: MÔN HỌC ĐÃ ĐĂNG KÝ (CHỈ HIỂN THỊ) ==========
            Panel bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Fill;
            bottomPanel.BackColor = Color.White;
            splitContainer.Panel2.Controls.Add(bottomPanel);

            // Tiêu đề phần dưới
            Label lblMyRegistrations = new Label();
            lblMyRegistrations.Text = "MÔN HỌC ĐÃ ĐĂNG KÝ";
            lblMyRegistrations.Dock = DockStyle.Top;
            lblMyRegistrations.Height = 40;
            lblMyRegistrations.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            lblMyRegistrations.ForeColor = Color.FromArgb(0, 123, 255);
            lblMyRegistrations.TextAlign = ContentAlignment.MiddleCenter;
            lblMyRegistrations.BackColor = Color.FromArgb(248, 249, 250);
            lblMyRegistrations.BorderStyle = BorderStyle.FixedSingle;
            bottomPanel.Controls.Add(lblMyRegistrations);

            // DataGridView môn đã đăng ký - CHỈ HIỂN THỊ, KHÔNG CHỌN ĐƯỢC
            dgvMyRegistrations = new DataGridView();
            dgvMyRegistrations.Dock = DockStyle.Fill;
            dgvMyRegistrations.AutoGenerateColumns = false;
            dgvMyRegistrations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMyRegistrations.AllowUserToAddRows = false;
            dgvMyRegistrations.ReadOnly = true; // Chỉ đọc
            dgvMyRegistrations.Enabled = false; // Tắt tương tác
            dgvMyRegistrations.BackgroundColor = Color.FromArgb(248, 249, 250);
            dgvMyRegistrations.GridColor = Color.LightGray;
            dgvMyRegistrations.ColumnHeadersHeight = 60;
            dgvMyRegistrations.Font = new Font("Microsoft Sans Serif", 10);
            dgvMyRegistrations.RowTemplate.Height = 30;
            dgvMyRegistrations.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
            dgvMyRegistrations.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(108, 117, 125); // Màu xám
            dgvMyRegistrations.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMyRegistrations.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            dgvMyRegistrations.EnableHeadersVisualStyles = false;

            // Cột MÃ MÔN
            dgvMyRegistrations.Columns.Add("CourseId", "MÃ MÔN");
            dgvMyRegistrations.Columns[0].Width = 231;
            dgvMyRegistrations.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            // Cột TÊN MÔN HỌC
            dgvMyRegistrations.Columns.Add("CourseName", "TÊN MÔN HỌC");
            dgvMyRegistrations.Columns[1].Width = 448;
            dgvMyRegistrations.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            // Cột TÍN CHỈ
            dgvMyRegistrations.Columns.Add("Credits", "TÍN CHỈ");
            dgvMyRegistrations.Columns[2].Width = 231;
            dgvMyRegistrations.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            // Cột TRẠNG THÁI
            dgvMyRegistrations.Columns.Add("Status", "TRẠNG THÁI");
            dgvMyRegistrations.Columns[3].Width = 231;
            dgvMyRegistrations.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            dgvMyRegistrations.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            bottomPanel.Controls.Add(dgvMyRegistrations);

            this.ResumeLayout(false);
        }

        private void LoadAllData()
        {
            LoadMyRegistrations(); // Load danh sách đã đăng ký trước
            LoadAvailableCourses(); // Rồi mới load tất cả môn học
        }

        private void LoadAvailableCourses()
        {
            if (dgvAvailableCourses == null) return;

            Console.WriteLine("[DEBUG] Student loading available courses...");
            string response = clientSocket.SendRequest("VIEW_COURSES");
            Console.WriteLine($"[DEBUG] Server response: {response}");

            dgvAvailableCourses.Rows.Clear();

            if (response.StartsWith("SUCCESS"))
            {
                try
                {
                    string json = response.Substring(8);
                    dynamic courses = JsonConvert.DeserializeObject(json);
                    foreach (var course in courses)
                    {
                        string courseId = course["CourseId"].ToString();
                        bool isRegistered = registeredCourseIds.Contains(courseId);

                        dgvAvailableCourses.Rows.Add(
                            course["CourseId"],
                            course["CourseName"],
                            course["Credits"],
                            course["AvailableSlots"],
                            isRegistered ? "ĐÃ ĐĂNG KÝ" : "CÓ THỂ ĐĂNG KÝ"
                        );

                        // Tô màu dòng đã đăng ký
                        if (isRegistered)
                        {
                            int rowIndex = dgvAvailableCourses.Rows.Count - 1;
                            dgvAvailableCourses.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 205); // Màu vàng nhạt
                            dgvAvailableCourses.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(133, 100, 4);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] LoadAvailableCourses: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"[DEBUG] Server returned error: {response}");
            }
        }

        private void LoadMyRegistrations()
        {
            if (dgvMyRegistrations == null) return;

            string response = clientSocket.SendRequest($"VIEW_REGISTRATIONS|{studentId}");
            dgvMyRegistrations.Rows.Clear();
            registeredCourseIds.Clear(); // Xóa danh sách cũ

            if (response.StartsWith("SUCCESS"))
            {
                try
                {
                    string json = response.Substring(8);
                    if (json.Trim() != "[]")
                    {
                        dynamic registrations = JsonConvert.DeserializeObject(json);
                        foreach (var reg in registrations)
                        {
                            string courseId = reg["CourseId"].ToString();
                            registeredCourseIds.Add(courseId); // Thêm vào danh sách đã đăng ký

                            dgvMyRegistrations.Rows.Add(
                                reg["CourseId"],
                                reg["CourseName"],
                                "3", // Mặc định 3 tín chỉ
                                reg["Status"] ?? "Đã đăng ký"
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] LoadMyRegistrations: {ex.Message}");
                }
            }
        }

        // ========== EVENT HANDLERS ==========

        private void BtnRegisterCourse_Click(object sender, EventArgs e)
        {
            if (dgvAvailableCourses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn môn học để đăng ký!", "Thông báo",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string courseId = dgvAvailableCourses.SelectedRows[0].Cells["CourseId"].Value.ToString();
            string courseName = dgvAvailableCourses.SelectedRows[0].Cells["CourseName"].Value.ToString();
            string status = dgvAvailableCourses.SelectedRows[0].Cells["Status"].Value.ToString();

            // Kiểm tra đã đăng ký chưa
            if (status == "ĐÃ ĐĂNG KÝ")
            {
                MessageBox.Show($"Bạn đã đăng ký môn {courseName} rồi!", "Thông báo",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra còn chỗ trống không
            int availableSlots = Convert.ToInt32(dgvAvailableCourses.SelectedRows[0].Cells["AvailableSlots"].Value);
            if (availableSlots <= 0)
            {
                MessageBox.Show($"Môn {courseName} đã hết chỗ trống!", "Thông báo",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show($"Bạn có chắc muốn đăng ký môn:\n{courseId} - {courseName}?",
                                                 "Xác nhận đăng ký",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                string response = clientSocket.SendRequest($"REGISTER|{studentId}|{courseId}");

                if (response.StartsWith("SUCCESS"))
                {
                    MessageBox.Show($"Đăng ký môn {courseName} thành công!", "Thành công",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAllData(); // Load lại cả 2 bảng
                }
                else
                {
                    string errorMsg = response.StartsWith("ERROR") ? response.Substring(6) : response;
                    MessageBox.Show("Lỗi: " + errorMsg, "Lỗi",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnCancelRegistration_Click(object sender, EventArgs e)
        {
            if (dgvAvailableCourses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn môn học để hủy đăng ký!", "Thông báo",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string courseId = dgvAvailableCourses.SelectedRows[0].Cells["CourseId"].Value.ToString();
            string courseName = dgvAvailableCourses.SelectedRows[0].Cells["CourseName"].Value.ToString();
            string status = dgvAvailableCourses.SelectedRows[0].Cells["Status"].Value.ToString();

            // Kiểm tra có phải môn đã đăng ký không
            if (status != "ĐÃ ĐĂNG KÝ")
            {
                MessageBox.Show($"Bạn chưa đăng ký môn {courseName}!", "Thông báo",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show($"Bạn có chắc muốn hủy đăng ký môn:\n{courseId} - {courseName}?",
                                                 "Xác nhận hủy đăng ký",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                string response = clientSocket.SendRequest($"DELETE_REGISTRATION|{studentId}|{courseId}");

                if (response.StartsWith("SUCCESS"))
                {
                    MessageBox.Show($"Hủy đăng ký môn {courseName} thành công!", "Thành công",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAllData(); // Load lại cả 2 bảng
                }
                else
                {
                    string errorMsg = response.StartsWith("ERROR") ? response.Substring(6) : response;
                    MessageBox.Show("Lỗi: " + errorMsg, "Lỗi",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}