using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.ThongKe;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;
using OfficeOpenXml;
using System.Drawing;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class ThongKeController : Controller
    {
        private readonly IThongKeService _thongKeService;

        public ThongKeController(IThongKeService thongKeService)
        {
            _thongKeService = thongKeService;
        }

        // Trang chính Thống kê
        public async Task<IActionResult> Index(string? tuKhoa, string? loaiBaoCao, DateTime? tuNgay, DateTime? denNgay)
        {
            ViewBag.TuKhoa = tuKhoa;
            ViewBag.Loai = loaiBaoCao;
            ViewBag.TuNgay = tuNgay;
            ViewBag.DenNgay = denNgay;

            var vm = await _thongKeService.GetTongQuanAsync(tuKhoa, loaiBaoCao, tuNgay, denNgay);
            return View(vm);
        }

        // ---------------------------
        // Các biểu đồ (truyền bộ lọc)
        // ---------------------------

        [HttpGet]
        public async Task<IActionResult> BieuDoTrangThaiUngVien(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var data = await _thongKeService.GetBieuDoTheoTrangThaiUngVienAsync(tuKhoa, tuNgay, denNgay);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> BieuDoNguonUngVien(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var data = await _thongKeService.GetBieuDoNguonUngVienAsync(tuKhoa, tuNgay, denNgay);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> BieuDoTheoViTri(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var data = await _thongKeService.GetBieuDoTheoViTriUngTuyenAsync(tuKhoa, tuNgay, denNgay);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> BieuDoTheoPhongBan(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var data = await _thongKeService.GetBieuDoTheoPhongBanAsync(tuKhoa, tuNgay, denNgay);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> BieuDoXuHuongNopHoSo(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var data = await _thongKeService.GetXuHuongTheoThangAsync(tuKhoa, tuNgay, denNgay);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> BieuDoDiemDanhGia(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var data = await _thongKeService.GetBieuDoDanhGiaUngVienAsync(tuKhoa, tuNgay, denNgay);
            return Json(data);
        }

        // Bảng vị trí tuyển thành công
        [HttpGet]
        public async Task<IActionResult> ViTriTuyenThanhCong(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var data = await _thongKeService.GetViTriTuyenThanhCongAsync(tuKhoa, tuNgay, denNgay);
            return PartialView("_ViTriThanhCongPartial", data);
        }
        [HttpGet]
        public async Task<IActionResult> XuatBaoCao(string? tuKhoa, string? loaiBaoCao, DateTime? tuNgay, DateTime? denNgay)
        {
            var reportData = await _thongKeService.GetTongQuanAsync(tuKhoa, loaiBaoCao, tuNgay, denNgay);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("BaoCaoUngVien");

            // Tiêu đề
            ws.Cell(1, 1).Value = "BÁO CÁO THỐNG KÊ ỨNG VIÊN";
            ws.Cell(2, 1).Value = $"Từ ngày: {(tuNgay?.ToString("dd/MM/yyyy") ?? "Không rõ")}";
            ws.Cell(2, 2).Value = $"Đến ngày: {(denNgay?.ToString("dd/MM/yyyy") ?? "Không rõ")}";

            // Thống kê tổng quan
            ws.Cell(4, 1).Value = "Tổng ứng viên";
            ws.Cell(4, 2).Value = reportData.TongUngVien;

            ws.Cell(5, 1).Value = "Đã tuyển";
            ws.Cell(5, 2).Value = reportData.SoDaTuyen;

            ws.Cell(6, 1).Value = "Đang xử lý";
            ws.Cell(6, 2).Value = reportData.SoDangXuLy;

            ws.Cell(7, 1).Value = "Vị trí đang tuyển";
            ws.Cell(7, 2).Value = reportData.SoViTriDangTuyen;

            // Danh sách ứng viên đã tuyển
            ws.Cell(9, 1).Value = "STT";
            ws.Cell(9, 2).Value = "Họ tên";
            ws.Cell(9, 3).Value = "Email";
            ws.Cell(9, 4).Value = "Vị trí";
            ws.Cell(9, 5).Value = "Ngày nộp";

            int row = 10;
            int stt = 1;

            foreach (var uv in reportData.UngVienDaTuyen)
            {
                ws.Cell(row, 1).Value = stt++;
                ws.Cell(row, 2).Value = uv.HoTen;
                ws.Cell(row, 3).Value = uv.Email;
                ws.Cell(row, 4).Value = uv.TenViTri;
                ws.Cell(row, 5).Value = uv.NgayNop.ToString("dd/MM/yyyy");
                row++;
            }

            // Format
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            string fileName = $"BaoCaoUngVien_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [HttpPost]
        public async Task<IActionResult> XuatBaoCaoDayDu([FromBody] XuatBaoCaoVM vm)
        {
            // ✅ Bắt buộc phải có dòng này nếu dùng EPPlus từ bản 5 trở lên
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();

            var sheet = package.Workbook.Worksheets.Add("Tổng Quan");

            sheet.Cells["A1"].Value = "Từ khóa:";
            sheet.Cells["B1"].Value = vm.TuKhoa ?? "Tất cả";

            sheet.Cells["A2"].Value = "Từ ngày:";
            sheet.Cells["B2"].Value = vm.TuNgay?.ToString("dd/MM/yyyy") ?? "Không giới hạn";

            sheet.Cells["A3"].Value = "Đến ngày:";
            sheet.Cells["B3"].Value = vm.DenNgay?.ToString("dd/MM/yyyy") ?? "Không giới hạn";

            sheet.Cells["A5"].Value = "Biểu đồ";
            sheet.Cells["B5"].Value = "Hình ảnh";

            int row = 6;
            foreach (var chart in vm.ChartImages)
            {
                var bytes = Convert.FromBase64String(chart.ImageBase64.Replace("data:image/png;base64,", ""));
                using var stream = new MemoryStream(bytes);
                using var image = Image.FromStream(stream);

                var picture = sheet.Drawings.AddPicture(chart.Id, image);
                picture.SetPosition(row - 1, 0, 1, 0);  // Dòng, offset Y, Cột, offset X
                picture.SetSize(600, 300);

                row += 20;
            }

            var excelBytes = package.GetAsByteArray();
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaoCaoDayDu.xlsx");
        }


    }
}
