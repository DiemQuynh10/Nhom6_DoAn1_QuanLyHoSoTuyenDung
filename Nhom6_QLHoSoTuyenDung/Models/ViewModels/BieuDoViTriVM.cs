using Nhom6_QLHoSoTuyenDung.Models.Entities;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels
{
    public class QuyTrinhTuyenDungItem
    {
        public string Ten { get; set; } = "";
        public int SoLuong { get; set; }
        public string GhiChu { get; set; } = "";
        public int PhanTramThayDoi { get; set; }
    }

    public class BieuDoViTriVM
    {
        public List<ViTriTuyenDung> DanhSachViTri { get; set; } = new();
        public Dictionary<string, int> PhanBoTrangThai { get; set; } = new();
        public List<string> Thang { get; set; } = new();
        public List<int> SoLuongViTriMoi { get; set; } = new();
        public List<int> SoLuongHoanThanh { get; set; } = new(); // nếu muốn biểu đồ 2 đường
        public List<QuyTrinhTuyenDungItem> QuyTrinhTuyenDung { get; set; } = new(); // <- CẦN CÓ
        public List<HoatDongVM> HoatDongGanDay { get; set; } = new();
    }

}
