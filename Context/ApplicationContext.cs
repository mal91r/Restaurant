using Microsoft.EntityFrameworkCore;
using Npgsql;
using Restaurant.Models;

namespace Restaurant.Context
{
    public class ApplicationContext : DbContext
    {
        private NpgsqlConnection _connection;
        public DbSet<User>? Users { get; set; }
        public DbSet<Session>? Sessions { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<Dish>? Dishes { get; set; }
        public DbSet<OrderDish>? OrderDishes { get; set; }


        private void CreateUserTable()
        {
            NpgsqlCommand cmd = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS \"user\"\r\n" +
                                                  "(\r\n    " +
                                                  "id            SERIAL PRIMARY KEY,\r\n    " +
                                                  "username      VARCHAR(50) UNIQUE  NOT NULL,\r\n    " +
                                                  "email         VARCHAR(100) UNIQUE NOT NULL,\r\n    " +
                                                  "password_hash VARCHAR(255)        NOT NULL,\r\n    " +
                                                  "role          VARCHAR(10)         NOT NULL CHECK (role IN ('customer', 'chef', 'manager')),\r\n    " +
                                                  "created_at    TIMESTAMP DEFAULT CURRENT_TIMESTAMP,\r\n    " +
                                                  "updated_at    TIMESTAMP DEFAULT CURRENT_TIMESTAMP\r\n" +
                                                  ");", _connection);
            cmd.ExecuteNonQuery();
        }

        private void CreateSessionTable()
        {
            NpgsqlCommand cmd = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS session\r\n" +
                                                  "(\r\n    " +
                                                  "id            serial PRIMARY KEY,\r\n    " +
                                                  "user_id       INT          NOT NULL,\r\n    " +
                                                  "session_token TEXT NOT NULL,\r\n    " +
                                                  "expires_at    TIMESTAMP    NOT NULL,\r\n    " +
                                                  "FOREIGN KEY (user_id) REFERENCES \"user\" (id)\r\n" +
                                                  ");", _connection);
            cmd.ExecuteNonQuery();
        }

        private void CreateDishTable()
        {
            NpgsqlCommand cmd = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS dish\r\n" +
                                                  "(\r\n    " +
                                                  "id           SERIAL PRIMARY KEY,\r\n    " +
                                                  "name         VARCHAR(100)   NOT NULL,\r\n    " +
                                                  "description  TEXT,\r\n    price        DECIMAL(10, 2) NOT NULL,\r\n    " +
                                                  "quantity     INT            NOT NULL,\r\n    " +
                                                  "is_available BOOLEAN        NOT NULL,\r\n    " +
                                                  "created_at   TIMESTAMP DEFAULT CURRENT_TIMESTAMP,\r\n    " +
                                                  "updated_at   TIMESTAMP DEFAULT CURRENT_TIMESTAMP\r\n" +
                                                  ");", _connection);
            cmd.ExecuteNonQuery();
        }

        private void CreateOrderTable()
        {
            NpgsqlCommand cmd = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS \"order\"\r\n" +
                                                  "(\r\n    " +
                                                  "id               SERIAL PRIMARY KEY,\r\n    " +
                                                  "user_id          INT         NOT NULL,\r\n    " +
                                                  "status           VARCHAR(50) NOT NULL,\r\n    " +
                                                  "special_requests TEXT,\r\n    " +
                                                  "created_at       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,\r\n    " +
                                                  "updated_at       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,\r\n    " +
                                                  "FOREIGN KEY (user_id) REFERENCES \"user\" (id)\r\n" +
                                                  ");", _connection);
            cmd.ExecuteNonQuery();
        }

        private void CreateOrderDishTable()
        {
            NpgsqlCommand cmd = new NpgsqlCommand("CREATE TABLE order_dish\r\n" +
                                                  "(\r\n    " +
                                                  "id       SERIAL PRIMARY KEY,\r\n    " +
                                                  "order_id INT            NOT NULL,\r\n    " +
                                                  "dish_id  INT            NOT NULL,\r\n    " +
                                                  "quantity INT            NOT NULL,\r\n    " +
                                                  "price    DECIMAL(10, 2) NOT NULL,\r\n    " +
                                                  "FOREIGN KEY (order_id) REFERENCES \"order\" (id),\r\n    " +
                                                  "FOREIGN KEY (dish_id) REFERENCES dish (id)\r\n" +
                                                  ");", _connection);
            cmd.ExecuteNonQuery();
        }

        private void CreateTables()
        {
            _connection =
                new NpgsqlConnection("Server=localhost;Port=15432;Database=restaurant;Username=mal91r;Password=123456");
            _connection.Open();
            CreateUserTable();
            CreateSessionTable();
            CreateDishTable();
            CreateOrderTable();
            CreateOrderDishTable();
        }
        public ApplicationContext()
        {
            //CreateTables();
            //Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server=localhost;Port=15432;Database=restaurant;Username=mal91r;Password=123456");
        }
    }
}
