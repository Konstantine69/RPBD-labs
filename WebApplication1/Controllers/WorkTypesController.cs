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
    public class WorkTypesController : Controller
    {
        private readonly ProdajnikContext _context;
        private readonly CachedDataService _cachedDataService;

        public WorkTypesController(ProdajnikContext context, CachedDataService cachedDataService)
        {
            _context = context;
            _cachedDataService = cachedDataService;
        }

        // GET: WorkTypes
        public async Task<IActionResult> Index(string licenseNumberFilter, string classifierCodeFilter, int page = 1, int pageSize = 20)
        {
            var query = _cachedDataService.GetWorkTypes();

            // Фильтрация
            if (!string.IsNullOrEmpty(licenseNumberFilter))
            {
                query = query.Where(workType => workType.LicenseNumber.Contains(licenseNumberFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(classifierCodeFilter))
            {
                query = query.Where(workType => workType.ClassifierCode.Contains(classifierCodeFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Пагинация
            int totalItems = query.Count();
            var workTypes = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Передаем данные во ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.LicenseNumberFilter = licenseNumberFilter;
            ViewBag.ClassifierCodeFilter = classifierCodeFilter;

            return View(workTypes);
        }


        // GET: WorkTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workType = await _context.WorkTypes
                .FirstOrDefaultAsync(m => m.WorkTypeId == id);
            if (workType == null)
            {
                return NotFound();
            }

            return View(workType);
        }

        // GET: WorkTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorkTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkTypeId,LicenseNumber,LicenseDate,LicenseExpirationDate,ClassifierCode")] WorkType workType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workType);
        }

        // GET: WorkTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workType = await _context.WorkTypes.FindAsync(id);
            if (workType == null)
            {
                return NotFound();
            }
            return View(workType);
        }

        // POST: WorkTypes/Edit/5
       [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkTypeId,LicenseNumber,LicenseDate,LicenseExpirationDate,ClassifierCode")] WorkType workType)
        {
            if (id != workType.WorkTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkTypeExists(workType.WorkTypeId))
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
            return View(workType);
        }

        // GET: WorkTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workType = await _context.WorkTypes
                .FirstOrDefaultAsync(m => m.WorkTypeId == id);
            if (workType == null)
            {
                return NotFound();
            }

            return View(workType);
        }

        // POST: WorkTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workType = await _context.WorkTypes.FindAsync(id);
            if (workType != null)
            {
                _context.WorkTypes.Remove(workType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkTypeExists(int id)
        {
            return _context.WorkTypes.Any(e => e.WorkTypeId == id);
        }
    }
}
