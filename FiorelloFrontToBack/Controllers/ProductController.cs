using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiorelloFrontToBack.DAL;
using FiorelloFrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloFrontToBack.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Product> model = _context.Products
                                            .Where(p => p.HasDeleted == false)
                                              .Take(8)
                                                .ToList();
            ViewBag.PrCount = _context.Products.Count();
            return View(model);
        }
        public IActionResult GetProducts(int skip)
        {
            List<Product> model = _context.Products
                                             .Where(p=>p.HasDeleted==false)
                                              .Skip(skip)
                                                .Take(8)
                                                  .ToList();
            return PartialView("_ProductPartial",model);
        } 
    }
}
