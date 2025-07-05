using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models
{
    public class UngVien
    {
        [Key]
        [Column("id")]
        public string MaUngVien { get; set; }

        [Required]
        [Column("ho_ten")]
        public string HoTen { get; set; }

        [Column("ngay_sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }
        public enum GioiTinhEnum { Nam, Nu, Khac }

        [Column("gioi_tinh")]
        public GioiTinhEnum GioiTinh { get; set; }

        [Required, EmailAddress]
        [Column("email")]
        public string Email { get; set; }

        [Column("sdt")]
        [Phone]
        public string SoDienThoai { get; set; }

        [Required]
        [Column("vi_tri_ung_tuyen_id")]
        public string ViTriUngTuyenId { get; set; }

        [Column("link_cv")]
        public string LinkCV { get; set; }

        [Column("kinh_nghiem")]
        public string KinhNghiem { get; set; }

        [Column("thanh_tich")]
        public string ThanhTich { get; set; }

        [Column("mo_ta")]
        public string MoTa { get; set; }

        [Column("trang_thai")]
        public string TrangThai { get; set; }  // VD: Moi, Da duyet, Phong van, Dat, Truot

        [Column("ngay_nop")]
        [DataType(DataType.Date)]
        public DateTime? NgayNop { get; set; }

        [Column("nguon_ung_tuyen")]
        public string NguonUngTuyen { get; set; }

        [ForeignKey("ViTriUngTuyenId")]
        public virtual ViTriTuyenDung? ViTriUngTuyen { get; set; }

        public virtual ICollection<HoSoLuuTru>? HoSoLuuTrus { get; set; }
        public virtual ICollection<LichPhongVan>? LichPhongVans { get; set; }
    }
}
