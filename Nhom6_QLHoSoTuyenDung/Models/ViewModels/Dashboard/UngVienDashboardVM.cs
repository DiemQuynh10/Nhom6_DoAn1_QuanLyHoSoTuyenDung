namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.Dashboard
{
    public class UngVienDashboardVM
    {
        public int TongHoSo { get; set; }
        public int UngVienMoi { get; set; }
        public int SoPhongVan { get; set; }
        public double HieuQua { get; set; }

        public List<string> TrangThaiLabels { get; set; } = new();
        public List<int> TrangThaiValues { get; set; } = new();

        public List<int> NopHoSoData { get; set; } = new();
        public List<int> PhongVanData { get; set; } = new();

        public List<string> NguonLabels { get; set; } = new();
        public List<int> NguonData { get; set; } = new();

        public List<dynamic> UngVienMoiNhat { get; set; } = new();
        public List<dynamic> LichPhongVanSapToi { get; set; } = new();

        public List<HoatDongDashboardVM> HoatDongGanDay { get; set; } = new();
    }
}
