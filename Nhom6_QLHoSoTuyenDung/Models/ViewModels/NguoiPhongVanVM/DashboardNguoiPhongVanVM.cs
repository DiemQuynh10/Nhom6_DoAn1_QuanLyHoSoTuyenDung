using Nhom6_QLHoSoTuyenDung.Models.Entities;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM
{
    public class DashboardNguoiPhongVanVM
    {
        public int TongSoPhongVan { get; set; }
        public int TangTruongThangTruoc { get; set; }
        public double TyLeThanhCong { get; set; }
        public int ThoiGianTB { get; set; }
        public string ThayDoiThoiGianTB { get; set; } // Thay đổi so với hôm qua (dương/âm/phút)
        public int SoPhongVanHomNay { get; set; }
        public int SoDaHoanThanhHomNay { get; set; }
        public int ThoiGianPhongVanTiepTheo { get; set; }
      

        public List<string> PhongVanLabels { get; set; } = new();
        public List<int> PhongVanValues { get; set; } = new();
        public List<int> ThanhCongValues { get; set; } = new();
        public List<LichPhongVanVM> LichPhongVanSapToi { get; set; } = new();
        public List<LichPhongVanHomNayVM> LichHomNay { get; set; } = new();
        public List<HoatDongGanDayVM> HoatDongGanDay { get; set; } = new();
        public string HoTen { get; set; } = "";
        public string ChucDanh { get; set; } = "";
        public int SoThanhCong { get; set; }
        public int SoKhongPhuHop { get; set; }
        public int TiLeXepHang { get; set; }
        public int XepHang { get; set; }
        public int TongThanhVien { get; set; }
        public int SoBuoiThamGia { get; set; }
        public double TyLeDungGio { get; set; }
        public int SoUngVienDanhGia { get; set; }
     //   public List<int> TrangThaiCounts { get; set; } = new();
        public List<int> TrangThaiUngVienCounts { get; set; } = new();
        public List<string> TrangThaiUngVienLabels { get; set; } = new();
        public List<LichPhongVanVM> LichTreHen { get; set; } = new();
        public List<LichPhongVanVM> ConLai { get; set; } = new(); // ✅ có gán new()


    }

}
