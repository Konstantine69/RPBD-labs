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
    public class CustomersController : Controller
    {
        private readonly ProdajnikContext _context;
        private readonly CachedDataService _cachedDataService;

        public CustomersController(ProdajnikContext context, CachedDataService cachedDataService)
        {
            _context = context;
            _cachedDataService = cachedDataService;

        }

        // GET: Customers
        public async Task<IActionResult> Index(string organizationNameFilter, string cityFilter, int page = 1, int pageSize = 20)
        {
            var modelsQuery = _cachedDataService.GetCustomers(); // Начальный запрос к базе данных

            // Фильтрация
            if (!string.IsNullOrEmpty(organizationNameFilter))
            {
                modelsQuery = modelsQuery.Where(c => c.OrganizationName.Contains(organizationNameFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(cityFilter))
            {
                modelsQuery = modelsQuery.Where(c => c.City.Contains(cityFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Пагинация
            int totalItems = modelsQuery.Count(); // Общее количество записей
            var customers = modelsQuery
                .Skip((page - 1) * pageSize) // Пропускаем записи для предыдущих страниц
                .Take(pageSize) // Берем только записи текущей страницы
                .ToList();

            // Передаем данные во ViewBag для сохранения фильтров и пагинации
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.OrganizationNameFilter = organizationNameFilter;
            ViewBag.CityFilter = cityFilter;

            return View(customers);
        }


        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,OrganizationName,City,Address,PhoneNumber")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,OrganizationName,City,Address,PhoneNumber")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
