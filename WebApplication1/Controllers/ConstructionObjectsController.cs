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
    public class ConstructionObjectsController : Controller
    {
        private readonly ProdajnikContext _context;
        private readonly CachedDataService _cachedDataService;

        public ConstructionObjectsController(ProdajnikContext context, CachedDataService cachedDataService)
        {
            _context = context;
            _cachedDataService = cachedDataService;

        }

        // GET: ConstructionObjects
        public async Task<IActionResult> Index(string objectNameFilter, string contractorFilter, int page = 1, int pageSize = 20)
        {
            var modelsQuery = _cachedDataService.GetConstructionObjects();

            // Фильтрация
            if (!string.IsNullOrEmpty(objectNameFilter))
            {
                modelsQuery = modelsQuery.Where(obj => obj.ObjectName.Contains(objectNameFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(contractorFilter))
            {
                modelsQuery = modelsQuery.Where(obj => obj.GeneralContractor.Contains(contractorFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Пагинация
            int totalItems = modelsQuery.Count();
            var constructionObjects = modelsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Передаем данные во ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.ObjectNameFilter = objectNameFilter;
            ViewBag.ContractorFilter = contractorFilter;

            return View(constructionObjects);
        }


        // GET: ConstructionObjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var constructionObject = await _context.ConstructionObjects
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(m => m.ObjectId == id);
            if (constructionObject == null)
            {
                return NotFound();
            }

            return View(constructionObject);
        }

        // GET: ConstructionObjects/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "OrganizationName");
            return View();
        }

        // POST: ConstructionObjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ObjectId,ObjectName,CustomerId,GeneralContractor,ContractDate,WorkList,DeliveryDate,CommissioningDate,Photo")] ConstructionObject constructionObject)
        {
            if (ModelState.IsValid)
            {
                _context.Add(constructionObject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", constructionObject.CustomerId);
            return View(constructionObject);
        }

        // GET: ConstructionObjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var constructionObject = await _context.ConstructionObjects.FindAsync(id);
            if (constructionObject == null)
            {
                return NotFound();
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "OrganizationName", constructionObject.CustomerId);
            return View(constructionObject);
        }

        // POST: ConstructionObjects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ObjectId,ObjectName,CustomerId,GeneralContractor,ContractDate,WorkList,DeliveryDate,CommissioningDate,Photo")] ConstructionObject constructionObject)
        {
            if (id != constructionObject.ObjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(constructionObject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConstructionObjectExists(constructionObject.ObjectId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", constructionObject.CustomerId);
            return View(constructionObject);
        }

        // GET: ConstructionObjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var constructionObject = await _context.ConstructionObjects
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(m => m.ObjectId == id);
            if (constructionObject == null)
            {
                return NotFound();
            }

            return View(constructionObject);
        }

        // POST: ConstructionObjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var constructionObject = await _context.ConstructionObjects.FindAsync(id);
            if (constructionObject != null)
            {
                _context.ConstructionObjects.Remove(constructionObject);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConstructionObjectExists(int id)
        {
            return _context.ConstructionObjects.Any(e => e.ObjectId == id);
        }
    }
}
