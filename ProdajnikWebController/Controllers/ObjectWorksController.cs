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
    public class ObjectWorksController : Controller
    {
        private readonly ProdajnikContext _context;

        public ObjectWorksController(ProdajnikContext context)
        {
            _context = context;
        }

        // GET: ObjectWorks
        public async Task<IActionResult> Index()
        {
            var prodajnikContext = _context.ObjectWorks.Include(o => o.Object).Include(o => o.WorkType);
            return View(await prodajnikContext.ToListAsync());
        }

        
    }
}
