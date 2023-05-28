using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurant.Context;
using Restaurant.Models;

namespace Restaurant.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public UsersController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("GetUser")]
        public ActionResult GetUser(string token)
        {
            var session = _context.Sessions.FirstOrDefault(s => s.SessionToken == token);
            if (session == null)
            {
                return NotFound("Пользователь не найден!");
            }
            else
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == session.UserId);
                return Ok(user);
            }
        }

        [HttpPost("EditRole")]
        public ActionResult EditRole (int id, string role)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound("Пользователь не найден!");
            }

            if (role != "customer" && role != "chef" && role != "manager")
            {
                return BadRequest($"Роль {role} не существует!");
            }

            user.Role = role;
            user.UpdatedAt = DateTime.Now;  
            _context.Users.Update(user);
            _context.SaveChanges();
            return Ok(user);
        }

        [HttpPost("Delete")]
        public ActionResult Delete(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ApplicationContext.Users'  is null.");
            }
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                var session = _context.Sessions.FirstOrDefault(s => s.UserId == id);
                _context.Sessions.Remove(session);
            }
            
            _context.SaveChangesAsync();
            return Ok();
        }
    }
}
