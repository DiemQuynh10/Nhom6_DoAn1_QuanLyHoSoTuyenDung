﻿using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.PhongVanVM;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;

public interface ILichPhongVanService
{
    Task<PhongVanDashboardVM> GetDashboardAsync();

    Task<TaoLichPhongVanVM?> GetFormDataAsync(string ungVienId);
    Task<(bool isSuccess, string message)> CreateLichAsync(LichPhongVan model);
    Task<LichPhongVan?> GetLichByUngVienIdAsync(string ungVienId);
    Task<List<UngVien>> GetUngViensChuaCoLichAsync();

}
