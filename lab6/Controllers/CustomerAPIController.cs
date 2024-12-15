using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab6.Data;
using lab6.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace lab6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersAPIController : ControllerBase
    {
        private readonly ProdajnikContext _context;

        public CustomersAPIController(ProdajnikContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        [SwaggerOperation(Summary = "Получить список клиентов", Description = "Возвращает всех клиентов, с возможностью фильтрации по имени организации и городу.")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers(string organizationNameFilter = null, string cityFilter = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Customers.AsQueryable();

            // Применение фильтров
            if (!string.IsNullOrEmpty(organizationNameFilter))
            {
                query = query.Where(c => c.OrganizationName.Contains(organizationNameFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(cityFilter))
            {
                query = query.Where(c => c.City.Contains(cityFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Пагинация
            int totalItems = await query.CountAsync();
            var customers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Заголовок с общим количеством элементов
            Response.Headers["X-Total-Count"] = totalItems.ToString();

            return Ok(customers);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить клиента по ID", Description = "Возвращает данные конкретного клиента по его ID.")]
        [SwaggerResponse(200, "Клиент найден", typeof(Customer))]
        [SwaggerResponse(404, "Клиент не найден")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Обновить данные клиента", Description = "Обновляет данные клиента по его ID.")]
        [SwaggerResponse(204, "Данные клиента успешно обновлены")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(404, "Клиент не найден")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customers
        [HttpPost]
        [SwaggerOperation(Summary = "Создать нового клиента", Description = "Создает нового клиента в базе данных.")]
        [SwaggerResponse(201, "Клиент успешно создан", typeof(Customer))]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить клиента", Description = "Удаляет клиента по его ID.")]
        [SwaggerResponse(204, "Клиент успешно удалён")]
        [SwaggerResponse(404, "Клиент не найден")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
