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
    public class ObjectWorksController : Controller
    {
        private readonly ProdajnikContext _context;
        private readonly CachedDataService _cachedDataService;

        public ObjectWorksController(ProdajnikContext context, CachedDataService cachedDataService)
        {
            _context = context;
            _cachedDataService = cachedDataService;
        }

        // GET: ObjectWorks
        public async Task<IActionResult> Index(string objectNameFilter, string classifierCodeFilter, int page = 1, int pageSize = 10)
        {
            var query = _cachedDataService.GetObjectWorks();

            // Фильтрация
            if (!string.IsNullOrEmpty(objectNameFilter))
            {
                query = query.Where(ow => ow.Object.ObjectName.Contains(objectNameFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(classifierCodeFilter))
            {
                query = query.Where(ow => ow.WorkType.ClassifierCode.Contains(classifierCodeFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Пагинация
            int totalItems = query.Count();
            var objectWorks = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Передача данных во ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.ObjectNameFilter = objectNameFilter;
            ViewBag.ClassifierCodeFilter = classifierCodeFilter;

            return View(objectWorks);
        }


        // GET: ObjectWorks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objectWork = await _context.ObjectWorks
                .Include(o => o.Object)
                .Include(o => o.WorkType)
                .FirstOrDefaultAsync(m => m.ObjectWorkId == id);
            if (objectWork == null)
            {
                return NotFound();
            }

            return View(objectWork);
        }

        // GET: ObjectWorks/Create
        public IActionResult Create()
        {
            ViewData["ObjectId"] = new SelectList(_context.ConstructionObjects, "ObjectId", "ObjectName");
            ViewData["WorkTypeId"] = new SelectList(_context.WorkTypes, "WorkTypeId", "ClassifierCode");
            return View();
        }

        // POST: ObjectWorks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ObjectWorkId,ObjectId,WorkTypeId")] ObjectWork objectWork)
        {
            if (ModelState.IsValid)
            {
                _context.Add(objectWork);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ObjectId"] = new SelectList(_context.ConstructionObjects, "ObjectId", "ObjectId", objectWork.ObjectId);
            ViewData["WorkTypeId"] = new SelectList(_context.WorkTypes, "WorkTypeId", "WorkTypeId", objectWork.WorkTypeId);
            return View(objectWork);
        }

        // GET: ObjectWorks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objectWork = await _context.ObjectWorks.FindAsync(id);
            if (objectWork == null)
            {
                return NotFound();
            }

            ViewData["ObjectId"] = new SelectList(_context.ConstructionObjects, "ObjectId", "ObjectName", objectWork.ObjectId);
            ViewData["WorkTypeId"] = new SelectList(_context.WorkTypes, "WorkTypeId", "ClassifierCode", objectWork.WorkTypeId);
            return View(objectWork);
        }

        // POST: ObjectWorks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ObjectWorkId,ObjectId,WorkTypeId")] ObjectWork objectWork)
        {
            if (id != objectWork.ObjectWorkId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(objectWork);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObjectWorkExists(objectWork.ObjectWorkId))
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
            ViewData["ObjectId"] = new SelectList(_context.ConstructionObjects, "ObjectId", "ObjectId", objectWork.ObjectId);
            ViewData["WorkTypeId"] = new SelectList(_context.WorkTypes, "WorkTypeId", "WorkTypeId", objectWork.WorkTypeId);
            return View(objectWork);
        }

        // GET: ObjectWorks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objectWork = await _context.ObjectWorks
                .Include(o => o.Object)
                .Include(o => o.WorkType)
                .FirstOrDefaultAsync(m => m.ObjectWorkId == id);
            if (objectWork == null)
            {
                return NotFound();
            }

            return View(objectWork);
        }

        // POST: ObjectWorks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objectWork = await _context.ObjectWorks.FindAsync(id);
            if (objectWork != null)
            {
                _context.ObjectWorks.Remove(objectWork);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObjectWorkExists(int id)
        {
            return _context.ObjectWorks.Any(e => e.ObjectWorkId == id);
        }
    }
}
