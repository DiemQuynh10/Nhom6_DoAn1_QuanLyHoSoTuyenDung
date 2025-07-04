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
    public class NhanViensController : Controller
    {
        private readonly AppDbContext _context;

        public NhanViensController(AppDbContext context)
        {
            _context = context;
        }

        // GET: NhanViens
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.NhanViens.Include(n => n.PhongBan);
            return View(await appDbContext.ToListAsync());
        }

        // GET: NhanViens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .Include(n => n.PhongBan)
                .FirstOrDefaultAsync(m => m.MaNhanVien == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // GET: NhanViens/Create
        public IActionResult Create()
        {
            ViewData["PhongBanId"] = new SelectList(_context.Set<PhongBan>(), "Id", "Id");
            return View();
        }

        // POST: NhanViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNhanVien,HoTen,MaSoNV,NgaySinh,ChucVu,PhongBanId,Email,SoDienThoai,NgayVaoCongTy,KinhNghiem,MoTa,MucLuong")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhanVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PhongBanId"] = new SelectList(_context.Set<PhongBan>(), "Id", "Id", nhanVien.PhongBanId);
            return View(nhanVien);
        }

        // GET: NhanViens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            ViewData["PhongBanId"] = new SelectList(_context.Set<PhongBan>(), "Id", "Id", nhanVien.PhongBanId);
            return View(nhanVien);
        }

        // POST: NhanViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaNhanVien,HoTen,MaSoNV,NgaySinh,ChucVu,PhongBanId,Email,SoDienThoai,NgayVaoCongTy,KinhNghiem,MoTa,MucLuong")] NhanVien nhanVien)
        {
            if (id != nhanVien.MaNhanVien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhanVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienExists(nhanVien.MaNhanVien))
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
            ViewData["PhongBanId"] = new SelectList(_context.Set<PhongBan>(), "Id", "Id", nhanVien.PhongBanId);
            return View(nhanVien);
        }

        // GET: NhanViens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .Include(n => n.PhongBan)
                .FirstOrDefaultAsync(m => m.MaNhanVien == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // POST: NhanViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien != null)
            {
                _context.NhanViens.Remove(nhanVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanVienExists(string id)
        {
            return _context.NhanViens.Any(e => e.MaNhanVien == id);
        }
    }
}
