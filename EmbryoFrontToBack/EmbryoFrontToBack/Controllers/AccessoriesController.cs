using EmbryoFrontToBack.Data;
using EmbryoFrontToBack.Models;
using EmbryoFrontToBack.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmbryoFrontToBack.Controllers
{
    public class AccessoriesController : Controller
    {
        private readonly AppDbContext _context;

        public AccessoriesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            //HttpContext.Session.SetString("name", "Sadiq");
            //Response.Cookies.Append("surname", "Nahmetov", new CookieOptions { MaxAge = TimeSpan.FromDays(5)});
              
            ViewBag.count = await _context.Accessories.Where(m=>!m.IsDeleted).CountAsync();

            IEnumerable<Accessories> accessories = await _context.Accessories.Where(m=>!m.IsDeleted).Take(3).OrderBy(m=>m.Id).ToListAsync();

            AccessoriesVM accessoriesVM = new AccessoriesVM
            {
               Accessories = accessories
            };
            return View(accessoriesVM);
        }

        public async Task<IActionResult> LoadMore(int skip)
        {
            IEnumerable<Accessories> accessories = await _context.Accessories.Where(m=>!m.IsDeleted).Skip(skip).Take(3).ToListAsync();
            return PartialView("_ProductPartial", accessories);
        }

        //public IActionResult Test()
        //{
        //    var sessionData = HttpContext.Session.GetString("name");
        //    var cookieData = Request.Cookies["surname"];
        //    return Json(sessionData + "-" + cookieData);
        //}


        [HttpPost]
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            //var dbAccesories = await _context.Accessories.FirstOrDefaultAsync(m =>m.Id == id);
            
            var dbAccesories = await _context.Accessories.FindAsync(id);
         
            if (dbAccesories == null)
            {
                return NotFound();
            }

            List<BasketVM> basket;

            if (Request.Cookies["basket"]!= null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            }
            else
            {
                basket = new List<BasketVM>();
            }

            BasketVM existProduct = basket.FirstOrDefault(m => m.Id == dbAccesories.Id);

            if (existProduct == null)
            {
                basket.Add(new BasketVM
                {
                    Id = dbAccesories.Id,
                    Count = 1
                }); ;
            }
            else
            {
                existProduct.Count++;
            }

          

            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
            return RedirectToAction("Index");
        }
    }
}
