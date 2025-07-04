using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models
{
    public class LichPhongVan
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Required]
        [Column("phong_phong_van_id")]
        public string PhongPhongVanId { get; set; }

        [Required]
        [Column("ung_vien_id")]
        public string UngVienId { get; set; }

        [Required]
        [Column("vi_tri_id")]
        public string ViTriId { get; set; }

        [Column("thoi_gian")]
        [DataType(DataType.DateTime)]
        public DateTime? ThoiGian { get; set; }

        [Column("trang_thai")]
        public string? TrangThai { get; set; } // VD: Đã lên lịch, Hoàn thành, Hủy

        [Column("ghi_chu")]
        public string? GhiChu { get; set; }

        // -----------------------------
        // Navigation Property
        // -----------------------------

        [ForeignKey("PhongPhongVanId")]
        public virtual PhongPhongVan? PhongPhongVan { get; set; }

        [ForeignKey("UngVienId")]
        public virtual UngVien? UngVien { get; set; }

        [ForeignKey("ViTriId")]
        public virtual ViTriTuyenDung? ViTriTuyenDung { get; set; }

        public virtual ICollection<NhanVienThamGiaPhongVan>? NhanVienThamGiaPVs { get; set; }
        public virtual ICollection<DanhGiaPhongVan>? DanhGiaPhongVans { get; set; }
    }
}
