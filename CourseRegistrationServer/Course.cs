using System;

namespace CourseRegistrationServer
{
    public class Course
    {
        public string CourseId { get; set; }        // Mã môn (VD: "CTT101")
        public string CourseName { get; set; }      // Tên môn (VD: "Lập trình C#")
        public int Credits { get; set; }            // Số tín chỉ (VD: 3)
        public int AvailableSlots { get; set; }     // Số chỗ còn lại

        public Course() { }

        public Course(string courseId, string courseName, int credits, int availableSlots)
        {
            CourseId = courseId;
            CourseName = courseName;
            Credits = credits;
            AvailableSlots = availableSlots;
        }

        public override string ToString()
        {
            return $"[{CourseId}] {CourseName} - {Credits} tín chỉ - Còn: {AvailableSlots} chỗ";
        }
    }
}