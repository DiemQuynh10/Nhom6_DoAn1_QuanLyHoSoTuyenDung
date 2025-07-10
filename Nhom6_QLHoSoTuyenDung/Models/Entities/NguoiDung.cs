using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.Entities
{
    public class NguoiDung
    {
        [Key]
        [Column("nhan_vien_id")]
        public string NhanVienId { get; set; }

        [Required]
        [Column("ten_dang_nhap")]
        public string TenDangNhap { get; set; }

        [Required]
        [Column("mat_khau")]
        public string MatKhau { get; set; }

        [Required]
        [Column("vai_tro")]
        public string VaiTro { get; set; } = string.Empty; // enum: admin, hr

        [Column("phong_ban_id")]
        public string? PhongBanId { get; set; }

        [Column("ho_ten")]
        public string? HoTen { get; set; }

        [EmailAddress]
        [Column("email")]
        public string? Email { get; set; }

        [Phone]
        [Column("sdt")]
        public string? SoDienThoai { get; set; }

        [Column("ngay_tao")]
        [DataType(DataType.Date)]
        public DateTime? NgayTao { get; set; }

        // -----------------------------
        // Navigation Property
        // -----------------------------

        [ForeignKey("NhanVienId")]
        public virtual NhanVien? NhanVien { get; set; }

        [ForeignKey("PhongBanId")]
        public virtual PhongBan? PhongBan { get; set; }
    }
}
