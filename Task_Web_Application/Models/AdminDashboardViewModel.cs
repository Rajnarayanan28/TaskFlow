namespace Task_Web_Application.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int TotalUsers { get; set; }

        public double CompletionRate { get; set; }
    }
}