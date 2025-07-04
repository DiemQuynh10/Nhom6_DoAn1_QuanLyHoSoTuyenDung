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
    public class HoSoLuuTrusController : Controller
    {
        private readonly AppDbContext _context;

        public HoSoLuuTrusController(AppDbContext context)
        {
            _context = context;
        }

        // GET: HoSoLuuTrus
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.HoSoLuuTrus.Include(h => h.UngVien).Include(h => h.ViTriTuyenDung);
            return View(await appDbContext.ToListAsync());
        }

        // GET: HoSoLuuTrus/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoSoLuuTru = await _context.HoSoLuuTrus
                .Include(h => h.UngVien)
                .Include(h => h.ViTriTuyenDung)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hoSoLuuTru == null)
            {
                return NotFound();
            }

            return View(hoSoLuuTru);
        }

        // GET: HoSoLuuTrus/Create
        public IActionResult Create()
        {
            ViewData["UngVienId"] = new SelectList(_context.UngViens, "MaUngVien", "MaUngVien");
            ViewData["ViTriId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri");
            return View();
        }

        // POST: HoSoLuuTrus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UngVienId,ViTriId,LyDoLuuTru,NgayLuu")] HoSoLuuTru hoSoLuuTru)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoSoLuuTru);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UngVienId"] = new SelectList(_context.UngViens, "MaUngVien", "MaUngVien", hoSoLuuTru.UngVienId);
            ViewData["ViTriId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri", hoSoLuuTru.ViTriId);
            return View(hoSoLuuTru);
        }

        // GET: HoSoLuuTrus/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoSoLuuTru = await _context.HoSoLuuTrus.FindAsync(id);
            if (hoSoLuuTru == null)
            {
                return NotFound();
            }
            ViewData["UngVienId"] = new SelectList(_context.UngViens, "MaUngVien", "MaUngVien", hoSoLuuTru.UngVienId);
            ViewData["ViTriId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri", hoSoLuuTru.ViTriId);
            return View(hoSoLuuTru);
        }

        // POST: HoSoLuuTrus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UngVienId,ViTriId,LyDoLuuTru,NgayLuu")] HoSoLuuTru hoSoLuuTru)
        {
            if (id != hoSoLuuTru.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoSoLuuTru);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoSoLuuTruExists(hoSoLuuTru.Id))
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
            ViewData["UngVienId"] = new SelectList(_context.UngViens, "MaUngVien", "MaUngVien", hoSoLuuTru.UngVienId);
            ViewData["ViTriId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri", hoSoLuuTru.ViTriId);
            return View(hoSoLuuTru);
        }

        // GET: HoSoLuuTrus/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoSoLuuTru = await _context.HoSoLuuTrus
                .Include(h => h.UngVien)
                .Include(h => h.ViTriTuyenDung)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hoSoLuuTru == null)
            {
                return NotFound();
            }

            return View(hoSoLuuTru);
        }

        // POST: HoSoLuuTrus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hoSoLuuTru = await _context.HoSoLuuTrus.FindAsync(id);
            if (hoSoLuuTru != null)
            {
                _context.HoSoLuuTrus.Remove(hoSoLuuTru);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoSoLuuTruExists(string id)
        {
            return _context.HoSoLuuTrus.Any(e => e.Id == id);
        }
    }
}
