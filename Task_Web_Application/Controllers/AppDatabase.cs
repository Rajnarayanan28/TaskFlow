using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using Task_Web_Application.Models;

namespace Task_Web_Application.Controllers
{
    public class AppDatabase : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=LAPTOP-JRRIHSFS;Initial catalog=Db_Task_Data;Integrated security=true;TrustServerCertificate=true");
        }
        public DbSet<RegisterModel> registerModels { get; set; }

        public DbSet<TaskModel> addTask { get; set; }

        public DbSet<TaskHistory> TaskHistories { get; set; }
    }
}
