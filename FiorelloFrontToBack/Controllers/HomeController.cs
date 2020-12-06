using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiorelloFrontToBack.DAL;
using FiorelloFrontToBack.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloFrontToBack.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            HttpContext.Session.SetString("name", "Mustafa");
            Response.Cookies.Append("surname", "Ahmadov", new CookieOptions {
                MaxAge = TimeSpan.FromSeconds(40),
            }); 
            HomeVM homeVM = new HomeVM
            {
                Sliders = _context.Sliders.ToList(),
                SliderContent = _context.SliderContents.FirstOrDefault(),
                Categories = _context.Categories.Where(c => c.HasDeleted == false).ToList(),
                Products = _context.Products.Include(p => p.Category).Where(p => p.HasDeleted == false).Take(8).ToList(),
                About = _context.Abouts.FirstOrDefault(),
                AboutInfos = _context.AboutInfo.ToList(),
                Experts = _context.Experts.Include(e => e.Profession).ToList(),
                Blogs = _context.Blogs.ToList()
            };
            return View(homeVM);
        }
        public IActionResult Basket()
        {
            string cookie = Request.Cookies["surname"];
            string session = HttpContext.Session.GetString("name");
            return Content($"{session} {cookie}");
        }
    }
}
