using Nhom6_QLHoSoTuyenDung.Models.ViewModels.PhongVanVM;

namespace Nhom6_QLHoSoTuyenDung.Services.Interfaces
{
    public interface IDanhGiaPhongVanService
    {
        Task<DanhGiaPhongVanVM?> GetFormAsync(string lichId);
        Task<bool> LuuAsync(DanhGiaPhongVanVM vm, int nguoiDungId);
    }


}
