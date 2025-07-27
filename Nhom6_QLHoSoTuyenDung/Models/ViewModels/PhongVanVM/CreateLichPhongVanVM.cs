namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.PhongVanVM
{
    public class CreateLichPhongVanVM
    {
        public string UngVienId { get; set; } = null!;
        public string PhongPhongVanId { get; set; } = null!;
        public DateTime? ThoiGian { get; set; }
        public List<string> NhanVienIds { get; set; } = new();
    }

}
