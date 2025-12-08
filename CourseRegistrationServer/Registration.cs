using System;

namespace CourseRegistrationServer
{
    public class Registration
    {
        public string StudentId { get; set; }       // Mã sinh viên
        public string CourseId { get; set; }        // Mã môn
        public DateTime RegistrationDate { get; set; } // Ngày đăng ký
        public string Status { get; set; }          // Trạng thái (Success/Failed)

        public Registration() { }

        public Registration(string studentId, string courseId)
        {
            StudentId = studentId;
            CourseId = courseId;
            RegistrationDate = DateTime.Now;
            Status = "Pending";
        }

        public override string ToString()
        {
            return $"SV: {StudentId} | Môn: {CourseId} | Ngày: {RegistrationDate:dd/MM/yyyy HH:mm:ss} | Trạng thái: {Status}";
        }
    }
}