using lab6.Data;
using lab6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations; // Для аннотаций Swagger

namespace lab6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConstructionObjectsAPIController : ControllerBase
    {
        private readonly ProdajnikContext _context;

        public ConstructionObjectsAPIController(ProdajnikContext context)
        {
            _context = context;
        }

        // GET: api/ConstructionObjects
        [HttpGet]
        [SwaggerOperation(Summary = "Получить список строительных объектов", Description = "Возвращает все строительные объекты с их связанными данными и поддержкой фильтров.")]
        public async Task<ActionResult<IEnumerable<ConstructionObject>>> GetConstructionObjects(int page = 1, int pageSize = 20)
        {
            var query = _context.ConstructionObjects
                .Include(co => co.Customer)
                .Include(co => co.ObjectWorks)
                    .ThenInclude(ow => ow.WorkType)
                .Include(co => co.ObjectMaterials)
                    .ThenInclude(om => om.Material)
                .AsQueryable();

            // Пагинация
            int totalItems = await query.CountAsync();
            var constructionObjects = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Заголовок с общим количеством элементов
            Response.Headers["X-Total-Count"] = totalItems.ToString();

            return Ok(constructionObjects);
        }

        // GET: api/ConstructionObjects/5
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить строительный объект по ID", Description = "Возвращает информацию о строительном объекте с заданным ID.")]
        [SwaggerResponse(200, "Объект найден", typeof(ConstructionObject))]
        [SwaggerResponse(404, "Объект не найден")]
        public async Task<ActionResult<ConstructionObject>> GetConstructionObject(int id)
        {
            var constructionObject = await _context.ConstructionObjects
                .Include(co => co.Customer)
                .Include(co => co.ObjectWorks)
                    .ThenInclude(ow => ow.WorkType)
                .Include(co => co.ObjectMaterials)
                    .ThenInclude(om => om.Material)
                .FirstOrDefaultAsync(co => co.ObjectId == id);

            if (constructionObject == null)
            {
                return NotFound();
            }

            return constructionObject;
        }

        // PUT: api/ConstructionObjects/5
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Обновить строительный объект", Description = "Обновляет информацию о строительном объекте по ID.")]
        [SwaggerResponse(204, "Объект успешно обновлён")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(404, "Объект не найден")]
        public async Task<IActionResult> PutConstructionObject(int id, ConstructionObject constructionObject)
        {
            if (id != constructionObject.ObjectId)
            {
                return BadRequest();
            }

            _context.Entry(constructionObject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConstructionObjectExists(id))
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

        // POST: api/ConstructionObjects
        [HttpPost]
        [SwaggerOperation(Summary = "Создать новый строительный объект", Description = "Создаёт новый объект и добавляет его в базу данных.")]
        [SwaggerResponse(201, "Объект успешно создан", typeof(ConstructionObject))]
        public async Task<ActionResult<ConstructionObject>> PostConstructionObject(ConstructionObject constructionObject)
        {
            _context.ConstructionObjects.Add(constructionObject);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConstructionObject", new { id = constructionObject.ObjectId }, constructionObject);
        }

        // DELETE: api/ConstructionObjects/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить строительный объект", Description = "Удаляет строительный объект по ID.")]
        [SwaggerResponse(204, "Объект успешно удалён")]
        [SwaggerResponse(404, "Объект не найден")]
        public async Task<IActionResult> DeleteConstructionObject(int id)
        {
            var constructionObject = await _context.ConstructionObjects.FindAsync(id);
            if (constructionObject == null)
            {
                return NotFound();
            }

            _context.ConstructionObjects.Remove(constructionObject);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ConstructionObjectExists(int id)
        {
            return _context.ConstructionObjects.Any(co => co.ObjectId == id);
        }
    }
}
