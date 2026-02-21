namespace Task_Web_Application.Models
{
    public class TaskHistory
    {
        public int Id { get; set; }

        public string From { get; set; }
        public string To { get; set; }
        public string Sub { get; set; }
        public string Task { get; set; }

        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

        public DateTime ArchivedOn { get; set; }
    }
}