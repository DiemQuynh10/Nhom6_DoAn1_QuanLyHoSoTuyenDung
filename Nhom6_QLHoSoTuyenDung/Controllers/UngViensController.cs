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
    public class UngViensController : Controller
    {
        private readonly AppDbContext _context;

        public UngViensController(AppDbContext context)
        {
            _context = context;
        }

        // GET: UngViens
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.UngViens.Include(u => u.ViTriUngTuyen);
            return View(await appDbContext.ToListAsync());
        }

        // GET: UngViens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ungVien = await _context.UngViens               
                .Include(u => u.ViTriUngTuyen)
                .FirstOrDefaultAsync(m => m.MaUngVien == id);
            if (ungVien == null)
            {
                return NotFound();
            }

            return View(ungVien);
        }

        // GET: UngViens/Create
        public IActionResult Create()
        {
            ViewData["ViTriUngTuyenId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri");
            return View();
        }

        // POST: UngViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaUngVien,HoTen,NgaySinh,Email,SoDienThoai,ViTriUngTuyenId,LinkCV,KinhNghiem,ThanhTich,MoTa,TrangThai,NgayNop,NguonUngTuyen")] UngVien ungVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ungVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ViTriUngTuyenId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri", ungVien.ViTriUngTuyenId);
            return View(ungVien);
        }

        // GET: UngViens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ungVien = await _context.UngViens.FindAsync(id);
            if (ungVien == null)
            {
                return NotFound();
            }
            ViewData["ViTriUngTuyenId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri", ungVien.ViTriUngTuyenId);
            return View(ungVien);
        }

        // POST: UngViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaUngVien,HoTen,NgaySinh,Email,SoDienThoai,ViTriUngTuyenId,LinkCV,KinhNghiem,ThanhTich,MoTa,TrangThai,NgayNop,NguonUngTuyen")] UngVien ungVien)
        {
            if (id != ungVien.MaUngVien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ungVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UngVienExists(ungVien.MaUngVien))
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
            ViewData["ViTriUngTuyenId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri", ungVien.ViTriUngTuyenId);
            return View(ungVien);
        }

        // GET: UngViens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ungVien = await _context.UngViens
                .Include(u => u.ViTriUngTuyen)
                .FirstOrDefaultAsync(m => m.MaUngVien == id);
            if (ungVien == null)
            {
                return NotFound();
            }

            return View(ungVien);
        }

        // POST: UngViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var ungVien = await _context.UngViens.FindAsync(id);
            if (ungVien != null)
            {
                _context.UngViens.Remove(ungVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UngVienExists(string id)
        {
            return _context.UngViens.Any(e => e.MaUngVien == id);
        }
    }
}
