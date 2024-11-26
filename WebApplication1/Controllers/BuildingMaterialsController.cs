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
    public class BuildingMaterialsController : Controller
    {
        private readonly ProdajnikContext _context;
        private readonly CachedDataService _cachedDataService;

        public BuildingMaterialsController(ProdajnikContext context, CachedDataService cachedDataService)
        {
            _context = context;
            _cachedDataService = cachedDataService;
        }

        // GET: BuildingMaterials
        public async Task<IActionResult> Index(string materialNameFilter, string manufacturerFilter, int page = 1, int pageSize = 20)
        {
            var query = _cachedDataService.GetBuildingMaterials();

            // Фильтрация
            if (!string.IsNullOrEmpty(materialNameFilter))
            {
                query = query.Where(material => material.MaterialName.Contains(materialNameFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(manufacturerFilter))
            {
                query = query.Where(material => material.Manufacturer.Contains(manufacturerFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Пагинация
            int totalItems = query.Count();
            var buildingMaterials = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Передача данных во ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.MaterialNameFilter = materialNameFilter;
            ViewBag.ManufacturerFilter = manufacturerFilter;

            return View(buildingMaterials);
        }


        // GET: BuildingMaterials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buildingMaterial = await _context.BuildingMaterials
                .FirstOrDefaultAsync(m => m.MaterialId == id);
            if (buildingMaterial == null)
            {
                return NotFound();
            }

            return View(buildingMaterial);
        }

        // GET: BuildingMaterials/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BuildingMaterials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaterialId,MaterialName,Manufacturer,PurchaseVolume,CertificateNumber,CertificateDate,Photo")] BuildingMaterial buildingMaterial)
        {
            if (ModelState.IsValid)
            {
                _context.Add(buildingMaterial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(buildingMaterial);
        }

        // GET: BuildingMaterials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buildingMaterial = await _context.BuildingMaterials.FindAsync(id);
            if (buildingMaterial == null)
            {
                return NotFound();
            }
            return View(buildingMaterial);
        }

        // POST: BuildingMaterials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaterialId,MaterialName,Manufacturer,PurchaseVolume,CertificateNumber,CertificateDate,Photo")] BuildingMaterial buildingMaterial)
        {
            if (id != buildingMaterial.MaterialId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(buildingMaterial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BuildingMaterialExists(buildingMaterial.MaterialId))
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
            return View(buildingMaterial);
        }

        // GET: BuildingMaterials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buildingMaterial = await _context.BuildingMaterials
                .FirstOrDefaultAsync(m => m.MaterialId == id);
            if (buildingMaterial == null)
            {
                return NotFound();
            }

            return View(buildingMaterial);
        }

        // POST: BuildingMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var buildingMaterial = await _context.BuildingMaterials.FindAsync(id);
            if (buildingMaterial != null)
            {
                _context.BuildingMaterials.Remove(buildingMaterial);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BuildingMaterialExists(int id)
        {
            return _context.BuildingMaterials.Any(e => e.MaterialId == id);
        }
    }
}
