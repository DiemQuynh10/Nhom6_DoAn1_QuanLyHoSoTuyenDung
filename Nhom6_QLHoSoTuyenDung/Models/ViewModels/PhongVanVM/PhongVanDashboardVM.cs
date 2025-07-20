using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.PhongVanVM
{
    public class PhongVanDashboardVM
    {
        public int TongSoLich { get; set; }
        public int DaPhongVan { get; set; }
        public int ChuaPhongVan { get; set; }

        public List<LichPhongVan> DanhSachLich { get; set; } = new();

        public List<string> ViTriLabels { get; set; } = new();
        public List<int> ViTriCounts { get; set; } = new();
        public List<string> TrangThaiLabels { get; set; }
        public List<int> TrangThaiValues { get; set; }

        public List<LichPhongVanSapToiVM> LichPhongVanSapToi { get; set; } = new();

    }

}
