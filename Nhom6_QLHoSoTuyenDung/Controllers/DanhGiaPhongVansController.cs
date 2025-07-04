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
    public class DanhGiaPhongVansController : Controller
    {
        private readonly AppDbContext _context;

        public DanhGiaPhongVansController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DanhGiaPhongVans
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.DanhGiaPhongVans.Include(d => d.LichPhongVan).Include(d => d.NhanVienDanhGia);
            return View(await appDbContext.ToListAsync());
        }

        // GET: DanhGiaPhongVans/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGiaPhongVan = await _context.DanhGiaPhongVans
                .Include(d => d.LichPhongVan)
                .Include(d => d.NhanVienDanhGia)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (danhGiaPhongVan == null)
            {
                return NotFound();
            }

            return View(danhGiaPhongVan);
        }

        // GET: DanhGiaPhongVans/Create
        public IActionResult Create()
        {
            ViewData["LichPhongVanId"] = new SelectList(_context.Set<LichPhongVan>(), "Id", "Id");
            ViewData["NhanVienDanhGiaId"] = new SelectList(_context.Set<NhanVien>(), "MaNhanVien", "MaNhanVien");
            return View();
        }

        // POST: DanhGiaPhongVans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LichPhongVanId,NhanVienDanhGiaId,DiemDanhGia,NhanXet,DeXuat")] DanhGiaPhongVan danhGiaPhongVan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(danhGiaPhongVan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LichPhongVanId"] = new SelectList(_context.Set<LichPhongVan>(), "Id", "Id", danhGiaPhongVan.LichPhongVanId);
            ViewData["NhanVienDanhGiaId"] = new SelectList(_context.Set<NhanVien>(), "MaNhanVien", "MaNhanVien", danhGiaPhongVan.NhanVienDanhGiaId);
            return View(danhGiaPhongVan);
        }

        // GET: DanhGiaPhongVans/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGiaPhongVan = await _context.DanhGiaPhongVans.FindAsync(id);
            if (danhGiaPhongVan == null)
            {
                return NotFound();
            }
            ViewData["LichPhongVanId"] = new SelectList(_context.Set<LichPhongVan>(), "Id", "Id", danhGiaPhongVan.LichPhongVanId);
            ViewData["NhanVienDanhGiaId"] = new SelectList(_context.Set<NhanVien>(), "MaNhanVien", "MaNhanVien", danhGiaPhongVan.NhanVienDanhGiaId);
            return View(danhGiaPhongVan);
        }

        // POST: DanhGiaPhongVans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,LichPhongVanId,NhanVienDanhGiaId,DiemDanhGia,NhanXet,DeXuat")] DanhGiaPhongVan danhGiaPhongVan)
        {
            if (id != danhGiaPhongVan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(danhGiaPhongVan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhGiaPhongVanExists(danhGiaPhongVan.Id))
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
            ViewData["LichPhongVanId"] = new SelectList(_context.Set<LichPhongVan>(), "Id", "Id", danhGiaPhongVan.LichPhongVanId);
            ViewData["NhanVienDanhGiaId"] = new SelectList(_context.Set<NhanVien>(), "MaNhanVien", "MaNhanVien", danhGiaPhongVan.NhanVienDanhGiaId);
            return View(danhGiaPhongVan);
        }

        // GET: DanhGiaPhongVans/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGiaPhongVan = await _context.DanhGiaPhongVans       
                .Include(d => d.LichPhongVan)
                .Include(d => d.NhanVienDanhGia)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (danhGiaPhongVan == null)
            {
                return NotFound();
            }

            return View(danhGiaPhongVan);
        }

        // POST: DanhGiaPhongVans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var danhGiaPhongVan = await _context.DanhGiaPhongVans.FindAsync(id);
            if (danhGiaPhongVan != null)
            {
                _context.DanhGiaPhongVans.Remove(danhGiaPhongVan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DanhGiaPhongVanExists(string id)
        {
            return _context.DanhGiaPhongVans.Any(e => e.Id == id);
        }
    }
}
