using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.PhongVanVM
{
    public class LichPhongVanDashboardVM
    {
        public int TongSoLich { get; set; }
        public int DaPhongVan { get; set; }
        public int ChuaPhongVan { get; set; }

        public List<string> ViTriLabels { get; set; } = new();
        public List<int> ViTriCounts { get; set; } = new();

        public List<LichPhongVan> DanhSachLich { get; set; } = new();

        public List<LichPhongVanHomNayVM> LichHomNay { get; set; } = new();
        public List<LichPhongVanVM> LichPhongVanSapToi { get; set; } = new();
    }
}
