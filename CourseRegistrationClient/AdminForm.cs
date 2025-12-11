using System;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Drawing;
using System.Collections.Generic;

namespace CourseRegistrationClient
{
    public class AdminForm : Form
    {
        private ClientSocket clientSocket;
        private string userId;
        private string fullName;
        private TabControl tabControl;
        private DataGridView dgvCourses;
        private DataGridView dgvUsers;
        private DataGridView dgvRegistrations;
        private DataGridView dgvCourseStudents; // NEW: Hiển thị SV đăng ký môn

        public AdminForm(ClientSocket socket, string id, string name)
        {
            clientSocket = socket;
            userId = id;
            fullName = name;

            this.Text = "Admin - Hệ thống đăng ký Môn học";
            this.Size = new Size(1200, 800); // Tăng chiều cao
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Top toolbar
            Panel toolbar = new Panel();
            toolbar.Dock = DockStyle.Top;
            toolbar.Height = 40;
            toolbar.BackColor = Color.FromArgb(0, 123, 255);
            this.Controls.Add(toolbar);

            Label lblTitle = new Label();
            lblTitle.Text = "QUẢN TRỊ VIÊN" ;
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

            // TabControl
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Microsoft Sans Serif", 10);
            this.Controls.Add(tabControl);

            // Tab 1: Quản lý Môn học
            TabPage tabCourses = new TabPage("Quản lý Môn học");
            tabCourses.BackColor = Color.White;
            CreateCoursesTab(tabCourses);
            tabControl.TabPages.Add(tabCourses);

            // Tab 2: Quản lý Sinh viên
            TabPage tabUsers = new TabPage("Quản lý Sinh viên");
            tabUsers.BackColor = Color.White;
            CreateUsersTab(tabUsers);
            tabControl.TabPages.Add(tabUsers);

            // Tab 3: Xem Đăng ký Sinh viên
            TabPage tabRegistrations = new TabPage("Xem Đăng ký Sinh viên");
            tabRegistrations.BackColor = Color.White;
            CreateRegistrationsTab(tabRegistrations);
            tabControl.TabPages.Add(tabRegistrations);

            this.ResumeLayout(false);

            LoadAllData();
        }

        private void CreateCoursesTab(TabPage tab)
        {
            // Panel chính - Split container
            SplitContainer splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Orientation = Orientation.Horizontal;
            splitContainer.SplitterDistance = 400; // Môn học chiếm 400px
            tab.Controls.Add(splitContainer);

            // Panel trên: Danh sách môn học + nút
            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Fill;
            topPanel.BackColor = Color.White;
            splitContainer.Panel1.Controls.Add(topPanel);

            // Panel nút bấm
            Panel buttonPanel = new Panel();
            buttonPanel.Height = 60;
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.BackColor = Color.FromArgb(248, 249, 250);
            buttonPanel.BorderStyle = BorderStyle.FixedSingle;
            topPanel.Controls.Add(buttonPanel);

            // Nút Thêm môn (mở popup)
            Button btnAddCourse = new Button()
            {
                Text = "➕ THÊM MÔN",
                Location = new Point(15, 15),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnAddCourse.Click += BtnAddCoursePopup_Click;
            buttonPanel.Controls.Add(btnAddCourse);

            // Nút Xóa môn đã chọn
            Button btnDeleteCourse = new Button()
            {
                Text = "🗑️ XÓA MÔN",
                Location = new Point(180, 15),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnDeleteCourse.Click += BtnDeleteCourse_Click;
            buttonPanel.Controls.Add(btnDeleteCourse);

            // Nút Làm mới
            Button btnRefreshCourses = new Button()
            {
                Text = "🔄 LÀM MỚI",
                Location = new Point(345, 15),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnRefreshCourses.Click += (s, e) => LoadCoursesData();
            buttonPanel.Controls.Add(btnRefreshCourses);

            // DataGridView môn học
            dgvCourses = new DataGridView();
            dgvCourses.Dock = DockStyle.Fill;
            dgvCourses.AutoGenerateColumns = false;
            dgvCourses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCourses.AllowUserToAddRows = false;
            dgvCourses.BackgroundColor = Color.White;
            dgvCourses.GridColor = Color.LightGray;
            dgvCourses.ColumnHeadersHeight = 135; // *** ĐÃ CHỈNH SỬA: Chiều cao tiêu đề cột giảm xuống 30 ***
            dgvCourses.Font = new Font("Microsoft Sans Serif", 10);
            dgvCourses.RowTemplate.Height = 30; // Đã chỉnh sửa từ 35 xuống 25
            dgvCourses.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
            dgvCourses.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgvCourses.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCourses.EnableHeadersVisualStyles = false;
            dgvCourses.SelectionChanged += DgvCourses_SelectionChanged; // NEW: Load SV khi chọn môn

            // Đảm bảo tiêu đề cột được căn giữa 
            dgvCourses.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // ... (các thiết lập cột)

            dgvCourses.Columns.Add("CourseId", "MÃ MÔN");
            dgvCourses.Columns[0].Width = 231;
            dgvCourses.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvCourses.Columns.Add("CourseName", "TÊN MÔN HỌC");
            dgvCourses.Columns[1].Width = 440;
            dgvCourses.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvCourses.Columns.Add("Credits", "TÍN CHỈ");
            dgvCourses.Columns[2].Width = 231;
            dgvCourses.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvCourses.Columns.Add("AvailableSlots", "CHỖ TRỐNG");
            dgvCourses.Columns[3].Width = 231;
            dgvCourses.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            dgvCourses.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            topPanel.Controls.Add(dgvCourses);

            // Panel dưới: Danh sách sinh viên đăng ký môn đã chọn
            Panel bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Fill;
            bottomPanel.BackColor = Color.White;
            splitContainer.Panel2.Controls.Add(bottomPanel);

            // Label tiêu đề
            Label lblStudents = new Label();
            lblStudents.Text = "SINH VIÊN ĐÃ ĐĂNG KÝ MÔN NÀY";
            lblStudents.Dock = DockStyle.Top;
            lblStudents.Height = 40;
            lblStudents.Font = new Font("Microsoft Sans Serif", 11, FontStyle.Bold);
            lblStudents.ForeColor = Color.FromArgb(0, 123, 255);
            lblStudents.TextAlign = ContentAlignment.MiddleCenter;
            lblStudents.BackColor = Color.FromArgb(248, 249, 250);
            lblStudents.BorderStyle = BorderStyle.FixedSingle;
            bottomPanel.Controls.Add(lblStudents);

            // DataGridView sinh viên đăng ký
            dgvCourseStudents = new DataGridView();
            dgvCourseStudents.Dock = DockStyle.Fill;
            dgvCourseStudents.AutoGenerateColumns = false;
            dgvCourseStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCourseStudents.AllowUserToAddRows = false;
            dgvCourseStudents.BackgroundColor = Color.White;
            dgvCourseStudents.GridColor = Color.LightGray;
            dgvCourseStudents.ColumnHeadersHeight = 35;
            dgvCourseStudents.Font = new Font("Microsoft Sans Serif", 9);
            dgvCourseStudents.RowTemplate.Height = 30;

            dgvCourseStudents.Columns.Add("StudentId", "MÃ SV");
            dgvCourseStudents.Columns[0].Width = 100;
            dgvCourseStudents.Columns.Add("FullName", "HỌ TÊN");
            dgvCourseStudents.Columns[1].Width = 250;
            dgvCourseStudents.Columns.Add("Username", "TÀI KHOẢN");
            dgvCourseStudents.Columns[2].Width = 150;

            // Nút xóa sinh viên khỏi môn
            DataGridViewButtonColumn btnRemoveStudent = new DataGridViewButtonColumn();
            btnRemoveStudent.HeaderText = "THAO TÁC";
            btnRemoveStudent.Text = "XÓA KHỎI MÔN";
            btnRemoveStudent.UseColumnTextForButtonValue = true;
            btnRemoveStudent.Width = 150;
            dgvCourseStudents.Columns.Add(btnRemoveStudent);
            dgvCourseStudents.CellClick += DgvCourseStudents_CellClick;

            bottomPanel.Controls.Add(dgvCourseStudents);
        }

        private void CreateUsersTab(TabPage tab)
        {
            // Panel chính
            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            tab.Controls.Add(mainPanel);

            // Panel nút
            Panel buttonPanel = new Panel();
            buttonPanel.Height = 60;
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.BackColor = Color.FromArgb(248, 249, 250);
            buttonPanel.BorderStyle = BorderStyle.FixedSingle;
            mainPanel.Controls.Add(buttonPanel);

            // Nút Thêm sinh viên
            Button btnAddUser = new Button()
            {
                Text = "➕ THÊM SINH VIÊN",
                Location = new Point(15, 15),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnAddUser.Click += BtnAddUserPopup_Click;
            buttonPanel.Controls.Add(btnAddUser);

            // Nút Xóa sinh viên
            Button btnDeleteUser = new Button()
            {
                Text = "🗑️ XÓA SINH VIÊN",
                Location = new Point(210, 15),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnDeleteUser.Click += BtnDeleteUser_Click;
            buttonPanel.Controls.Add(btnDeleteUser);

            // Nút Làm mới
            Button btnRefreshUsers = new Button()
            {
                Text = "🔄 LÀM MỚI",
                Location = new Point(405, 15),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnRefreshUsers.Click += (s, e) => LoadUsersData();
            buttonPanel.Controls.Add(btnRefreshUsers);

            // DataGridView sinh viên
            dgvUsers = new DataGridView();
            dgvUsers.Dock = DockStyle.Fill;
            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.BackgroundColor = Color.White;
            dgvUsers.GridColor = Color.LightGray;
            dgvUsers.ColumnHeadersHeight = 40;
            dgvUsers.Font = new Font("Microsoft Sans Serif", 10);
            dgvUsers.RowTemplate.Height = 35;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsers.EnableHeadersVisualStyles = false;

            dgvUsers.Columns.Add("UserId", "ID");
            dgvUsers.Columns[0].Width = 80;
            dgvUsers.Columns.Add("Username", "TÊN ĐĂNG NHẬP");
            dgvUsers.Columns[1].Width = 180;
            dgvUsers.Columns.Add("FullName", "HỌ TÊN");
            dgvUsers.Columns[2].Width = 250;

            // Nút chỉnh sửa
            DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn();
            btnEdit.HeaderText = "CHỈNH SỬA";
            btnEdit.Text = "SỬA";
            btnEdit.UseColumnTextForButtonValue = true;
            btnEdit.Width = 100;
            dgvUsers.Columns.Add(btnEdit);

            // Nút đổi mật khẩu
            DataGridViewButtonColumn btnChangePass = new DataGridViewButtonColumn();
            btnChangePass.HeaderText = "MẬT KHẨU";
            btnChangePass.Text = "ĐỔI MK";
            btnChangePass.UseColumnTextForButtonValue = true;
            btnChangePass.Width = 100;
            dgvUsers.Columns.Add(btnChangePass);

            dgvUsers.CellClick += DgvUsers_CellClick;
            dgvUsers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            mainPanel.Controls.Add(dgvUsers);
        }

        private void CreateRegistrationsTab(TabPage tab)
        {
            // Panel chính
            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            tab.Controls.Add(mainPanel);

            // Panel nút
            Panel buttonPanel = new Panel();
            buttonPanel.Height = 60;
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.BackColor = Color.FromArgb(248, 249, 250);
            buttonPanel.BorderStyle = BorderStyle.FixedSingle;
            mainPanel.Controls.Add(buttonPanel);

            Button btnRefresh = new Button()
            {
                Text = "🔄 LÀM MỚI",
                Location = new Point(15, 15),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnRefresh.Click += (s, e) => LoadRegistrationsData();
            buttonPanel.Controls.Add(btnRefresh);

            // DataGridView
            dgvRegistrations = new DataGridView();
            dgvRegistrations.Dock = DockStyle.Fill;
            dgvRegistrations.AutoGenerateColumns = false;
            dgvRegistrations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRegistrations.AllowUserToAddRows = false;
            dgvRegistrations.BackgroundColor = Color.White;
            dgvRegistrations.GridColor = Color.LightGray;
            dgvRegistrations.ColumnHeadersHeight = 40;
            dgvRegistrations.Font = new Font("Microsoft Sans Serif", 10);
            dgvRegistrations.RowTemplate.Height = 35;
            dgvRegistrations.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
            dgvRegistrations.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgvRegistrations.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRegistrations.EnableHeadersVisualStyles = false;

            dgvRegistrations.Columns.Add("RegistrationId", "ID");
            dgvRegistrations.Columns[0].Width = 80;
            dgvRegistrations.Columns.Add("UserId", "ID SINH VIÊN");
            dgvRegistrations.Columns[1].Width = 120;
            dgvRegistrations.Columns.Add("CourseId", "MÃ MÔN");
            dgvRegistrations.Columns[2].Width = 100;
            dgvRegistrations.Columns.Add("CourseName", "TÊN MÔN");
            dgvRegistrations.Columns[3].Width = 250;
            dgvRegistrations.Columns.Add("Credits", "TÍN CHỈ");
            dgvRegistrations.Columns[4].Width = 100;
            dgvRegistrations.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvRegistrations.Columns.Add("Semester", "HỌC KỲ");
            dgvRegistrations.Columns[5].Width = 120;
            dgvRegistrations.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvRegistrations.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            mainPanel.Controls.Add(dgvRegistrations);
        }

        // ========== CÁC SỰ KIỆN MỚI ==========

        private void BtnAddCoursePopup_Click(object sender, EventArgs e)
        {
            // Tạo form popup thêm môn học
            using (var addCourseForm = new Form())
            {
                addCourseForm.Text = "Thêm môn học mới";
                addCourseForm.Size = new Size(500, 350);
                addCourseForm.StartPosition = FormStartPosition.CenterParent;
                addCourseForm.Font = new Font("Microsoft Sans Serif", 10);

                // Controls
                var lblId = new Label { Text = "Mã môn:", Location = new Point(30, 30), Size = new Size(100, 25) };
                var txtId = new TextBox { Location = new Point(150, 30), Size = new Size(300, 25) };

                var lblName = new Label { Text = "Tên môn:", Location = new Point(30, 70), Size = new Size(100, 25) };
                var txtName = new TextBox { Location = new Point(150, 70), Size = new Size(300, 25) };

                var lblCredits = new Label { Text = "Số tín chỉ:", Location = new Point(30, 110), Size = new Size(100, 25) };
                var txtCredits = new TextBox { Location = new Point(150, 110), Size = new Size(100, 25) };

                var lblSlots = new Label { Text = "Số chỗ trống:", Location = new Point(30, 150), Size = new Size(100, 25) };
                var txtSlots = new TextBox { Location = new Point(150, 150), Size = new Size(100, 25) };

                var btnSave = new Button
                {
                    Text = "LƯU MÔN HỌC",
                    Location = new Point(150, 200),
                    Size = new Size(150, 40),
                    BackColor = Color.FromArgb(40, 167, 69),
                    ForeColor = Color.White,
                    Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
                };

                var btnCancel = new Button
                {
                    Text = "HỦY",
                    Location = new Point(320, 200),
                    Size = new Size(130, 40),
                    BackColor = Color.FromArgb(108, 117, 125),
                    ForeColor = Color.White
                };

                btnSave.Click += (s, ev) =>
                {
                    if (string.IsNullOrEmpty(txtId.Text) || string.IsNullOrEmpty(txtName.Text) ||
                        string.IsNullOrEmpty(txtCredits.Text) || string.IsNullOrEmpty(txtSlots.Text))
                    {
                        MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string response = clientSocket.SendRequest($"ADD_COURSE|{txtId.Text.Trim()}|{txtName.Text.Trim()}|{txtCredits.Text.Trim()}|{txtSlots.Text.Trim()}");

                    if (response.StartsWith("SUCCESS"))
                    {
                        MessageBox.Show("Thêm môn học thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCoursesData();
                        addCourseForm.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("Lỗi: " + response.Substring(6), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                btnCancel.Click += (s, ev) => addCourseForm.Close();

                addCourseForm.Controls.AddRange(new Control[] { lblId, txtId, lblName, txtName, lblCredits, txtCredits, lblSlots, txtSlots, btnSave, btnCancel });
                addCourseForm.ShowDialog();
            }
        }

        private void BtnAddUserPopup_Click(object sender, EventArgs e)
        {
            // Tạo form popup thêm sinh viên (tương tự như thêm môn học)
            using (var addUserForm = new Form())
            {
                addUserForm.Text = "Thêm sinh viên mới";
                addUserForm.Size = new Size(500, 300);
                addUserForm.StartPosition = FormStartPosition.CenterParent;
                addUserForm.Font = new Font("Microsoft Sans Serif", 10);

                var lblUser = new Label { Text = "Tài khoản:", Location = new Point(30, 30), Size = new Size(100, 25) };
                var txtUser = new TextBox { Location = new Point(150, 30), Size = new Size(300, 25) };

                var lblPass = new Label { Text = "Mật khẩu:", Location = new Point(30, 70), Size = new Size(100, 25) };
                var txtPass = new TextBox { Location = new Point(150, 70), Size = new Size(300, 25), UseSystemPasswordChar = true };

                var lblName = new Label { Text = "Họ tên:", Location = new Point(30, 110), Size = new Size(100, 25) };
                var txtName = new TextBox { Location = new Point(150, 110), Size = new Size(300, 25) };

                var btnSave = new Button
                {
                    Text = "LƯU SINH VIÊN",
                    Location = new Point(150, 160),
                    Size = new Size(150, 40),
                    BackColor = Color.FromArgb(40, 167, 69),
                    ForeColor = Color.White,
                    Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
                };

                var btnCancel = new Button
                {
                    Text = "HỦY",
                    Location = new Point(320, 160),
                    Size = new Size(130, 40),
                    BackColor = Color.FromArgb(108, 117, 125),
                    ForeColor = Color.White
                };

                btnSave.Click += (s, ev) =>
                {
                    if (string.IsNullOrEmpty(txtUser.Text) || string.IsNullOrEmpty(txtPass.Text) || string.IsNullOrEmpty(txtName.Text))
                    {
                        MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string response = clientSocket.SendRequest($"REGISTER_USER|{txtUser.Text.Trim()}|{txtPass.Text.Trim()}|{txtName.Text.Trim()}");

                    if (response.StartsWith("SUCCESS"))
                    {
                        MessageBox.Show("Thêm sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUsersData();
                        addUserForm.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("Lỗi: " + response.Substring(6), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                btnCancel.Click += (s, ev) => addUserForm.Close();
                addUserForm.Controls.AddRange(new Control[] { lblUser, txtUser, lblPass, txtPass, lblName, txtName, btnSave, btnCancel });
                addUserForm.ShowDialog();
            }
        }

        private void DgvCourses_SelectionChanged(object sender, EventArgs e)
        {
            // Load danh sách sinh viên đăng ký môn đã chọn
            if (dgvCourses.SelectedRows.Count > 0)
            {
                string courseId = dgvCourses.SelectedRows[0].Cells[0].Value.ToString();
                LoadCourseStudents(courseId);
            }
        }

        private void DgvCourseStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Xử lý nút xóa sinh viên khỏi môn
            if (e.ColumnIndex == 3 && e.RowIndex >= 0) // Cột "THAO TÁC"
            {
                if (dgvCourses.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn môn học trước!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string studentId = dgvCourseStudents.Rows[e.RowIndex].Cells[0].Value.ToString();
                string courseId = dgvCourses.SelectedRows[0].Cells[0].Value.ToString();
                string studentName = dgvCourseStudents.Rows[e.RowIndex].Cells[1].Value.ToString();

                if (MessageBox.Show($"Xóa sinh viên {studentName} khỏi môn học?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Gửi request xóa đăng ký
                    string response = clientSocket.SendRequest($"DELETE_REGISTRATION|{studentId}|{courseId}");

                    if (response.StartsWith("SUCCESS"))
                    {
                        MessageBox.Show("Đã xóa sinh viên khỏi môn học!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCourseStudents(courseId);
                        LoadCoursesData(); // Cập nhật số chỗ trống
                    }
                    else
                    {
                        MessageBox.Show("Lỗi: " + response.Substring(6), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void DgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string userId = dgvUsers.Rows[e.RowIndex].Cells[0].Value.ToString();
            string username = dgvUsers.Rows[e.RowIndex].Cells[1].Value.ToString();
            string fullName = dgvUsers.Rows[e.RowIndex].Cells[2].Value.ToString();

            // Nút Sửa thông tin
            if (e.ColumnIndex == 3) // Cột "CHỈNH SỬA"
            {
                using (var editForm = new Form())
                {
                    editForm.Text = "Chỉnh sửa thông tin sinh viên";
                    editForm.Size = new Size(500, 250);
                    editForm.StartPosition = FormStartPosition.CenterParent;
                    editForm.Font = new Font("Microsoft Sans Serif", 10);

                    var lblName = new Label { Text = "Họ tên:", Location = new Point(30, 30), Size = new Size(100, 25) };
                    var txtName = new TextBox { Location = new Point(150, 30), Size = new Size(300, 25), Text = fullName };

                    var btnSave = new Button
                    {
                        Text = "LƯU THAY ĐỔI",
                        Location = new Point(150, 80),
                        Size = new Size(150, 40),
                        BackColor = Color.FromArgb(40, 167, 69),
                        ForeColor = Color.White,
                        Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
                    };

                    var btnCancel = new Button
                    {
                        Text = "HỦY",
                        Location = new Point(320, 80),
                        Size = new Size(130, 40),
                        BackColor = Color.FromArgb(108, 117, 125),
                        ForeColor = Color.White
                    };

                    btnSave.Click += (s, ev) =>
                    {
                        if (string.IsNullOrEmpty(txtName.Text))
                        {
                            MessageBox.Show("Vui lòng nhập họ tên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string response = clientSocket.SendRequest($"UPDATE_USER|{userId}|{txtName.Text.Trim()}|");

                        if (response.StartsWith("SUCCESS"))
                        {
                            MessageBox.Show("Cập nhật thông tin thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadUsersData();
                            editForm.DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            MessageBox.Show("Lỗi: " + response.Substring(6), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };

                    btnCancel.Click += (s, ev) => editForm.Close();
                    editForm.Controls.AddRange(new Control[] { lblName, txtName, btnSave, btnCancel });
                    editForm.ShowDialog();
                }
            }
            // Nút Đổi mật khẩu
            else if (e.ColumnIndex == 4) // Cột "MẬT KHẨU"
            {
                using (var passForm = new Form())
                {
                    passForm.Text = "Đổi mật khẩu sinh viên";
                    passForm.Size = new Size(500, 250);
                    passForm.StartPosition = FormStartPosition.CenterParent;
                    passForm.Font = new Font("Microsoft Sans Serif", 10);

                    var lblPass = new Label { Text = "Mật khẩu mới:", Location = new Point(30, 30), Size = new Size(120, 25) };
                    var txtPass = new TextBox { Location = new Point(150, 30), Size = new Size(300, 25), UseSystemPasswordChar = true };

                    var lblConfirm = new Label { Text = "Xác nhận:", Location = new Point(30, 70), Size = new Size(120, 25) };
                    var txtConfirm = new TextBox { Location = new Point(150, 70), Size = new Size(300, 25), UseSystemPasswordChar = true };

                    var btnSave = new Button
                    {
                        Text = "ĐỔI MẬT KHẨU",
                        Location = new Point(150, 120),
                        Size = new Size(150, 40),
                        BackColor = Color.FromArgb(40, 167, 69),
                        ForeColor = Color.White,
                        Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
                    };

                    var btnCancel = new Button
                    {
                        Text = "HỦY",
                        Location = new Point(320, 120),
                        Size = new Size(130, 40),
                        BackColor = Color.FromArgb(108, 117, 125),
                        ForeColor = Color.White
                    };

                    btnSave.Click += (s, ev) =>
                    {
                        if (string.IsNullOrEmpty(txtPass.Text))
                        {
                            MessageBox.Show("Vui lòng nhập mật khẩu mới!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (txtPass.Text != txtConfirm.Text)
                        {
                            MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string response = clientSocket.SendRequest($"UPDATE_USER|{userId}||{txtPass.Text.Trim()}");

                        if (response.StartsWith("SUCCESS"))
                        {
                            MessageBox.Show("Đổi mật khẩu thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            passForm.DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            MessageBox.Show("Lỗi: " + response.Substring(6), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };

                    btnCancel.Click += (s, ev) => passForm.Close();
                    passForm.Controls.AddRange(new Control[] { lblPass, txtPass, lblConfirm, txtConfirm, btnSave, btnCancel });
                    passForm.ShowDialog();
                }
            }
        }

        // ========== CÁC METHOD LOAD DATA ==========

        private void LoadAllData()
        {
            LoadCoursesData();
            LoadUsersData();
            LoadRegistrationsData();
        }

        private void LoadCoursesData()
        {
            if (dgvCourses == null) return;

            string response = clientSocket.SendRequest("VIEW_COURSES");
            dgvCourses.Rows.Clear();

            if (response.StartsWith("SUCCESS"))
            {
                try
                {
                    string json = response.Substring(8);
                    dynamic courses = JsonConvert.DeserializeObject(json);
                    foreach (var course in courses)
                    {
                        dgvCourses.Rows.Add(
                            course["CourseId"],
                            course["CourseName"],
                            course["Credits"],
                            course["AvailableSlots"]
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] LoadCoursesData: {ex.Message}");
                }
            }
        }

        private void LoadCourseStudents(string courseId)
        {
            if (dgvCourseStudents == null) return;

            dgvCourseStudents.Rows.Clear();

            // Lấy tất cả đăng ký
            string response = clientSocket.ViewAllRegistrations();

            if (response.StartsWith("SUCCESS"))
            {
                try
                {
                    string json = response.Substring(8);
                    dynamic registrations = JsonConvert.DeserializeObject(json);

                    // Lọc sinh viên đăng ký môn này
                    foreach (var reg in registrations)
                    {
                        if (reg["CourseId"].ToString() == courseId)
                        {
                            // Lấy thông tin sinh viên từ danh sách users
                            string usersResponse = clientSocket.SendRequest("GET_ALL_USERS");
                            if (usersResponse.StartsWith("SUCCESS"))
                            {
                                string usersJson = usersResponse.Substring(8);
                                dynamic users = JsonConvert.DeserializeObject(usersJson);

                                foreach (var user in users)
                                {
                                    if (user["UserId"].ToString() == reg["UserId"].ToString())
                                    {
                                        dgvCourseStudents.Rows.Add(
                                            user["UserId"],
                                            user["FullName"],
                                            user["Username"]
                                        );
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] LoadCourseStudents: {ex.Message}");
                }
            }
        }

        private void LoadUsersData()
        {
            if (dgvUsers == null) return;

            string response = clientSocket.SendRequest("GET_ALL_USERS");
            dgvUsers.Rows.Clear();

            if (response.StartsWith("SUCCESS"))
            {
                try
                {
                    string json = response.Substring(8);
                    if (json.Trim() != "[]")
                    {
                        dynamic users = JsonConvert.DeserializeObject(json);
                        foreach (var user in users)
                        {
                            // Chỉ hiển thị sinh viên (role = "Student")
                            if (user["Role"].ToString() == "Student")
                            {
                                dgvUsers.Rows.Add(
                                    user["UserId"],
                                    user["Username"],
                                    user["FullName"]
                                );
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] LoadUsersData: {ex.Message}");
                }
            }
        }

        private void LoadRegistrationsData()
        {
            if (dgvRegistrations == null) return;

            string response = clientSocket.ViewAllRegistrations();
            dgvRegistrations.Rows.Clear();

            if (response.StartsWith("SUCCESS"))
            {
                try
                {
                    string json = response.Substring(8);
                    dynamic registrations = JsonConvert.DeserializeObject(json);
                    foreach (var reg in registrations)
                    {
                        dgvRegistrations.Rows.Add(
                            reg["RegistrationId"],
                            reg["UserId"],
                            reg["CourseId"],
                            reg["CourseName"],
                            reg["Credits"],
                            reg["Semester"]
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] LoadRegistrationsData: {ex.Message}");
                }
            }
        }

        // ========== CÁC METHOD XÓA ĐÃ SỬA ==========

        private void BtnDeleteCourse_Click(object sender, EventArgs e)  // ĐÃ SỬA: Thêm (object sender, EventArgs e)
        {
            if (dgvCourses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn môn cần xóa", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string courseId = dgvCourses.SelectedRows[0].Cells[0].Value.ToString();
            string courseName = dgvCourses.SelectedRows[0].Cells[1].Value.ToString();

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa môn:\n{courseId} - {courseName}?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string response = clientSocket.SendRequest($"DELETE_COURSE|{courseId}");
                if (response.StartsWith("SUCCESS"))
                {
                    MessageBox.Show("Đã xóa môn học thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCoursesData();
                    dgvCourseStudents.Rows.Clear(); // Xóa danh sách SV
                }
                else
                {
                    MessageBox.Show("Lỗi: " + response.Substring(6), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDeleteUser_Click(object sender, EventArgs e)  // ĐÃ SỬA: Thêm (object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string userId = dgvUsers.SelectedRows[0].Cells[0].Value.ToString();
            string userName = dgvUsers.SelectedRows[0].Cells[2].Value.ToString();

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên:\n{userName}?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string response = clientSocket.SendRequest($"DELETE_USER|{userId}");
                if (response.StartsWith("SUCCESS"))
                {
                    MessageBox.Show("Đã xóa sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsersData();
                }
                else
                {
                    MessageBox.Show("Lỗi: " + response.Substring(6), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}