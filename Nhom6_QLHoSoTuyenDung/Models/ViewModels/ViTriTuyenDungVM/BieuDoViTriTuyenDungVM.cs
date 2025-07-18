using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.Dashboard;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.ViTriTuyenDungVM
{
    public class QuyTrinhTuyenDungItem
    {
        public string Ten { get; set; } = "";
        public int SoLuong { get; set; }
        public string GhiChu { get; set; } = "";
        public int PhanTramThayDoi { get; set; }
    }

    public class BieuDoViTriTuyenDungVM
    {
        public List<ViTriTuyenDung> DanhSachViTri { get; set; } = new();
        public Dictionary<string, int> PhanBoTrangThai { get; set; } = new();
        public List<string> Thang { get; set; } = new();
        public List<int> SoLuongViTriMoi { get; set; } = new();
        public List<int> SoLuongHoanThanh { get; set; } = new(); // nếu muốn biểu đồ 2 đường
        public List<QuyTrinhTuyenDungItem> QuyTrinhTuyenDung { get; set; } = new(); // <- CẦN CÓ
        public List<HoatDongDashboardVM> HoatDongGanDay { get; set; } = new();
    }

}
