using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurant.Context;
using Restaurant.Models;
using Restaurant.Requests;

namespace Restaurant.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public OrdersController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        public ActionResult Create(int userId, string specialRequests, List<OrderDishRequest> dishes)
        {
            Order order = new Order();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("Пользователь не найден!");
            }
            order.UserId = userId;
            order.Status = "В ожидании";
            order.SpecialRequests = specialRequests;
            order.CreatedAt = DateTime.Now;
            order.UpdatedAt = DateTime.Now;
            _context.Orders.Add(order);
            _context.SaveChanges();
            foreach (var dishOrder in dishes)
            {
                var dish = _context.Dishes.FirstOrDefault(d => d.Id == dishOrder.DishId);
                if (dish == null)
                {
                    DeleteOrder(order.Id);
                    return NotFound($"Блюбо с id = {dishOrder.DishId} не найдено!");
                }

                if (dish.Quantity < dishOrder.Quantity)
                {
                    DeleteOrder(order.Id);
                    return BadRequest($"Блюда с id = {dish.Id} в количестве {dishOrder.Quantity} нет в наличии!");
                }
                OrderDish orderDish = new OrderDish{DishId = dish.Id, OrderId = order.Id, Price = dish.Price, Quantity = dishOrder.Quantity};
                _context.OrderDishes.Add(orderDish);
            }
            _context.SaveChanges();
            return Ok("Заказ успешно создан");
        }

        [HttpGet("GetOrder")]
        public ActionResult GetOrder(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound("Заказ не найден!");
            }
            else
            {
                return Ok(order);
            }
        }

        [HttpGet("GetOrderDishes")]
        public ActionResult GetDishesFromOrder(int id)
        {
            var orderDishes = _context.OrderDishes.Where(o => o.OrderId == id);
            if (orderDishes.Any())
            {
                return Ok(orderDishes);
            }

            return NotFound("Не найдены блюда для этого заказа!");
        }

        private void DeleteOrder(int id)
        {
            var order = _context.Orders.Find(id);
            _context.Orders.Remove(order);
            _context.SaveChanges();
        }
    }
}
