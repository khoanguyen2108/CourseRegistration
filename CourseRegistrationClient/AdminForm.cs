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
        private Panel mainPanel;
        private SplitContainer splitContainer;
        private Panel coursesPanel;
        private Panel studentsPanel;
        private DataGridView dgvCourses;
        private DataGridView dgvCourseStudents;
        private DataGridView dgvAllStudents;
        private Button btnManageStudents;
        private Button btnBackToCourses;
        private bool showingStudents = false;

        public AdminForm(ClientSocket socket, string id, string name)
        {
            clientSocket = socket;
            userId = id;
            fullName = name;

            this.Text = "Admin - Hệ thống đăng ký Môn học";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

            InitializeComponent();
            this.Load += AdminForm_Load;
            this.Shown += (s, e) => LoadAllData();
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            LoadAllData();
            Console.WriteLine("[DEBUG] AdminForm loaded, calling LoadAllData()");
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
            lblTitle.Text = "QUẢN TRỊ VIÊN";
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

            // Panel chính
            mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = Color.White;
            this.Controls.Add(mainPanel);

            // Tạo giao diện quản lý môn học (mặc định)
            CreateCoursesInterface();

            // Tạo giao diện quản lý sinh viên (ẩn)
            CreateStudentsInterface();

            this.ResumeLayout(false);
        }

        private void CreateCoursesInterface()
        {
            coursesPanel = new Panel();
            coursesPanel.Dock = DockStyle.Fill;
            coursesPanel.BackColor = Color.White;
            mainPanel.Controls.Add(coursesPanel);

            // Split container
            splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Orientation = Orientation.Horizontal;
            splitContainer.SplitterDistance = 400;
            coursesPanel.Controls.Add(splitContainer);

            // ========== PHẦN TRÊN: DANH SÁCH MÔN HỌC ==========
            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Fill;
            topPanel.BackColor = Color.White;
            splitContainer.Panel1.Controls.Add(topPanel);

            // Panel nút bấm
            Panel buttonPanel = new Panel();
            buttonPanel.Height = 80;
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.BackColor = Color.FromArgb(248, 249, 250);
            buttonPanel.BorderStyle = BorderStyle.FixedSingle;
            topPanel.Controls.Add(buttonPanel);

            // Nút Thêm môn
            Button btnAddCourse = new Button()
            {
                Text = "THÊM MÔN",
                Location = new Point(15, 43),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnAddCourse.Click += BtnAddCoursePopup_Click;
            buttonPanel.Controls.Add(btnAddCourse);

            // Nút Xóa môn
            Button btnDeleteCourse = new Button()
            {
                Text = "XÓA MÔN",
                Location = new Point(150, 43),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnDeleteCourse.Click += BtnDeleteCourse_Click;
            buttonPanel.Controls.Add(btnDeleteCourse);

            // Nút Quản lý sinh viên
            btnManageStudents = new Button()
            {
                Text = "QUẢN LÝ",
                Location = new Point(285, 43),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnManageStudents.Click += BtnManageStudents_Click;
            buttonPanel.Controls.Add(btnManageStudents);

            // Nút Làm mới
            Button btnRefreshCourses = new Button()
            {
                Text = "LÀM MỚI",
                Location = new Point(420, 43),
                Size = new Size(130, 30),
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
            dgvCourses.ColumnHeadersHeight = 100;
            dgvCourses.Font = new Font("Microsoft Sans Serif", 10);
            dgvCourses.RowTemplate.Height = 30;
            dgvCourses.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
            dgvCourses.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgvCourses.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCourses.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            dgvCourses.EnableHeadersVisualStyles = false;
            dgvCourses.SelectionChanged += DgvCourses_SelectionChanged;

            dgvCourses.Columns.Add("CourseId", "MÃ MÔN");
            dgvCourses.Columns[0].Width = 231;
            dgvCourses.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            dgvCourses.Columns.Add("CourseName", "TÊN MÔN HỌC");
            dgvCourses.Columns[1].Width = 448;
            dgvCourses.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            dgvCourses.Columns.Add("Credits", "TÍN CHỈ");
            dgvCourses.Columns[2].Width = 231;
            dgvCourses.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            dgvCourses.Columns.Add("AvailableSlots", "CHỖ TRỐNG");
            dgvCourses.Columns[3].Width = 231;
            dgvCourses.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            dgvCourses.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            topPanel.Controls.Add(dgvCourses);

            // ========== PHẦN DƯỚI: SINH VIÊN ĐĂNG KÝ MÔN ==========
            Panel bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Fill;
            bottomPanel.BackColor = Color.White;
            splitContainer.Panel2.Controls.Add(bottomPanel);

            // Tiêu đề
            Label lblStudents = new Label();
            lblStudents.Text = "SINH VIÊN ĐÃ ĐĂNG KÝ MÔN NÀY";
            lblStudents.Dock = DockStyle.Top;
            lblStudents.Height = 40;
            lblStudents.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
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
            dgvCourseStudents.ColumnHeadersHeight = 60;
            dgvCourseStudents.Font = new Font("Microsoft Sans Serif", 10);
            dgvCourseStudents.RowTemplate.Height = 30;
            dgvCourseStudents.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
            dgvCourseStudents.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgvCourseStudents.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCourseStudents.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            dgvCourseStudents.EnableHeadersVisualStyles = false;

            dgvCourseStudents.Columns.Add("StudentId", "MÃ SV");
            dgvCourseStudents.Columns[0].Width = 250;
            dgvCourseStudents.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            dgvCourseStudents.Columns.Add("FullName", "HỌ TÊN");
            dgvCourseStudents.Columns[1].Width = 400;
            dgvCourseStudents.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            dgvCourseStudents.Columns.Add("Username", "TÀI KHOẢN");
            dgvCourseStudents.Columns[2].Width = 300;
            dgvCourseStudents.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            // Nút xóa sinh viên khỏi môn
            DataGridViewButtonColumn btnRemoveStudent = new DataGridViewButtonColumn();
            btnRemoveStudent.HeaderText = "THAO TÁC";
            btnRemoveStudent.Text = "XÓA";
            btnRemoveStudent.UseColumnTextForButtonValue = true;
            btnRemoveStudent.Width = 190;
            dgvCourseStudents.Columns.Add(btnRemoveStudent);
            dgvCourseStudents.CellClick += DgvCourseStudents_CellClick;

            bottomPanel.Controls.Add(dgvCourseStudents);
        }

        private void CreateStudentsInterface()
        {
            studentsPanel = new Panel();
            studentsPanel.Dock = DockStyle.Fill;
            studentsPanel.BackColor = Color.White;
            studentsPanel.Visible = false; // Ẩn ban đầu
            mainPanel.Controls.Add(studentsPanel);

            // Nút quay lại
            btnBackToCourses = new Button()
            {
                Text = "QUAY LẠI",
                Location = new Point(15, 44),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnBackToCourses.Click += BtnBackToCourses_Click;
            studentsPanel.Controls.Add(btnBackToCourses);

            // Panel nút bấm
            Panel buttonPanel = new Panel();
            buttonPanel.Height = 80;
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.BackColor = Color.FromArgb(248, 249, 250);
            buttonPanel.BorderStyle = BorderStyle.FixedSingle;
            studentsPanel.Controls.Add(buttonPanel);

            // Nút Thêm sinh viên
            Button btnAddStudent = new Button()
            {
                Text = "THÊM SV",
                Location = new Point(150, 43),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnAddStudent.Click += BtnAddUserPopup_Click;
            buttonPanel.Controls.Add(btnAddStudent);

            // Nút Xóa sinh viên
            Button btnDeleteStudent = new Button()
            {
                Text = "XÓA SV",
                Location = new Point(285, 43),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnDeleteStudent.Click += BtnDeleteUser_Click;
            buttonPanel.Controls.Add(btnDeleteStudent);

            // Nút Làm mới
            Button btnRefreshStudents = new Button()
            {
                Text = "LÀM MỚI",
                Location = new Point(420, 43),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnRefreshStudents.Click += (s, e) => LoadAllStudentsData();
            buttonPanel.Controls.Add(btnRefreshStudents);

            // DataGridView tất cả sinh viên
            dgvAllStudents = new DataGridView();
            dgvAllStudents.Dock = DockStyle.Fill;
            dgvAllStudents.AutoGenerateColumns = false;
            dgvAllStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAllStudents.AllowUserToAddRows = false;
            dgvAllStudents.BackgroundColor = Color.White;
            dgvAllStudents.GridColor = Color.LightGray;
            dgvAllStudents.ColumnHeadersHeight = 100;
            dgvAllStudents.Font = new Font("Microsoft Sans Serif", 10);
            dgvAllStudents.RowTemplate.Height = 30;
            dgvAllStudents.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
            dgvAllStudents.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgvAllStudents.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAllStudents.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            dgvAllStudents.EnableHeadersVisualStyles = false;

            dgvAllStudents.Columns.Add("UserId", "ID");
            dgvAllStudents.Columns[0].Width = 150;
            dgvAllStudents.Columns.Add("Username", "TÊN ĐĂNG NHẬP");
            dgvAllStudents.Columns[1].Width = 250;
            dgvAllStudents.Columns.Add("FullName", "HỌ TÊN");
            dgvAllStudents.Columns[2].Width = 410;

            // Nút chỉnh sửa
            DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn();
            btnEdit.HeaderText = "CHỈNH SỬA";
            btnEdit.Text = "SỬA";
            btnEdit.UseColumnTextForButtonValue = true;
            btnEdit.Width = 165;
            dgvAllStudents.Columns.Add(btnEdit);

            // Nút đổi mật khẩu
            DataGridViewButtonColumn btnChangePass = new DataGridViewButtonColumn();
            btnChangePass.HeaderText = "MẬT KHẨU";
            btnChangePass.Text = "ĐỔI MK";
            btnChangePass.UseColumnTextForButtonValue = true;
            btnChangePass.Width = 165;
            dgvAllStudents.Columns.Add(btnChangePass);

            dgvAllStudents.CellClick += DgvAllStudents_CellClick;
            dgvAllStudents.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            studentsPanel.Controls.Add(dgvAllStudents);
        }

        private void BtnManageStudents_Click(object sender, EventArgs e)
        {
            showingStudents = true;
            coursesPanel.Visible = false;
            studentsPanel.Visible = true;
            LoadAllStudentsData();
        }

        private void BtnBackToCourses_Click(object sender, EventArgs e)
        {
            showingStudents = false;
            studentsPanel.Visible = false;
            coursesPanel.Visible = true;
        }

        private void LoadAllData()
        {
            LoadCoursesData();
        }

        private void LoadAllStudentsData()
        {
            if (dgvAllStudents == null) return;

            string response = clientSocket.SendRequest("GET_ALL_USERS");
            dgvAllStudents.Rows.Clear();

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
                            if (user["Role"].ToString() == "Student")
                            {
                                dgvAllStudents.Rows.Add(
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
                    Console.WriteLine($"[ERROR] LoadAllStudentsData: {ex.Message}");
                }
            }
        }

        private void DgvAllStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string userId = dgvAllStudents.Rows[e.RowIndex].Cells[0].Value.ToString();
            string username = dgvAllStudents.Rows[e.RowIndex].Cells[1].Value.ToString();
            string fullName = dgvAllStudents.Rows[e.RowIndex].Cells[2].Value.ToString();

            // Nút Sửa thông tin
            if (e.ColumnIndex == 3)
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
                            LoadAllStudentsData();
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
            else if (e.ColumnIndex == 4)
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

        // ========== CÁC PHƯƠNG THỨC KHÁC GIỮ NGUYÊN ==========

        private void BtnAddCoursePopup_Click(object sender, EventArgs e)
        {
            // Giữ nguyên code popup thêm môn học
            using (var addCourseForm = new Form())
            {
                addCourseForm.Text = "Thêm môn học mới";
                addCourseForm.Size = new Size(500, 350);
                addCourseForm.StartPosition = FormStartPosition.CenterParent;
                addCourseForm.Font = new Font("Microsoft Sans Serif", 10);

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
            // Giữ nguyên code popup thêm sinh viên
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
                        if (showingStudents)
                            LoadAllStudentsData();
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
            if (dgvCourses.SelectedRows.Count > 0)
            {
                string courseId = dgvCourses.SelectedRows[0].Cells[0].Value.ToString();
                LoadCourseStudents(courseId);
            }
        }

        private void DgvCourseStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3 && e.RowIndex >= 0)
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
                    string response = clientSocket.SendRequest($"DELETE_REGISTRATION|{studentId}|{courseId}");

                    if (response.StartsWith("SUCCESS"))
                    {
                        MessageBox.Show("Đã xóa sinh viên khỏi môn học!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCourseStudents(courseId);
                        LoadCoursesData();
                    }
                    else
                    {
                        MessageBox.Show("Lỗi: " + response.Substring(6), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadCoursesData()
        {
            if (dgvCourses == null) return;

            Console.WriteLine("[DEBUG] Sending VIEW_COURSES request...");
            string response = clientSocket.SendRequest("VIEW_COURSES");
            Console.WriteLine($"[DEBUG] Server response: {response}");

            dgvCourses.Rows.Clear();

            if (response.StartsWith("SUCCESS"))
            {
                try
                {
                    string json = response.Substring(8);
                    Console.WriteLine($"[DEBUG] Courses JSON: {json}");
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
                    Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");
                }
            }
            else
            {
                Console.WriteLine($"[DEBUG] Server returned error: {response}");
            }
        }

        private void LoadCourseStudents(string courseId)
        {
            if (dgvCourseStudents == null) return;

            dgvCourseStudents.Rows.Clear();

            string response = clientSocket.SendRequest("VIEW_ALL_REGISTRATIONS");

            if (response.StartsWith("SUCCESS"))
            {
                try
                {
                    string json = response.Substring(8);
                    dynamic registrations = JsonConvert.DeserializeObject(json);

                    foreach (var reg in registrations)
                    {
                        if (reg["CourseId"].ToString() == courseId)
                        {
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

        private void BtnDeleteCourse_Click(object sender, EventArgs e)
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
                    dgvCourseStudents.Rows.Clear();
                }
                else
                {
                    MessageBox.Show("Lỗi: " + response.Substring(6), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dgvAllStudents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string userId = dgvAllStudents.SelectedRows[0].Cells[0].Value.ToString();
            string userName = dgvAllStudents.SelectedRows[0].Cells[2].Value.ToString();

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên:\n{userName}?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string response = clientSocket.SendRequest($"DELETE_USER|{userId}");
                if (response.StartsWith("SUCCESS"))
                {
                    MessageBox.Show("Đã xóa sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAllStudentsData();
                }
                else
                {
                    MessageBox.Show("Lỗi: " + response.Substring(6), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}