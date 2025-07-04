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
    public class ViTriTuyenDungsController : Controller
    {
        private readonly AppDbContext _context;

        public ViTriTuyenDungsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ViTriTuyenDungs
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.ViTriTuyenDungs.Include(v => v.PhongBan);
            return View(await appDbContext.ToListAsync());
        }

        // GET: ViTriTuyenDungs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viTriTuyenDung = await _context.ViTriTuyenDungs
                .Include(v => v.PhongBan)
                .FirstOrDefaultAsync(m => m.MaViTri == id);
            if (viTriTuyenDung == null)
            {
                return NotFound();
            }

            return View(viTriTuyenDung);
        }

        // GET: ViTriTuyenDungs/Create
        public IActionResult Create()
        {
            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "Id");
            return View();
        }

        // POST: ViTriTuyenDungs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaViTri,TenViTri,SoLuongCanTuyen,TrangThai,PhongBanId,KyNang,NgayTao")] ViTriTuyenDung viTriTuyenDung)
        {
            if (ModelState.IsValid)
            {
                _context.Add(viTriTuyenDung);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "Id", viTriTuyenDung.PhongBanId);
            return View(viTriTuyenDung);
        }

        // GET: ViTriTuyenDungs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viTriTuyenDung = await _context.ViTriTuyenDungs.FindAsync(id);
            if (viTriTuyenDung == null)
            {
                return NotFound();
            }
            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "Id", viTriTuyenDung.PhongBanId);
            return View(viTriTuyenDung);
        }

        // POST: ViTriTuyenDungs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaViTri,TenViTri,SoLuongCanTuyen,TrangThai,PhongBanId,KyNang,NgayTao")] ViTriTuyenDung viTriTuyenDung)
        {
            if (id != viTriTuyenDung.MaViTri)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viTriTuyenDung);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViTriTuyenDungExists(viTriTuyenDung.MaViTri))
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
            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "Id", viTriTuyenDung.PhongBanId);
            return View(viTriTuyenDung);
        }

        // GET: ViTriTuyenDungs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viTriTuyenDung = await _context.ViTriTuyenDungs
                .Include(v => v.PhongBan)
                .FirstOrDefaultAsync(m => m.MaViTri == id);
            if (viTriTuyenDung == null)
            {
                return NotFound();
            }

            return View(viTriTuyenDung);
        }

        // POST: ViTriTuyenDungs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var viTriTuyenDung = await _context.ViTriTuyenDungs.FindAsync(id);
            if (viTriTuyenDung != null)
            {
                _context.ViTriTuyenDungs.Remove(viTriTuyenDung);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ViTriTuyenDungExists(string id)
        {
            return _context.ViTriTuyenDungs.Any(e => e.MaViTri == id);
        }
    }
}
