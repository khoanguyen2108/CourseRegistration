using System;

namespace CourseRegistrationServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            ServerSocket server = new ServerSocket();
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║   HỆ THỐNG ĐĂNG KÝ HỌC PHẦN SERVER ║");
            Console.WriteLine("╚════════════════════════════════════╝");

            server.Start();
        }
    }
}