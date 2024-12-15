using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lab6.Data;
using lab6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace lab6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingMaterialsApiController : ControllerBase
    {
        private readonly ProdajnikContext _context;

        public BuildingMaterialsApiController(ProdajnikContext context)
        {
            _context = context;
        }

        // GET: api/BuildingMaterials
        [HttpGet]
        [SwaggerOperation(Summary = "Получить список строительных материалов с пагинацией.")]
        [SwaggerResponse(200, "Список строительных материалов успешно получен.", typeof(IEnumerable<BuildingMaterial>))]
        public async Task<ActionResult<IEnumerable<BuildingMaterial>>> GetBuildingMaterials(int page = 1, int pageSize = 20)
        {
            var query = _context.BuildingMaterials.AsQueryable();

            // Пагинация
            int totalItems = await query.CountAsync();
            var materials = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers["X-Total-Count"] = totalItems.ToString();

            return Ok(materials);
        }

        // GET: api/BuildingMaterials/5
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить информацию о конкретном строительном материале.")]
        [SwaggerResponse(200, "Информация о строительном материале успешно получена.", typeof(BuildingMaterial))]
        [SwaggerResponse(404, "Строительный материал не найден.")]
        public async Task<ActionResult<BuildingMaterial>> GetBuildingMaterial(int id)
        {
            var buildingMaterial = await _context.BuildingMaterials.FindAsync(id);

            if (buildingMaterial == null)
            {
                return NotFound();
            }

            return Ok(buildingMaterial);
        }

        // POST: api/BuildingMaterials
        [HttpPost]
        [SwaggerOperation(Summary = "Создать новый строительный материал.")]
        [SwaggerResponse(201, "Строительный материал успешно создан.", typeof(BuildingMaterial))]
        [SwaggerResponse(400, "Некорректные входные данные.")]
        public async Task<ActionResult<BuildingMaterial>> PostBuildingMaterial(BuildingMaterial buildingMaterial)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.BuildingMaterials.Add(buildingMaterial);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBuildingMaterial), new { id = buildingMaterial.MaterialId }, buildingMaterial);
        }

        // PUT: api/BuildingMaterials/5
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Обновить существующий строительный материал.")]
        [SwaggerResponse(204, "Строительный материал успешно обновлен.")]
        [SwaggerResponse(400, "Некорректные входные данные.")]
        [SwaggerResponse(404, "Строительный материал не найден.")]
        public async Task<IActionResult> PutBuildingMaterial(int id, BuildingMaterial buildingMaterial)
        {
            if (id != buildingMaterial.MaterialId)
            {
                return BadRequest();
            }

            _context.Entry(buildingMaterial).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BuildingMaterialExists(id))
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

        // DELETE: api/BuildingMaterials/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить конкретный строительный материал.")]
        [SwaggerResponse(204, "Строительный материал успешно удален.")]
        [SwaggerResponse(404, "Строительный материал не найден.")]
        public async Task<IActionResult> DeleteBuildingMaterial(int id)
        {
            var buildingMaterial = await _context.BuildingMaterials.FindAsync(id);
            if (buildingMaterial == null)
            {
                return NotFound();
            }

            _context.BuildingMaterials.Remove(buildingMaterial);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BuildingMaterialExists(int id)
        {
            return _context.BuildingMaterials.Any(e => e.MaterialId == id);
        }
    }
}
