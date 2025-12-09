using System;
using System.Windows.Forms;

namespace CourseRegistrationClient
{
    public partial class LoginForm : Form
    {
        private ClientSocket clientSocket;
        private string userId = "";
        private string userRole = "";
        private string fullName = "";

        public string UserId => userId;
        public string UserRole => userRole;
        public string FullName => fullName;

        public LoginForm(ClientSocket socket)
        {
            InitializeComponent();
            clientSocket = socket;
            this.Text = "??ng nh?p - H? th?ng ??ng ký Môn h?c";
            this.Size = new System.Drawing.Size(400, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Labels
            Label lblUsername = new Label();
            lblUsername.Text = "Tên ??ng nh?p:";
            lblUsername.Location = new System.Drawing.Point(20, 30);
            lblUsername.Size = new System.Drawing.Size(100, 20);
            lblUsername.Font = new System.Drawing.Font("Segoe UI", 10);
            this.Controls.Add(lblUsername);

            Label lblPassword = new Label();
            lblPassword.Text = "M?t kh?u:";
            lblPassword.Location = new System.Drawing.Point(20, 70);
            lblPassword.Size = new System.Drawing.Size(100, 20);
            lblPassword.Font = new System.Drawing.Font("Segoe UI", 10);
            this.Controls.Add(lblPassword);

            // TextBox
            TextBox txtUsername = new TextBox();
            txtUsername.Name = "txtUsername";
            txtUsername.Location = new System.Drawing.Point(130, 30);
            txtUsername.Size = new System.Drawing.Size(230, 20);
            txtUsername.Font = new System.Drawing.Font("Segoe UI", 10);
            this.Controls.Add(txtUsername);

            TextBox txtPassword = new TextBox();
            txtPassword.Name = "txtPassword";
            txtPassword.Location = new System.Drawing.Point(130, 70);
            txtPassword.Size = new System.Drawing.Size(230, 20);
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Font = new System.Drawing.Font("Segoe UI", 10);
            this.Controls.Add(txtPassword);

            // Buttons
            Button btnLogin = new Button();
            btnLogin.Text = "??ng nh?p";
            btnLogin.Location = new System.Drawing.Point(130, 120);
            btnLogin.Size = new System.Drawing.Size(95, 30);
            btnLogin.Font = new System.Drawing.Font("Segoe UI", 10);
            btnLogin.Click += (s, e) => BtnLogin_Click(txtUsername, txtPassword);
            this.Controls.Add(btnLogin);

            Button btnRegister = new Button();
            btnRegister.Text = "??ng ký";
            btnRegister.Location = new System.Drawing.Point(235, 120);
            btnRegister.Size = new System.Drawing.Size(95, 30);
            btnRegister.Font = new System.Drawing.Font("Segoe UI", 10);
            btnRegister.Click += (s, e) => BtnRegister_Click();
            this.Controls.Add(btnRegister);

            Button btnCancel = new Button();
            btnCancel.Text = "H?y";
            btnCancel.Location = new System.Drawing.Point(340, 120);
            btnCancel.Size = new System.Drawing.Size(20, 30);
            btnCancel.Visible = false;
            this.Controls.Add(btnCancel);

            this.ClientSize = new System.Drawing.Size(380, 180);
            this.Font = new System.Drawing.Font("Segoe UI", 10);
            this.ResumeLayout(false);
        }

        private void BtnLogin_Click(TextBox txtUsername, TextBox txtPassword)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng ?i?n ??y ?? thông tin", "L?i");
                return;
            }

            string response = clientSocket.SendRequest($"LOGIN|{username}|{password}");

            if (response.StartsWith("SUCCESS"))
            {
                string[] parts = response.Split('|');
                if (parts.Length >= 4)
                {
                    userId = parts[1].Trim();
                    userRole = parts[2].Trim();
                    fullName = parts[3].Trim();

                    Console.WriteLine($"[CLIENT] Login success - UserId: {userId}, Role: {userRole}, Name: {fullName}");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            else
            {
                string errorMsg = response.StartsWith("ERROR") ? response.Substring(6) : response;
                MessageBox.Show("??ng nh?p th?t b?i: " + errorMsg, "L?i");
            }
        }

        private void BtnRegister_Click()
        {
            RegisterForm registerForm = new RegisterForm(clientSocket);
            if (registerForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("??ng ký thành công! Vui lòng ??ng nh?p", "Thành công");
            }
        }
    }
}
