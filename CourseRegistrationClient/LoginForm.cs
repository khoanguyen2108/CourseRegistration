using System;
using System.Windows.Forms;

namespace CourseRegistrationClient
{
    public partial class LoginForm : Form
    {
        private ClientSocket clientSocket;
        public string UserId { get; private set; }
        public string UserRole { get; private set; }
        public string FullName { get; private set; }

        public LoginForm(ClientSocket socket)
        {
            InitializeComponent();
            this.clientSocket = socket;
            this.UserId = "";
            this.UserRole = "";
            this.FullName = "";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi");
                return;
            }

            // Gửi request đến server
            string response = clientSocket.SendRequest($"LOGIN|{username}|{password}");

            if (response.StartsWith("SUCCESS"))
            {
                // Parse response: SUCCESS|userId|role|fullName
                string[] parts = response.Split('|');
                if (parts.Length >= 4)
                {
                    UserId = parts[1];
                    UserRole = parts[2];
                    FullName = parts[3];

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Định dạng phản hồi không đúng!", "Lỗi");
                }
            }
            else
            {
                string errorMsg = response.Contains("|") ? response.Substring(response.IndexOf('|') + 1) : response;
                MessageBox.Show($"Đăng nhập thất bại: {errorMsg}", "Lỗi");
            }
        }

        // ĐỔI TÊN METHOD VÀ THÊM CHỨC NĂNG ĐĂNG KÝ
        private void btnExit_Click(object sender, EventArgs e)
        {
            // Hiện form đăng ký đơn giản bằng InputBox
            using (var registerForm = new Form())
            {
                registerForm.Text = "Đăng ký tài khoản";
                registerForm.Size = new System.Drawing.Size(350, 220);
                registerForm.StartPosition = FormStartPosition.CenterScreen;

                var lblUser = new Label { Text = "Username:", Location = new System.Drawing.Point(20, 30), Size = new System.Drawing.Size(80, 20) };
                var txtUser = new TextBox { Location = new System.Drawing.Point(110, 30), Size = new System.Drawing.Size(180, 20) };

                var lblPass = new Label { Text = "Password:", Location = new System.Drawing.Point(20, 60), Size = new System.Drawing.Size(80, 20) };
                var txtPass = new TextBox { Location = new System.Drawing.Point(110, 60), Size = new System.Drawing.Size(180, 20), PasswordChar = '*' };

                var lblName = new Label { Text = "Họ tên:", Location = new System.Drawing.Point(20, 90), Size = new System.Drawing.Size(80, 20) };
                var txtName = new TextBox { Location = new System.Drawing.Point(110, 90), Size = new System.Drawing.Size(180, 20) };

                var btnOK = new Button { Text = "Đăng ký", Location = new System.Drawing.Point(110, 130), Size = new System.Drawing.Size(80, 30) };
                var btnCancel = new Button { Text = "Hủy", Location = new System.Drawing.Point(210, 130), Size = new System.Drawing.Size(80, 30) };

                btnOK.Click += (s, ev) => {
                    string username = txtUser.Text.Trim();
                    string password = txtPass.Text.Trim();
                    string fullName = txtName.Text.Trim();

                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName))
                    {
                        MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Gửi request đăng ký đến server
                    string response = clientSocket.SendRequest($"REGISTER_USER|{username}|{password}|{fullName}");

                    if (response.StartsWith("SUCCESS"))
                    {
                        MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtUsername.Text = username; // Tự điền username vào form login
                        txtPassword.Text = "";
                        txtPassword.Focus();
                        registerForm.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        string errorMsg = response.Contains("|") ? response.Substring(response.IndexOf('|') + 1) : response;
                        MessageBox.Show($"Đăng ký thất bại: {errorMsg}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                btnCancel.Click += (s, ev) => registerForm.Close();

                registerForm.Controls.AddRange(new Control[] { lblUser, txtUser, lblPass, txtPass, lblName, txtName, btnOK, btnCancel });

                registerForm.ShowDialog();
            }
        }
    }
}