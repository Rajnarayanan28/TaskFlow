using Microsoft.AspNetCore.Mvc;
using Task_Web_Application.Models;

namespace Task_Web_Application.Controllers
{
    public class DataController : Controller
    {
        public AppDatabase _db;

        public DataController()
        {
            _db = new AppDatabase();
        }

        // ================= REGISTER =================

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel obj)
        {
            if (ModelState.IsValid)
            {
                _db.registerModels.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Login");
            }

            return View(obj);
        }

        // ================= LOGIN =================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel obj)
        {
            if (ModelState.IsValid)
            {
                var user = _db.registerModels
                              .FirstOrDefault(x => x.Email == obj.Email
                                                && x.Password == obj.Password);

                if (user != null)
                {
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("UserType", user.Type);

                    if (user.Type == "admin")
                        return RedirectToAction("AdminHomepage");

                    if (user.Type == "user")
                        return RedirectToAction("UserHomepage");
                }

                ViewBag.Error = "Invalid Email or Password";
            }

            return View(obj);
        }

        // ================= ADMIN HOMEPAGE =================

        public IActionResult AdminHomepage()
        {
            string type = HttpContext.Session.GetString("UserType");

            if (type != "admin")
                return RedirectToAction("Login");

            string adminUsername = HttpContext.Session.GetString("Username");

            var tasks = _db.addTask
                           .Where(x => x.From == adminUsername)
                           .ToList();

            int totalTasks = tasks.Count;
            int completed = tasks.Count(x => x.Status == true);
            int pending = tasks.Count(x => x.Status == false);
            int totalUsers = _db.registerModels.Count(x => x.Type == "user");

            double completionRate = totalTasks == 0 ? 0 :
                (double)completed / totalTasks * 100;

            var model = new AdminDashboardViewModel
            {
                TotalTasks = totalTasks,
                CompletedTasks = completed,
                PendingTasks = pending,
                TotalUsers = totalUsers,
                CompletionRate = completionRate
            };

            return View(model);
        }

        // ================= USER HOMEPAGE =================

        public IActionResult UserHomepage()
        {
            string type = HttpContext.Session.GetString("UserType");

            if (type != "user")
                return RedirectToAction("Login");

            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View();
        }

        // ================= ADD TASK =================

        [HttpGet]
        public IActionResult AddTask()
        {
            string type = HttpContext.Session.GetString("UserType");

            if (type != "admin")
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public IActionResult AddTask(TaskModel obj)
        {
            string type = HttpContext.Session.GetString("UserType");

            if (type != "admin")
                return RedirectToAction("Login");

            var verify = _db.registerModels
                            .Any(x => x.Username == obj.To && x.Type == "user");

            if (verify)
            {
                // ✅ AUTO SET FROM (ADMIN USERNAME)
                obj.From = HttpContext.Session.GetString("Username");

                // ✅ DEFAULT STATUS = PENDING
                obj.Status = false;

                _db.addTask.Add(obj);
                _db.SaveChanges();

                return RedirectToAction("AdminHomepage");
            }

            return BadRequest("Invalid recipient.");
        }

        // ================= VIEW TASK =================

        public IActionResult ViewTask()
        {
            string type = HttpContext.Session.GetString("UserType");

            if (type == null)
                return RedirectToAction("Login");

            if (type == "admin")
            {
                return View(_db.addTask.ToList());
            }

            if (type == "user")
            {
                string username = HttpContext.Session.GetString("Username");

                var userTasks = _db.addTask
                                   .Where(x => x.To == username)
                                   .ToList();

                return View(userTasks);
            }

            return RedirectToAction("Login");
        }

        // ================= UPDATE STATUS (UNDO SUPPORTED) =================

        [HttpPost]
        public IActionResult UpdateStatus(int id)
        {
            string type = HttpContext.Session.GetString("UserType");

            if (type != "user")
                return RedirectToAction("Login");

            var task = _db.addTask.FirstOrDefault(x => x.ID == id);
            if (task == null)
                return NotFound();

            string username = HttpContext.Session.GetString("Username");

            if (task.To != username)
                return Unauthorized();

            task.Status = !task.Status;
            _db.SaveChanges();

            return RedirectToAction("ViewTask");


        }


        // ================= COMPLETED TASKS (ADMIN - OWN ONLY) =================

        public IActionResult CompletedTasks()
        {
            string type = HttpContext.Session.GetString("UserType");

            if (type != "admin")
                return RedirectToAction("Login");

            string adminUsername = HttpContext.Session.GetString("Username");

            var completedTasks = _db.addTask
                                    .Where(x => x.Status == true &&
                                                x.From == adminUsername)
                                    .ToList();

            return View(completedTasks);
        }

        [HttpPost]
        public IActionResult ApproveTask(int id)
        {
            string type = HttpContext.Session.GetString("UserType");

            if (type != "admin")
                return RedirectToAction("Login");

            string adminUsername = HttpContext.Session.GetString("Username");

            var task = _db.addTask
                          .FirstOrDefault(x => x.ID == id &&
                                               x.From == adminUsername &&
                                               x.Status == true);

            if (task == null)
                return Unauthorized();

            var history = new TaskHistory
            {
                From = task.From,
                To = task.To,
                Sub = task.Sub,
                Task = task.Task,
                Start = task.Start,
                End = task.End,
                ArchivedOn = DateTime.Now
            };

            _db.TaskHistories.Add(history);
            _db.addTask.Remove(task);
            _db.SaveChanges();

            return RedirectToAction("CompletedTasks");
        }
        public IActionResult History()
        {
            string type = HttpContext.Session.GetString("UserType");

            if (type != "admin")
                return RedirectToAction("Login");

            string adminUsername = HttpContext.Session.GetString("Username");

            var history = _db.TaskHistories
                             .Where(x => x.From == adminUsername)
                             .OrderByDescending(x => x.ArchivedOn)
                             .ToList();

            return View(history);
        }
        [HttpPost]
        public IActionResult ClearHistory()
        {
            string type = HttpContext.Session.GetString("UserType");

            if (type != "admin")
                return RedirectToAction("Login");

            string adminUsername = HttpContext.Session.GetString("Username");

            var history = _db.TaskHistories
                             .Where(x => x.From == adminUsername)
                             .ToList();

            _db.TaskHistories.RemoveRange(history);
            _db.SaveChanges();

            return RedirectToAction("History");
        }



        // ================= LOGOUT =================

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
