using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Context;
using System.Text.RegularExpressions;
using Restaurant.Models;

namespace Restaurant.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public RegistrationController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPost]
        public ActionResult Registration(string name, string email, string password)
        {
            Regex validateEmailRegex = new Regex("^\\S+@\\S+\\.\\S+$");
            bool isCorrectEmail = validateEmailRegex.IsMatch(email);
            var existedUser = _context.Users.FirstOrDefault(u => u.Email == email);
            if (existedUser != null)
            {
                return Conflict("Пользователь с таким email уже существует");
            }

            if (!isCorrectEmail)
            {
                return BadRequest("Некорректный email");
            }

            User user = new User { Username = name, Email = email, Password = password, Role = "customer", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now};
            _context.Users.Add(user);
            _context.SaveChanges();
            user = _context.Users.FirstOrDefault(u => u.Email == email);
            return Ok(user);
        }
    }
}
