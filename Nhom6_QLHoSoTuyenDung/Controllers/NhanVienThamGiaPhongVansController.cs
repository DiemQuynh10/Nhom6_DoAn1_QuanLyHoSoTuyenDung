using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Entities;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{
    public class NhanVienThamGiaPhongVansController : Controller
    {
        private readonly AppDbContext _context;

        public NhanVienThamGiaPhongVansController(AppDbContext context)
        {
            _context = context;
        }

        // GET: NhanVienThamGiaPhongVans
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.NhanVienThamGiaPhongVans.Include(n => n.LichPhongVan).Include(n => n.NhanVien);
            return View(await appDbContext.ToListAsync());
        }

        // GET: NhanVienThamGiaPhongVans/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVienThamGiaPhongVan = await _context.NhanVienThamGiaPhongVans
                .Include(n => n.LichPhongVan)
                .Include(n => n.NhanVien)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nhanVienThamGiaPhongVan == null)
            {
                return NotFound();
            }

            return View(nhanVienThamGiaPhongVan);
        }

        // GET: NhanVienThamGiaPhongVans/Create
        public IActionResult Create()
        {
            ViewData["LichPhongVanId"] = new SelectList(_context.LichPhongVans, "Id", "Id");
            ViewData["NhanVienId"] = new SelectList(_context.NhanViens, "MaNhanVien", "MaNhanVien");
            return View();
        }

        // POST: NhanVienThamGiaPhongVans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LichPhongVanId,NhanVienId,VaiTro")] NhanVienThamGiaPhongVan nhanVienThamGiaPhongVan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhanVienThamGiaPhongVan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LichPhongVanId"] = new SelectList(_context.LichPhongVans, "Id", "Id", nhanVienThamGiaPhongVan.LichPhongVanId);
            ViewData["NhanVienId"] = new SelectList(_context.NhanViens, "MaNhanVien", "MaNhanVien", nhanVienThamGiaPhongVan.NhanVienId);
            return View(nhanVienThamGiaPhongVan);
        }

        // GET: NhanVienThamGiaPhongVans/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVienThamGiaPhongVan = await _context.NhanVienThamGiaPhongVans.FindAsync(id);
            if (nhanVienThamGiaPhongVan == null)
            {
                return NotFound();
            }
            ViewData["LichPhongVanId"] = new SelectList(_context.LichPhongVans, "Id", "Id", nhanVienThamGiaPhongVan.LichPhongVanId);
            ViewData["NhanVienId"] = new SelectList(_context.NhanViens, "MaNhanVien", "MaNhanVien", nhanVienThamGiaPhongVan.NhanVienId);
            return View(nhanVienThamGiaPhongVan);
        }

        // POST: NhanVienThamGiaPhongVans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,LichPhongVanId,NhanVienId,VaiTro")] NhanVienThamGiaPhongVan nhanVienThamGiaPhongVan)
        {
            if (id != nhanVienThamGiaPhongVan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhanVienThamGiaPhongVan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienThamGiaPhongVanExists(nhanVienThamGiaPhongVan.Id))
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
            ViewData["LichPhongVanId"] = new SelectList(_context.LichPhongVans, "Id", "Id", nhanVienThamGiaPhongVan.LichPhongVanId);
            ViewData["NhanVienId"] = new SelectList(_context.NhanViens, "MaNhanVien", "MaNhanVien", nhanVienThamGiaPhongVan.NhanVienId);
            return View(nhanVienThamGiaPhongVan);
        }

        // GET: NhanVienThamGiaPhongVans/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVienThamGiaPhongVan = await _context.NhanVienThamGiaPhongVans
                .Include(n => n.LichPhongVan)
                .Include(n => n.NhanVien)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nhanVienThamGiaPhongVan == null)
            {
                return NotFound();
            }

            return View(nhanVienThamGiaPhongVan);
        }

        // POST: NhanVienThamGiaPhongVans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nhanVienThamGiaPhongVan = await _context.NhanVienThamGiaPhongVans.FindAsync(id);
            if (nhanVienThamGiaPhongVan != null)
            {
                _context.NhanVienThamGiaPhongVans.Remove(nhanVienThamGiaPhongVan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanVienThamGiaPhongVanExists(string id)
        {
            return _context.NhanVienThamGiaPhongVans.Any(e => e.Id == id);
        }
    }
}
