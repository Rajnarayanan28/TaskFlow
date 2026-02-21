using System.ComponentModel.DataAnnotations;

namespace Task_Web_Application.Models
{
    public class RegisterModel
    {
        [Key]  // pk & auto
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime Dob { get; set; }
        public string Type { get; set; }
    }
}
