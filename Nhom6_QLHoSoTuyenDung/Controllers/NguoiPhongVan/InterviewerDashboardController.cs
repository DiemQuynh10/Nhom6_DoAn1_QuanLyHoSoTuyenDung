﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.Helpers;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;
using Nhom6_QLHoSoTuyenDung.Services.Implementations;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nhom6_QLHoSoTuyenDung.Controllers.NguoiPhongVan
{
    [Authorize(Roles = $"{RoleNames.Interviewer}")]
    public class InterviewerDashboardController : Controller
    {
        private readonly INguoiPhongVanService _phongVanService;
        private readonly IWebHostEnvironment _env;
        private readonly ITaiKhoanService _taiKhoanService;

        public InterviewerDashboardController(
            INguoiPhongVanService phongVanService,
            IWebHostEnvironment env,
            ITaiKhoanService taiKhoanService)
        {
            _phongVanService = phongVanService;
            _env = env;
            _taiKhoanService = taiKhoanService;
        }



        public async Task<IActionResult> Index()
        {
            var username = User.Identity?.Name ?? "";
            var vm = await _phongVanService.GetDashboardAsync(username);
            return View("DashboardInterviewer", vm);
        }

        public async Task<IActionResult> LichPhongVan()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("DangNhap", "NguoiDungs");

            var vm = await _phongVanService.GetLichPhongVanPageAsync(username!);
            return View(vm);
        }

        public async Task<IActionResult> LichSuPhongVan()
        {
            var id = HttpContext.Session.GetString("IDNguoiDung");

            if (string.IsNullOrEmpty(id))
                return RedirectToAction("DangNhap", "NguoiDungs");

            var tenNguoiDung = User.FindFirst("HoTen")?.Value ?? "Chưa rõ";
            var lichSu = await _phongVanService.GetLichSuPhongVanAsync(id, tenNguoiDung);

            var thongKe = await _phongVanService.GetThongKeLichSuPhongVanAsync(id);

            var vm = new LichSuPhongVanTongHopVM
            {
                DanhSachLichSu = lichSu,
                ThongKe = thongKe
            };

            return View(vm);

        }

        [HttpGet]
        public async Task<IActionResult> TrangThaiPhongVanChart()
        {
            var username = User.Identity?.Name ?? "";
            var danhSach = await _phongVanService.GetLichPhongVanTheoNhanVienAsync(username);

            var daXacNhan = danhSach.Count(l => l.TrangThai == TrangThaiPhongVanEnum.DaLenLich.ToString());
            var hoanThanh = danhSach.Count(l => l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString());
            var daHuy = danhSach.Count(l => l.TrangThai == TrangThaiPhongVanEnum.Huy.ToString());

            return Json(new
            {
                labels = new[] { "Đã xác nhận", "Hoàn thành", "Đã hủy" },
                values = new[] { daXacNhan, hoanThanh, daHuy }
            });
        }

        [HttpGet]
        public async Task<IActionResult> ThanhTichCuaToi()
        {
            var username = User.Identity?.Name ?? "";
            var dashboard = await _phongVanService.GetDashboardAsync(username);
            var tong = dashboard.TongSoPhongVan;
            var tyLe = dashboard.TyLeThanhCong;
            var soThanhCong = dashboard.SoDaHoanThanhHomNay;
            var soKhongPhuHop = tong - soThanhCong;

            return Json(new
            {
                Ten = username,
                ViTri = "Người phỏng vấn",
                TongPV = tong,
                ThanhCong = soThanhCong,
                KhongPhuHop = soKhongPhuHop,
                TyLe = tyLe,
                XepHang = 2,
                TongNguoi = 8
            });
        }

        [HttpGet]
        public async Task<IActionResult> HoatDongGanDay()
        {
            var username = User.Identity?.Name ?? "";
            var dashboard = await _phongVanService.GetDashboardAsync(username);
            return Json(dashboard.HoatDongGanDay);
        }

        public async Task<IActionResult> XemCV(string ungVienId)
        {
            var fileName = await _phongVanService.GetLinkCvAsync(ungVienId);
            if (string.IsNullOrEmpty(fileName))
                return NotFound("Không tìm thấy CV.");

            var filePath = Path.Combine(_env.WebRootPath, "cv", Path.GetFileName(fileName));
            if (!System.IO.File.Exists(filePath))
                return NotFound("CV không tồn tại.");

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // ⚠️ KHÔNG THÊM HEADER Content-Disposition
            return File(stream, "application/pdf");
        }
        public async Task<IActionResult> TrangThaiCho()
        {
            var username = User.Identity?.Name ?? "";
            var lichIds = await _phongVanService.GetDanhSachLichPhongVanTheoDeXuatAsync(username, DeXuatEnum.CanPhongVanLan2);
            var lichPhongVans = await _phongVanService.GetThongTinChiTietLichAsync(lichIds, username);
            return View("TrangThaiCho", lichPhongVans);
        }

        [HttpPost]
        public async Task<IActionResult> Huy(string id, string ghiChu)
        {
            // Hủy lịch phỏng vấn trong database
            var ketQua = await _phongVanService.HuyLichPhongVanAsync(id, ghiChu);

            if (!ketQua)
            {
                TempData["Error"] = "❌ Không tìm thấy lịch phỏng vấn.";
                return RedirectToAction("LichPhongVan");
            }

            // ✅ Lấy thông tin ứng viên để gửi email
            var lich = await _phongVanService.GetLichByIdAsync(id); // bạn cần thêm hàm này trong service
            if (lich?.UngVien != null)
            {
                var email = lich.UngVien.Email;
                var hoTen = lich.UngVien.HoTen;

                var subject = "⛔ Lịch phỏng vấn bị hủy";
                var body =
                    $"Thân gửi {hoTen},\n\n" +
                    $"Rất tiếc, lịch phỏng vấn của bạn đã bị hủy với lý do sau:\n\n" +
                    $"{ghiChu}\n\n" +
                    $"Chúng tôi sẽ liên hệ lại với bạn nếu có lịch mới phù hợp.\n\n" +
                    $"Trân trọng,\nPhòng Tuyển dụng";

                await _taiKhoanService.SendEmailAsync(email, subject, body);
            }
            TempData["Success"] = "✅ Đã hủy lịch phỏng vấn và gửi email thông báo cho ứng viên.";
            return RedirectToAction("LichPhongVan");
        }


    }
} 
