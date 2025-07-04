using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models
{
    public class ViTriTuyenDung
    {
        [Key]
        [Column("id")]
        public string MaViTri { get; set; }

        [Required]
        [Column("ten_vi_tri")]
        public string TenViTri { get; set; }

        [Column("so_luong_can_tuyen")]
        public int? SoLuongCanTuyen { get; set; }

        [Column("trang_thai")]
        public string? TrangThai { get; set; } // Đang tuyển, Tạm dừng, Đã đóng, Đã ban hành

        [Column("phong_ban_id")]
        public string? PhongBanId { get; set; }

        [Column("ky_nang")]
        public string? KyNang { get; set; }

        [Column("ngay_tao")]
        [DataType(DataType.Date)]
        public DateTime? NgayTao { get; set; }

        // -----------------------------
        // Navigation Property
        // -----------------------------

        [ForeignKey("PhongBanId")]
        public virtual PhongBan? PhongBan { get; set; }

        public virtual ICollection<UngVien>? UngViens { get; set; }
        public virtual ICollection<LichPhongVan>? LichPhongVans { get; set; }
        public virtual ICollection<ThongKeTuyenDung>? ThongKeTuyenDungs { get; set; }
    }
}
