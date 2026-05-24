using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseApp.Models;

namespace WarehouseApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context) => _context = context;

        // Вход
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var salt = "$2a$11$warehousei";
            var input = salt + dto.Password;

            // SQL использует NVARCHAR — значит кодировка Unicode (UTF-16 LE)
            var hashBytes = System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.Unicode.GetBytes(input));
            var hashString = "$2a$11$" + Convert.ToHexString(hashBytes);

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e =>
                    e.FullName.ToLower().Contains(dto.Login.ToLower()) &&
                    e.PasswordHash == hashString);

            if (employee == null)
                return Unauthorized(new { message = "Неверный логин или пароль" });

            HttpContext.Session.SetInt32("EmployeeId", employee.EmployeeId);
            HttpContext.Session.SetString("EmployeeName", employee.FullName);
            HttpContext.Session.SetString("Position", employee.Position ?? "Сотрудник");
            HttpContext.Session.SetString("Role", employee.Role ?? "Кладовщик");

            _context.SystemLogs.Add(new SystemLog
            {
                EmployeeId = employee.EmployeeId,
                ActionDescription = $"Вход в систему: {employee.FullName} ({employee.Position})",
                ActionTime = DateTime.Now
            });
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Добро пожаловать!",
                name = employee.FullName,
                position = employee.Position,
                employeeId = employee.EmployeeId
            });
        }

        // Выход
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var name = HttpContext.Session.GetString("EmployeeName");
            var empId = HttpContext.Session.GetInt32("EmployeeId");

            if (empId.HasValue)
            {
                _context.SystemLogs.Add(new SystemLog
                {
                    EmployeeId = empId.Value,
                    ActionDescription = $"Выход из системы: {name}",
                    ActionTime = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            HttpContext.Session.Clear();
            return Ok(new { message = "Выход выполнен" });
        }

        // Проверка — залогинен ли пользователь
        [HttpGet("me")]
        public IActionResult Me()
        {
            var name = HttpContext.Session.GetString("EmployeeName");
            if (string.IsNullOrEmpty(name))
                return Unauthorized();

            return Ok(new
            {
                name,
                position = HttpContext.Session.GetString("Position"),
                employeeId = HttpContext.Session.GetInt32("EmployeeId"),
                role = HttpContext.Session.GetString("Role")
            });
        }
    }

    public class LoginDto
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
