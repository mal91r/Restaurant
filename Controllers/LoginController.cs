using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Context;
using Restaurant.Models;

namespace Restaurant.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public LoginController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound("Пользователь с таким email не найден!");
            }
            else
            {
                if (user.Password != password)
                {
                    return NotFound("Неверный пароль!");
                }
                else
                {
                    Session session = _context.Sessions.FirstOrDefault(s => s.UserId == user.Id);
                    if (session == null)
                    {
                        var jwt = new JwtSecurityToken(
                            issuer: AuthOptions.ISSUER,
                            audience: AuthOptions.AUDIENCE,
                            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
                        session = new Session{ExpiresAt = DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)), SessionToken = token, UserId = user.Id};
                        _context.Sessions.Add(session);
                        _context.SaveChanges();
                    }
                    else
                    {
                        session.ExpiresAt = DateTime.UtcNow.Add(TimeSpan.FromMinutes(2));
                        _context.SaveChanges();
                    }
                    return Ok(session);
                }
            }
        }
    }
}
