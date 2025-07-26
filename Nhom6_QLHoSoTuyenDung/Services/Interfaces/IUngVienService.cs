﻿using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.UngVien;
using static Nhom6_QLHoSoTuyenDung.Controllers.UngViensController;

namespace Nhom6_QLHoSoTuyenDung.Services.Interfaces
{
    public interface IUngVienService
    {
        Task<List<UngVien>> GetAllAsync(UngVienBoLocDonGianVM filter);
        Task<int> AddAsync(UngVien model, IFormFile CvFile, IWebHostEnvironment env);
        Task<int> ImportFromExcelAsync(IFormFile file);
        Task<UngVien?> GetByIdAsync(string id);
        Task<Dictionary<string, object>> GetDashboardStatsAsync(List<UngVien> list);
        Task<string> GenerateNewMaUngVienAsync();
    }
}
