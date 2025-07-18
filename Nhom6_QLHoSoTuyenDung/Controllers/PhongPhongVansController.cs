using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.HR}")]
    public class PhongPhongVansController : Controller
    {
        private readonly AppDbContext _context;

        public PhongPhongVansController(AppDbContext context)
        {
            _context = context;
        }

        // GET: PhongPhongVans
        public async Task<IActionResult> Index()
        {
            return View(await _context.PhongPhongVans.ToListAsync());
        }

        // GET: PhongPhongVans/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phongPhongVan = await _context.PhongPhongVans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (phongPhongVan == null)
            {
                return NotFound();
            }

            return View(phongPhongVan);
        }

        // GET: PhongPhongVans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PhongPhongVans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TenPhong,DiaDiem,MoTa")] PhongPhongVan phongPhongVan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phongPhongVan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(phongPhongVan);
        }

        // GET: PhongPhongVans/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phongPhongVan = await _context.PhongPhongVans.FindAsync(id);
            if (phongPhongVan == null)
            {
                return NotFound();
            }
            return View(phongPhongVan);
        }

        // POST: PhongPhongVans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,TenPhong,DiaDiem,MoTa")] PhongPhongVan phongPhongVan)
        {
            if (id != phongPhongVan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phongPhongVan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhongPhongVanExists(phongPhongVan.Id))
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
            return View(phongPhongVan);
        }

        // GET: PhongPhongVans/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phongPhongVan = await _context.PhongPhongVans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (phongPhongVan == null)
            {
                return NotFound();
            }

            return View(phongPhongVan);
        }

        // POST: PhongPhongVans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var phongPhongVan = await _context.PhongPhongVans.FindAsync(id);
            if (phongPhongVan != null)
            {
                _context.PhongPhongVans.Remove(phongPhongVan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhongPhongVanExists(string id)
        {
            return _context.PhongPhongVans.Any(e => e.Id == id);
        }
    }
}
