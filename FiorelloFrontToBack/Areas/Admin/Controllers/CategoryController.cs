using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiorelloFrontToBack.DAL;
using FiorelloFrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloFrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Categories.Where(c=>c.HasDeleted==false).ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) return NotFound();
            bool isExist = _context.Categories.Where(c=>c.HasDeleted==false).Any(c => c.Name.ToLower() == category.Name.ToLower());
            if (isExist)
            {
                ModelState.AddModelError("Name", "This name already exist!");
                return View();
            }
            category.HasDeleted = false;
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            Category category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Category category = await _context.Categories.FindAsync(id);                  
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null) return NotFound();
            Category category = _context.Categories
                                    .Where(c => c.HasDeleted == false)
                                      .Include(c => c.Products)
                                       .FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            category.HasDeleted = true;
            category.DeletedTime = DateTime.Now;
            foreach (Product pro in category.Products)
            {
                pro.HasDeleted = true;
                pro.DeletedTime = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async  Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();
            Category category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Category category)
        {
            if (!ModelState.IsValid) return View();

            if (id == null) return NotFound();
            Category categoryFromDb = await _context.Categories.FindAsync(id);
            if (categoryFromDb == null) return NotFound();
            Category categoryRepeated = _context.Categories
                                          .Where(c => c.HasDeleted == false)
                                             .FirstOrDefault(c => c.Name.ToLower() == category.Name.ToLower());
            if (categoryRepeated != null)
            {
                ModelState.AddModelError("Name", "Bele adda kategoriya movcuddu");
                return View();
            }
            categoryFromDb.Name = category.Name;
            categoryFromDb.Description = category.Description;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
