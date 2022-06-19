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
    public class HomeProductController : Controller
    {
        private AppDbContext _context { get; }
        private IWebHostEnvironment _env { get; }
        public HomeProductController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.HomePageProducts.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HomePageProducts homePageProduct)
        {
            if (!ModelState.IsValid) return View(homePageProduct);
            if(homePageProduct.ImageUrl != null)
            {
                if(homePageProduct.ImageUrl.ContentType != "image/jpeg" && homePageProduct.ImageUrl.ContentType != "image/png" && homePageProduct.ImageUrl.ContentType != "image/webp")
                {
                    ModelState.AddModelError("ImageUrl", "Şəkil yalnız .png,.jpeg və ya .webp ola bilər!");
                    return View(homePageProduct);
                }
                if(homePageProduct.ImageUrl.Length /1024 > 3000)
                {
                    ModelState.AddModelError("ImageUrl", "Seklin olcusu 3mb-dan cox ola bilmez");
                    return View(homePageProduct);
                }

                string filename = homePageProduct.ImageUrl.FileName;
                if(filename.Length > 64)
                {
                    filename.Substring(filename.Length - 64, 64);
                }
                string newfileName = Guid.NewGuid().ToString() + filename;
                string path = Path.Combine(_env.WebRootPath, "assets", "images", newfileName);
                using(FileStream fs=new FileStream(path, FileMode.Create))
                {
                    await homePageProduct.ImageUrl.CopyToAsync(fs);
                }
                homePageProduct.Image = newfileName;
                await _context.HomePageProducts.AddAsync(homePageProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int id)
        {
            HomePageProducts homePageProducts = _context.HomePageProducts.Find(id);
            _context.HomePageProducts.Remove(homePageProducts);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            HomePageProducts homePageProducts = _context.HomePageProducts.FirstOrDefault(p => p.Id == id);
            if (homePageProducts == null) return NotFound();
            return View(homePageProducts);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HomePageProducts homePageProducts)
        {
            HomePageProducts homePageProducts1 = _context.HomePageProducts.FirstOrDefault(p => p.Id == homePageProducts.Id);
            if (homePageProducts1 == null) return NotFound();
            homePageProducts1.Name = homePageProducts.Name;
            homePageProducts1.Description = homePageProducts.Description;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
