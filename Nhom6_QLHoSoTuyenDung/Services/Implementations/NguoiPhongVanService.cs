using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Data;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.Helpers;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;

namespace Nhom6_QLHoSoTuyenDung.Services.Implementations
{
    public class NguoiPhongVanService : INguoiPhongVanService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public NguoiPhongVanService(AppDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        public async Task<DashboardNguoiPhongVanVM> GetDashboardAsync(string username)
        {
            var nguoiDung = await _context.NguoiDungs
                .Include(nd => nd.NhanVien)
                .FirstOrDefaultAsync(nd => nd.TenDangNhap == username);

            if (nguoiDung == null) return new DashboardNguoiPhongVanVM();

            var nhanVienId = nguoiDung.NhanVienId;

            var lichPhongVan = await _context.NhanVienThamGiaPhongVans
                .Where(x => x.NhanVienId == nhanVienId && x.LichPhongVan != null)
                .Include(x => x.LichPhongVan)
                    .ThenInclude(l => l.UngVien)
                .Include(x => x.LichPhongVan)
                    .ThenInclude(l => l.ViTriTuyenDung)
                .Include(x => x.LichPhongVan)
                    .ThenInclude(l => l.PhongPhongVan)
                .Select(x => x.LichPhongVan!)
                .ToListAsync();

            var today = DateTime.Today;
            var weekAgo = DateTime.Now.AddDays(-7);
            var startOfThisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var startOfLastMonth = startOfThisMonth.AddMonths(-1);
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);

            var lichHomNay = lichPhongVan
                .Where(l => l.ThoiGian.HasValue && l.ThoiGian.Value.Date == today)
                .OrderBy(l => l.ThoiGian)
                .Select(l => new LichPhongVanHomNayVM
                {
                    Id = l.Id ?? "",
                    HoTen = l.UngVien?.HoTen ?? "Không có tên",
                    ViTri = l.ViTriTuyenDung?.TenViTri ?? "Không rõ",
                    GioBatDau = l.ThoiGian!.Value,
                    GioKetThuc = l.ThoiGian.Value.AddMinutes(40),
                    HinhThuc = l.PhongPhongVan?.DiaDiem ?? "Online",
                    TrangThai = l.TrangThai ?? "Chưa xác định"
                })
                .ToList();

            var lichSapToi = lichPhongVan
                .Where(l =>
                    l.ThoiGian.HasValue &&
                    l.ThoiGian > DateTime.Now &&
                    l.TrangThai != TrangThaiPhongVanEnum.HoanThanh.ToString() &&
                    l.TrangThai != TrangThaiPhongVanEnum.Huy.ToString()
                )
                .OrderBy(l => l.ThoiGian)
                .Take(5)
                .Select(l => new LichPhongVanVM
                {
                    Id = l.Id ?? "",
                    HoTen = l.UngVien?.HoTen ?? "Không có tên",
                    ViTri = l.ViTriTuyenDung?.TenViTri ?? "Không rõ",
                    ThoiGian = l.ThoiGian,
                    NhanNhan = (l.ThoiGian.Value - DateTime.Now).TotalDays < 1 ? "Hôm nay" : ((l.ThoiGian.Value - DateTime.Now).TotalDays < 2 ? "Ngày mai" : ""),
                    Email = l.UngVien?.Email ?? "",
                    SoDienThoai = l.UngVien?.SoDienThoai ?? "",
                    KinhNghiem = l.UngVien?.KinhNghiem ?? "",
                    TenPhong = l.PhongPhongVan?.TenPhong ?? "Không rõ",
                    DiaDiem = l.PhongPhongVan?.DiaDiem ?? "",
                    TrangThai = l.TrangThai ?? "Chưa xác định",
                    UngVienId = l.UngVienId,
                    LinkCV = l.UngVien?.LinkCV
                })
                .ToList();
            var lichTreHen = lichPhongVan
    .Where(l =>
        l.ThoiGian.HasValue &&
        l.ThoiGian < DateTime.Now &&
        l.TrangThai != TrangThaiPhongVanEnum.HoanThanh.ToString() &&
        l.TrangThai != TrangThaiPhongVanEnum.Huy.ToString()
    )
    .OrderByDescending(l => l.ThoiGian)
    .Select(l => new LichPhongVanVM
    {
        Id = l.Id ?? "",
        HoTen = l.UngVien?.HoTen ?? "Không có tên",
        ViTri = l.ViTriTuyenDung?.TenViTri ?? "Không rõ",
        ThoiGian = l.ThoiGian,
        Email = l.UngVien?.Email ?? "",
        SoDienThoai = l.UngVien?.SoDienThoai ?? "",
        KinhNghiem = l.UngVien?.KinhNghiem ?? "",
        TenPhong = l.PhongPhongVan?.TenPhong ?? "Không rõ",
        DiaDiem = l.PhongPhongVan?.DiaDiem ?? "",
        TrangThai = l.TrangThai ?? "Chưa xác định",
        UngVienId = l.UngVienId,
        LinkCV = l.UngVien?.LinkCV,
    })
    .ToList();


            var hoatDongGanDay = lichPhongVan
    .Where(l => (l.ThoiGian ?? DateTime.MinValue) >= weekAgo)
    .OrderByDescending(l => l.ThoiGian ?? DateTime.MinValue)
    .Take(10)
    .Select(l =>
    {
        var hoatDong = new HoatDongGanDayVM();
        var thoiGian = l.ThoiGian ?? DateTime.Now;
        var span = DateTime.Now - thoiGian;
        var thoiGianTruoc = ThoiGianHelper.TinhTuLuc(thoiGian);

        string hoTenUngVien = l.UngVien?.HoTen ?? "Không rõ";
        string viTri = l.ViTriTuyenDung?.TenViTri ?? "Vị trí không rõ";

        if (l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString())
        {
            hoatDong.NoiDung = $"Phỏng vấn đã hoàn thành với {hoTenUngVien} ({viTri})";
            hoatDong.BieuTuong = "bi-check-circle-fill";
            hoatDong.Mau = "success";
        }
        else if (l.TrangThai == TrangThaiPhongVanEnum.Huy.ToString())
        {
            var huyLuc = l.GhiChu?.Contains("trễ") == true ? "vì trễ" : "";
            hoatDong.NoiDung = $"Lịch phỏng vấn với {hoTenUngVien} đã bị huỷ {huyLuc}".Trim();
            hoatDong.BieuTuong = "bi-x-circle-fill";
            hoatDong.Mau = "danger";
        }
        else if (l.ThoiGian.HasValue && l.ThoiGian.Value > DateTime.Now && (l.ThoiGian.Value - DateTime.Now).TotalHours <= 24)
        {
            hoatDong.NoiDung = $"Sắp có lịch phỏng vấn với {hoTenUngVien} ({viTri})";
            hoatDong.BieuTuong = "bi-calendar-event-fill";
            hoatDong.Mau = "primary";
        }
        else
        {
            hoatDong.NoiDung = $"Lịch phỏng vấn với {hoTenUngVien} đã được lên lịch";
            hoatDong.BieuTuong = "bi-clock-fill";
            hoatDong.Mau = "info";
        }


        hoatDong.ThoiGianTruoc = thoiGianTruoc;
        return hoatDong;
    })
    .ToList();

            int daHoanThanhHomNay = lichHomNay.Count(l => l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString());

            // Tính số phỏng vấn tháng này và tháng trước
            var tongThangNay = lichPhongVan.Count(l => l.ThoiGian >= startOfThisMonth);
            var tongThangTruoc = lichPhongVan.Count(l => l.ThoiGian >= startOfLastMonth && l.ThoiGian < startOfThisMonth);
            int tangTruong = tongThangTruoc == 0 ? tongThangNay : tongThangNay - tongThangTruoc;

            // Tính tỷ lệ thành công theo đề xuất
            int tongHoanThanh = lichPhongVan.Count(l => l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString());
            int soThanhCong = lichPhongVan.Count(l => l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString() && l.UngVien?.TrangThai == TrangThaiUngVienEnum.DaTuyen.ToString());
            int tyLeThanhCong = tongHoanThanh == 0 ? 0 : (int)Math.Round((double)soThanhCong * 100 / tongHoanThanh);

            // Duyệt qua enum và đếm từng loại
            var labels = new List<string>();
            var counts = new List<int>();

            foreach (TrangThaiPhongVanEnum trangThai in Enum.GetValues(typeof(TrangThaiPhongVanEnum)))
            {
                string displayName = trangThai.GetDisplayName();
                int count = lichPhongVan.Count(l => l.TrangThai == trangThai.ToString());

                labels.Add(displayName);
                counts.Add(count);
            }

            // Dữ liệu biểu đồ xu hướng theo thứ
            var phongVanTheoNgay = new Dictionary<string, int>();
            var thanhCongTheoNgay = new Dictionary<string, int>();

            for (int i = 0; i < 5; i++)
            {
                var day = startOfWeek.AddDays(i);
                string label = "T" + (i + 2);
                var lichTrongNgay = lichPhongVan.Where(l => l.ThoiGian.HasValue && l.ThoiGian.Value.Date == day.Date).ToList();
                phongVanTheoNgay[label] = lichTrongNgay.Count;
                thanhCongTheoNgay[label] = lichTrongNgay.Count(l => l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString() && l.UngVien?.TrangThai == TrangThaiUngVienEnum.DaTuyen.ToString());
            }

            return new DashboardNguoiPhongVanVM
            {
                HoTen = nguoiDung.NhanVien?.HoTen ?? nguoiDung.TenDangNhap,
                ChucDanh = nguoiDung.NhanVien?.ChucVu ?? "",

                TongSoPhongVan = lichPhongVan.Count,
                SoThanhCong = soThanhCong,
                SoKhongPhuHop = lichPhongVan.Count(l =>
                    l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString() &&
                    l.UngVien?.TrangThai == TrangThaiUngVienEnum.TuChoi.ToString()),
                TangTruongThangTruoc = tangTruong,
                TyLeThanhCong = tyLeThanhCong,
                ThoiGianTB = 40,
                ThayDoiThoiGianTB = "+3 phút",
                SoPhongVanHomNay = lichHomNay.Count,
                SoDaHoanThanhHomNay = daHoanThanhHomNay,

                PhongVanLabels = phongVanTheoNgay.Keys.ToList(),
                PhongVanValues = phongVanTheoNgay.Values.ToList(),
                ThanhCongValues = thanhCongTheoNgay.Values.ToList(),

                TrangThaiUngVienLabels = labels,
                TrangThaiUngVienCounts = counts,

                LichHomNay = lichHomNay,
                LichPhongVanSapToi = lichSapToi,
                LichTreHen = lichTreHen,
                HoatDongGanDay = hoatDongGanDay,
                ConLai = lichPhongVan
    .Where(l =>
        l.ThoiGian.HasValue &&
        l.ThoiGian.Value.Date == today &&
        l.TrangThai != TrangThaiPhongVanEnum.HoanThanh.ToString() &&
        l.TrangThai != TrangThaiPhongVanEnum.Huy.ToString())
    .Select(l => new LichPhongVanVM
    {
        Id = l.Id ?? "",
        HoTen = l.UngVien?.HoTen ?? "Không có tên",
        ViTri = l.ViTriTuyenDung?.TenViTri ?? "Không rõ",
        ThoiGian = l.ThoiGian,
        Email = l.UngVien?.Email ?? "",
        SoDienThoai = l.UngVien?.SoDienThoai ?? "",
        KinhNghiem = l.UngVien?.KinhNghiem ?? "",
        TenPhong = l.PhongPhongVan?.TenPhong ?? "Không rõ",
        DiaDiem = l.PhongPhongVan?.DiaDiem ?? "",
        TrangThai = l.TrangThai ?? "Chưa xác định",
        UngVienId = l.UngVienId,
        LinkCV = l.UngVien?.LinkCV
    })
    .ToList(),

            };
        }



        public async Task<string?> GetLinkCvAsync(string ungVienId)
        {
            var ungVien = await _context.UngViens.FindAsync(ungVienId);
            return ungVien?.LinkCV;
        }



        public async Task<List<LichSuPhongVanVM>> GetLichSuPhongVanAsync(string id, string tenNguoiDung)

        {
            var lichSus = await _context.LichPhongVans
                .Include(l => l.UngVien).ThenInclude(u => u.ViTriUngTuyen)
                .Include(l => l.PhongPhongVan)
                .Include(l => l.DanhGiaPhongVans)
                .Where(l => l.NhanVienThamGiaPVs.Any(nv => nv.NhanVienId == id) &&
                            (l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString() ||
                             l.TrangThai == TrangThaiPhongVanEnum.Huy.ToString()))
                .Select(l => new LichSuPhongVanVM
                {
                    HoTenUngVien = l.UngVien.HoTen,
                    Email = l.UngVien.Email,
                    Sdt = l.UngVien.SoDienThoai,
                    ViTriUngTuyen = l.UngVien.ViTriUngTuyen.TenViTri,
                    TenPhong = l.PhongPhongVan.TenPhong,
                    ThoiGian = l.ThoiGian,
                    TenNguoiPhongVan = tenNguoiDung, // nếu cần gán tên người đang login
                    LinkCV = l.UngVien.LinkCV,
                    TrangThai = string.IsNullOrWhiteSpace(l.UngVien.TrangThai)
    ? "Không rõ"
    : l.UngVien.TrangThai.ToEnum<TrangThaiUngVienEnum>().GetDisplayName(),

                    TrangThaiPhongVan = string.IsNullOrWhiteSpace(l.TrangThai)
    ? "Không rõ"
    : l.TrangThai.ToEnum<TrangThaiPhongVanEnum>().GetDisplayName(),




                    // Điểm đánh giá
                    DiemKyThuat = l.DanhGiaPhongVans.FirstOrDefault().KyNangChuyenMon,
                    DiemGiaoTiep = l.DanhGiaPhongVans.FirstOrDefault().GiaoTiep,
                    DiemThaiDo = l.DanhGiaPhongVans.FirstOrDefault().ThaiDoLamViec,
                    DiemTrungBinh = (
                        (l.DanhGiaPhongVans.FirstOrDefault().KyNangChuyenMon ?? 0) +
                        (l.DanhGiaPhongVans.FirstOrDefault().GiaoTiep ?? 0) +
                        (l.DanhGiaPhongVans.FirstOrDefault().ThaiDoLamViec ?? 0) +
                        (l.DanhGiaPhongVans.FirstOrDefault().GiaiQuyetVanDe ?? 0) +
                        (l.DanhGiaPhongVans.FirstOrDefault().TinhThanHocHoi ?? 0)
                    ) / 5,

                    NhanXet = l.DanhGiaPhongVans.FirstOrDefault().NhanXet,
                    DeXuat = l.DanhGiaPhongVans.FirstOrDefault().DeXuat,

                    // Tạm gán bổ sung để hiển thị
                    AvatarText = l.UngVien.HoTen.Substring(0, 1).ToUpper(),
                    KyNang = l.UngVien.ViTriUngTuyen.KyNang ?? "Đang cập nhật",
                    PhongBan = l.UngVien.ViTriUngTuyen.PhongBan.TenPhong
                })
                .ToListAsync();

            return lichSus;
        }

        public async Task<LichSuPhongVanThongKeVM> GetThongKeLichSuPhongVanAsync(string nguoiDungId)
        {
            var lichHoanThanh = await _context.LichPhongVans
                .Include(l => l.NhanVienThamGiaPVs) // ❗ Bắt buộc phải có dòng này
                  .Include(l => l.DanhGiaPhongVans)
                .Where(l => l.NhanVienThamGiaPVs!.Any(nv => nv.NhanVienId == nguoiDungId)
                    && (l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString()
                        || l.TrangThai == TrangThaiPhongVanEnum.Huy.ToString()))
                .ToListAsync();

            int tong = lichHoanThanh.Count;

            int daTiepNhan = lichHoanThanh.Count(l =>
                l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString());

            int tuChoi = lichHoanThanh.Count(l =>
                l.TrangThai == TrangThaiPhongVanEnum.Huy.ToString());

            return new LichSuPhongVanThongKeVM
            {
                TongPhongVan = tong,
                DaTiepNhan = daTiepNhan,
                TuChoi = tuChoi
            };
        }
        public async Task<List<LichPhongVan>> GetLichPhongVanTheoNhanVienAsync(string username)
        {
            var nguoiDung = await _context.NguoiDungs
                .Include(nd => nd.NhanVien)
                .FirstOrDefaultAsync(nd => nd.TenDangNhap == username);

            if (nguoiDung?.NhanVienId == null) return new List<LichPhongVan>();

            var nhanVienId = nguoiDung.NhanVienId;

            return await _context.NhanVienThamGiaPhongVans
                .Where(x => x.NhanVienId == nhanVienId && x.LichPhongVan != null)
                .Include(x => x.LichPhongVan) // ✅ Bắt buộc phải có 3 dòng này
                    .ThenInclude(l => l.UngVien)
                .Include(x => x.LichPhongVan)
                    .ThenInclude(l => l.ViTriTuyenDung)
                .Include(x => x.LichPhongVan)
                    .ThenInclude(l => l.PhongPhongVan)
                .Select(x => x.LichPhongVan!)
                .ToListAsync();
        }

  
        public async Task<bool> HuyLichPhongVanAsync(string id, string ghiChu)
        {
            var lich = await _context.LichPhongVans.FindAsync(id);
            if (lich == null) return false;

            lich.TrangThai = TrangThaiPhongVanEnum.Huy.ToString();
            lich.GhiChu = ghiChu;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<LichPhongVanPageVM> GetLichPhongVanPageAsync(string username)
        {
            var lichPhongVan = await GetLichPhongVanTheoNhanVienAsync(username);

            // 📌 1. Lấy tất cả lịch còn lại (chưa hủy, chưa hoàn thành, chưa trễ)
            var conLai = lichPhongVan
                .Where(l => l.ThoiGian > DateTime.Now
                    && l.TrangThai != TrangThaiPhongVanEnum.HoanThanh.ToString()
                    && l.TrangThai != TrangThaiPhongVanEnum.Huy.ToString())
                .OrderBy(l => l.ThoiGian) // để lát lấy lịch sớm nhất
                .ToList();

            // 📌 2. Lấy lịch gần nhất trong danh sách còn lại
            var lichGanNhatEntity = conLai
    .FirstOrDefault(l => l.ThoiGian <= DateTime.Now.AddHours(1));
            var lichGanNhat = lichGanNhatEntity != null ? new LichPhongVanVM
            {
                Id = lichGanNhatEntity.Id ?? "",
                HoTen = lichGanNhatEntity.UngVien?.HoTen ?? "Không có tên",
                ViTri = lichGanNhatEntity.ViTriTuyenDung?.TenViTri ?? "Không rõ",
                ThoiGian = lichGanNhatEntity.ThoiGian,
                Email = lichGanNhatEntity.UngVien?.Email ?? "",
                SoDienThoai = lichGanNhatEntity.UngVien?.SoDienThoai ?? "",
                KinhNghiem = lichGanNhatEntity.UngVien?.KinhNghiem ?? "",
                TenPhong = lichGanNhatEntity.PhongPhongVan?.TenPhong ?? "Không rõ",
                DiaDiem = lichGanNhatEntity.PhongPhongVan?.DiaDiem ?? "",
                TrangThai = lichGanNhatEntity.TrangThai ?? "Chưa xác định",
                UngVienId = lichGanNhatEntity.UngVienId,
                LinkCV = lichGanNhatEntity.UngVien?.LinkCV
            } : null;

            // 📌 3. Loại lịch gần nhất ra khỏi danh sách còn lại
            if (lichGanNhatEntity != null)
            {
                conLai.Remove(lichGanNhatEntity);
            }

            // 📌 4. Lấy lịch trễ hẹn (đã quá giờ, chưa hoàn thành, chưa hủy)
            var lichTreHen = lichPhongVan
                .Where(l => l.ThoiGian <= DateTime.Now
                    && l.TrangThai != TrangThaiPhongVanEnum.HoanThanh.ToString()
                    && l.TrangThai != TrangThaiPhongVanEnum.Huy.ToString())
                .Select(l => new LichPhongVanVM
                {
                    Id = l.Id ?? "",
                    HoTen = l.UngVien?.HoTen ?? "Không có tên",
                    ViTri = l.ViTriTuyenDung?.TenViTri ?? "Không rõ",
                    ThoiGian = l.ThoiGian,
                    Email = l.UngVien?.Email ?? "",
                    SoDienThoai = l.UngVien?.SoDienThoai ?? "",
                    KinhNghiem = l.UngVien?.KinhNghiem ?? "",
                    TenPhong = l.PhongPhongVan?.TenPhong ?? "Không rõ",
                    DiaDiem = l.PhongPhongVan?.DiaDiem ?? "",
                    TrangThai = l.TrangThai ?? "Chưa xác định",
                    UngVienId = l.UngVienId,
                    LinkCV = l.UngVien?.LinkCV
                })
                .ToList();

            return new LichPhongVanPageVM
            {
                LichGanNhat = lichGanNhat,
                LichTreHen = lichTreHen,
                ConLai = conLai.Select(l => new LichPhongVanVM
                {
                    Id = l.Id ?? "",
                    HoTen = l.UngVien?.HoTen ?? "Không có tên",
                    ViTri = l.ViTriTuyenDung?.TenViTri ?? "Không rõ",
                    ThoiGian = l.ThoiGian,
                    Email = l.UngVien?.Email ?? "",
                    SoDienThoai = l.UngVien?.SoDienThoai ?? "",
                    KinhNghiem = l.UngVien?.KinhNghiem ?? "",
                    TenPhong = l.PhongPhongVan?.TenPhong ?? "Không rõ",
                    DiaDiem = l.PhongPhongVan?.DiaDiem ?? "",
                    TrangThai = l.TrangThai ?? "Chưa xác định",
                    UngVienId = l.UngVienId,
                    LinkCV = l.UngVien?.LinkCV
                }).ToList()
            };
        }


        // Implementation
        public async Task<List<string>> GetDanhSachLichPhongVanTheoDeXuatAsync(string username, DeXuatEnum deXuat)
        {
            var nguoiDung = await _context.NguoiDungs
                .Include(nd => nd.NhanVien)
                .FirstOrDefaultAsync(nd => nd.TenDangNhap == username);

            if (nguoiDung?.NhanVienId == null) return new List<string>();

            var maNhanVien = nguoiDung.NhanVienId;

            return await _context.DanhGiaPhongVans
                .Where(d => d.NhanVienDanhGiaId == maNhanVien && d.DeXuat == deXuat.ToString())
                .Select(d => d.LichPhongVanId)
                .ToListAsync();
        }


        public async Task<List<DaPhongVanVM>> GetThongTinChiTietLichAsync(List<string> lichIds, string username)
        {
            var nguoiDung = await _context.NguoiDungs
                .Include(nd => nd.NhanVien)
                .FirstOrDefaultAsync(nd => nd.TenDangNhap == username);

            var maNhanVien = nguoiDung?.NhanVienId ?? "";

            var lichPhongVans = await _context.LichPhongVans
                .Include(l => l.UngVien)
                .Include(l => l.ViTriTuyenDung)
                .Include(l => l.DanhGiaPhongVans)
                .Where(l => lichIds.Contains(l.Id))
                .ToListAsync();

            return lichPhongVans.Select(l => new DaPhongVanVM
            {
                LichId = l.Id,
                TenUngVien = l.UngVien?.HoTen ?? "Không rõ",
                Email = l.UngVien?.Email ?? "",
                ViTri = l.ViTriTuyenDung?.TenViTri ?? "",
                ThoiGian = l.ThoiGian ?? DateTime.Now,
                LinkCV = l.UngVien?.LinkCV,
                DiemTB = l.DanhGiaPhongVans
                    .FirstOrDefault(d => d.NhanVienDanhGiaId == maNhanVien)?.DiemDanhGia,
                NhanXet = l.DanhGiaPhongVans
                    .FirstOrDefault(d => d.NhanVienDanhGiaId == maNhanVien)?.NhanXet
            }).ToList();
        }



    }
}
