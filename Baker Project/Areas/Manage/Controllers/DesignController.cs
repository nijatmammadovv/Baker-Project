using Baker_Project.Data_Access_Layer;
using Baker_Project.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Baker_Project.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class DesignController : Controller
    {
        private AppDbContext _context { get; }
        private IWebHostEnvironment _env { get; }
        public DesignController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Designs.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Design design)
        {
            if (!ModelState.IsValid) return View(design);
            if (design.ImageUrl != null)
            {
                if (design.ImageUrl.ContentType != "image/jpeg" && design.ImageUrl.ContentType != "image/png" && design.ImageUrl.ContentType != "image/webp")
                {
                    ModelState.AddModelError("ImageUrL", "Sekil .png,.jpeg veya .webp olmalidir");
                    return View(design);
                }
                if (design.ImageUrl.Length / 1024 > 3000)
                {
                    ModelState.AddModelError("ImageUrl", "Sekilin olcusu 3mbdan cox ola bilmez");
                    return View(design);
                }
                string filename = design.ImageUrl.FileName;
                if (filename.Length > 64)
                {
                    filename.Substring(filename.Length - 64, 64);
                }
                string newfileName = Guid.NewGuid().ToString() + filename;
                string path = Path.Combine(_env.WebRootPath, "assets", "images", newfileName);
                using (FileStream fs = new FileStream(path,FileMode.Create))
                {
                    design.ImageUrl.CopyTo(fs);
                }
                design.Image = newfileName;
                _context.Designs.Add(design);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public IActionResult Delete(int id)
        {
            Design design = _context.Designs.Find(id);
            _context.Designs.Remove(design);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            Design design = _context.Designs.FirstOrDefault(d => d.Id == id);
            if (design == null) return NotFound();
            return View(design);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Design design)
        {
            Design design1 = _context.Designs.FirstOrDefault(d => d.Id == design.Id);
            design1.Name = design.Name;
            design1.Description = design.Description;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
