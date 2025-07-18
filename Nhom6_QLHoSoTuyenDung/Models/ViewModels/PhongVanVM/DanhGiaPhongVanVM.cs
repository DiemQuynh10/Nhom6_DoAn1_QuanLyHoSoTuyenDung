using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.PhongVanVM
{
    public class DanhGiaPhongVanVM
    {
        // Dùng để ánh xạ khi lưu
        [Required]
        public string LichPhongVanId { get; set; }

        // Thông tin hiển thị trong View (không cần validate)
        public string TenUngVien { get; set; } = string.Empty;
        public string TenViTri { get; set; } = string.Empty;
        public string TenPhong { get; set; } = string.Empty;

        [Display(Name = "Thời gian phỏng vấn")]
        public DateTime ThoiGian { get; set; }

        // Dữ liệu đầu vào từ người đánh giá
        [Required(ErrorMessage = "Vui lòng nhập điểm đánh giá")]
        [Range(0, 10, ErrorMessage = "Điểm đánh giá phải từ 0 đến 10")]
        public float DiemDanhGia { get; set; }

        [Display(Name = "Nhận xét")]
        public string? NhanXet { get; set; }

        [Display(Name = "Đề xuất")]
        public DeXuatEnum? DeXuat { get; set; }
        public string? KinhNghiem { get; set; }
        public string? LinkCV { get; set; }

    }
}
