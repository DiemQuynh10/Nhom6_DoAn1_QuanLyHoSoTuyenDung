using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models
{
    public class ThongKeTuyenDung
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Column("vi_tri_id")]
        public string? ViTriId { get; set; }

        [Column("so_luong_ung_vien")]
        public int? SoLuongUngVien { get; set; }

        [Column("so_luong_dat")]
        public int? SoLuongDat { get; set; }

        [Column("so_luong_truot")]
        public int? SoLuongTruot { get; set; }

        [Column("thoi_gian_thong_ke")]
        [DataType(DataType.Date)]
        public DateTime? ThoiGianThongKe { get; set; }

        // -----------------------------
        // Navigation Property
        // -----------------------------

        [ForeignKey("ViTriId")]
        public virtual ViTriTuyenDung? ViTriTuyenDung { get; set; }
    }
}
