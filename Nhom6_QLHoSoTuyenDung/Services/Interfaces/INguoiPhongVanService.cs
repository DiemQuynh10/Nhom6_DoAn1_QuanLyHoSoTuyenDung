using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;

public interface INguoiPhongVanService
{
    Task<DashboardNguoiPhongVanVM> GetDashboardAsync(string username);
}
