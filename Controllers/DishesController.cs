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
    public class DishesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public DishesController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetMenu()
        {
            var dishes = _context.Dishes.Select(d => new {d.Id, d.Name, d.Description, d.Price, d.IsAvailable}).ToList();
            return Ok(dishes);
        }

        [HttpPost("Create")]
        public ActionResult Create(string name, string description, int price, int quantity)
        {
            Dish dish = new Dish{Name = name, Description = description, Price = price, Quantity = quantity};
            dish.CreatedAt = DateTime.Now;
            dish.UpdatedAt = DateTime.Now;
            if (quantity > 0)
            {
                dish.IsAvailable = true;
            }
            else if (quantity == 0)
            {
                dish.IsAvailable = false;
            }
            else
            {
                return BadRequest("Количество доступных блюд не может быть отрицательным");
            }
            _context.Dishes.Add(dish);
            _context.SaveChanges();
            return Ok(dish);
        }

        [HttpPost("Edit")]
        public ActionResult Edit(int id, string name, string description, int price, int quantity)
        {
            var dish = _context.Dishes.FirstOrDefault(d => d.Id == id);
            if (dish == null)
            {
                return NotFound("Блюдо не найдено!");
            }
            dish.Name = name;
            dish.Description = description;
            dish.Price = price;
            dish.Quantity = quantity;
            dish.UpdatedAt = DateTime.Now;
            if (quantity > 0)
            {
                dish.IsAvailable = true;
            }
            else if (quantity == 0)
            {
                dish.IsAvailable = false;
            }
            else
            {
                return BadRequest("Количество доступных блюд не может быть отрицательным");
            }

            _context.Dishes.Update(dish);
            _context.SaveChanges();
            return Ok(dish);
        }

        [HttpPost("Delete")]
        public ActionResult Delete(int id)
        {
            var dish = _context.Dishes.FirstOrDefault(d => d.Id == id);
            if (dish == null)
            {
                return NotFound("Блюдо не найдено!");
            }
            _context.Dishes.Remove(dish);
            _context.SaveChanges();
            return Ok("Блюдо удалено!");
        }
    }
}
