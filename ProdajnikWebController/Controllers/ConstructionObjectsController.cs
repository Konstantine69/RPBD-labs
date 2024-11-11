using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProdajnikWebController.Data;
using ProdajnikWebController.Models;

namespace ProdajnikWebController.Controllers
{
    public class ConstructionObjectsController : Controller
    {
        private readonly ProdajnikContext _context;

        public ConstructionObjectsController(ProdajnikContext context)
        {
            _context = context;
        }

        // GET: ConstructionObjects
        public async Task<IActionResult> Index()
        {
            var prodajnikContext = _context.ConstructionObjects.Include(c => c.Customer);
            return View(await prodajnikContext.ToListAsync());
        }

    }
}
