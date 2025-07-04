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
    public class ThongKeTuyenDungsController : Controller
    {
        private readonly AppDbContext _context;

        public ThongKeTuyenDungsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ThongKeTuyenDungs
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.ThongKeTuyenDungs.Include(t => t.ViTriTuyenDung);
            return View(await appDbContext.ToListAsync());
        }

        // GET: ThongKeTuyenDungs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thongKeTuyenDung = await _context.ThongKeTuyenDungs
                .Include(t => t.ViTriTuyenDung)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thongKeTuyenDung == null)
            {
                return NotFound();
            }

            return View(thongKeTuyenDung);
        }

        // GET: ThongKeTuyenDungs/Create
        public IActionResult Create()
        {
            ViewData["ViTriId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri");
            return View();
        }

        // POST: ThongKeTuyenDungs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ViTriId,SoLuongUngVien,SoLuongDat,SoLuongTruot,ThoiGianThongKe")] ThongKeTuyenDung thongKeTuyenDung)
        {
            if (ModelState.IsValid)
            {
                _context.Add(thongKeTuyenDung);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ViTriId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri", thongKeTuyenDung.ViTriId);
            return View(thongKeTuyenDung);
        }

        // GET: ThongKeTuyenDungs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thongKeTuyenDung = await _context.ThongKeTuyenDungs.FindAsync(id);
            if (thongKeTuyenDung == null)
            {
                return NotFound();
            }
            ViewData["ViTriId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri", thongKeTuyenDung.ViTriId);
            return View(thongKeTuyenDung);
        }

        // POST: ThongKeTuyenDungs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ViTriId,SoLuongUngVien,SoLuongDat,SoLuongTruot,ThoiGianThongKe")] ThongKeTuyenDung thongKeTuyenDung)
        {
            if (id != thongKeTuyenDung.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thongKeTuyenDung);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThongKeTuyenDungExists(thongKeTuyenDung.Id))
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
            ViewData["ViTriId"] = new SelectList(_context.Set<ViTriTuyenDung>(), "MaViTri", "MaViTri", thongKeTuyenDung.ViTriId);
            return View(thongKeTuyenDung);
        }

        // GET: ThongKeTuyenDungs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thongKeTuyenDung = await _context.ThongKeTuyenDungs
                .Include(t => t.ViTriTuyenDung)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thongKeTuyenDung == null)
            {
                return NotFound();
            }

            return View(thongKeTuyenDung);
        }

        // POST: ThongKeTuyenDungs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var thongKeTuyenDung = await _context.ThongKeTuyenDungs.FindAsync(id);
            if (thongKeTuyenDung != null)
            {
                _context.ThongKeTuyenDungs.Remove(thongKeTuyenDung);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThongKeTuyenDungExists(string id)
        {
            return _context.ThongKeTuyenDungs.Any(e => e.Id == id);
        }
    }
}
