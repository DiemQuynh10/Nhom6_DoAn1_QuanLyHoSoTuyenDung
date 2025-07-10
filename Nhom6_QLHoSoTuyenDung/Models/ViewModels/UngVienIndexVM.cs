using Nhom6_QLHoSoTuyenDung.Models.Entities;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels
{
    public class UngVienIndexVM
    {
        public IEnumerable<UngVien> DanhSach { get; set; } = new List<UngVien>();
        public FilterViewModel BoLoc { get; set; } = new FilterViewModel();
    }
}
