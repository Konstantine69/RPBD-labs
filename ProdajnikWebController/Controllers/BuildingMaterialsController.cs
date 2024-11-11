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
    public class BuildingMaterialsController : Controller
    {
        private readonly ProdajnikContext _context;

        public BuildingMaterialsController(ProdajnikContext context)
        {
            _context = context;
        }

        // GET: BuildingMaterials
        public async Task<IActionResult> Index()
        {
            return View(await _context.BuildingMaterials.ToListAsync());
        }

        
    }
}
