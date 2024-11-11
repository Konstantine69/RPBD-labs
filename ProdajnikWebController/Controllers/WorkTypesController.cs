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
    public class WorkTypesController : Controller
    {
        private readonly ProdajnikContext _context;

        public WorkTypesController(ProdajnikContext context)
        {
            _context = context;
        }

        // GET: WorkTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.WorkTypes.ToListAsync());
        }
        
    }
}
