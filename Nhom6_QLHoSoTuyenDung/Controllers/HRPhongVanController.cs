﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{
    [Authorize(Roles = $"{RoleNames.HR},{RoleNames.Admin}")]
    public class HRPhongVanController : Controller
    {
        private readonly ILichPhongVanService _lichService;

        public HRPhongVanController(ILichPhongVanService lichService)
        {
            _lichService = lichService;
        }

        public async Task<IActionResult> TrangThaiCho()
        {
            var danhSach = await _lichService.GetUngViensChuaCoLichVong2Async();
            return View("TrangThaiChoHR", danhSach);
        }
    }
}
