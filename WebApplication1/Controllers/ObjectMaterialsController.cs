using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProdajnikWeb.Data;
using ProdajnikWeb.Models;
using ProdajnikWeb.Service;

namespace ProdajnikWeb.Controllers
{
    public class ObjectMaterialsController : Controller
    {
        private readonly ProdajnikContext _context;
        private readonly CachedDataService _cachedDataService;
        
        public ObjectMaterialsController(ProdajnikContext context, CachedDataService cachedDataService)
        {
            _context = context;
            _cachedDataService = cachedDataService;
        }

        // GET: ObjectMaterials
        public async Task<IActionResult> Index(string materialNameFilter, string objectNameFilter, int page = 1, int pageSize = 20)
        {
            var query = _cachedDataService.GetObjectMaterials();

            // Фильтрация
            if (!string.IsNullOrEmpty(materialNameFilter))
            {
                query = query.Where(om => om.Material.MaterialName.Contains(materialNameFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(objectNameFilter))
            {
                query = query.Where(om => om.Object.ObjectName.Contains(objectNameFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Пагинация
            int totalItems = query.Count();
            var objectMaterials = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Передача данных во ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.MaterialNameFilter = materialNameFilter;
            ViewBag.ObjectNameFilter = objectNameFilter;

            return View(objectMaterials);
        }



        // GET: ObjectMaterials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objectMaterial = await _context.ObjectMaterials
                .Include(o => o.Material)
                .Include(o => o.Object)
                .FirstOrDefaultAsync(m => m.ObjectMaterialId == id);
            if (objectMaterial == null)
            {
                return NotFound();
            }

            return View(objectMaterial);
        }

        // GET: ObjectMaterials/Create
        public IActionResult Create()
        {
            ViewData["MaterialId"] = new SelectList(_context.BuildingMaterials, "MaterialId", "MaterialName");
            ViewData["ObjectId"] = new SelectList(_context.ConstructionObjects, "ObjectId", "ObjectName");
            return View();
        }

        // POST: ObjectMaterials/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ObjectMaterialId,ObjectId,MaterialId")] ObjectMaterial objectMaterial)
        {
            if (ModelState.IsValid)
            {
                _context.Add(objectMaterial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaterialId"] = new SelectList(_context.BuildingMaterials, "MaterialId", "MaterialId", objectMaterial.MaterialId);
            ViewData["ObjectId"] = new SelectList(_context.ConstructionObjects, "ObjectId", "ObjectId", objectMaterial.ObjectId);
            return View(objectMaterial);
        }

        // GET: ObjectMaterials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objectMaterial = await _context.ObjectMaterials.FindAsync(id);
            if (objectMaterial == null)
            {
                return NotFound();
            }

            ViewData["MaterialId"] = new SelectList(_context.BuildingMaterials, "MaterialId", "MaterialName", objectMaterial.MaterialId);
            ViewData["ObjectId"] = new SelectList(_context.ConstructionObjects, "ObjectId", "ObjectName", objectMaterial.ObjectId);
            return View(objectMaterial);
        }

        // POST: ObjectMaterials/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ObjectMaterialId,ObjectId,MaterialId")] ObjectMaterial objectMaterial)
        {
            if (id != objectMaterial.ObjectMaterialId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(objectMaterial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObjectMaterialExists(objectMaterial.ObjectMaterialId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaterialId"] = new SelectList(_context.BuildingMaterials, "MaterialId", "MaterialId", objectMaterial.MaterialId);
            ViewData["ObjectId"] = new SelectList(_context.ConstructionObjects, "ObjectId", "ObjectId", objectMaterial.ObjectId);
            return View(objectMaterial);
        }

        // GET: ObjectMaterials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objectMaterial = await _context.ObjectMaterials
                .Include(o => o.Material)
                .Include(o => o.Object)
                .FirstOrDefaultAsync(m => m.ObjectMaterialId == id);
            if (objectMaterial == null)
            {
                return NotFound();
            }

            return View(objectMaterial);
        }

        // POST: ObjectMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objectMaterial = await _context.ObjectMaterials.FindAsync(id);
            if (objectMaterial != null)
            {
                _context.ObjectMaterials.Remove(objectMaterial);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObjectMaterialExists(int id)
        {
            return _context.ObjectMaterials.Any(e => e.ObjectMaterialId == id);
        }
    }
}
