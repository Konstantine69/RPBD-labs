using Microsoft.AspNetCore.Mvc;

namespace Lab6.Controllers
{
    public class ConstructionObjController : Controller
    {
        // Метод для отображения страницы управления автомобилями
        [HttpGet]
        public IActionResult Index()
        {
            return View();  // Возвращаем представление Index.cshtml
        }
    }
}
