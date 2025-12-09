using System;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CourseRegistrationClient
{
    public class StudentForm : Form
    {
        private ClientSocket clientSocket;
        private string userId;
        private string fullName;
        private DataGridView dgvCourses;
        private DataGridView dgvRegistered;

        public StudentForm(ClientSocket socket, string id, string name)
        {
            clientSocket = socket;
            userId = id;
            fullName = name;
            
            InitializeComponent();
            this.Text = $"Sinh viên - {fullName} - H? th?ng ??ng ký Môn h?c";
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Toolbar
            Panel pnlToolbar = new Panel();
            pnlToolbar.Dock = DockStyle.Top;
            pnlToolbar.Height = 40;
            pnlToolbar.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(pnlToolbar);

            Label lblWelcome = new Label();
            lblWelcome.Text = $"Xin chào: {fullName}";
            lblWelcome.Location = new System.Drawing.Point(10, 10);
            lblWelcome.Size = new System.Drawing.Size(200, 20);
            pnlToolbar.Controls.Add(lblWelcome);

            Button btnLogout = new Button();
            btnLogout.Text = "??ng xu?t";
            btnLogout.Location = new System.Drawing.Point(700, 8);
            btnLogout.Size = new System.Drawing.Size(80, 25);
            btnLogout.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            pnlToolbar.Controls.Add(btnLogout);

            // TabControl
            TabControl tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            this.Controls.Add(tabControl);

            // Tab 1: Danh sách môn h?c
            TabPage tabCourses = new TabPage("Danh sách Môn h?c");
            CreateCoursesTab(tabCourses);
            tabControl.TabPages.Add(tabCourses);

            // Tab 2: Môn ?ã ??ng ký
            TabPage tabRegistered = new TabPage("Môn ?ã ??ng ký");
            CreateRegisteredTab(tabRegistered);
            tabControl.TabPages.Add(tabRegistered);

            this.ClientSize = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

            Console.WriteLine("[CLIENT] Loading courses data...");
            LoadCoursesData();
            Console.WriteLine("[CLIENT] Loading registered data...");
            LoadRegisteredData();
            Console.WriteLine("[CLIENT] Data loaded successfully");
        }

        private void CreateCoursesTab(TabPage tab)
        {
            // Panel button
            Panel pnlButton = new Panel();
            pnlButton.Dock = DockStyle.Top;
            pnlButton.Height = 40;
            pnlButton.BorderStyle = BorderStyle.FixedSingle;
            tab.Controls.Add(pnlButton);

            Button btnRegister = new Button();
            btnRegister.Text = "??ng ký môn (F5)";
            btnRegister.Location = new System.Drawing.Point(10, 8);
            btnRegister.Size = new System.Drawing.Size(100, 25);
            btnRegister.Click += (s, e) => BtnRegister_Click();
            pnlButton.Controls.Add(btnRegister);

            Button btnRefresh = new Button();
            btnRefresh.Text = "Làm m?i (F9)";
            btnRefresh.Location = new System.Drawing.Point(120, 8);
            btnRefresh.Size = new System.Drawing.Size(100, 25);
            btnRefresh.Click += (s, e) => LoadCoursesData();
            pnlButton.Controls.Add(btnRefresh);

            // DataGridView
            dgvCourses = new DataGridView();
            dgvCourses.Dock = DockStyle.Fill;
            dgvCourses.AutoGenerateColumns = false;
            dgvCourses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCourses.AllowUserToAddRows = false;

            dgvCourses.Columns.Add("CourseId", "Mã môn");
            dgvCourses.Columns.Add("CourseName", "Tên môn");
            dgvCourses.Columns.Add("Credits", "Tín ch?");
            dgvCourses.Columns.Add("AvailableSlots", "Ch? tr?ng");

            tab.Controls.Add(dgvCourses);
        }

        private void CreateRegisteredTab(TabPage tab)
        {
            // Panel button
            Panel pnlButton = new Panel();
            pnlButton.Dock = DockStyle.Top;
            pnlButton.Height = 40;
            pnlButton.BorderStyle = BorderStyle.FixedSingle;
            tab.Controls.Add(pnlButton);

            Button btnRefresh = new Button();
            btnRefresh.Text = "Làm m?i (F9)";
            btnRefresh.Location = new System.Drawing.Point(10, 8);
            btnRefresh.Size = new System.Drawing.Size(100, 25);
            btnRefresh.Click += (s, e) => LoadRegisteredData();
            pnlButton.Controls.Add(btnRefresh);

            // DataGridView
            dgvRegistered = new DataGridView();
            dgvRegistered.Dock = DockStyle.Fill;
            dgvRegistered.AutoGenerateColumns = false;
            dgvRegistered.AllowUserToAddRows = false;

            dgvRegistered.Columns.Add("CourseId", "Mã môn");
            dgvRegistered.Columns.Add("Status", "Tr?ng thái");
            dgvRegistered.Columns.Add("RegisteredAt", "Th?i gian ??ng ký");

            tab.Controls.Add(dgvRegistered);
        }

        private void BtnRegister_Click()
        {
            if (dgvCourses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng ch?n môn c?n ??ng ký", "L?i");
                return;
            }

            string courseId = dgvCourses.SelectedRows[0].Cells[0].Value.ToString();
            string courseName = dgvCourses.SelectedRows[0].Cells[1].Value.ToString();

            if (MessageBox.Show($"??ng ký môn: {courseName}?", "Xác nh?n", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string response = clientSocket.SendRequest($"REGISTER|{userId}|{courseId}");
                if (response.StartsWith("SUCCESS"))
                {
                    MessageBox.Show("??ng ký thành công", "Thành công");
                    LoadCoursesData();
                    LoadRegisteredData();
                }
                else
                {
                    string errorMsg = response.StartsWith("ERROR") ? response.Substring(6) : response;
                    MessageBox.Show("L?i: " + errorMsg, "L?i");
                }
            }
        }

        private void LoadCoursesData()
        {
            if (dgvCourses == null)
            {
                Console.WriteLine("[ERROR] dgvCourses is null");
                return;
            }

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

        private void LoadRegisteredData()
        {
            if (dgvRegistered == null)
            {
                Console.WriteLine("[ERROR] dgvRegistered is null");
                return;
            }

            string response = clientSocket.SendRequest($"VIEW_REGISTRATIONS|{userId}");
            dgvRegistered.Rows.Clear();

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
                            dgvRegistered.Rows.Add(
                                reg["CourseId"],
                                reg["Status"],
                                reg["RegisteredAt"]
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] LoadRegisteredData: {ex.Message}");
                }
            }
        }
    }
}
