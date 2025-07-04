using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models
{
    public class NhanVien
    {
        [Key]
        [Column("id")]
        public string MaNhanVien { get; set; }

        [Required]
        [Column("ho_ten")]
        public string HoTen { get; set; }

        [Required]
        [Column("ma_nv")]
        public string MaSoNV { get; set; }

        [Column("ngay_sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Column("chuc_vu")]
        public string? ChucVu { get; set; }

        [Column("phong_ban_id")]
        public string? PhongBanId { get; set; }

        [EmailAddress]
        [Column("email")]
        public string? Email { get; set; }

        [Phone]
        [Column("sdt")]
        public string? SoDienThoai { get; set; }

        [Column("ngay_vao_cty")]
        [DataType(DataType.Date)]
        public DateTime? NgayVaoCongTy { get; set; }

        [Column("kinh_nghiem")]
        public string? KinhNghiem { get; set; }

        [Column("mo_ta")]
        public string? MoTa { get; set; }

        [Column("muc_luong")]
        public float? MucLuong { get; set; }

        // -----------------------------
        // Navigation Property
        // -----------------------------

        [ForeignKey("PhongBanId")]
        public virtual PhongBan? PhongBan { get; set; }
    }
}
