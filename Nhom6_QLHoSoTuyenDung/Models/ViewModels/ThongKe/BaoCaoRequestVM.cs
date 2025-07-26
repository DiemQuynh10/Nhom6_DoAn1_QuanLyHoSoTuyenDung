namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.ThongKe
{
    public class BaoCaoRequestVM
    {
        public string? TuKhoa { get; set; }
        public string? TrangThai { get; set; }
        public string? ViTriId { get; set; }
        public string? PhongBanId { get; set; }

        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }

        public string? LoaiBaoCao { get; set; }

        public List<ChartImageVM>? ChartImages { get; set; }
    }

    public class ChartImageVM
    {
        public string? Id { get; set; }
        public string? ImageBase64 { get; set; }
    }
}
