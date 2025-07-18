using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.Dashboard;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.ViTriTuyenDungVM;

namespace Nhom6_QLHoSoTuyenDung.Services.Interfaces
{
    public interface IViTriTuyenDungService
    {
        List<ViTriTuyenDung> GetAll(string? keyword, string? trangThai, string? phongBanId);
        List<int> DemSoLuongHoanThanhTheoThang(List<ViTriTuyenDung> danhSach);
        ViTriTuyenDung? GetById(string id);
        void Create(ViTriTuyenDung model);
        void Update(ViTriTuyenDung model);
        void Delete(string id);
        bool Exists(string id);

        Dictionary<string, int> PhanBoTrangThai(List<ViTriTuyenDung> danhSach);
        (List<string>, List<int>) DemTheoThang(List<ViTriTuyenDung> danhSach);
        List<QuyTrinhTuyenDungItem> ThongKeQuyTrinh(List<UngVien> danhSach);
        List<HoatDongDashboardVM> LayHoatDongGanDay();
    }
}
