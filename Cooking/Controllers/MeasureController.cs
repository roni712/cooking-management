using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cooking.Models;
using Microsoft.AspNetCore.Http;

namespace Cooking.Controllers
{
    public class MeasureController : Controller
    {
        private readonly CookingContext _context;

        public MeasureController(CookingContext context)
        {
            _context = context;
        }

        // GET: Measure
        public async Task<IActionResult> Index(string categoryID)
        {
            if (!string.IsNullOrEmpty(categoryID))
            {
                Response.Cookies.Append("CategoryID", categoryID);
                HttpContext.Session.SetString("CategoryID", categoryID);
            }
            else if (Request.Query["categoryID"].Any())
            {
                Response.Cookies.Append("BusRouteCode", categoryID);
                HttpContext.Session.SetString("BusRouteCode", categoryID);
                categoryID = Request.Query["categoryID"];
            }
            else if (Request.Cookies["CategoryID"] != null)
            {
                categoryID = Request.Cookies["CategoryID"].ToString();
            }
            else if (HttpContext.Session.GetString("CategoryID") != null)
            {
                categoryID = HttpContext.Session.GetString("CategoryID");
            }
            else
            {
            
                TempData["message"] = "please select meaurement";
                return RedirectToAction("Index", "Category");
            }
          
            var category = _context.Category.Where(m => m.CategoryCode == categoryID).FirstOrDefault();
            ViewData["catName"] = category.Name;
            ViewData["catmes"] = categoryID;
            TempData["suc"] = "data added successfully";
            var measureContext = _context.Measure
              .Where(m => m.CategoryCode == categoryID)
              .OrderBy(m => m.Name);
            
            return View(await measureContext.ToListAsync());
        }

        // GET: Measure/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measure = await _context.Measure
                .FirstOrDefaultAsync(m => m.MeasureCode == id);
            if (measure == null)
            {
                return NotFound();
            }

            return View(measure);
        }

        // GET: Measure/Create
        public IActionResult Create()
        {
             var categoryID = Request.Cookies["CategoryID"].ToString();
            var catname = _context.Category.Where(m => m.CategoryCode == categoryID).FirstOrDefault();
            ViewData["Cname"] = catname.Name ;
            return View();
        }

        // POST: Measure/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MeasureCode,Name,CategoryCode,RatioToBase")] Measure measure)
        {
           
            var cookie = Request.Cookies["CategoryID"].ToString();
            measure.CategoryCode = cookie;
            if (ModelState.IsValid && measure.RatioToBase > 1.0)
            {
                _context.Add(measure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
           
            return View(measure);
        }

        // GET: Measure/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measure = await _context.Measure.FindAsync(id);
            if (measure == null)
            {
                return NotFound();
            }
            return View(measure);
        }

        // POST: Measure/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MeasureCode,Name,CategoryCode,RatioToBase")] Measure measure)
        {
            if (id != measure.MeasureCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(measure);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeasureExists(measure.MeasureCode))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(measure);
        }

        // GET: Measure/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measure = await _context.Measure
                .FirstOrDefaultAsync(m => m.MeasureCode == id);
            if (measure == null)
            {
                return NotFound();
            }

            return View(measure);
        }

        // POST: Measure/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var measure = await _context.Measure.FindAsync(id);
            _context.Measure.Remove(measure);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MeasureExists(string id)
        {
            return _context.Measure.Any(e => e.MeasureCode == id);
        }
    }
}
