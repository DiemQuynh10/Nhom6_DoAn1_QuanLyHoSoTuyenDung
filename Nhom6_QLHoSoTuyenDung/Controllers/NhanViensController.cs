using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Data;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class NhanViensController : Controller
    {
        private readonly AppDbContext _context;
        public NhanViensController(AppDbContext context) => _context = context;

        // GET: /NhanViens
        public async Task<IActionResult> Index(
            string? keyword,
            string? phongBan,
            string? chucVu,
            List<string>? kinhNghiemFilters)
        {
            var qs = _context.NhanViens
                             .Include(n => n.PhongBan)
                             .AsQueryable();

            // --- Filter chung ---
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim().ToLower();
                qs = qs.Where(n =>
                    (n.HoTen ?? "").ToLower().Contains(k) ||
                    (n.Email ?? "").ToLower().Contains(k) ||
                    (n.MaSoNV ?? "").ToLower().Contains(k) ||
                    (n.ChucVu ?? "").ToLower().Contains(k) ||
                    (n.MoTa ?? "").ToLower().Contains(k) ||
                    (n.SoDienThoai ?? "").ToLower().Contains(k) ||
                    (n.PhongBan!.TenPhong ?? "").ToLower().Contains(k));
            }
            if (!string.IsNullOrEmpty(phongBan))
                qs = qs.Where(n => n.PhongBan != null && n.PhongBan.TenPhong == phongBan);
            if (!string.IsNullOrEmpty(chucVu))
                qs = qs.Where(n => n.ChucVu == chucVu);

            var list = await qs.ToListAsync();

            // --- Lọc theo kinh nghiệm ---
            if (kinhNghiemFilters?.Any() == true)
            {
                list = list.Where(n =>
                {
                    var txt = (n.KinhNghiem ?? "").Split(' ').FirstOrDefault() ?? "0";
                    if (!int.TryParse(txt, out int x)) return false;
                    return kinhNghiemFilters.Any(r =>
                        r == "1-2" ? x >= 1 && x <= 2 :
                        r == "3-5" ? x >= 3 && x <= 5 :
                        r == "6-8" ? x >= 6 && x <= 8 :
                        r == ">8" ? x > 8 : false);
                }).ToList();
            }

            // --- Chuẩn bị dữ liệu cho ViewBag ---
            ViewBag.PhongBans = await _context.PhongBans.Select(p => p.TenPhong).Distinct().ToListAsync();
            ViewBag.PhongBanList = await _context.PhongBans.ToListAsync();
            ViewBag.ChucVus = await _context.NhanViens
                                              .Where(n => n.ChucVu != null)
                                              .Select(n => n.ChucVu!)
                                              .Distinct().ToListAsync();
            ViewBag.SelectedKN = kinhNghiemFilters ?? new List<string>();
            ViewBag.AccountIds = await _context.NguoiDungs.Select(u => u.NhanVienId).ToListAsync();
            ViewBag.PhongBanCounts = (ViewBag.PhongBans as List<string>)
                                        .Select(lbl => list.Count(n => n.PhongBan != null && n.PhongBan.TenPhong == lbl))
                                        .ToList();
            ViewBag.RawDates = list
                                        .Where(n => n.NgayVaoCongTy.HasValue)
                                        .Select(n => n.NgayVaoCongTy.Value.ToString("yyyy-MM"))
                                        .ToList();

            // --- Lấy ứng viên "Đạt" sang ViewModel ---
            var datUngViens = await _context.UngViens
                .Include(u => u.ViTriUngTuyen)
                    .ThenInclude(v => v.PhongBan)
                .Where(u => u.TrangThai == "DaTuyen")
                .ToListAsync();

            var candidateVms = datUngViens.Select(u => new CandidateVm
            {
                MaUngVien = u.MaUngVien,
                HoTen = u.HoTen,
                NgaySinh = u.NgaySinh,
                GioiTinh = (int)u.GioiTinh,
                Email = u.Email ?? "",
                SoDienThoai = u.SoDienThoai ?? "",
                ViTri = u.ViTriUngTuyen?.TenViTri ?? "",
                ChucVu = u.ViTriUngTuyen?.PhongBan?.TenPhong ?? "",
                KinhNghiem = u.KinhNghiem,
                ThanhTich = u.ThanhTich,
                NgayNop = u.NgayNop
            }).ToList();
            ViewBag.Candidates = candidateVms;

            var users = await _context.NguoiDungs
                         .Include(u => u.NhanVien)
                         .ToListAsync();
            ViewBag.Users = users;

            return View(list);
        }

        [HttpPost]
        public async Task<JsonResult> VerifyPassword(string currentPassword)
        {
            var currentUsername = User.Identity.Name;
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDangNhap == currentUsername);
            if (user == null || user.MatKhau != currentPassword)
                return Json(new { success = false });
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteConfirmed(string MaNhanVien, string currentPassword)
        {
            var currentUsername = User.Identity.Name;
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDangNhap == currentUsername);
            if (user == null || user.MatKhau != currentPassword)
                return Json(new { success = false, message = "Mật khẩu xác nhận không đúng." });

            var nv = await _context.NhanViens.FindAsync(MaNhanVien);
            if (nv == null)
                return Json(new { success = false, message = "Nhân viên không tồn tại." });

            _context.NhanViens.Remove(nv);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // GET: /NhanViens/GenerateCodes?phongBanId=...
        [HttpGet]
        public IActionResult GenerateCodes(string phongBanId)
        {
            var maNV = GenerateUniqueMaNV();
            var maSoNV = GenerateUniqueMaSoNV(phongBanId);
            return Json(new { maNV, maSoNV });
        }

        // Sinh MaNhanVien duy nhất dạng "NV"+4 chữ số
        private string GenerateUniqueMaNV()
        {
            var existing = _context.NhanViens.Select(n => n.MaNhanVien).ToHashSet();
            var rnd = new Random();
            string ma;
            int attempts = 0;

            do
            {
                ma = "NV" + rnd.Next(1000, 9999);
                attempts++;
                if (attempts > 100) throw new Exception("Không thể tạo mã nhân viên duy nhất.");
            } while (existing.Contains(ma));

            return ma;
        }

        private string GenerateUniqueMaSoNV(string phongBanId)
        {
            var prefix = phongBanId.Length >= 3 ? phongBanId.Substring(2) : phongBanId;
            var existing = _context.NhanViens.Select(n => n.MaSoNV).ToHashSet();
            var rnd = new Random();
            string maSo;
            int attempts = 0;

            do
            {
                maSo = $"{prefix}-{rnd.Next(1, 999):D3}";
                attempts++;
                if (attempts > 100) throw new Exception("Không thể tạo mã số nhân viên duy nhất.");
            } while (existing.Contains(maSo));

            return maSo;
        }

        // POST: /NhanViens/CreateNhanVien
        [HttpPost]
        public async Task<IActionResult> CreateNhanVien([FromForm] NhanVien nv, [FromForm] string? MaUngVien)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                });
            }

            // Sinh mã
            nv.MaNhanVien = GenerateUniqueMaNV();
            nv.MaSoNV = GenerateUniqueMaSoNV(nv.PhongBanId ?? "");
            _context.NhanViens.Add(nv);

            // Nếu thêm từ ứng viên: xóa lịch + xóa ứng viên
            if (!string.IsNullOrEmpty(MaUngVien))
            {
                var lichPv = await _context.LichPhongVans.Where(l => l.UngVienId == MaUngVien).ToListAsync();
                _context.LichPhongVans.RemoveRange(lichPv);
                var uv = await _context.UngViens.FindAsync(MaUngVien);
                if (uv != null) _context.UngViens.Remove(uv);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
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
        // POST: /NhanViens/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            [Bind("MaNhanVien,HoTen,MaSoNV,NgaySinh,ChucVu,PhongBanId,Email,SoDienThoai,NgayVaoCongTy,KinhNghiem,MoTa,MucLuong")]
            NhanVien nhanVien)
        {
            if (id != nhanVien.MaNhanVien) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "TenPhong", nhanVien.PhongBanId);
                return View(nhanVien);
            }

            try
            {
                _context.Update(nhanVien);
                await _context.SaveChangesAsync();
                // ← set the toast message
                TempData["SuccessMessage"] = $"Sửa nhân viên mã {nhanVien.MaNhanVien} thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhanVienExists(nhanVien.MaNhanVien))
                    return NotFound();
                throw;
            }
        }

        // POST: /NhanViens/EditConfirmed
        [HttpPost]
        public async Task<JsonResult> EditConfirmed(string MaNhanVien, string currentPassword)
        {
            var currentUsername = User.Identity.Name;
            var user = await _context.NguoiDungs
                                     .FirstOrDefaultAsync(u => u.TenDangNhap == currentUsername);
            if (user == null || user.MatKhau != currentPassword)
                return Json(new { success = false, message = "Mật khẩu xác nhận không đúng." });

            // everything’s OK → send back the URL to redirect to
            var url = Url.Action("Edit", "NhanViens", new { id = MaNhanVien });
            return Json(new { success = true, redirectUrl = url });
        }

        private bool NhanVienExists(string maNhanVien)
            => _context.NhanViens.Any(e => e.MaNhanVien == maNhanVien);


        // POST: /NhanViens/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var nv = await _context.NhanViens.FindAsync(id);
            if (nv == null)
                return Json(new { success = false, message = "Không tìm thấy nhân viên" });

            _context.NhanViens.Remove(nv);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // POST: /NhanViens/CreateAccount?id=...&vaiTro=...
        [HttpPost]
        public async Task<JsonResult> CreateAccount(
     string id,
     string vaiTro,
     string currentPassword)
        {
            // 1) Lấy user hiện tại từ Claims
            var currentUsername = User.Identity.Name;
            var currentUser = await _context.NguoiDungs
                                            .FirstOrDefaultAsync(u => u.TenDangNhap == currentUsername);
            if (currentUser == null || currentUser.MatKhau != currentPassword)
            {
                return Json(new
                {
                    success = false,
                    message = "Mật khẩu xác nhận không đúng."
                });
            }

            // 2) Đã có TK chưa?
            if (await _context.NguoiDungs.AnyAsync(u => u.NhanVienId == id))
                return Json(new
                {
                    success = false,
                    message = "Tài khoản đã tồn tại."
                });

            // 3) Tạo mới
            var nv = await _context.NhanViens.FindAsync(id);
            if (nv == null)
                return Json(new
                {
                    success = false,
                    message = "Nhân viên không tồn tại."
                });

            var user = new NguoiDung
            {
                NhanVienId = id,
                TenDangNhap = nv.MaSoNV,
                MatKhau = "123456",
                VaiTro = vaiTro,
                PhongBanId = nv.PhongBanId,
                HoTen = nv.HoTen,
                Email = nv.Email,
                SoDienThoai = nv.SoDienThoai,
                NgayTao = DateTime.Now
            };
            _context.NguoiDungs.Add(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // GET: /NhanViens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var nv = await _context.NhanViens
                                   .Include(n => n.PhongBan)
                                   .FirstOrDefaultAsync(n => n.MaNhanVien == id);
            if (nv == null)
                return NotFound();

            return View(nv);  // sẽ tìm Views/NhanViens/Details.cshtml
        }


        // GET: /NhanViens/GetCandidate/{id}
        [HttpGet]
        public async Task<IActionResult> GetCandidate(string id)
        {
            var uv = await _context.UngViens.FindAsync(id);
            if (uv == null)
                return Json(new { success = false });

            return Json(new
            {
                success = true,
                ma = uv.MaUngVien,
                ten = uv.HoTen,
                email = uv.Email,
                sdt = uv.SoDienThoai,
                ngaysinh = uv.NgaySinh?.ToString("yyyy-MM-dd"),
                kinhn = uv.KinhNghiem,
                mota = uv.MoTa
            });
        }
    }
}
