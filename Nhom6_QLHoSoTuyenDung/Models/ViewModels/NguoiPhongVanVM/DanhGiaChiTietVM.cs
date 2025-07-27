using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM
{
    public class DanhGiaChiTietVM
    {
        [Required]
        public string LichPhongVanId { get; set; }

        [Range(1, 10)]
        public float KyNangChuyenMon { get; set; }

        [Range(1, 10)]
        public float GiaoTiep { get; set; }

        [Range(1, 10)]
        public float GiaiQuyetVanDe { get; set; }

        [Range(1, 10)]
        public float ThaiDoLamViec { get; set; }

        [Range(1, 10)]
        public float TinhThanHocHoi { get; set; }

        public string? NhanXet { get; set; }

        public string? DeXuat { get; set; }

        // ✅ Thêm điểm đánh giá trung bình
        public float DiemDanhGia { get; set; }
    }
}
