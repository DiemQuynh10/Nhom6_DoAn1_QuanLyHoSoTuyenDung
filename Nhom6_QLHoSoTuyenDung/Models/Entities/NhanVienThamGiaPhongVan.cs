using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.Entities
{
    public class NhanVienThamGiaPhongVan
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Required]
        [Column("lich_phong_van_id")]
        public string LichPhongVanId { get; set; }

        [Required]
        [Column("nhan_vien_id")]
        public string NhanVienId { get; set; }

        [Column("vai_tro")]
        public string? VaiTro { get; set; } // Vai trò trong buổi PV: Chủ trì, Ghi chép, Đánh giá...

        // -----------------------------
        // Navigation Property
        // -----------------------------

        [ForeignKey("LichPhongVanId")]
        public virtual LichPhongVan? LichPhongVan { get; set; }

        [ForeignKey("NhanVienId")]
        public virtual NhanVien? NhanVien { get; set; }
    }
}
