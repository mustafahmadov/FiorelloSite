using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FiorelloFrontToBack.DAL;
using FiorelloFrontToBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloFrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Sliders.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (slider.Photos == null) return View();
            foreach (IFormFile photo in slider.Photos)
            {
                if (photo == null)
                {
                    ModelState.AddModelError("Photos", "Zehmet olmasa shekil daxil edin");
                    return View();
                }
                if (!photo.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("Photos", "Fayl shekil tipinde olmalidir");
                    return View();
                }
                if (photo.Length / 1024 > 200)
                {
                    ModelState.AddModelError("Photos", "Maksimum olcu 200kb ola biler");
                    return View();
                }
                string fileName = Guid.NewGuid().ToString() + photo.FileName;
                string path = Path.Combine(_env.WebRootPath, "img", fileName);

                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }
                Slider newSlider = new Slider();
                newSlider.Image = fileName;
                await _context.Sliders.AddAsync(newSlider);
                
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //public async Task<IActionResult> Create(Slider slider)
        //{
        //    if (slider.Photo == null)
        //    {
        //        ModelState.AddModelError("Photo", "Zehmet olmasa shekil daxil edin");
        //        return View();
        //    }
        //    if (!slider.Photo.ContentType.Contains("image/"))
        //    {
        //        ModelState.AddModelError("Photo", "Fayl shekil tipinde olmalidir");
        //        return View();
        //    }
        //    if (slider.Photo.Length / 1024 > 200)
        //    {
        //        ModelState.AddModelError("Photo", "Maksimum olcu 200kb ola biler");
        //        return View();
        //    }
        //    string fileName = Guid.NewGuid().ToString() + slider.Photo.FileName;
        //    string path = Path.Combine(_env.WebRootPath, "img", fileName);

        //    using (FileStream fileStream = new FileStream(path, FileMode.Create))
        //    {
        //        await slider.Photo.CopyToAsync(fileStream);
        //    }
        //    slider.Image = fileName;
        //    await _context.Sliders.AddAsync(slider);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Slider slider = await _context.Sliders.FindAsync(id);
            if (slider == null) return NotFound();
            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null) return NotFound();
            Slider slider = await _context.Sliders.FindAsync(id);
            if (slider == null) return NotFound();
            string path = Path.Combine(_env.WebRootPath, "img", slider.Image);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();
            Slider slider = await _context.Sliders.FindAsync(id);
            if (slider == null) return NotFound();
            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Slider slider)
        {
            if (id == null) return NotFound();
            if (slider.Photo != null)
            {
                if (!slider.Photo.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("Photo", "Zehmet olmasa shekil daxil edin");
                    return View();
                }
                if (slider.Photo.Length / 1024 > 200)
                {
                    ModelState.AddModelError("Photo", "Shekilin olcusu 200kbdan az olmalidir");
                    return View();
                }
                Slider dbSlider = await _context.Sliders.FindAsync(id);
                if (dbSlider == null) return NotFound();
                string path = Path.Combine(_env.WebRootPath, "img", dbSlider.Image);
                string fileName = Guid.NewGuid().ToString() + slider.Photo.FileName;
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                string newPath = Path.Combine(_env.WebRootPath, "img", fileName);

                using (FileStream fileStream = new FileStream(newPath, FileMode.Create))
                {
                    await slider.Photo.CopyToAsync(fileStream);
                }
                dbSlider.Image = fileName;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
