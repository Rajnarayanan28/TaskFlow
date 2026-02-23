using Microsoft.AspNetCore.Mvc;
using Task_Web_Application.Models;

namespace Task_Web_Application.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDatabase _db;

        public AccountController()
        {
            _db = new AppDatabase();
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Data");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string currentPassword, string newPassword)
        {
            string email = HttpContext.Session.GetString("UserEmail");

            if (email == null)
                return RedirectToAction("Login", "Data");

            var user = _db.registerModels.FirstOrDefault(x => x.Email == email);

            if (user == null)
                return RedirectToAction("Login", "Data");

            if (user.Password != currentPassword)
            {
                ViewBag.Error = "Current password is incorrect";
                return View();
            }

            user.Password = newPassword;
            _db.SaveChanges();

            return RedirectToAction("Login", "Data");
        }
        [HttpGet]
        public IActionResult DeleteAccount()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Data");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAccount(string password)
        {
            string email = HttpContext.Session.GetString("UserEmail");
            string username = HttpContext.Session.GetString("Username");

            if (email == null)
                return RedirectToAction("Login", "Data");

            var user = _db.registerModels.FirstOrDefault(x => x.Email == email);

            if (user == null)
                return RedirectToAction("Login", "Data");

            // 🔐 Verify password
            if (user.Password != password)
            {
                ViewBag.Error = "Incorrect password.";
                return View();
            }

            // 🗑 Delete related tasks (important!)
            var tasks = _db.addTask.Where(x => x.From == username || x.To == username).ToList();
            _db.addTask.RemoveRange(tasks);

            var history = _db.TaskHistories.Where(x => x.From == username).ToList();
            _db.TaskHistories.RemoveRange(history);

            // 🗑 Delete user
            _db.registerModels.Remove(user);

            _db.SaveChanges();

            // 🚪 Logout
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Data");
        }

    }
}