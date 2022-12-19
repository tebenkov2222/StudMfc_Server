using Microsoft.AspNetCore.Mvc;

namespace ISTU_MFC.Controllers
{
    public class SharedController: Controller
    {
        public IActionResult Error()
        {
            return View();
        }
    }
}