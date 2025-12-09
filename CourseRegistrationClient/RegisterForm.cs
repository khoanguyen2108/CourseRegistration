using System;
using System.Windows.Forms;

namespace CourseRegistrationClient
{
    public partial class RegisterForm : Form
    {
        private ClientSocket clientSocket;

        public RegisterForm(ClientSocket socket)
        {
            InitializeComponent();
            clientSocket = socket;
            this.Text = "??ng ký tài kho?n - H? th?ng ??ng ký Môn h?c";
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

            Label lblConfirm = new Label();
            lblConfirm.Text = "Xác nh?n m?t kh?u:";
            lblConfirm.Location = new System.Drawing.Point(20, 110);
            lblConfirm.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(lblConfirm);

            Label lblFullName = new Label();
            lblFullName.Text = "H? tên:";
            lblFullName.Location = new System.Drawing.Point(20, 150);
            lblFullName.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(lblFullName);

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

            TextBox txtConfirm = new TextBox();
            txtConfirm.Name = "txtConfirm";
            txtConfirm.Location = new System.Drawing.Point(130, 110);
            txtConfirm.Size = new System.Drawing.Size(200, 20);
            txtConfirm.UseSystemPasswordChar = true;
            this.Controls.Add(txtConfirm);

            TextBox txtFullName = new TextBox();
            txtFullName.Name = "txtFullName";
            txtFullName.Location = new System.Drawing.Point(130, 150);
            txtFullName.Size = new System.Drawing.Size(200, 20);
            this.Controls.Add(txtFullName);

            // Buttons
            Button btnRegister = new Button();
            btnRegister.Text = "??ng ký";
            btnRegister.Location = new System.Drawing.Point(130, 190);
            btnRegister.Size = new System.Drawing.Size(95, 30);
            btnRegister.Click += (s, e) => BtnRegister_Click(s, e, txtUsername, txtPassword, txtConfirm, txtFullName);
            this.Controls.Add(btnRegister);

            Button btnCancel = new Button();
            btnCancel.Text = "H?y";
            btnCancel.Location = new System.Drawing.Point(235, 190);
            btnCancel.Size = new System.Drawing.Size(95, 30);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancel);

            this.ClientSize = new System.Drawing.Size(380, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        private void BtnRegister_Click(object sender, EventArgs e, TextBox txtUsername, TextBox txtPassword, TextBox txtConfirm, TextBox txtFullName)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string confirm = txtConfirm.Text;
            string fullName = txtFullName.Text.Trim();

            // Validation
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("Vui lòng ?i?n ??y ?? thông tin", "L?i");
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("M?t kh?u không trùng kh?p", "L?i");
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("M?t kh?u ph?i có ít nh?t 6 ký t?", "L?i");
                return;
            }

            string response = clientSocket.SendRequest($"REGISTER_USER|{username}|{password}|{fullName}");

            if (response.StartsWith("SUCCESS"))
            {
                MessageBox.Show("??ng ký thành công!", "Thành công");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                string errorMsg = response.StartsWith("ERROR") ? response.Substring(6) : response;
                MessageBox.Show("??ng ký th?t b?i: " + errorMsg, "L?i");
            }
        }
    }
}
