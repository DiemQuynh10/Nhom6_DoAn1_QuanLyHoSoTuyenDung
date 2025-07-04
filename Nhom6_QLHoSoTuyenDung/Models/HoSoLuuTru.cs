using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models
{
    public class HoSoLuuTru
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Required]
        [Column("ung_vien_id")]
        public string UngVienId { get; set; }

        [Column("vi_tri_id")]
        public string? ViTriId { get; set; }

        [Column("ly_do_luu_tru")]
        public string? LyDoLuuTru { get; set; }  // VD: Đã loại, tự rút, không liên hệ được...

        [Column("ngay_luu")]
        [DataType(DataType.Date)]
        public DateTime? NgayLuu { get; set; }

        // -----------------------------
        // Navigation Property
        // -----------------------------

        [ForeignKey("UngVienId")]
        public virtual UngVien? UngVien { get; set; }

        [ForeignKey("ViTriId")]
        public virtual ViTriTuyenDung? ViTriTuyenDung { get; set; }
    }
}
