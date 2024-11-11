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
    public class ObjectMaterialsController : Controller
    {
        private readonly ProdajnikContext _context;

        public ObjectMaterialsController(ProdajnikContext context)
        {
            _context = context;
        }

        // GET: ObjectMaterials
        public async Task<IActionResult> Index()
        {
            var prodajnikContext = _context.ObjectMaterials.Include(o => o.Material).Include(o => o.Object);
            return View(await prodajnikContext.ToListAsync());
        }

    }
}
