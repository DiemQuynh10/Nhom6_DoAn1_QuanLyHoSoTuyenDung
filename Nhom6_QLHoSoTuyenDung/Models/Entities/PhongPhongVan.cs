using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.Entities
{
    public class PhongPhongVan
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Required]
        [Column("ten_phong")]
        public string TenPhong { get; set; }

        [Column("dia_diem")]
        public string? DiaDiem { get; set; }

        [Column("mo_ta")]
        public string? MoTa { get; set; }

        // -----------------------------
        // Navigation Property
        // -----------------------------

        public virtual ICollection<LichPhongVan>? LichPhongVans { get; set; }
    }
}
