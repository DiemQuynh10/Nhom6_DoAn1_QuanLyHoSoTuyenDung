using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nhom6_QLHoSoTuyenDung.Models.Entities
{
    public class DanhGiaPhongVan
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Required]
        [Column("lich_phong_van_id")]
        public string LichPhongVanId { get; set; }

        [Required]
        [Column("nhan_vien_danh_gia_id")]
        public string NhanVienDanhGiaId { get; set; }

        [Column("diem_danh_gia")]
        public int? DiemDanhGia { get; set; }

        [Column("nhan_xet")]
        public string? NhanXet { get; set; }

        [Column("de_xuat")]
        public string? DeXuat { get; set; } // Enum: Tiếp nhận, Từ chối, Cần bổ sung...

        // -----------------------------
        // Navigation Property
        // -----------------------------

        [ForeignKey("LichPhongVanId")]
        public virtual LichPhongVan? LichPhongVan { get; set; }

        [ForeignKey("NhanVienDanhGiaId")]
        public virtual NhanVien? NhanVienDanhGia { get; set; }
    }
}
