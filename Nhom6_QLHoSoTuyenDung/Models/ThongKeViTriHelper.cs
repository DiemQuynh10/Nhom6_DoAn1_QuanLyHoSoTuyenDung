namespace Nhom6_QLHoSoTuyenDung.Models
{
    public class ThongKeViTriHelper
    {
        // Phân bố trạng thái
        public static Dictionary<string, int> DemTheoTrangThai(List<ViTriTuyenDung> ds)
        {
            return ds.GroupBy(v => v.TrangThai ?? "Không rõ")
                     .ToDictionary(g => g.Key, g => g.Count());
        }

        // Phân bố theo tháng (xu hướng tạo mới)
        public static (List<string> Thang, List<int> SoLuong) DemTheoThang(List<ViTriTuyenDung> ds)
        {
            var thangData = ds.Where(v => v.NgayTao.HasValue)
                              .GroupBy(v => v.NgayTao.Value.Month)
                              .OrderBy(g => g.Key)
                              .ToList();

            var thang = thangData.Select(g => $"T{g.Key}").ToList();
            var soLuong = thangData.Select(g => g.Count()).ToList();

            return (thang, soLuong);
        }

        // (Tuỳ chọn) Nếu có trạng thái "Hoàn thành"
        public static List<int> DemTrangThaiHoanThanhTheoThang(List<ViTriTuyenDung> ds)
        {
            return ds.Where(v => v.TrangThai == "Hoàn thành" && v.NgayTao.HasValue)
                     .GroupBy(v => v.NgayTao.Value.Month)
                     .OrderBy(g => g.Key)
                     .Select(g => g.Count())
                     .ToList();
        }
        public static List<QuyTrinhTuyenDungItem> ThongKeQuyTrinhTuyenDung(List<UngVien> ungViens)
        {
            DateTime dauTuan = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            DateTime cuoiTuan = dauTuan.AddDays(6);

            int hoSoMoi = ungViens.Count(u => u.NgayNop >= dauTuan && u.NgayNop <= cuoiTuan);
            int dangXemXet = ungViens.Count(u => u.TrangThai == "Đang xem xét");
            int phongVan = ungViens.Count(u => u.TrangThai == "Phỏng vấn");
            int deXuat = ungViens.Count(u => u.TrangThai == "Đề xuất");

            return new List<QuyTrinhTuyenDungItem>
    {
        new() { Ten = "Hồ sơ mới", SoLuong = hoSoMoi, GhiChu = "Tuần này", PhanTramThayDoi = 15 },
        new() { Ten = "Đang xem xét", SoLuong = dangXemXet, GhiChu = "Chờ duyệt", PhanTramThayDoi = -5 },
        new() { Ten = "Phỏng vấn", SoLuong = phongVan, GhiChu = "Đã lên lịch", PhanTramThayDoi = 8 },
        new() { Ten = "Đề xuất", SoLuong = deXuat, GhiChu = "Chờ quyết định", PhanTramThayDoi = 3 },
    };
        }

    }
}
