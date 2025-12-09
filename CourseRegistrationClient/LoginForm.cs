using System;
using System.Windows.Forms;

namespace CourseRegistrationClient
{
    public partial class LoginForm : Form
    {
        public string UserId { get; set; }
        public string UserRole { get; set; }
        public string FullName { get; set; }
        private ClientSocket clientSocket;

        public LoginForm(ClientSocket socket)
        {
            InitializeComponent();
            clientSocket = socket;
            this.Text = "??ng nh?p - H? th?ng ??ng ký Môn h?c";
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Labels
            Label lblUsername = new Label();
            lblUsername.Text = "Tên ??ng nh?p:";
            lblUsername.Location = new System.Drawing.Point(20, 30);
            lblUsername.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(lblUsername);

            Label lblPassword = new Label();
            lblPassword.Text = "M?t kh?u:";
            lblPassword.Location = new System.Drawing.Point(20, 70);
            lblPassword.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(lblPassword);

            // TextBox
            TextBox txtUsername = new TextBox();
            txtUsername.Name = "txtUsername";
            txtUsername.Location = new System.Drawing.Point(130, 30);
            txtUsername.Size = new System.Drawing.Size(200, 20);
            this.Controls.Add(txtUsername);

            TextBox txtPassword = new TextBox();
            txtPassword.Name = "txtPassword";
            txtPassword.Location = new System.Drawing.Point(130, 70);
            txtPassword.Size = new System.Drawing.Size(200, 20);
            txtPassword.UseSystemPasswordChar = true;
            this.Controls.Add(txtPassword);

            // Buttons
            Button btnLogin = new Button();
            btnLogin.Text = "??ng nh?p";
            btnLogin.Location = new System.Drawing.Point(130, 110);
            btnLogin.Size = new System.Drawing.Size(95, 30);
            btnLogin.Click += (s, e) => BtnLogin_Click(s, e, txtUsername, txtPassword);
            this.Controls.Add(btnLogin);

            Button btnRegister = new Button();
            btnRegister.Text = "??ng ký";
            btnRegister.Location = new System.Drawing.Point(235, 110);
            btnRegister.Size = new System.Drawing.Size(95, 30);
            btnRegister.Click += BtnRegister_Click;
            this.Controls.Add(btnRegister);

            // Label info
            Label lblInfo = new Label();
            lblInfo.Name = "lblInfo";
            lblInfo.Location = new System.Drawing.Point(20, 160);
            lblInfo.Size = new System.Drawing.Size(350, 60);
            lblInfo.AutoSize = true;
            lblInfo.Text = "Tài kho?n demo:\nAdmin: admin / admin123\nSinh viên: student / student123";
            this.Controls.Add(lblInfo);

            this.ClientSize = new System.Drawing.Size(380, 230);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        private void BtnLogin_Click(object sender, EventArgs e, TextBox txtUsername, TextBox txtPassword)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nh?p tên ??ng nh?p và m?t kh?u", "L?i");
                return;
            }

            string response = clientSocket.SendRequest($"LOGIN|{username}|{password}");
            
            if (response.StartsWith("SUCCESS"))
            {
                string[] parts = response.Split('|');
                if (parts.Length >= 4)
                {
                    UserId = parts[1];
                    UserRole = parts[2];
                    FullName = parts[3];

                    Console.WriteLine($"[CLIENT] Login success: {FullName} ({UserRole})");
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

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm regForm = new RegisterForm(clientSocket);
            if (regForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("??ng ký thành công! Vui lòng ??ng nh?p.", "Thành công");
            }
        }
    }
}
