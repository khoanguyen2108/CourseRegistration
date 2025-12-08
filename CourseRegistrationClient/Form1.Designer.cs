namespace CourseRegistrationClient
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtStudentId = new System.Windows.Forms.TextBox();
            this.txtCourseId = new System.Windows.Forms.TextBox();
            this.btnViewCourses = new System.Windows.Forms.Button();
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnViewRegistrations = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();

            // label1
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(300, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "HỆ THỐNG ĐĂNG KÝ HỌC PHẦN";

            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Mã Sinh Viên:";

            // txtStudentId
            this.txtStudentId.Location = new System.Drawing.Point(120, 47);
            this.txtStudentId.Name = "txtStudentId";
            this.txtStudentId.Size = new System.Drawing.Size(200, 20);
            this.txtStudentId.TabIndex = 2;

            // label3
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Mã Môn Học:";

            // txtCourseId
            this.txtCourseId.Location = new System.Drawing.Point(120, 77);
            this.txtCourseId.Name = "txtCourseId";
            this.txtCourseId.Size = new System.Drawing.Size(200, 20);
            this.txtCourseId.TabIndex = 4;

            // btnViewCourses
            this.btnViewCourses.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btnViewCourses.ForeColor = System.Drawing.Color.White;
            this.btnViewCourses.Location = new System.Drawing.Point(12, 110);
            this.btnViewCourses.Name = "btnViewCourses";
            this.btnViewCourses.Size = new System.Drawing.Size(120, 30);
            this.btnViewCourses.TabIndex = 5;
            this.btnViewCourses.Text = "Xem Môn Học";
            this.btnViewCourses.UseVisualStyleBackColor = false;
            this.btnViewCourses.Click += new System.EventHandler(this.btnViewCourses_Click);

            // btnRegister
            this.btnRegister.BackColor = System.Drawing.Color.ForestGreen;
            this.btnRegister.ForeColor = System.Drawing.Color.White;
            this.btnRegister.Location = new System.Drawing.Point(140, 110);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(120, 30);
            this.btnRegister.TabIndex = 6;
            this.btnRegister.Text = "Đăng Ký";
            this.btnRegister.UseVisualStyleBackColor = false;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);

            // btnViewRegistrations
            this.btnViewRegistrations.BackColor = System.Drawing.Color.Goldenrod;
            this.btnViewRegistrations.ForeColor = System.Drawing.Color.White;
            this.btnViewRegistrations.Location = new System.Drawing.Point(268, 110);
            this.btnViewRegistrations.Name = "btnViewRegistrations";
            this.btnViewRegistrations.Size = new System.Drawing.Size(150, 30);
            this.btnViewRegistrations.TabIndex = 7;
            this.btnViewRegistrations.Text = "Lịch Sử Đăng Ký";
            this.btnViewRegistrations.UseVisualStyleBackColor = false;
            this.btnViewRegistrations.Click += new System.EventHandler(this.btnViewRegistrations_Click);

            // txtResult
            this.txtResult.Location = new System.Drawing.Point(12, 150);
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.Size = new System.Drawing.Size(406, 250);
            this.txtResult.TabIndex = 8;
            this.txtResult.Text = "";

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 410);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnViewRegistrations);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.btnViewCourses);
            this.Controls.Add(this.txtCourseId);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtStudentId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Ứng Dụng Đăng Ký Học Phần";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtStudentId;
        private System.Windows.Forms.TextBox txtCourseId;
        private System.Windows.Forms.Button btnViewCourses;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Button btnViewRegistrations;
        private System.Windows.Forms.RichTextBox txtResult;
    }
}