using Nhom6_QLHoSoTuyenDung.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM
{
    // File: ViewModels/NguoiPhongVanVM/LichSuPhongVanVM.cs

    public class LichSuPhongVanVM
    {
        public string HoTenUngVien { get; set; }
        public string Email { get; set; }
        public string Sdt { get; set; }
        public string ViTriUngTuyen { get; set; }
        public DateTime? ThoiGian { get; set; }
        public string TenPhong { get; set; }
        public string TenNguoiPhongVan { get; set; }
        public string TrangThai { get; set; }
        public string LinkCV { get; set; }
        public string? TrangThaiPhongVan { get; set; }

        // 🔽 Các trường tính toán hiển thị
        public float? DiemKyThuat { get; set; }
        public float? DiemGiaoTiep { get; set; }
        public float? DiemThaiDo { get; set; }
        public float? DiemTrungBinh { get; set; }
        public string NhanXet { get; set; }
        public string DeXuat { get; set; }

        // 🔽 Trường mở rộng chỉ để hiển thị (không liên quan DB)
        public string AvatarText { get; set; }
        public string MucLuong { get; set; }
        public string KyNang { get; set; }
        public string PhongBan { get; set; }
        public string NgayPhongVan => ThoiGian?.ToString("dd/MM/yyyy") ?? "";
        public string GioBatDau => ThoiGian?.ToString("HH:mm") ?? "";
        public string GioKetThuc => ThoiGian?.AddMinutes(45).ToString("HH:mm") ?? "";
        public string LyDoTuChoi => TrangThai == "TuChoi" ? DeXuat : null;
        public string DeXuatHienThi
        {
            get
            {
                if (string.IsNullOrEmpty(DeXuat)) return "";

                if (Enum.TryParse<DeXuatEnum>(DeXuat, out var enumValue))
                {
                    var displayAttr = enumValue.GetType()
                        .GetField(enumValue.ToString())
                        ?.GetCustomAttributes(typeof(DisplayAttribute), false)
                        .FirstOrDefault() as DisplayAttribute;

                    return displayAttr?.Name ?? DeXuat;
                }

                return DeXuat; // fallback nếu là "vào vòng 2" v.v.
            }
        }

    }

}
