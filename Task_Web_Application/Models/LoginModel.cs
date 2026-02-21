namespace Task_Web_Application.Models
{
    public class LoginModel
    {
        //[Required(ErrorMessage = "Email Mandtory")]
        public string Email { get; set; }


        //[Required(ErrorMessage = "Password Mandtory")]
        public string Password { get; set; }
    }
}
