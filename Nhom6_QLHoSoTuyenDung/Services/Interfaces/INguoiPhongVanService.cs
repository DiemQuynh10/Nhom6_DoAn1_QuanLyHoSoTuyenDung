using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;

public interface INguoiPhongVanService
{
    Task<DashboardNguoiPhongVanVM> GetDashboardAsync(string username);
    Task<string?> GetLinkCvAsync(string ungVienId);
    Task<List<LichSuPhongVanVM>> GetLichSuPhongVanAsync(string nguoiDungId, string tenNguoiDung);
    Task<LichSuPhongVanThongKeVM> GetThongKeLichSuPhongVanAsync(string nguoiDungId);
    Task<List<LichPhongVan>> GetLichPhongVanTheoNhanVienAsync(string username);
    Task<List<DaPhongVanVM>> GetLichPhongVanDaDanhGiaAsync(string userId);
    Task<bool> HuyLichPhongVanAsync(string id, string ghiChu);
    Task<LichPhongVanPageVM> GetLichPhongVanPageAsync(string username);

}
