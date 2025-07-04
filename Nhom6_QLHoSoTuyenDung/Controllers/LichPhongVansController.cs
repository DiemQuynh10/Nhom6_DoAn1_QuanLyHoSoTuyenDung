using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{
    public class LichPhongVansController : Controller
    {
        private readonly AppDbContext _context;

        public LichPhongVansController(AppDbContext context)
        {
            _context = context;
        }

        // GET: LichPhongVans
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.LichPhongVans.Include(l => l.PhongPhongVan).Include(l => l.UngVien).Include(l => l.ViTriTuyenDung);
            return View(await appDbContext.ToListAsync());
        }

        // GET: LichPhongVans/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichPhongVan = await _context.LichPhongVans
                .Include(l => l.PhongPhongVan)
                .Include(l => l.UngVien)
                .Include(l => l.ViTriTuyenDung)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lichPhongVan == null)
            {
                return NotFound();
            }

            return View(lichPhongVan);
        }

        // GET: LichPhongVans/Create
        public IActionResult Create()
        {
            ViewData["PhongPhongVanId"] = new SelectList(_context.Set<PhongPhongVan>(), "Id", "Id");
            ViewData["UngVienId"] = new SelectList(_context.UngViens, "MaUngVien", "MaUngVien");
            ViewData["ViTriId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri");
            return View();
        }

        // POST: LichPhongVans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PhongPhongVanId,UngVienId,ViTriId,ThoiGian,TrangThai,GhiChu")] LichPhongVan lichPhongVan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lichPhongVan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PhongPhongVanId"] = new SelectList(_context.Set<PhongPhongVan>(), "Id", "Id", lichPhongVan.PhongPhongVanId);
            ViewData["UngVienId"] = new SelectList(_context.UngViens, "MaUngVien", "MaUngVien", lichPhongVan.UngVienId);
            ViewData["ViTriId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri", lichPhongVan.ViTriId);
            return View(lichPhongVan);
        }

        // GET: LichPhongVans/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichPhongVan = await _context.LichPhongVans.FindAsync(id);
            if (lichPhongVan == null)
            {
                return NotFound();
            }
            ViewData["PhongPhongVanId"] = new SelectList(_context.Set<PhongPhongVan>(), "Id", "Id", lichPhongVan.PhongPhongVanId);
            ViewData["UngVienId"] = new SelectList(_context.UngViens, "MaUngVien", "MaUngVien", lichPhongVan.UngVienId);
            ViewData["ViTriId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri", lichPhongVan.ViTriId);
            return View(lichPhongVan);
        }

        // POST: LichPhongVans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,PhongPhongVanId,UngVienId,ViTriId,ThoiGian,TrangThai,GhiChu")] LichPhongVan lichPhongVan)
        {
            if (id != lichPhongVan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichPhongVan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichPhongVanExists(lichPhongVan.Id))
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
            ViewData["PhongPhongVanId"] = new SelectList(_context.Set<PhongPhongVan>(), "Id", "Id", lichPhongVan.PhongPhongVanId);
            ViewData["UngVienId"] = new SelectList(_context.UngViens, "MaUngVien", "MaUngVien", lichPhongVan.UngVienId);
            ViewData["ViTriId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri", lichPhongVan.ViTriId);
            return View(lichPhongVan);
        }

        // GET: LichPhongVans/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichPhongVan = await _context.LichPhongVans
                .Include(l => l.PhongPhongVan)
                .Include(l => l.UngVien)
                .Include(l => l.ViTriTuyenDung)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lichPhongVan == null)
            {
                return NotFound();
            }

            return View(lichPhongVan);
        }

        // POST: LichPhongVans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var lichPhongVan = await _context.LichPhongVans.FindAsync(id);
            if (lichPhongVan != null)
            {
                _context.LichPhongVans.Remove(lichPhongVan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LichPhongVanExists(string id)
        {
            return _context.LichPhongVans.Any(e => e.Id == id);
        }
    }
}
