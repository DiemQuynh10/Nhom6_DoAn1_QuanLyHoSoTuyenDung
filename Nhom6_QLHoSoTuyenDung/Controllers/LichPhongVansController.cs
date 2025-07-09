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

        // -------------------------
        // 1. Trả form tạo lịch (popup)
        // -------------------------
        public async Task<IActionResult> TaoLichPopup(string ungVienId)
        {
            var ungVien = await _context.UngViens
                .Include(u => u.ViTriUngTuyen)
                .FirstOrDefaultAsync(u => u.MaUngVien == ungVienId);

            if (ungVien == null)
                return NotFound();

            ViewBag.UngVien = ungVien;
            ViewBag.ViTri = ungVien.ViTriUngTuyen;

            var phongList = _context.PhongPhongVans
                .Select(p => new SelectListItem
                {
                    Value = p.Id,
                    Text = p.TenPhong + " - " + p.DiaDiem
                }).ToList();

            ViewBag.PhongList = phongList;

            return PartialView("_FormTaoLichPhongVan", new LichPhongVan());
        }

        // -------------------------
        // 2. Nhận và xử lý POST từ form popup
        // -------------------------
        [HttpPost]
        public async Task<IActionResult> CreateLichFromPopup(LichPhongVan model)
        {
            // ✅ Kiểm tra ứng viên có tồn tại không
            var ungVien = await _context.UngViens.FirstOrDefaultAsync(u => u.MaUngVien == model.UngVienId);
            if (ungVien == null)
            {
                return Json(new { success = false, message = "Ứng viên không tồn tại." });
            }

            // ✅ Gán lại vị trí ứng tuyển từ ứng viên
            model.ViTriId = ungVien.ViTriUngTuyenId;
            model.Id = Guid.NewGuid().ToString();

            // ❌ Không cho chọn thời gian trong quá khứ
            if (model.ThoiGian < DateTime.Now)
            {
                return Json(new { success = false, message = "Không thể tạo lịch với thời gian trong quá khứ." });
            }

            // ✅ Kiểm tra trùng lịch trong cùng phòng, cách nhau < 30 phút
            var lichCungPhong = await _context.LichPhongVans
                .Where(l => l.PhongPhongVanId == model.PhongPhongVanId)
                .ToListAsync();

            var clash = lichCungPhong.Any(l =>
                Math.Abs((l.ThoiGian - model.ThoiGian).Value.TotalMinutes) < 30
            );

            if (clash)
            {
                return Json(new { success = false, message = "Phòng đã có lịch phỏng vấn gần thời gian này. Vui lòng chọn thời gian khác." });
            }

            // ✅ Thêm vào DB
            _context.LichPhongVans.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã tạo lịch phỏng vấn thành công!" });
        }


        // -------------------------
        // 3. (Tùy chọn) Xem chi tiết lịch theo ứng viên
        // -------------------------
        public async Task<IActionResult> ByUngVien(string id)
        {
            var lich = await _context.LichPhongVans
                .Include(l => l.PhongPhongVan)
                .Include(l => l.UngVien)
                .FirstOrDefaultAsync(l => l.UngVienId == id);

            if (lich == null)
            {
                return Content("<p class='text-muted'>Chưa có lịch phỏng vấn cho ứng viên này.</p>", "text/html");
            }

            var html = $@"
                <div>
                    <p><strong>Ứng viên:</strong> {lich.UngVien.HoTen}</p>
                    <p><strong>Thời gian:</strong> {lich.ThoiGian:dd/MM/yyyy HH:mm}</p>
                    <p><strong>Phòng:</strong> {lich.PhongPhongVan?.TenPhong} - {lich.PhongPhongVan?.DiaDiem}</p>
                    <p><strong>Trạng thái:</strong> {lich.TrangThai}</p>
                    <p><strong>Ghi chú:</strong> {lich.GhiChu}</p>
                </div>";
            return Content(html, "text/html");
        }
    }
}
