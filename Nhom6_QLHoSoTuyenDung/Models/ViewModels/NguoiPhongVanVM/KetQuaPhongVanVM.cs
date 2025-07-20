namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM
{
    public class KetQuaPhongVanVM
    {
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string ViTriUngTuyen { get; set; }
        public string AvatarText { get; set; }
        public DateTime? NgayPhongVan { get; set; }
        public TimeSpan? GioBatDau { get; set; }
        public TimeSpan? GioKetThuc { get; set; }
        public string NguoiPhongVan { get; set; }

        public float? DiemKyThuat { get; set; }
        public float? DiemGiaoTiep { get; set; }
        public float? DiemThaiDo { get; set; }
        public float? DiemTrungBinh =>
            (DiemKyThuat + DiemGiaoTiep + DiemThaiDo) / 3;

        public string TrangThai { get; set; } // Đã tuyển / Từ chối
        public string? LyDoTuChoi { get; set; } // nếu có

        public string PhongBan { get; set; }
        public string? MucLuong { get; set; }
        public DateTime? NgayBatDau { get; set; }

        public List<string> KyNang { get; set; } = new();
    }

}
