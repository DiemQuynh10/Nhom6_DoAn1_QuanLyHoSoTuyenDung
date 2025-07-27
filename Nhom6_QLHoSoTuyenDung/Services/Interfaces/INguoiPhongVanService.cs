using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;

public interface INguoiPhongVanService
{
    Task<DashboardNguoiPhongVanVM> GetDashboardAsync(string username);
    Task<string?> GetLinkCvAsync(string ungVienId);
    Task<List<LichSuPhongVanVM>> GetLichSuPhongVanAsync(string nguoiDungId, string tenNguoiDung);
    Task<LichSuPhongVanThongKeVM> GetThongKeLichSuPhongVanAsync(string nguoiDungId);
    Task<List<LichPhongVan>> GetLichPhongVanTheoNhanVienAsync(string username);
    Task<bool> HuyLichPhongVanAsync(string id, string ghiChu);
    Task<LichPhongVanPageVM> GetLichPhongVanPageAsync(string username);
    // Interface
    Task<List<string>> GetDanhSachLichPhongVanTheoDeXuatAsync(string nguoiDungId, DeXuatEnum deXuat);
    Task<List<DaPhongVanVM>> GetThongTinChiTietLichAsync(List<string> lichIds, string nguoiDungId);
    Task<LichPhongVan?> GetLichByIdAsync(string id);


}
