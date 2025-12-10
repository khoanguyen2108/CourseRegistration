using CourseRegistrationClient;
using System;
using System.Windows.Forms;

namespace CourseRegistration
{
    public partial class LoginForm : Form
    {
        // ===== Constructor =====
        public LoginForm()
        {
            InitializeComponent();
        }

        // ===== Button Đăng nhập =====
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (username == "admin" && password == "123")
            {
                AdminForm adminForm = new AdminForm();
                adminForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
            }
        }

        // ===== Button Thoát =====
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
