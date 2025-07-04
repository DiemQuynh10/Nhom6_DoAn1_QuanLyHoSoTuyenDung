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
    public class NguoiDungsController : Controller
    {
        private readonly AppDbContext _context;

        public NguoiDungsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: NguoiDungs
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.NguoiDungs.Include(n => n.NhanVien).Include(n => n.PhongBan);
            return View(await appDbContext.ToListAsync());
        }

        // GET: NguoiDungs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.NhanVien)
                .Include(n => n.PhongBan)
                .FirstOrDefaultAsync(m => m.NhanVienId == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        // GET: NguoiDungs/Create
        public IActionResult Create()
        {
            ViewData["NhanVienId"] = new SelectList(_context.Set<NhanVien>(), "MaNhanVien", "MaNhanVien");
            ViewData["PhongBanId"] = new SelectList(_context.Set<PhongBan>(), "Id", "Id");
            return View();
        }

        // POST: NguoiDungs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NhanVienId,TenDangNhap,MatKhau,VaiTro,PhongBanId,HoTen,Email,SoDienThoai,NgayTao")] NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nguoiDung);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NhanVienId"] = new SelectList(_context.Set<NhanVien>(), "MaNhanVien", "MaNhanVien", nguoiDung.NhanVienId);
            ViewData["PhongBanId"] = new SelectList(_context.Set<PhongBan>(), "Id", "Id", nguoiDung.PhongBanId);
            return View(nguoiDung);
        }

        // GET: NguoiDungs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }
            ViewData["NhanVienId"] = new SelectList(_context.Set<NhanVien>(), "MaNhanVien", "MaNhanVien", nguoiDung.NhanVienId);
            ViewData["PhongBanId"] = new SelectList(_context.Set<PhongBan>(), "Id", "Id", nguoiDung.PhongBanId);
            return View(nguoiDung);
        }

        // POST: NguoiDungs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NhanVienId,TenDangNhap,MatKhau,VaiTro,PhongBanId,HoTen,Email,SoDienThoai,NgayTao")] NguoiDung nguoiDung)
        {
            if (id != nguoiDung.NhanVienId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nguoiDung);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguoiDungExists(nguoiDung.NhanVienId))
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
            ViewData["NhanVienId"] = new SelectList(_context.Set<NhanVien>(), "MaNhanVien", "MaNhanVien", nguoiDung.NhanVienId);
            ViewData["PhongBanId"] = new SelectList(_context.Set<PhongBan>(), "Id", "Id", nguoiDung.PhongBanId);
            return View(nguoiDung);
        }

        // GET: NguoiDungs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.NhanVien)
                .Include(n => n.PhongBan)
                .FirstOrDefaultAsync(m => m.NhanVienId == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        // POST: NguoiDungs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung != null)
            {
                _context.NguoiDungs.Remove(nguoiDung);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NguoiDungExists(string id)
        {
            return _context.NguoiDungs.Any(e => e.NhanVienId == id);
        }
    }
}
