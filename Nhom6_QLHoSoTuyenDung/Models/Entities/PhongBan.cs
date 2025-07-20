using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.Entities
{
    public class PhongBan
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Required]
        [Column("ten_phong")]
        public string TenPhong { get; set; }

        [Column("mo_ta")]
        public string? MoTa { get; set; }

        // -----------------------------
        // Navigation Property
        // -----------------------------

        public virtual ICollection<NhanVien>? NhanViens { get; set; }
        public virtual ICollection<NguoiDung>? NguoiDungs { get; set; }
        public virtual ICollection<ViTriTuyenDung>? ViTriTuyenDungs { get; set; }
    }
}
