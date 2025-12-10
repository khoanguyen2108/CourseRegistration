using System;
using System.Windows.Forms;

namespace CourseRegistrationClient
{
    public class AddCourseForm : Form
    {
        private ClientSocket clientSocket;

        private TextBox txtCode;
        private TextBox txtName;
        private TextBox txtCredits;
        private Button btnSave;

        public AddCourseForm(ClientSocket socket)
        {
            clientSocket = socket;

            this.Text = "Thêm môn học";
            this.Size = new System.Drawing.Size(350, 260);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Label Mã môn
            Label lblCode = new Label();
            lblCode.Text = "Mã môn học:";
            lblCode.Location = new System.Drawing.Point(20, 25);
            lblCode.Font = new System.Drawing.Font("Segoe UI", 10);
            this.Controls.Add(lblCode);

            txtCode = new TextBox();
            txtCode.Location = new System.Drawing.Point(130, 25);
            txtCode.Size = new System.Drawing.Size(180, 25);
            this.Controls.Add(txtCode);

            // Label Tên môn
            Label lblName = new Label();
            lblName.Text = "Tên môn:";
            lblName.Location = new System.Drawing.Point(20, 70);
            lblName.Font = new System.Drawing.Font("Segoe UI", 10);
            this.Controls.Add(lblName);

            txtName = new TextBox();
            txtName.Location = new System.Drawing.Point(130, 70);
            txtName.Size = new System.Drawing.Size(180, 25);
            this.Controls.Add(txtName);

            // Label Số tín chỉ
            Label lblCredits = new Label();
            lblCredits.Text = "Số tín chỉ:";
            lblCredits.Location = new System.Drawing.Point(20, 115);
            lblCredits.Font = new System.Drawing.Font("Segoe UI", 10);
            this.Controls.Add(lblCredits);

            txtCredits = new TextBox();
            txtCredits.Location = new System.Drawing.Point(130, 115);
            txtCredits.Size = new System.Drawing.Size(180, 25);
            this.Controls.Add(txtCredits);

            // Save button
            btnSave = new Button();
            btnSave.Text = "Lưu";
            btnSave.Location = new System.Drawing.Point(120, 160);
            btnSave.Size = new System.Drawing.Size(100, 35);
            btnSave.Font = new System.Drawing.Font("Segoe UI", 10);
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            this.ResumeLayout(false);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text.Trim();
            string name = txtName.Text.Trim();
            string credits = txtCredits.Text.Trim();

            if (code == "" || name == "" || credits == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi");
                return;
            }

            string response = clientSocket.SendRequest($"ADD_COURSE|{code}|{name}|{credits}");

            if (response.StartsWith("SUCCESS"))
            {
                MessageBox.Show("Thêm môn học thành công!", "Thành công");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Thêm môn thất bại!", "Lỗi");
            }
        }
    }
}
