﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.Dashboard;
using Nhom6_QLHoSoTuyenDung.Services;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{

    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.HR)]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHoatDongService _hoatDongService;

        public HomeController(AppDbContext context, IHoatDongService hoatDongService)
        {
            _context = context;
            _hoatDongService = hoatDongService;
        }

        public IActionResult Index(string search, string status, string source, string time, int? quarter, int? year)
        {
            var vm = new UngVienDashboardVM();
            var today = DateTime.Today;

            // Query lọc theo search, source, time (chưa lọc status)
            var baseQuery = _context.UngViens.Include(x => x.ViTriUngTuyen).AsQueryable();
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);
            if (!string.IsNullOrEmpty(search))
                baseQuery = baseQuery.Where(u => u.HoTen.Contains(search) || u.Email.Contains(search));

            if (!string.IsNullOrEmpty(source))
                baseQuery = baseQuery.Where(u => u.NguonUngTuyen == source);

            if (!string.IsNullOrEmpty(time))
            {
                if (time == "week")
                {
                    baseQuery = baseQuery.Where(u => u.NgayNop >= startOfWeek);
                }
                else if (time == "month")
                {
                    baseQuery = baseQuery.Where(u => u.NgayNop >= new DateTime(today.Year, today.Month, 1));
                }
                else if (time == "year")
                {
                    baseQuery = baseQuery.Where(u => u.NgayNop >= new DateTime(today.Year, 1, 1));
                }
            }

            if (year.HasValue)
                baseQuery = baseQuery.Where(u => u.NgayNop.HasValue && u.NgayNop.Value.Year == year.Value);

            if (quarter.HasValue)
            {
                var startMonth = (quarter.Value - 1) * 3 + 1;
                var endMonth = startMonth + 2;
                baseQuery = baseQuery.Where(u => u.NgayNop.HasValue && u.NgayNop.Value.Month >= startMonth && u.NgayNop.Value.Month <= endMonth);
            }

            // Tính hiệu quả trước khi lọc trạng thái
            var tongHoSo = baseQuery.Count();
            var daTuyen = baseQuery.Count(u => u.TrangThai == "Đã tuyển");
            vm.HieuQua = tongHoSo > 0 ? Math.Round(daTuyen * 100.0 / tongHoSo, 1) : 0;

            // Tiếp tục query cho dashboard (bao gồm lọc theo trạng thái)
            var query = baseQuery;
            if (!string.IsNullOrEmpty(status))
                query = query.Where(u => u.TrangThai == status);

            // Tổng hồ sơ sau lọc trạng thái
            vm.TongHoSo = query.Count();

            // Ứng viên mới
            if (time == "week")
            {
                vm.UngVienMoi = query.Count(u => u.NgayNop >= startOfWeek);
            }
            else if (time == "month")
            {
                vm.UngVienMoi = query.Count(u => u.NgayNop >= new DateTime(today.Year, today.Month, 1));
            }
            else if (time == "year")
            {
                vm.UngVienMoi = query.Count(u => u.NgayNop >= new DateTime(today.Year, 1, 1));
            }
            else
            {   
                vm.UngVienMoi = query.Count(u => u.NgayNop >= startOfWeek);
            }

            // Số phỏng vấn
            var lichQuery = _context.LichPhongVans.Include(l => l.UngVien).AsQueryable();
            if (time == "week")
            {
                lichQuery = lichQuery.Where(l => l.ThoiGian.HasValue && l.ThoiGian.Value >= startOfWeek);
            }
            else if (time == "month")
            {
                lichQuery = lichQuery.Where(l => l.ThoiGian.HasValue && l.ThoiGian.Value >= new DateTime(today.Year, today.Month, 1));
            }
            else if (time == "year")
            {
                lichQuery = lichQuery.Where(l => l.ThoiGian.HasValue && l.ThoiGian.Value >= new DateTime(today.Year, 1, 1));
            }
            else
            {
                lichQuery = lichQuery.Where(l => l.ThoiGian.HasValue && l.ThoiGian.Value >= startOfWeek);
            }

            vm.SoPhongVan = lichQuery.Count();

            // Pie trạng thái
            vm.TrangThaiLabels = query.GroupBy(u => u.TrangThai).Select(g => g.Key ?? "Không xác định").ToList();
            vm.TrangThaiValues = query.GroupBy(u => u.TrangThai).Select(g => g.Count()).ToList();

            // Biểu đồ theo tháng
            vm.NopHoSoData = Enumerable.Range(1, 12)
                .Select(m => query.Count(u => u.NgayNop.HasValue && u.NgayNop.Value.Month == m)).ToList();

            vm.PhongVanData = Enumerable.Range(1, 12)
                .Select(m => _context.LichPhongVans.Count(l => l.ThoiGian.HasValue && l.ThoiGian.Value.Month == m)).ToList();

            vm.NguonLabels = query.GroupBy(u => u.NguonUngTuyen).Select(g => g.Key ?? "Khác").ToList();
            vm.NguonData = query.GroupBy(u => u.NguonUngTuyen).Select(g => g.Count()).ToList();

            // Ứng viên mới nhất
            vm.UngVienMoiNhat = query
                .Where(u => u.NgayNop.HasValue && u.NgayNop.Value >= startOfWeek)
                .OrderByDescending(u => u.NgayNop)
                .Take(10)
                .Select(u => new
                {
                    u.HoTen,
                    ViTri = u.ViTriUngTuyen.TenViTri,
                    u.TrangThai
                }).ToList<dynamic>();


            // Lịch phỏng vấn sắp tới
            vm.LichPhongVanSapToi = _context.LichPhongVans
                .Include(l => l.UngVien).Include(l => l.ViTriTuyenDung)
                .Where(l => l.ThoiGian > DateTime.Now)
                .OrderBy(l => l.ThoiGian)
                .Take(3)
                .Select(l => new
                {
                    HoTen = l.UngVien.HoTen,
                    ViTri = l.ViTriTuyenDung.TenViTri,
                    Ngay = l.ThoiGian.Value.ToString("dd/MM"),
                    Gio = l.ThoiGian.Value.ToString("HH:mm")
                }).ToList<dynamic>();


            var currentUser = User.Identity?.Name ?? "system";

            vm.HoatDongGanDay = _hoatDongService.GetHoatDongGanDay(currentUser, startOfWeek);


            return View("~/Views/Home/Index.cshtml", vm);
        }
    }
}