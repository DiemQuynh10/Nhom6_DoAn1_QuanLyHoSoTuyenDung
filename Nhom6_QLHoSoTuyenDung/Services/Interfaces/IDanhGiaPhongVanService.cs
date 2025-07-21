using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;

namespace Nhom6_QLHoSoTuyenDung.Services.Interfaces
{
    public interface IDanhGiaPhongVanService
    {
        Task<DanhGiaPhongVanVM?> GetFormAsync(string lichId);
        Task<bool> LuuAsync(DanhGiaPhongVanVM vm, string nguoiDungId);

        Task<bool> LuuChiTietAsync(DanhGiaChiTietVM vm, string nguoiDungId);

    }


}
